FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["E-commerceAPI.csproj", "Ecommerce/"]
RUN dotnet restore "Ecommerce/Ecommerce/Ecommerce.csproj"

COPY . .
WORKDIR "/src/Ecommerce"
RUN dotnet publish "Ecommerce.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Ecommerce.dll"]
