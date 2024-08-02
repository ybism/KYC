FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["KYC360/KYC360.csproj", "KYC360/"]
RUN dotnet restore "KYC360/KYC360.csproj"
COPY . .
WORKDIR "/src/KYC360"
RUN dotnet build "KYC360.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KYC360.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KYC360.dll"]
