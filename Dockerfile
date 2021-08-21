#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY ["./src/MySelfLog.Backend.Host/MySelfLog.Backend.Host.csproj", "MySelfLog.Backend.Host/"]
COPY ["./src/MySelfLog.Backend.Adapter/MySelfLog.Backend.Adapter.csproj", "MySelfLog.Backend.Adapter/"]
COPY ["./src/MySelfLog.Backend.Domain/MySelfLog.Backend.Domain.csproj", "MySelfLog.Backend.Domain/"]
RUN dotnet restore "MySelfLog.Backend.Host/MySelfLog.Backend.Host.csproj"
COPY . .
RUN dotnet build "./src/MySelfLog.Backend.Host/MySelfLog.Backend.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/MySelfLog.Backend.Host/MySelfLog.Backend.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MySelfLog.Backend.Host.dll"]