# 🌤️ Frontend aplikacji pogodowej (React)

To jest frontend aplikacji pogodowej napisany w **React**. Interfejs użytkownika komunikuje się z backendem opartym na **.NET Core**, który pobiera dane pogodowe z **OpenWeather API**.

**Ten plik dotyczy wyłącznie uruchamiania aplikacji frontendowej.**

## Wymagania wstępne

Aby uruchomić frontend, potrzebujesz zainstalowanego środowiska **Node.js**:

- [Node.js](https://nodejs.org/) – zalecana wersja LTS (Long-Term Support)
- npm – instalowany automatycznie wraz z Node.js

Sprawdź wersje zainstalowanego środowiska:

```bash
node -v
npm -v
```

 ## Pierwsze uruchomienie aplikacji

 **Aby aplikacja frontendowa mogła poprawnie komunikować się z serwerem backendowym, należy odpowiednio skonfigurować plik .env, znajdujący się w katalogu /frontend.**

**W pliku .env należy ustawić adres oraz port API, zgodnie z tym, na jakim działa backend.**
**
 ```env
 REACT_APP_API_BASE_URL=https://localhost:7081/api
 ```
 
Wykonaj poniższe polecenia w katalogu frontend, aby zainstalować zależności i uruchomić aplikację w trybie deweloperskim:

```bash
npm install
npm start
```

## Jeśli zależności są już zainstalowane, wystarczy użyć:

```bash
npm start
```
**Aplikacja powinna uruchomić się lokalnie pod adresem: http://localhost:3000**


## Polecenie do zbudowania aplikacji

```bash
npm run build
```
