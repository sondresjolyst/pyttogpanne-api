FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:12b1135f5f8b3c0a214f2a680ff2033dff4f684f445d6c697d9524ceeb303586 AS base
WORKDIR /app
EXPOSE 7297
USER app

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:bef136a371963aaa46ec97882845ef6f54dec15728bb4b128b1c2a400ffbc9b4 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["pyttogpanne-api.csproj", "."]
RUN dotnet restore "./pyttogpanne-api.csproj"
COPY . .
RUN dotnet build "./pyttogpanne-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./pyttogpanne-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="http://*:7297"
USER app
ENTRYPOINT ["dotnet", "pyttogpanne-api.dll"]
