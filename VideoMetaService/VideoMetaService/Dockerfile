# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["VideoMetaService/VideoMetaService/VideoMetaService.csproj", "VideoMetaService/VideoMetaService/"]
RUN dotnet restore "VideoMetaService/VideoMetaService/VideoMetaService.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/VideoMetaService/VideoMetaService"
RUN dotnet build "VideoMetaService.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "VideoMetaService.csproj" -c Release -o /app/publish

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoMetaService.dll"]
