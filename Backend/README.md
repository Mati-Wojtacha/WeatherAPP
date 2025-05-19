# 🌤️ Backend Aplikacji Pogodowej (.NET Core)

To jest backend aplikacji pogodowej stworzony w technologii ASP.NET Core. Backend odpowiada za komunikację z zewnętrznym API pogodowym OpenWeather, przetwarzanie danych oraz udostępnianie ich aplikacji frontendowej (React) za pomocą REST API.

**Ten plik dotyczy wyłącznie uruchamiania aplikacji backendowej.**

## Wymagania wstępne

Aby uruchomić frontend, potrzebujesz zainstalowanego środowiska **Node.js**:

- [SDK .NET 8.0 lub nowszy]
- (Opcjonalnie) Visual Studio 2022 / Visual Studio Code


Sprawdzenie wersji SDK:

```bash
dotnet --version
```
 ## Konfiguracja aplikacji (appsettings.json)
Upewnij się, że w katalogu projektu backendowego znajduje się plik appsettings.json z poniższą konfiguracją:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "CitiesDb": "Data Source=Cities.db"
  },
  "OpenWeatherSettings": {
    "CitiesURL": "http://bulk.openweathermap.org/sample/city.list.json.gz",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/",
    "ApiKey": "[API_KEY_FROM_OPEN_WEATHER]",
    "CacheExpirationHours": 2
  }
}
```
**Uwaga: Zastąp wartość "[API_KEY_FROM_OPEN_WEATHER]" swoim kluczem API z OpenWeather. aby aplikacja pobierała poprawnie dane z OpenWeather**


 ## Uruchomienie aplikacji
 
Wykonaj poniższe polecenie w katalogu backend/WeatherApi, aby uruchomić aplikację w trybie deweloperskim na porcie 7081

```bash
dotnet run --urls "https://localhost:7081"
```
**Po uruchomieniu aplikacja powinna być dostępna pod adresem: https://localhost:7081/swagger/index.html**

## Jeśli zależności są już zainstalowane, wystarczy użyć:

```bash
npm start
```
**Aplikacja powinna uruchomić się lokalnie pod adresem: http://localhost:3000**
Uwaga przy pierwszym uruchomieniu może być konieczne zaakceptowanie certyfikatu deweloperskiego HTTPS. Jeśli wystąpią problemy, użyj poniższego polecenia:

```bash
dotnet dev-certs https --trust

```

###Pierwsze uruchomienie aplikacji może potrwać dłużej – aplikacja importuje do lokalnej bazy dane z nazwami miast, które są udostępniane przez OpenWeather.###