FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApexLawFirm.API.csproj", "./"]
RUN dotnet restore "./ApexLawFirm.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ApexLawFirm.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApexLawFirm.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApexLawFirm.API.dll"]
