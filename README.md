# N5 Challenge

Proyecto con el desarrollo del problema planteado.

Para el detalle de arquitectura, patrones de diseño utilizados y guía de despliegue completa, ver [Services/Security/DOCUMENTATION.md](Services/Security/DOCUMENTATION.md).

## Levantar todo con un solo comando

El `docker-compose.yaml` en la raíz del repositorio centraliza la ejecución de toda la aplicación: frontend, backend y sus dependencias (PostgreSQL, Kafka, Kafka UI, Elasticsearch), cada una en su propio contenedor.

```powershell
.\setup.ps1
```

Este script (PowerShell) verifica que Docker Desktop y el .NET SDK estén disponibles, construye y levanta todos los contenedores, espera a que PostgreSQL esté listo, aplica las migraciones de EF Core y abre la interfaz web (`http://localhost:3000`). Alternativa manual, sin el script:

```bash
docker compose up
# luego aplicar migraciones (ver Services/Security/DOCUMENTATION.md, sección Despliegue)
```

## Archivos de configuración
Los archivos de configuración para base de datos, kafka y elastic search, se encuentran ubicados en:

```
Services/Security/Security.Presentation/appsettings.json
```

Se encuentran en el siguiente formato:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SecurityDb;Username=postgres;Password=PasswordO1."
  },
  "ProjectConfiguration": {
    "KafkaConnection": "localhost:9092",
    "KafkaTopic": "mytopic",
    "ElasticSearchConnection": "http://localhost:9200"
  }
}
```




## Ubicación del Dockerfile
El archivo dockerfile para el proyecto se encuentra ubicado en:

```
Services/Security/Dockerfile
```

Ejecutar en esa misma ruta de directorio.

## Construir imagen y contenedor
Para construir y ejecutar el contenedor:

```bash
cd Services/Security/
docker build -t security_dotnet .
docker run -p 5080:80 security_dotnet
```

## Comando de prueba

```bash
curl -X GET localhost:5080/api/Permissions/Test
# debe imprimir "Llamado"
```

Para levantar kafka, postgres, elasticsearch y el frontend junto con el backend, usar `docker compose up` desde la **raíz del repositorio** (no desde esta carpeta) — ver la sección "Levantar todo con un solo comando" arriba, o `.\setup.ps1`.

La interfaz web (`Services/Security/Security.Frontend`, React + TypeScript + Redux Toolkit/Saga + Bootstrap) queda disponible en `http://localhost:3000`. Ver [Services/Security/DOCUMENTATION.md](Services/Security/DOCUMENTATION.md) para correrla en modo desarrollo o construir su imagen por separado.