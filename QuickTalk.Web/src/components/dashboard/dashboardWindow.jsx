import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getDirectChatrooms, getGroupChatrooms, logoutUser } from "../../services/api";
import ChatroomsList from "./chatroomsList";
import ChatroomWindow from "./chatroomWindow";
import { useAuth } from "../../context/AuthContext";
import { IoMdLogOut } from "react-icons/io";
import * as signalR from "@microsoft/signalr";

const Dashboard = () => {
  const { user, setUser } = useAuth();
  const navigate = useNavigate();

  const [chatrooms, setChatrooms] = useState([]);
  const [selectedChatroom, setSelectedChatroom] = useState(null);
  const [chatroomsUpdated, setChatroomsUpdated] = useState(false);

  // state to store new messages for each chatroom
  const [receivedMessages, setReceivedMessages] = useState({});
  const [connection, setConnection] = useState(null);
  const [unreadCounts, setUnreadCounts] = useState({});

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
  }, [user, chatroomsUpdated]);

  useEffect(() => {
    if (!user?.token) return;
  
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_URL + "/chatHub", {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();
  
    newConnection
      .start()
      .then(() => {
        console.log("Connected to SignalR");
        chatrooms.forEach((chatroom) => {
          newConnection
            .invoke("JoinChatroom", chatroom.chatroomID.toString())
            .then(() => console.log(`Joined chatroom: ${chatroom.chatroomID}`))
            .catch((err) => console.error("JoinChatroom error:", err));
        });
      })
      .catch((err) => console.error("SignalR Connection Error:", err));
  
    newConnection.on("ReceiveMessage", (chatroomId, sender, newMessage) => {
      setReceivedMessages((prev) => ({
        ...prev,
        [chatroomId]: [
          ...(prev[chatroomId] || []),
          { sender, content: newMessage, sentAt: new Date().toISOString() },
        ],
      }));
  
      setUnreadCounts((prevCounts) => ({
        ...prevCounts,
        [chatroomId]: (prevCounts[chatroomId] || 0) + 1,
      }));
  
      setChatroomsUpdated((prev) => !prev);
    });
  
    setConnection(newConnection);
  
    return () => {
      newConnection
        .stop()
        .catch((err) => console.error("Error stopping connection:", err));
    };
  }, [chatrooms]);

  const handleChatroomSelect = (chatroom) => {
    setSelectedChatroom(chatroom);

    // Mark messages as "seen" (reset count)
    setUnreadCounts((prev) => ({
      ...prev,
      [chatroom.chatroomID]: 0,
      [selectedChatroom?.chatroomID]: 0,
    }));
  };

  const handleLogout = async () => {
    try {
      if (connection) {
        await connection.stop();
        console.log("SignalR connection stopped.");
      }

      await logoutUser();

      // Navigate to login and remove user state.
      navigate("/login");
      setUser(null);

      console.log("Logged out successfully.");
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-gradient-to-br from-blue-500 to-purple-600">
      <div className="p-6 rounded-2xl shadow-2xl w-[75vw] h-[95vh] flex flex-col bg-white/10 border-gray-100 backdrop-blur-lg">
        {/* Header Section */}
        <div className="flex justify-between items-center w-full px-4 py-2 bg-gray-100 shadow-md border-b border-gray-300 rounded-t-lg">
          <h1 className="text-2xl p-2 font-semibold text-gray-700">Quick Talk</h1>
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
              unreadCounts={unreadCounts}
            />
          </div>

          <div className="col-span-9 p-6 shadow-xl border border-gray-300 bg-gray-100 rounded-br-lg">
            <ChatroomWindow
              selectedChatroom={selectedChatroom}
              receivedMessages={receivedMessages}
              setReceivedMessages={setReceivedMessages}
              connection={connection}
              setChatroomsUpdated={setChatroomsUpdated}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
