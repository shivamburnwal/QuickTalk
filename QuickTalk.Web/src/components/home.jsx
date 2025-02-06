import React from 'react';
import { Link } from 'react-router-dom';

const Home = () => {
  return (
    <div className="min-h-screen bg-gradient-to-r from-blue-500 via-teal-500 to-blue-800 text-white flex flex-col justify-center items-center py-16 px-4">
      <div className="text-center max-w-2xl mx-auto">
        <h1 className="text-4xl font-bold mb-4">Welcome to QuickTalk</h1>
        <p className="text-xl mb-8">QuickTalk is your go-to chat application for seamless communication. Whether you want to have direct conversations or join chatrooms, QuickTalk makes messaging easy, fast, and secure.</p>
        <div className="flex justify-center gap-8">
          <Link
            to="/register"
            className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-6 rounded-lg shadow-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            Register
          </Link>
          <Link
            to="/login"
            className="bg-teal-600 hover:bg-teal-700 text-white font-semibold py-3 px-6 rounded-lg shadow-md focus:outline-none focus:ring-2 focus:ring-teal-500"
          >
            Login
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Home;
