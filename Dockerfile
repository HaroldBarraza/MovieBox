FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY *.csproj .
RUN dotnet restore MovieBox.csproj
COPY . .
RUN dotnet publish MovieBox.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
CMD dotnet MOVIEBOX.dll --urls "http://0.0.0.0:$PORT"
