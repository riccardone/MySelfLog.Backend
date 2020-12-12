#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["MySelfLog.Host/MySelfLog.Host.csproj", "MySelfLog.Host/"]
RUN dotnet restore "MySelfLog.Host/MySelfLog.Host.csproj"
COPY . .
WORKDIR "/src/MySelfLog.Host"
RUN dotnet build "MySelfLog.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MySelfLog.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MySelfLog.Host.dll"]