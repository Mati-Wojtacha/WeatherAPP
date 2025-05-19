import React from 'react';

const WeatherMain = ({ icon, temp, description }) => {
  if (!icon || !temp || !description) return null;

  return (
      <div className="weather-main">
        <img
          src={`https://openweathermap.org/img/wn/${icon}@2x.png`}
          alt={description}
        />
        <div className="weather-temp">
          <span>{Math.round(temp)}°C</span>
          <div>{description}</div>
        </div>
      </div>
  );
};

export default WeatherMain;