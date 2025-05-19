import React, { useState, useEffect } from 'react';
import {fetchWeather, fetchActualWeather} from './services/api';
import LocationSearch from './components/LocationSearch';
import WeatherDisplay from './components/WeatherDisplay';
import ActualWeatherDisplay from './components/ActualWeatherDisplay';
import Loader from './components/Loader';
import './App.css';

function App() {
  const [weather, setWeather] = useState(null);
  const [actualWeather, setActualWeather] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const loadWeather = async (cityId) => {
    setLoading(true);
    setError(null);
    setWeather(null);
    setActualWeather(null);

    try {
      const forecastData = await fetchWeather(cityId);
      const actualData = await fetchActualWeather(cityId);

      setWeather(forecastData);
      setActualWeather(actualData);
    } catch (err) {
      if (err?.status === 404) {
        setError('Nie znaleziono pogody dla tego miasta.');
      } else {
        setError('Wystąpił błąd serwera.');
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadWeather(6695624);
  }, []);

return (
  <div className="app">
    {actualWeather && <ActualWeatherDisplay actualWeatherData={actualWeather} />}
    <div className="search-section">
      <LocationSearch onSearch={loadWeather} />
    </div>
    <div className="content-container">
      {loading && <Loader />}
      {error && <div className="error-message">{error}</div>}
      {weather && !loading && <WeatherDisplay weatherData={weather} />}
    </div>
  </div>
);
}

export default App;