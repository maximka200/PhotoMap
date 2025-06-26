# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
COPY ./PhotoMapAPI/wwwroot /app/wwwroot
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "PhotoMapAPI.dll"]

