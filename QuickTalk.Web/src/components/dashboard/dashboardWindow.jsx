import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getDirectChatrooms, getGroupChatrooms, logoutUser } from "../../services/api";
import ChatroomsList from "./chatroomsList";
import ChatroomWindow from "./chatroomWindow";
import { useAuth } from "../../context/AuthContext";
import { IoMdLogOut } from "react-icons/io";

const Dashboard = () => {
  const { user, setUser } = useAuth();
  const navigate = useNavigate();

  const [chatrooms, setChatrooms] = useState([]);
  const [selectedChatroom, setSelectedChatroom] = useState(null);

  useEffect(() => {
    if (!user?.token) return;

    const fetchChatrooms = async () => {
      try {
        const [directResponse, groupResponse] = await Promise.all([
          getDirectChatrooms(user.id),
          getGroupChatrooms(user.id),
        ]);

        setChatrooms([...directResponse.data, ...groupResponse.data]);
      } catch (error) {
        console.error("Error fetching chatrooms:", error);
      }
    };

    setTimeout(fetchChatrooms, 100);
  }, [user]);

  const handleChatroomSelect = (chatroom) => {
    setSelectedChatroom(chatroom);
  };

  const handleLogout = async () => {
    try {
      await logoutUser();

      // Navigate to login and remove user state.
      navigate("/login");
      setUser(null);
      
      console.log("Logged out successfully.");
    } catch (error) {
      console.error("Logout failed:", error);
    }
  }

  return (
    <div className="min-h-screen flex justify-center items-center bg-gradient-to-br from-blue-500 to-purple-600">
      <div className="p-6 rounded-2xl shadow-2xl w-[75vw] h-[95vh] flex flex-col bg-white/10 border-gray-100 backdrop-blur-lg">
        {/* Header Section */}
        <div className="flex justify-between items-center w-full px-4 py-2 bg-gray-100 shadow-md border-b border-gray-300 rounded-t-lg">
          <h1 className="text-xl p-2 font-semibold text-gray-700">Quick Talk</h1>
          <button
            onClick={handleLogout}
            className="p-2 rounded-full shadow hover:bg-indigo-400 transition flex items-center justify-center"
            title="Logout"
          >
            <IoMdLogOut size={16} />
          </button>
        </div>

        {/* Main Content (Fixed the Gap Issue) */}
        <div className="flex-1 grid grid-cols-12 w-full">
          <div className="col-span-3 p-6 shadow-xl border border-gray-300 bg-gray-100 rounded-bl-lg border-r">
            <ChatroomsList
              chatrooms={chatrooms}
              onChatroomSelect={handleChatroomSelect}
              selectedChatroomId={selectedChatroom?.chatroomID}
            />
          </div>

          <div className="col-span-9 p-6 shadow-xl border border-gray-300 bg-gray-100 rounded-br-lg">
            <ChatroomWindow selectedChatroom={selectedChatroom} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
