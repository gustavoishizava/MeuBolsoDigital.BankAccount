FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o publish --no-restore

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/app/newrelic \
CORECLR_PROFILER_PATH=/app/newrelic/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=e4916eead05c80de27fdd2dc93f185796460NRAL \
NEW_RELIC_APP_NAME=ms-bank-accounts

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MBD.BankAccounts.API.dll"]