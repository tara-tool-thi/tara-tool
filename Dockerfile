FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/THIARA.csproj ./src/
RUN dotnet restore ./src/THIARA.csproj
COPY src/ ./src/
RUN dotnet publish ./src/THIARA.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "THIARA.dll"]
