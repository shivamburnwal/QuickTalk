import React, { useState, useEffect } from "react";
import { getDirectChatrooms, getGroupChatrooms } from "../../services/api";
import ChatroomsList from "./chatroomsList";
import ChatroomWindow from "./chatroomWindow";
import { useAuth } from "../../context/AuthContext";

const Dashboard = () => {
  const { user } = useAuth();

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

  return (
    <div className="min-h-screen flex justify-center items-center bg-gradient-to-br from-blue-500 to-purple-600">
      <div className="p-10 rounded-2xl shadow-2xl w-[75vw] h-[90vh] flex justify-center items-center backdrop-blur-lg bg-white/10">
        <div className="grid grid-cols-12 w-full h-full gap-2">
          <div className="col-span-3 p-6 rounded-lg shadow-xl border border-gray-100 bg-gray-100">
            <ChatroomsList
              chatrooms={chatrooms}
              onChatroomSelect={handleChatroomSelect}
              selectedChatroomId={selectedChatroom?.chatroomID}
            />
          </div>

          <div className="col-span-9 p-6 rounded-lg shadow-xl border border-gray-100 bg-gray-100">
            <ChatroomWindow selectedChatroom={selectedChatroom} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
