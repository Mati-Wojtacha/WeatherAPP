import React from 'react';
import WeatherDetails from './weather_subcomponets/WeatherDetails';
import WeatherMain from './weather_subcomponets/WeatherMain';

const WeatherDisplay = ({ weatherData }) => {
  if (!weatherData || !weatherData.list || !weatherData.city) {
    return <div className="weather-error">Brak danych pogodowych</div>;
  }

  return (
    <div className="weather-container">
      <h2 className="weather-title">
        Prognoza godzinowa – {weatherData.city.name}, {weatherData.city.country}
      </h2>
      <p>
        Współrzędne: {weatherData.city.coord.lat}, {weatherData.city.coord.lon}; Strefa czasowa: UTC {weatherData.city.timezone / 3600 >= 0 ? '+' : ''}{weatherData.city.timezone / 3600}
      </p>

      {weatherData.list.map((item, index) => (
        <div key={index} className="weather-display-box">
          <h3 className="weather-subtitle">{item.dt_txt}</h3>
          <div className="weather-row">
            <WeatherMain icon={item.weather[0].icon} temp={item.main.temp} description={item.weather[0].description} />
            <WeatherDetails data={item} />
          </div>
        </div>
      ))}
    </div>
  );
};


export default WeatherDisplay;