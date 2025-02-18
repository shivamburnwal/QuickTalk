import React from "react";
import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div className="min-h-screen flex justify-center items-center bg-gray-100">
      <div className="p-10 rounded-2xl shadow-2xl bg-gradient-to-r from-blue-500 via-teal-500 to-blue-800 w-[70vw] h-[85vh] flex flex-col justify-center items-center">
        <div className="text-center">
          <h1 className="text-5xl font-bold mb-6 text-white leading-tight">
            Welcome to QuickTalk
          </h1>
          <p className="text-xl mb-12 text-white opacity-90 mx-auto max-w-2xl text-center">
            QuickTalk is your go-to chat application for seamless communication.
            Whether you want to have direct conversations or join chatrooms,
            QuickTalk makes messaging easy, fast, and secure.
          </p>
          <div className="flex justify-center gap-8">
            <Link
              to="/register"
              className="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg shadow-xl transform hover:scale-105 transition-all duration-300 ease-in-out focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              Register
            </Link>
            <Link
              to="/login"
              className="bg-teal-600 hover:bg-teal-700 text-white font-semibold py-3 px-8 rounded-lg shadow-xl transform hover:scale-105 transition-all duration-300 ease-in-out focus:outline-none focus:ring-2 focus:ring-teal-500"
            >
              Login
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
