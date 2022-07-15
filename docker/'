FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8500

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY . . 
WORKDIR /DynamicDataTest.Api 
RUN dotnet restore 
RUN dotnet build -c Release -o /app/build

FROM build as publish
RUN dotnet publish "DynamicDataTest.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicDataTest.Api.dll"]
