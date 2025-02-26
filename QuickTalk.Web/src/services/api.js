import axios from 'axios';
import { getToken } from '../utils/auth';

// Function to get the JWT token from local storage
const getAuthToken = () => getToken();

// Create base API request.
const baseApiUrl = import.meta.env.VITE_API_URL;
const api = axios.create({
  baseURL: baseApiUrl,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add the JWT token to the header dynamically for certain API calls
api.interceptors.request.use(
  (config) => {
    const token = getAuthToken();
    if (token && config.url !== '/Auth/login' && config.url !== '/Auth/register') {  // Only add token for API calls that need it
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);


// API Callings..
export const registerUser = async (userData) => {
  return await api.post('/Auth/register', userData);
};

export const loginUser = async (loginData) => {
  return await api.post('/Auth/login', loginData);
};

export const getDirectChatrooms = async (userId) => {
  return await api.get(`/Chatrooms/user/${userId}/Direct`);
};

export const getGroupChatrooms = async (userId) => {
  return await api.get(`/Chatrooms/user/${userId}/Group`);
};

export const getChatroom = async (chatroomId) => {
  return await api.get(`/Chatrooms/${chatroomId}`);
};

export const getMessage = async (messageId) => {
  return await api.get(`/Message/${messageId}`)
}

export const sendMessage = async (messageData) => {
  return await api.post('/Message/send', messageData);
};


// Other API requests can be added here

export default api;
