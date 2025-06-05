# === Base runtime image ===
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# === Build stage ===
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all .csproj files
COPY library-app.Contracts/library-app.Contracts.csproj library-app.Contracts/
COPY library-app.Data/library-app.Data.csproj library-app.Data/
COPY library-app.Domain/library-app.Identity.csproj library-app.Domain/
COPY library-app.Models/library-app.Models.csproj library-app.Models/
COPY library-app.Repository/library-app.Repository.csproj library-app.Repository/
COPY library-app.Service/library-app.Service.csproj library-app.Service/
COPY library-app/library-app.Core.csproj library-app/

# Restore dependencies
RUN dotnet restore "library-app/library-app.Core.csproj"

# Copy everything
COPY . .

# Build
WORKDIR /src/library-app
RUN dotnet build "library-app.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

# === Migration stage ===
FROM build AS migrate
WORKDIR /src/library-app
RUN dotnet tool install --global dotnet-ef \
    && export PATH="$PATH:/root/.dotnet/tools" \
    && ~/.dotnet/tools/dotnet-ef database update --project "library-app.Core.csproj"

# === Publish stage ===
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "library-app.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# === Final runtime stage ===
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "library-app.Core.dll"]
