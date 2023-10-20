#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LoadBalancerTask/LoadBalancerAPI.csproj", "LoadBalancerTask/"]
COPY ["LoadBalancer.BLL/LoadBalancer.BLL.csproj", "LoadBalancer.BLL/"]
COPY ["LoadBalancer.DAL/LoadBalancer.DAL.csproj", "LoadBalancer.DAL/"]
RUN dotnet restore "LoadBalancerTask/LoadBalancerAPI.csproj"
COPY . .
WORKDIR "/src/LoadBalancerTask"
RUN dotnet build "LoadBalancerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoadBalancerAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "LoadBalancerAPI.dll"]
