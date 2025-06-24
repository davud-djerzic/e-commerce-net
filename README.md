# 🛒 E-commerce API (.NET 8)

Ovo je backend REST API za e-commerce sistem razvijen u ASP.NET Core 8.0. 
Sistem podržava korisnike, proizvode, kategorije, narudžbe, JWT autentifikaciju, keširanje sa Redisom, email notifikacije i generisanje PDF izvještaja. 
PostgreSQL se koristi kao baza podataka.

## ✨ Tehnologije

- ASP.NET Core 8.0 Web API  
- Entity Framework Core  
- PostgreSQL  
- Redis (keširanje)  
- JWT autentifikacija i autorizacija  
- SendGrid (slanje emailova)  
- iTextSharp (PDF generisanje)  
- Docker (za bazu)  
- Swagger (API dokumentacija)  

---

## 📁 Struktura projekta

Ecommerce/
├── Controllers/
├── DTOs/
├── Models/
├── Services/
├── Repositories/
├── Data/
├── Migrations/
├── Helpers/
├── Ecommerce.csproj
└── Program.cs

---

## 🧑‍💼 Funkcionalnosti

- ✅ Registracija i login korisnika (JWT)
- 🔐 Zaštićeni API rute za korisnike i administratore
- 🗂️ Upravljanje kategorijama i proizvodima
- 📦 Kreiranje narudžbi sa više proizvoda
- 🧾 Pregled, izmjena i brisanje narudžbi
- 💬 Slanje potvrda putem emaila (SendGrid)
- 📄 Generisanje PDF izvještaja narudžbi
- 🚀 Redis keširanje za optimizaciju učitavanja
- 🐳 Docker za pokretanje baze

---

## ▶️ Pokretanje projekta

1. **Kloniraj repozitorij**
git clone https://github.com/davud-djerzic/e-commerce-net.git
cd e-commerce-net
