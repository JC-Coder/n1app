FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VisitorInfoApi/VisitorInfoApi.csproj", "VisitorInfoApi/"]
RUN dotnet restore "VisitorInfoApi/VisitorInfoApi.csproj"
COPY . .
WORKDIR "/src/VisitorInfoApi"
RUN dotnet build "VisitorInfoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VisitorInfoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VisitorInfoApi.dll"]
