#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Culinary-Assistant-Notifications/Culinary-Assistant-Notifications.csproj", "Culinary-Assistant-Notifications/"]
RUN dotnet restore "./Culinary-Assistant-Notifications/Culinary-Assistant-Notifications.csproj"
COPY . .
WORKDIR "/src/Culinary-Assistant-Notifications"
RUN dotnet build "./Culinary-Assistant-Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Culinary-Assistant-Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Culinary-Assistant-Notifications.dll"]