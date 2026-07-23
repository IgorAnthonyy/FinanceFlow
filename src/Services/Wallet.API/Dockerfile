FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Wallet.API/Wallet.API.csproj", "src/Services/Wallet.API/"]
COPY ["src/Shared/FinanceFlow.Contracts/FinanceFlow.Contracts.csproj", "src/Shared/FinanceFlow.Contracts/"]
COPY ["src/Shared/FinanceFlow.SharedKernel/FinanceFlow.SharedKernel.csproj", "src/Shared/FinanceFlow.SharedKernel/"]
RUN dotnet restore "src/Services/Wallet.API/Wallet.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Wallet.API"
RUN dotnet build "Wallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.API.dll"]