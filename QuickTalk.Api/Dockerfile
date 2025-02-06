# Use the official ASP.NET Core image as the base
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR "/src"

# Copy the .csproj and restore any dependencies (via NuGet)
COPY ["QuickTalk.Api.csproj", "/src/"]

# Install the dependencies
RUN dotnet restore "/src/QuickTalk.Api.csproj"

# Set permissions for the /app directory before building
RUN mkdir -p /app/build/
RUN chmod -R 777 /app

# Copy all source files
COPY . .

# Build the app
WORKDIR "/src"
RUN dotnet build "QuickTalk.Api.csproj" -c Release -o /app/build

# Expose ports for debugging
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

# Publish the app
FROM build AS publish
RUN dotnet publish "QuickTalk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Copy the published app to the base image and configure the entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickTalk.Api.dll"]
