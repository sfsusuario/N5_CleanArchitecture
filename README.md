# N5 Challenge

Poryecto con el desarrollo al problem planteado.

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
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "ProjectConfiguration": {
    "KafkaConnection": "localhost:9092",
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
docker run -p 5000:80 security_dotnet
```

## Comando de prueba

```bash
curl -X GET localhost:5000/api/Permissions/Test
# debe imprimir "Llamado"
```
