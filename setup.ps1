<#
.SYNOPSIS
    Instalacion e inicializacion de un solo paso para N5_CleanArchitecture.

.DESCRIPTION
    - Verifica que Docker Desktop y el .NET SDK esten disponibles (instala la herramienta
      dotnet-ef si falta).
    - Construye y levanta todo el stack con Docker Compose: zookeeper, kafka, kafka-ui, postgres,
      elasticsearch, kibana, el backend (producer) y el frontend.
    - Espera a que PostgreSQL acepte conexiones y aplica las migraciones de EF Core contra
      el contenedor (publicado en el host en el puerto 5433, no 5432, para no chocar con un
      PostgreSQL local si ya tuvieras uno instalado).
    - Verifica que la API responda y abre el frontend en el navegador.

    Nota: el backend se publica en el host en el puerto 5080 (no 5000). En muchas maquinas
    Windows con Hyper-V/WSL2, el puerto 5000 cae dentro de un rango reservado por el sistema
    ('netsh interface ipv4 show excludedportrange protocol=tcp'), lo que hace fallar el mapeo
    de puertos de Docker con un error de permisos de socket.

.PARAMETER SkipBrowser
    No abrir el navegador automaticamente al finalizar.

.EXAMPLE
    .\setup.ps1
#>

[CmdletBinding()]
param(
    [switch]$SkipBrowser
)

$ErrorActionPreference = 'Stop'

$RepoRoot = $PSScriptRoot
Set-Location $RepoRoot

$PostgresPassword = 'PasswordO1.'
$PostgresConnectionString = "Host=localhost;Port=5433;Database=SecurityDb;Username=postgres;Password=$PostgresPassword"

function Write-Step {
    param([string]$Message)
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Test-CommandExists {
    param([string]$Name)
    return [bool](Get-Command $Name -ErrorAction SilentlyContinue)
}

function Invoke-NativeCommandQuiet {
    <#
        Runs a native command with all output suppressed and returns its exit code.
        With $ErrorActionPreference = 'Stop' (set for this whole script) PowerShell 5.1 wraps
        every stderr line from a REDIRECTED native command into a terminating NativeCommandError
        - even when the command is expected to "fail" as part of normal control flow, like a
        readiness probe. Temporarily switching to 'Continue' around the call avoids that, while
        still returning $LASTEXITCODE so callers can check success/failure themselves.
    #>
    param(
        [Parameter(Mandatory)][string]$FilePath,
        [Parameter(ValueFromRemainingArguments)][string[]]$ArgumentList
    )
    $previousPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        & $FilePath @ArgumentList *> $null
    }
    finally {
        $ErrorActionPreference = $previousPreference
    }
    return $LASTEXITCODE
}

# 1. Prerequisitos ------------------------------------------------------------
Write-Step "Verificando prerequisitos"

if (-not (Test-CommandExists 'docker')) {
    Write-Host "Docker no esta instalado o no esta en el PATH. Instala Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Red
    exit 1
}

$dockerExitCode = Invoke-NativeCommandQuiet -FilePath 'docker' -ArgumentList 'info'
if ($dockerExitCode -ne 0) {
    Write-Host "Docker Desktop no parece estar corriendo. Abrelo y vuelve a ejecutar este script." -ForegroundColor Red
    exit 1
}

if (-not (Test-CommandExists 'dotnet')) {
    Write-Host "El .NET SDK no esta instalado. Instalalo desde https://dotnet.microsoft.com/download (se necesita para aplicar las migraciones de EF Core desde este script)." -ForegroundColor Red
    exit 1
}

$efInstalled = dotnet tool list --global | Select-String 'dotnet-ef'
if (-not $efInstalled) {
    Write-Step "Instalando la herramienta global dotnet-ef"
    dotnet tool install --global dotnet-ef | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "No se pudo instalar dotnet-ef. Instalala manualmente con 'dotnet tool install --global dotnet-ef'." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Prerequisitos OK." -ForegroundColor Green

# 2. Build + levantar el stack completo ---------------------------------------
Write-Step "Construyendo las imagenes (backend + frontend)"
docker compose build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Fallo el build de las imagenes. Revisa el mensaje de arriba." -ForegroundColor Red
    exit 1
}

Write-Step "Levantando el stack (zookeeper, kafka, kafka-ui, postgres, elasticsearch, kibana, backend, frontend)"
docker compose up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "Fallo 'docker compose up'. Revisa el mensaje de arriba." -ForegroundColor Red
    exit 1
}

# 3. Esperar a que PostgreSQL acepte conexiones -------------------------------
Write-Step "Esperando a que PostgreSQL este listo"

$maxAttempts = 30
$attempt = 0
$pgReady = $false

while ($attempt -lt $maxAttempts -and -not $pgReady) {
    $attempt++
    $probeExitCode = Invoke-NativeCommandQuiet -FilePath 'docker' -ArgumentList `
        'compose', 'exec', '-T', 'postgres', 'pg_isready', '-U', 'postgres'
    if ($probeExitCode -eq 0) {
        $pgReady = $true
    }
    else {
        Write-Host "  PostgreSQL todavia no responde (intento $attempt/$maxAttempts)..."
        Start-Sleep -Seconds 3
    }
}

if (-not $pgReady) {
    Write-Host "PostgreSQL no respondio a tiempo. Revisa 'docker compose logs postgres'." -ForegroundColor Red
    exit 1
}

Write-Host "PostgreSQL listo." -ForegroundColor Green

# 4. Migraciones de EF Core ----------------------------------------------------
Write-Step "Aplicando migraciones de EF Core contra el PostgreSQL del contenedor"

dotnet ef database update `
    --project Services/Security/Security.Infrastructure `
    --startup-project Services/Security/Security.Presentation `
    --connection $PostgresConnectionString

if ($LASTEXITCODE -ne 0) {
    Write-Host "Las migraciones fallaron. Revisa el mensaje de error de arriba." -ForegroundColor Red
    exit 1
}

Write-Host "Migraciones aplicadas." -ForegroundColor Green

# 5. Smoke test del backend ----------------------------------------------------
Write-Step "Verificando que la API responde"

$maxApiAttempts = 20
$apiReady = $false
for ($i = 0; $i -lt $maxApiAttempts -and -not $apiReady; $i++) {
    try {
        $response = Invoke-WebRequest -Uri 'http://localhost:5080/api/Permissions/Test' -UseBasicParsing -TimeoutSec 3
        if ($response.StatusCode -eq 200) {
            $apiReady = $true
        }
    }
    catch {
        Start-Sleep -Seconds 3
    }
}

if ($apiReady) {
    Write-Host "API respondiendo en http://localhost:5080" -ForegroundColor Green
}
else {
    Write-Host "La API todavia no respondio a tiempo; puede seguir iniciando. Revisa 'docker compose logs producer'." -ForegroundColor Yellow
}

# 6. Abrir el navegador ---------------------------------------------------------
if (-not $SkipBrowser) {
    Write-Step "Abriendo la interfaz web"
    Start-Process 'http://localhost:3000'
}

Write-Host ""
Write-Host "Listo. Servicios disponibles:" -ForegroundColor Cyan
Write-Host "  Frontend:      http://localhost:3000"
Write-Host "  API:           http://localhost:5080/api/Permissions/Test"
Write-Host "  Kafka UI:      http://localhost:8080"
Write-Host "  ElasticSearch: http://localhost:9200"
Write-Host "  Kibana:        http://localhost:5601"
Write-Host "  PostgreSQL:    localhost:5433 (db SecurityDb, user postgres)"
Write-Host ""
Write-Host "Para ver logs:   docker compose logs -f [servicio]"
Write-Host "Para detener:    docker compose down"
