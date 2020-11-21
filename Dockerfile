#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ZeroTouchHR.EmployeeBenefits.WorkerService/ZeroTouchHR.EmployeeBenefits.WorkerService.csproj", "ZeroTouchHR.EmployeeBenefits.WorkerService/"]
COPY ["ZeroTouchHR.Services/ZeroTouchHR.Services.csproj", "ZeroTouchHR.Services/"]
COPY ["ZeroTouchHR.Domain/ZeroTouchHR.Domain.csproj", "ZeroTouchHR.Domain/"]
RUN dotnet restore "ZeroTouchHR.EmployeeBenefits.WorkerService/ZeroTouchHR.EmployeeBenefits.WorkerService.csproj"
COPY . .
WORKDIR "/src/ZeroTouchHR.EmployeeBenefits.WorkerService"
RUN dotnet build "ZeroTouchHR.EmployeeBenefits.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZeroTouchHR.EmployeeBenefits.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZeroTouchHR.EmployeeBenefits.WorkerService.dll"]