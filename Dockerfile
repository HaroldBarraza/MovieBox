FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY *.csproj .
RUN dotnet restore MOVIEBOX.csproj
COPY . .
RUN dotnet publish MOVIEBOX.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
CMD dotnet MOVIEBOX.dll --urls "http://0.0.0.0:$PORT"
