#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 7000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MaziStore.ApiServer.Home/MaziStore.ApiServer.Home.csproj", "MaziStore.ApiServer.Home/"]
RUN dotnet restore "MaziStore.ApiServer.Home/MaziStore.ApiServer.Home.csproj"
COPY . .
WORKDIR "/src/MaziStore.ApiServer.Home"
RUN dotnet build "MaziStore.ApiServer.Home.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "MaziStore.ApiServer.Home.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MaziStore.ApiServer.Home.dll"]