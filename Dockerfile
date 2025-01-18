FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Kopiraj projekte i restore zavisnosti
COPY ["E-commerceAPI.csproj", "Ecommerce/"]
RUN dotnet restore "Ecommerce/Ecommerce/Ecommerce.csproj"

# Kopiraj ostatak fajlova i izgradnja aplikacije
COPY . .
WORKDIR "/src/Ecommerce"
RUN dotnet publish "Ecommerce.csproj" -c Release -o /app/publish

# Koristi .NET runtime sliku za pokretanje aplikacije
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

# Postavi promenljive okruženja ako su potrebne (npr. za vezu s bazom)
# ENV ConnectionStrings__DefaultConnection="Host=postgres;Port=5433;Database=Ecommerce;Username=davud;Password=davud"

# Pokreni aplikaciju
ENTRYPOINT ["dotnet", "Ecommerce.dll"]
