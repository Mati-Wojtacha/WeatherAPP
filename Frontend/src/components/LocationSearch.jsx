import React, { useState, useEffect } from 'react';
import { searchCities } from '../services/api';

const LocationSearch = ({ onSearch }) => {
  const [country, setCountry] = useState('PL');
  const [cityQuery, setCityQuery] = useState('Warszawa');
  const [citySuggestions, setCitySuggestions] = useState([]);
  const [isSearching, setIsSearching] = useState(false);
  const [selectedCity, setSelectedCity] = useState(null);
  const [error, setError] = useState(null);


useEffect(() => {
  const timer = setTimeout(() => {
    if (country.length === 2 && cityQuery.length >= 2) {
      setIsSearching(true);
      setError(null);

      searchCities(country, cityQuery)
        .then(data => {
          setCitySuggestions(data);
          setError(null);
          if (data.length === 1) {
            setSelectedCity(data[0]);
          } else {
            setSelectedCity(null);
          }
        })
        .catch(err => {
          if (err?.status === 404) {
            setCitySuggestions([]);
            setSelectedCity(null);
            setError('Nie znaleziono miast.');
          } else {
            console.error('Search error:', err);
            setCitySuggestions([]);
            setSelectedCity(null);
            setError('Wystąpił błąd podczas wyszukiwania.');
          }
        })
        .finally(() => setIsSearching(false));
    }
  }, 500);

  return () => clearTimeout(timer);
}, [country, cityQuery]);


  const handleSubmit = (e) => {
    e.preventDefault();
    if (selectedCity && selectedCity.id) {
      onSearch(selectedCity.id);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="search-form">
  <div className="search-inputs-table-wrapper">
    <div className="form-inputs">
      <div className="form-group">
        <label htmlFor="country">Kraj (kod ISO):</label>
        <input
          id="country"
          type="text" 
          value={country} 
          onChange={(e) => setCountry(e.target.value.toUpperCase())} 
          maxLength="2"
          pattern="[A-Za-z]{2}"
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="city">Wyszukaj miasto:</label>
        <input
          id="city"
          type="text"
          value={cityQuery}
          onChange={(e) => {
            setCityQuery(e.target.value);
            setSelectedCity(null);
          }}
          required
        />
        {isSearching && <span className="searching-indicator">Wyszukiwanie...</span>}
      </div>
    </div>

    <div className="suggestions-table-wrapper">
      {!isSearching && citySuggestions.length > 0 && (
        <table className="suggestions-table">
          <thead>
            <tr>
              <th>Miasto</th>
              <th>Kraj</th>
              <th>Szerokość</th>
              <th>Długość</th>
            </tr>
          </thead>
          <tbody>
            {citySuggestions.map(city => (
              <tr
                key={city.id}
                className={selectedCity?.id === city.id ? 'selected' : ''}
                onClick={() => setSelectedCity(city)}
              >
                <td>{city.name}</td>
                <td>{city.country}</td>
                <td>{city.coord.lat}</td>
                <td>{city.coord.lon}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  </div>

  <button
    type="submit"
    disabled={!selectedCity || isSearching}
    className="full-width-button"
  >
    Pokaż pogodę
  </button>
  {error && <div className="error-message">{error}</div>}
</form>
  );
};

export default LocationSearch;