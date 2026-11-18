# 1. Base runtime environment
FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. SDK build environment
FROM ://microsoft.com AS build
WORKDIR /src

# Copy all project files using their relative root locations
COPY ["RaceDay.Api/RaceDay.Api.csproj", "RaceDay.Api/"]
COPY ["RaceDay.Contracts/RaceDay.Contracts.csproj", "RaceDay.Contracts/"]
COPY ["RaceDay.Mvc/RaceDay.Mvc.csproj", "RaceDay.Mvc/"]
COPY ["Raceday.Shared/Raceday.Shared.csproj", "Raceday.Shared/"]

# Restore dependencies for your API project
RUN dotnet restore "RaceDay.Api/RaceDay.Api.csproj"

# Copy all remaining source code files
COPY . .
WORKDIR "/src/RaceDay.Api"
RUN dotnet build "RaceDay.Api.csproj" -c Release -o /app/build

# 3. Publish the build artifacts
FROM build AS publish
RUN dotnet publish "RaceDay.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final production container setup
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.Api.dll"]
