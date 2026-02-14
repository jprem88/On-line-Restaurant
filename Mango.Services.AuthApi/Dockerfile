# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy everything from solution root (build context)
COPY . .

# Restore dependencies for this API project
RUN dotnet restore "Mango.Services.AuthApi/Mango.Services.AuthApi.csproj"

# Build this API project
RUN dotnet build "Mango.Services.AuthApi/Mango.Services.AuthApi.csproj" -c Release -o /app/build

# Publish this API project
RUN dotnet publish "Mango.Services.AuthApi/Mango.Services.AuthApi.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Set entrypoint
ENTRYPOINT ["dotnet", "Mango.Services.AuthApi.dll"]
