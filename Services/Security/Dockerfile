
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY ["./Security.Application/Security.Application.csproj", "src/Security.Application/"]
COPY ["./Security.Domain/Security.Domain.csproj", "src/Security.Domain/"]
COPY ["./Security.Infrastructure/Security.Infrastructure.csproj", "src/Security.Infrastructure/"]
COPY ["./Security.Presentation/Security.Presentation.csproj", "src/Security.Presentation/"]
COPY ["./Security.Presentation/dist/*", "/app/dist"]

# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "src/Security.Presentation/Security.Presentation.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/Security.Presentation/"
RUN dotnet build -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app

COPY --from=publish /app/publish .
RUN ls -l
ENTRYPOINT [ "dotnet", "Security.Presentation.dll" ]

#docker build -t security_dotnet .
#docker run -p 5000:80 security_dotnet