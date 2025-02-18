import React, { useState, useEffect } from "react";
import { getDirectChatrooms, getGroupChatrooms } from "../../services/api";
import ChatroomsList from "./chatroomsList";
import ChatWindow from "./chatroomWindow";

const Dashboard = () => {
  const [chatrooms, setChatrooms] = useState([]);
  const [selectedChatroom, setSelectedChatroom] = useState(null);

  useEffect(() => {
    const fetchChatrooms = async () => {
      try {
        // Fetch Direct Chatrooms and Group Chatrooms
        const directResponse = await getDirectChatrooms();
        const groupResponse = await getGroupChatrooms();

        console.log(directResponse, groupResponse);

        // Combine both responses into one list
        const allChatrooms = [...directResponse.data, ...groupResponse.data];

        setChatrooms(allChatrooms); // Set the combined chatrooms list
      } catch (error) {
        console.error("Error fetching chatrooms:", error);
      }
    };

    fetchChatrooms();
  }, []);

  const handleChatroomSelect = (chatroom) => {
    setSelectedChatroom(chatroom);
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-gray-200">
      <div className="p-10 rounded-2xl shadow-2xl w-[70vw] h-[85vh] flex justify-center items-center">
        <div className="grid grid-cols-12 w-full h-full gap-4">
          <div className="col-span-3 p-6 rounded-lg shadow-lg border border-gray-300">
            <ChatroomsList
              chatrooms={chatrooms}
              onChatroomSelect={handleChatroomSelect}
              selectedChatroomId={selectedChatroom?.id}
            />
          </div>

          <div className="col-span-9 p-6 rounded-lg shadow-lg border border-gray-300">
            <ChatWindow selectedChatroom={selectedChatroom} />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
