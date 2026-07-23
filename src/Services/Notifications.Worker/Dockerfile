FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Notifications.Worker/Notifications.Worker.csproj", "src/Services/Notifications.Worker/"]
COPY ["src/Shared/FinanceFlow.Contracts/FinanceFlow.Contracts.csproj", "src/Shared/FinanceFlow.Contracts/"]
COPY ["src/Shared/FinanceFlow.SharedKernel/FinanceFlow.SharedKernel.csproj", "src/Shared/FinanceFlow.SharedKernel/"]
RUN dotnet restore "src/Services/Notifications.Worker/Notifications.Worker.csproj"
COPY . .
WORKDIR "/src/src/Services/Notifications.Worker"
RUN dotnet build "Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.Worker.dll"]