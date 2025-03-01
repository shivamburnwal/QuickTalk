import axios from 'axios';
import { getToken, setToken, removeToken } from '../utils/auth';

// Create base API request.
const baseApiUrl = import.meta.env.VITE_API_URL;
const api = axios.create({
  baseURL: baseApiUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, //Enable sending cookies with requests.
});

// Add the JWT token to the header dynamically for certain API calls
api.interceptors.request.use(
  async (config) => {
    let token = getToken();

    // Skip Authorization for specific endpoints
    const skipUrls = ['/Auth/register', '/Auth/login', '/Auth/refresh-token'];
    if (!skipUrls.includes(config.url)) {
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      } else {
        console.warn("No token found, request may fail.");
      }
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Check and update token if required.
export const setupInterceptor = (navigate) => {
  api.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config;
  
      if(error.response && error.response.status == 401 && !originalRequest._retry){
        
        // Prevent retrying the refresh token request itself
        if (originalRequest.url.includes('/Auth/refresh-token')) {
          console.log("Refresh token is invalid or expired. Logging out...");
          removeToken();
          navigate('/login');
          return Promise.reject(error);
        }

        console.log("Refreshing token...");
        originalRequest._retry = true;
  
        try {
          var refreshResponse = await refreshToken();
          const newToken = refreshResponse.data.token;
  
          // Update new token in local storage
          setToken(newToken);
  
          // Retry the original request with new token
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return api(originalRequest);
        } 
        catch (refreshError) {
          removeToken();
          navigate('/login');
          return Promise.reject(refreshError);
        }
      }
      return Promise.reject(error);
    }
  );
}

// API Callings..
export const registerUser = async (userData) => api.post("/Auth/register", userData);
export const loginUser = async (loginData) => api.post("/Auth/login", loginData);
export const refreshToken = async () => api.post("/Auth/refresh-token");
export const logoutUser = async () => {
  try {
    await api.post("/Auth/logout");
    removeToken();
  } catch (error) {
    console.error("Logout failed:", error);
    throw error;
  }
};

export const getDirectChatrooms = async (userId) => api.get(`/Chatrooms/user/${userId}/Direct`);
export const getGroupChatrooms = async (userId) => api.get(`/Chatrooms/user/${userId}/Group`);
export const getChatroom = async (chatroomId) => api.get(`/Chatrooms/${chatroomId}`);
export const getMessage = async (messageId) => api.get(`/Message/${messageId}`);
export const sendMessage = async (messageData) => api.post("/Message/send", messageData);

// Other API requests can be added here

export default api;
