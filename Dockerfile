FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore MOVIEBOX.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish MOVIEBOX.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MOVIEBOX.dll", "--urls", "http://0.0.0.0:8080"]