import axios from 'axios';

const baseApiUrl = import.meta.env.VITE_API_URL;
const api = axios.create({
  baseURL: baseApiUrl,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const registerUser = async (userData) => {
  console.log(userData.Username, userData.Password);
  return await api.post('/Auth/register', userData);
};

export const loginUser = async (loginData) => {
  return await api.post('/Auth/login', loginData);
};



// Other API requests can be added here

export default api;
