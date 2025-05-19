import React from 'react';

import WeatherMain from './weather_subcomponets/WeatherMain';
import WeatherDetails from './weather_subcomponets/WeatherDetails';


const ActualWeatherDisplay = ({ actualWeatherData }) => {
  if (!actualWeatherData) return null;

  return (
    <div className="weather-sticky">
      <h2 className="weather-title">Aktualna pogoda – {actualWeatherData.name}</h2>
      <div className="weather-row">
        <WeatherMain icon={actualWeatherData.weather[0].icon} temp={actualWeatherData.main.temp} description={actualWeatherData.weather[0].description} />
        <WeatherDetails data={actualWeatherData} />
      </div>
    </div>
  );
};

export default ActualWeatherDisplay;
