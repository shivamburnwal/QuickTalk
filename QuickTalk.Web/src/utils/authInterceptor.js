import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { setupInterceptor } from "../services/api";

const AuthInterceptor = () => {
  const navigate = useNavigate();

  useEffect(() => {
    setupInterceptor(navigate);
  }, [navigate]);

  return null;
};

export default AuthInterceptor;
