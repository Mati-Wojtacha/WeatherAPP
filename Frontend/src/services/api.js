const API_BASE_URL = process.env.REACT_APP_API_BASE_URL 

const defaultFetchOptions = {
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
};

const apiFetch = async (endpoint) => {
  const url = `${API_BASE_URL}${endpoint}`;
  console.log(`[API] GET ${url}`);

  try {
    const response = await fetch(url, defaultFetchOptions);
    return await handleResponse(response);
  } catch (error) {
    console.error(`[API] Fetch error for ${endpoint}:`, error.message);
    throw error; 
  }
};

const handleResponse = async (response) => {
  if (response.ok) {
    return await response.json();
  }

  let errorMessage = `Request failed with status ${response.status}`;

  try {
    const errorBody = await response.json();
    errorMessage = errorBody.message || JSON.stringify(errorBody);
  } catch {
    const errorText = await response.text();
    if (errorText) errorMessage = errorText;
  }

  const error = new Error(errorMessage);
  error.status = response.status;
  throw error;
};

export const fetchWeather = async (cityId) => {
  return await apiFetch(`/weather/${cityId}`);
};

export const fetchActualWeather = async (cityId) => {
  return await apiFetch(`/weather/actual/${cityId}`);
};

export const searchCities = async (countryCode, cityName) => {
  if (!countryCode || !cityName) {
    throw new Error('Country code and city name are required');
  }

  const endpoint = `/city/by-country-and-name?countryCode=${encodeURIComponent(
    countryCode
  )}&nameFilter=${encodeURIComponent(cityName)}`;

  return await apiFetch(endpoint);
};