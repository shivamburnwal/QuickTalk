import React, { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { getToken } from "../utils/auth";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = getToken("authToken");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        setUser({
          id: decoded.nameid,
          username: decoded.unique_name,
          token: token,
        });
      } catch (error) {
        console.error("Invalid token", error);
        localStorage.removeItem("authToken");
      }
    }
  }, []);

  return (
    <AuthContext.Provider value={{ user, setUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);