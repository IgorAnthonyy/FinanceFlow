FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Identity.API/Identity.API.csproj", "src/Services/Identity.API/"]
COPY ["src/Shared/FinanceFlow.Contracts/FinanceFlow.Contracts.csproj", "src/Shared/FinanceFlow.Contracts/"]
COPY ["src/Shared/FinanceFlow.SharedKernel/FinanceFlow.SharedKernel.csproj", "src/Shared/FinanceFlow.SharedKernel/"]
RUN dotnet restore "src/Services/Identity.API/Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity.API"
RUN dotnet build "Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.API.dll"]