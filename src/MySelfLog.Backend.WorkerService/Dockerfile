#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MySelfLog.Backend.WorkerService/MySelfLog.Backend.WorkerService.csproj", "MySelfLog.Backend.WorkerService/"]
RUN dotnet restore "MySelfLog.Backend.WorkerService/MySelfLog.Backend.WorkerService.csproj"
COPY . .
WORKDIR "/src/MySelfLog.Backend.WorkerService"
RUN dotnet build "MySelfLog.Backend.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MySelfLog.Backend.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MySelfLog.Backend.WorkerService.dll"]