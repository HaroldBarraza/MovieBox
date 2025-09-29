FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# Copiar el resto del código
COPY . .
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MOVIEBOX.dll", "--urls", "http://0.0.0.0:8080"]