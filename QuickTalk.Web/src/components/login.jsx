import React, { useState } from 'react';
import { setToken } from '../utils/auth';
import { loginUser } from '../services/api';
import { createLoginDto } from '../dto/AuthDTOs'
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from "jwt-decode";
import { useAuth } from '../context/AuthContext';

const LoginPage = () => {
  const [formData, setFormData] = useState({ email: '', password: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { setUser } = useAuth();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const requestData = createLoginDto(formData.email, formData.password);
      const response = await loginUser(requestData);
      console.log(response);
      
      // Decode token and update user immediately
      const decoded = jwtDecode(response.data.token);
      setUser({
        id: decoded.nameid,
        username: decoded.unique_name,
        token: response.data.token,
      });
      setToken(response.data.token);

      navigate('/dashboard');
    } catch (err) {
      console.error('Login failed:', err);
      setError('Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-gray-100">
      <div className="bg-white p-8 rounded-xl shadow-lg w-full max-w-sm">
        <h2 className="text-2xl font-bold text-center mb-6">Welcome Back</h2>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full py-2 px-4 mt-1 border border-gray-300 rounded-lg shadow-sm text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          <div className="mb-4">
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">Password</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="w-full py-2 px-4 mt-1 border border-gray-300 rounded-lg shadow-sm text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          {error && <p className="text-red-500 text-sm text-center">{error}</p>} {/* Display error message */}
          <button
            type="submit"
            disabled={loading}
            className={`w-full mt-3 py-3 ${loading ? 'bg-gray-400' : 'bg-blue-500 hover:bg-blue-600'} text-white font-semibold rounded-lg shadow-md focus:outline-none focus:ring-2 focus:ring-blue-500`}
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
        <div className="mt-4 text-center">
          <p className="text-sm text-gray-600">Don't have an account? <a href="/register" className="text-blue-500 hover:text-blue-700">Register</a></p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
