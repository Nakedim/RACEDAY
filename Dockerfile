
# ==========================================
# 1. Base runtime environment
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# ==========================================
# 2. Build environment
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["RaceDay.Api/RaceDay.Api.csproj", "RaceDay.Api/"]
COPY ["RaceDay.Mvc/RaceDay.Mvc.csproj", "RaceDay.Mvc/"]
COPY ["RaceDay.Contracts/RaceDay.Contracts.csproj", "RaceDay.Contracts/"]


# Restore both applications
RUN dotnet restore "RaceDay.Api/RaceDay.Api.csproj"
RUN dotnet restore "RaceDay.Mvc/RaceDay.Mvc.csproj"

# Copy remaining source code
COPY . .

# ==========================================
# 3. Publish API
# ==========================================
WORKDIR /src/RaceDay.Api

RUN dotnet publish "RaceDay.Api.csproj" \
    -c Release \
    -o /app/publish/api \
    /p:UseAppHost=false

# ==========================================
# 4. Publish MVC
# ==========================================
WORKDIR /src/RaceDay.Mvc

RUN dotnet publish "RaceDay.Mvc.csproj" \
    -c Release \
    -o /app/publish/mvc \
    /p:UseAppHost=false

# ==========================================
# 5. Final runtime environment
# ==========================================
FROM base AS final

WORKDIR /app

# Copy published API
COPY --from=build /app/publish/api ./api

# Copy published MVC
COPY --from=build /app/publish/mvc ./mvc

# Install process manager
RUN apt-get update && apt-get install -y supervisor \
    && rm -rf /var/lib/apt/lists/*

# Configure Supervisor
RUN mkdir -p /etc/supervisor/conf.d

COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]
