export const getToken = () => {
    return localStorage.getItem('authToken');
};
  
export const setToken = (token) => {
  localStorage.setItem('authToken', token);
};

export const logout = () => {
  localStorage.removeItem('authToken');
};
