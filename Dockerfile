#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /source
COPY ["src/DemoApp.API/DemoApp.API.csproj", "src/DemoApp.API/"]
RUN dotnet restore "src/DemoApp.API/DemoApp.API.csproj"
COPY . .
RUN dotnet build "src/DemoApp.API/DemoApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/DemoApp.API/DemoApp.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApp.API.dll"]