# Security Service — Documentación técnica

Este documento describe la arquitectura, los patrones de diseño y el proceso de despliegue del microservicio **Security**, que gestiona solicitudes de permisos de empleados (alta, consulta y modificación).

## 1. Arquitectura

### Capas del proyecto

El servicio sigue **Clean Architecture** (también llamada Onion Architecture): las dependencias siempre apuntan hacia adentro, hacia el Dominio, que no depende de ninguna otra capa.

```
Services/Security/
├── Security.Domain          Entidades, contratos (interfaces), comandos/queries CQRS. No depende de nada más.
├── Security.Application     Casos de uso: handlers de MediatR, mapeo con AutoMapper, validación con FluentValidation.
├── Security.Infrastructure  Implementaciones concretas: EF Core (PostgreSQL/Npgsql), Kafka, Elasticsearch.
├── Security.Presentation    API REST (ASP.NET Core): controllers, Program.cs (composition root), Swagger.
├── Security.Test            Tests unitarios (xUnit + Moq + Shouldly) y de repositorio (EF Core InMemory).
└── Security.Frontend        SPA React + TypeScript que consume la API (ver sección 1.1).
```

### 1.1 Frontend (`Security.Frontend`)

Proyecto React independiente (no forma parte de la solución .NET) que consume la API REST del backend:

- **React 18 + TypeScript**, empaquetado con **Vite**.
- **Redux Toolkit + redux-saga**: el estado de `permissions` vive en un slice de RTK (`src/store/permissions/permissionsSlice.ts`); toda la lógica asíncrona (llamadas HTTP) vive en sagas (`permissionsSaga.ts`) que escuchan acciones `*Requested`, llaman a la API con `call()` y despachan `*Succeeded`/`*Failed` con `put()` — el store se configura sin `redux-thunk` (`thunk: false`) para que el saga sea el único mecanismo de efectos secundarios.
- **react-bootstrap + Bootstrap 5** para los componentes de UI (Navbar, Table, Form, Card).
- **axios** como cliente HTTP, con la URL base de la API tomada de `VITE_API_BASE_URL` (variable de build de Vite).

Estructura relevante:

```
Security.Frontend/src/
├── api/permissionsApi.ts              Cliente HTTP (axios) hacia /api/Permissions/*
├── types/permission.ts                Interfaces TS que reflejan los DTOs del backend
├── store/
│   ├── store.ts                       configureStore + sagaMiddleware
│   ├── rootSaga.ts
│   └── permissions/
│       ├── permissionsSlice.ts        Estado + acciones (RTK)
│       └── permissionsSaga.ts         Efectos: fetch/request/modify permission
└── components/                        Layout, PermissionsList, PermissionForm
```

Funcionalidad: lista de permisos (`GetPermissions`), alta (`RequestPermission`) y edición (`ModifyPermission`) desde un único formulario, con recarga automática de la lista tras cada operación exitosa (la saga despacha `fetchPermissionsRequested` al finalizar). El campo "Tipo de permiso" es un `<select>` poblado desde `GET /api/PermissionTypes` (`permissionTypesSlice`/`permissionTypesSaga`, mismo patrón que `permissions`) en vez de un Id numérico libre; el link "+ Nuevo tipo" junto al label despliega un mini-formulario inline que llama a `POST /api/PermissionTypes` y refresca la lista al terminar.

Referencias de proyecto (siempre hacia adentro):

```
Presentation → Application, Domain, Infrastructure
Infrastructure → Domain, Application
Application → Domain
Domain → (nada)
```

### Flujo de una request — ejemplo `POST /api/Permissions/RequestPermission`

```
Cliente HTTP
   │
   ▼
PermissionsController.RequestPermission()          (Presentation)
   │  _mediator.Send(command)
   ▼
RequestPermissionHandler.Handle()                   (Application)
   │  1. Mapea RequestPermissionCommand → Permissions (AutoMapper)
   │  2. BeginTransactionAsync()
   │  3. _repoCommand.RequestAsync(entity) + Save()    (persiste en PostgreSQL ← fuente de verdad,
   │                                                     y genera el Id autoincremental)
   │  4. OutboxMessages.RequestAsync(...) x2 + Save()  (escribe 2 filas: canal Kafka y canal
   │                                                     Elasticsearch, con el Id ya conocido)
   │  5. CommitTransactionAsync()                      (todo lo anterior se confirma junto)
   │  6. Mapea Permissions → PermissionResponse
   ▼
Respuesta 200 OK con PermissionResponse       (Kafka/Elasticsearch NO se llamaron todavía)

OutboxDispatcherService (background, cada 5s)
   │  Lee OutboxMessages con ProcessedAt == null
   │  Por cada fila: deserializa el payload y llama a Kafka/Elasticsearch (con retry + circuit
   │  breaker vía Polly). Si tiene éxito, marca ProcessedAt; si falla, incrementa RetryCount y
   │  la deja pendiente para el próximo ciclo.
   ▼
Kafka topic "mytopic" / índice Elasticsearch "permissions" actualizados
```

`ModifyPermission` sigue el mismo esquema (transacción + outbox). `GetPermissions` es la excepción: al ser una lectura sin cambio de estado que persistir, sigue llamando a Kafka directamente (con la misma resiliencia de Polly, pero sin pasar por el outbox — no hay nada que la escritura del outbox necesitaría hacer atómico con una simple consulta).

### Servicios externos

| Servicio | Uso | Cliente |
|---|---|---|
| PostgreSQL | Persistencia de `Permissions`, `PermissionTypes` y `OutboxMessages` | EF Core 6 (`SecurityContext`) vía `Npgsql.EntityFrameworkCore.PostgreSQL` |
| Kafka | Evento de auditoría por cada operación (REQUEST/MODIFY/GET) | Confluent.Kafka, vía `OutboxDispatcherService` (REQUEST/MODIFY) o directo (GET) |
| Elasticsearch | Índice de búsqueda de permisos (`permissions`) | NEST, vía `OutboxDispatcherService` |

PostgreSQL es la única fuente de verdad transaccional. Gracias al patrón Outbox, un `RequestPermission`/`ModifyPermission` exitoso ya no depende de que Kafka o Elasticsearch estén disponibles en ese instante — sus notificaciones quedan garantizadas por el `OutboxDispatcherService`, que reintenta indefinidamente hasta lograr entregarlas.

> **Nota de diseño — por qué PostgreSQL y no SQL Server**: el proyecto originalmente usaba SQL Server, pero su imagen de Linux (`sqlpal`, la capa de emulación de APIs de Windows que usan *todas* las variantes de SQL Server para contenedores, incluida Azure SQL Edge) resultó incompatible con Docker Desktop/WSL2 en algunas máquinas — crasheaba al iniciar sin importar la versión probada (2019, 2022, Azure SQL Edge). PostgreSQL es un binario Linux nativo sin esa capa de compatibilidad, arranca en segundos, y el esquema de este proyecto (3 tablas simples, sin stored procedures ni features específicas de T-SQL) migró sin fricción. Un efecto colateral del cambio: `PermissionDate` se mapea explícitamente como `date` de Postgres (no `timestamp with time zone`) en `SecurityContext.OnModelCreating`, porque Npgsql 6+ exige que todo `DateTime` escrito en una columna `timestamptz` tenga `Kind = Utc`, y la fecha que manda el frontend no lo tiene — mapearla como `date` evita el problema de raíz y es semánticamente más correcto para un dato que no tiene componente horario.

## 2. Patrones de diseño y ubicación en el código

| Patrón | Dónde | Notas |
|---|---|---|
| Clean / Onion Architecture | Estructura de proyectos (`Security.Domain/Application/Infrastructure/Presentation`) | Las dependencias de `.csproj` reflejan la regla de dependencia hacia adentro |
| CQRS + Mediator | `Security.Domain/CQRS/*` (comandos/queries), `Security.Application/Handlers/*` (handlers) | MediatR enruta cada `IRequest` a su `IRequestHandler` |
| Repository | `Security.Domain/Repositories/*` (contratos), `Security.Infrastructure/Repositories/*` (implementación EF Core) | Separa el acceso a datos de la lógica de negocio |
| Unit of Work | `Security.Infrastructure/Repositories/UnitOfWork.cs` | Agrupa los repositorios de un mismo `SecurityContext`; `Save()` es el único punto que llama `SaveChangesAsync()`, dando atomicidad real cuando un handler toca más de un repositorio |
| AutoMapper (Object Mapper) | `Security.Application/Mapper/*` | `SecurityMappingProfile` define los mapeos; `PermissionsMapper` expone un `IMapper` singleton perezoso |
| Dependency Injection | `Security.Presentation/Program.cs` | Composition root: registra `DbContext`, MediatR, repositorios, Kafka/Elasticsearch, FluentValidation, el `OutboxDispatcherService` |
| Pipeline Behaviour (validación) | `Security.Application/Behaviours/ValidationBehaviour.cs` + `Security.Application/Validators/*` | Se ejecuta antes de cada handler de MediatR; si un `FluentValidation.IValidator` falla, lanza `ValidationException` sin que el handler llegue a ejecutarse |
| **Outbox** | Entidad: `Security.Domain/Entities/OutboxMessage.cs`. Escritura: `RequestPermissionHandler`/`ModifyPermissionHandler` (vía `IUnitOfWork.OutboxMessages`). Lectura/despacho: `Security.Infrastructure/BackgroundServices/OutboxDispatcherService.cs` | La fila de outbox se persiste en la misma transacción que el cambio de negocio (`BeginTransactionAsync`/`CommitTransactionAsync` en `UnitOfWork`); un `BackgroundService` la entrega después, de forma asíncrona. Así una caída de Kafka/Elasticsearch nunca hace fallar la request ni pierde el evento |
| **Circuit Breaker + Retry** | `Security.Infrastructure/Resilience/ResiliencePolicyFactory.cs`, usado por `KafkaCommandExternal` y `ElasticSearchCommandExternal` | Con Polly: 3 reintentos con backoff exponencial (200ms, 400ms, 800ms) y luego un circuit breaker que abre tras 5 fallos consecutivos por 30s. El retry envuelve al circuit breaker, así que una vez abierto el circuito, los reintentos fallan rápido (`BrokenCircuitException`) en vez de seguir golpeando una dependencia caída |
| **Flux/Redux (frontend)** | `Security.Frontend/src/store/*` | Estado unidireccional: componente despacha acción → reducer (RTK slice) actualiza estado síncrono → saga observa la acción y ejecuta el efecto async → despacha una nueva acción con el resultado |
| **Saga (frontend)** | `Security.Frontend/src/store/permissions/permissionsSaga.ts` | `redux-saga`: generadores que orquestan llamadas HTTP asíncronas (`call`) y despacho de acciones (`put`) de forma testeable y declarativa, en vez de lógica async dispersa en los componentes |

## 3. Despliegue

### Opción A — `setup.ps1` (recomendado: todo automatizado)

El `docker-compose.yaml` que centraliza la ejecución de toda la aplicación (frontend, backend y sus 4 dependencias — PostgreSQL, Kafka, Elasticsearch, Zookeeper, cada una en su propio contenedor) vive en la **raíz del repositorio**. `setup.ps1` (también en la raíz) automatiza todo el flujo sobre ese archivo:

```powershell
.\setup.ps1
```

Paso a paso, el script:

1. Verifica que Docker Desktop esté corriendo y que el .NET SDK esté instalado (instala la herramienta `dotnet-ef` si falta).
2. `docker compose build` + `docker compose up -d` — construye y levanta los 6 contenedores.
3. Espera (con reintentos) a que PostgreSQL acepte conexiones dentro del contenedor.
4. Aplica las migraciones de EF Core contra ese PostgreSQL (`dotnet ef database update --connection "Host=localhost;Port=5433;..."`, sin tocar ningún PostgreSQL local que tengas instalado — el contenedor publica en el puerto 5433 del host, no el 5432 por defecto, justamente para evitar ese choque).
5. Verifica que `GET /api/Permissions/Test` responda 200.
6. Abre `http://localhost:3000` en el navegador (usar `-SkipBrowser` para omitir este paso).

Para detener todo: `docker compose down`. Para ver logs de un servicio puntual: `docker compose logs -f <servicio>` (`producer`, `frontend`, `postgres`, `kafka`, `kafka-ui`, `elasticsearch`, `zookeeper`).

### Opción A.2 — Docker Compose manual (sin el script)

```bash
docker compose up          # desde la raíz del repositorio
```

Las cadenas de conexión del contenedor `producer` se pasan por variables de entorno en `docker-compose.yaml` apuntando a los hostnames internos de la red de Docker (no a `localhost`); el `frontend`, en cambio, corre en el navegador del host, así que su `VITE_API_BASE_URL` apunta a `http://localhost:5080` (el puerto mapeado de `producer`), no a un hostname interno de Docker.

La primera vez, aplicar las migraciones de EF Core contra el PostgreSQL del contenedor (ver comando en la Opción B, con `--connection "Host=localhost;Port=5433;Database=SecurityDb;Username=postgres;Password=PasswordO1."`).

Probar:

```bash
curl -X GET localhost:5080/api/Permissions/Test
# debe imprimir "Llamado"
```

Abrir `http://localhost:3000` para la interfaz web.

### Opción B — Local sin Docker

Requisitos: PostgreSQL accesible, .NET 6 SDK.

1. Ajustar `Services/Security/Security.Presentation/appsettings.json` (o `appsettings.Development.json`) con tu connection string (formato `Host=...;Port=5432;Database=...;Username=...;Password=...`), endpoint de Kafka y de Elasticsearch.
2. Aplicar migraciones:
   ```bash
   dotnet ef database update --project Services/Security/Security.Infrastructure --startup-project Services/Security/Security.Presentation
   ```
3. Levantar la API:
   ```bash
   dotnet run --project Services/Security/Security.Presentation
   ```
4. Swagger disponible en modo Development (`https://localhost:7014/swagger` o el puerto configurado en `launchSettings.json`).

El `OutboxDispatcherService` arranca automáticamente junto con la API (no requiere ningún paso extra) y hace polling cada 5 segundos sobre la tabla `OutboxMessages`. Para verificar que un `RequestPermission`/`ModifyPermission` efectivamente notificó a Kafka/Elasticsearch, consultar esa tabla: `ProcessedAt` no nulo significa entregado; `RetryCount`/`LastError` indican reintentos en curso si Kafka o Elasticsearch no están disponibles.

### Correr los tests

```bash
dotnet test Services/Security/Security.Test/Security.Test.csproj
```

Los tests son unitarios (handlers, controller, mapeo AutoMapper) más tests de repositorio contra una base EF Core InMemory — no requieren Docker ni PostgreSQL real.

### Construir solo la imagen Docker (backend)

```bash
cd Services/Security
docker build -t security_dotnet .
docker run -p 5080:80 security_dotnet
```

### Frontend — modo desarrollo (sin Docker)

Requisitos: Node.js 20+.

```bash
cd Services/Security/Security.Frontend
cp .env.example .env      # ajustar VITE_API_BASE_URL si el backend no corre en localhost:5080
npm install
npm run dev                # sirve en http://localhost:5173 con hot-reload
```

El backend debe estar corriendo (Opción A o B) y permitir el origen `http://localhost:5173` por CORS — ya configurado en `Program.cs` (`FrontendCorsPolicy`).

### Frontend — construir solo la imagen Docker

```bash
cd Services/Security/Security.Frontend
docker build -t security_frontend --build-arg VITE_API_BASE_URL=http://localhost:5080 .
docker run -p 3000:80 security_frontend
```

`VITE_API_BASE_URL` se debe pasar como `--build-arg` (no como variable de entorno del contenedor en runtime): Vite la incrusta en el bundle JS durante `npm run build`, así que cambiarla después de construir la imagen no tiene efecto — hay que reconstruir con el valor correcto.

### Troubleshooting: el contenedor `postgres` no publica en el puerto esperado

Si ya tenés un PostgreSQL corriendo nativo en el host (Windows lo instala como servicio y queda escuchando en `5432`), el contenedor `postgres` de este proyecto **no** choca con él porque se publica en el puerto `5433` del host (`docker-compose.yaml`: `"5433:5432"`) — internamente, dentro de la red de Docker, sigue siendo el puerto `5432` estándar. Si necesitás conectarte con un cliente externo (pgAdmin, DBeaver, `psql`) al Postgres del contenedor, usá `localhost:5433`, no `5432`.

(Nota histórica: la base de datos original de este proyecto era SQL Server, pero su imagen de Linux resultó incompatible con Docker Desktop/WSL2 en algunas máquinas — ver la nota de diseño en la sección "Servicios externos" más arriba.)

## 4. Limitaciones conocidas / mejoras futuras

- **Sin autenticación real**: `Program.cs` llama `UseAuthorization()`, pero no hay `UseAuthentication()` ni ningún `[Authorize]` en los controllers, así que hoy es un placeholder inerte. Para producción se recomienda JWT (o el esquema de identidad que use el resto del sistema) — se dejó fuera de alcance de esta iteración por exceder lo esperado en una prueba técnica de un solo microservicio.
- **Outbox dispatcher in-process, no distribuido**: `OutboxDispatcherService` corre como `BackgroundService` dentro del mismo proceso de la API (decisión consciente para no agregar un proyecto/contenedor nuevo). Si se escalan varias réplicas de la API, cada una correría su propio dispatcher compitiendo por las mismas filas — para producción con múltiples instancias convendría un `SELECT ... FOR UPDATE SKIP LOCKED` (o equivalente) al leer el batch pendiente, o mover el dispatcher a un Worker Service independiente con una sola réplica.
- **Sin tests de integración contra infraestructura real**: `KafkaCommandExternal` y `ElasticSearchCommandExternal` no tienen test directo porque requerirían Kafka/Elasticsearch reales (por ejemplo vía Testcontainers). Se cubrieron indirectamente mockeando sus interfaces en los tests de los handlers y del `OutboxDispatcherService`.
- **Validación de negocio limitada**: los validadores de FluentValidation comprueban campos vacíos/formato, pero no verifican contra la base de datos que `PermissionType` exista en `PermissionTypes` (evita acoplar `Security.Application` a `Security.Infrastructure`).
- **Sin purga del outbox**: las filas de `OutboxMessages` ya procesadas se quedan en la tabla indefinidamente. Un job de limpieza periódico (borrar filas con `ProcessedAt` de hace más de N días) sería lo esperable en producción.
- **CORS con orígenes hardcodeados**: `FrontendCorsPolicy` en `Program.cs` permite explícitamente `localhost:5173`/`localhost:3000`. Para un dominio de producción real habría que mover esos orígenes a `appsettings.json`/variables de entorno en vez de tenerlos fijos en el código.
- **Sin tests de frontend**: no se agregaron tests (Vitest/RTL) para los componentes React ni para las sagas — fuera de alcance de esta iteración, mencionado aquí como gap conocido.
