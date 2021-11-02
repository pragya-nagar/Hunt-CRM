FROM microsoft/dotnet:2.2-sdk AS build-env
ARG VERSION=0.0.1
WORKDIR /src

COPY . .
WORKDIR /src/Synergy.CRM.Services.Host
RUN dotnet restore
RUN dotnet publish -p:Version=$VERSION --no-restore -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Synergy.CRM.Services.Host.dll"]