import React from 'react';
import { getWindDirection } from '../../utils/windDirection';

const WeatherDetails = ({ data }) => {
  if (!data) return null;

  return (
    <div className="weather-details">
      <div className="detail">
        <span>Odczuwalna: </span>
        <strong>{Math.round(data.main.feels_like)}°C</strong>
      </div>
      <div className="detail">
        <span>Wilgotność: </span>
        <strong>{data.main.humidity}%</strong>
      </div>
      <div className="detail">
        <span>Wiatr: </span>
        <strong>{data.wind.speed} m/s</strong>
      </div>
      <div className="detail">
        <span>Kierunek wiatru: </span>
        <strong>{getWindDirection(data.wind.deg)} ({data.wind.deg}°)</strong>
      </div>
      <div className="detail">
        <span>Zachmurzenie: </span>
        <strong>{data.clouds.all}%</strong>
      </div>
      <div className="detail">
        <span>Widoczność: </span>
        <strong>{data.visibility} m</strong>
      </div>
    </div>
  );
};

export default WeatherDetails;