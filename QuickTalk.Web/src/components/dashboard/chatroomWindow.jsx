import React, { useState, useEffect } from "react";
import { getChatroom, sendMessage } from "../../services/api";
import { createSendMessageRequestDto } from "../../dto/MessageDTOs";

const ChatWindow = ({ selectedChatroom }) => {
  const [chatroom, setChatroom] = useState(null);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    const fetchChatroom = async () => {
      if (selectedChatroom) {
        try {
          const response = await getChatroom(selectedChatroom.chatroomID);
          console.log(response, messages);
          setChatroom(response.data);
          setMessages(response.data.messages);
        } catch (error) {
          console.error("Error fetching chatroom:", error);
          setMessages([]);
        }
      }
    };

    fetchChatroom();
  }, [selectedChatroom]); // Fetch chatroom whenever selectedChatroom changes

  const handleMessageChange = (e) => {
    setMessage(e.target.value);
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (message.trim() !== "") {
      const newMessage = createSendMessageRequestDto(
        (senderUsername = "currentUsername"),
        (content = message),
        (chatRoomID = selectedChatroom.id)
      );

      try {
        await sendMessage(newMessage);
        setMessages((prevMessages) => [...prevMessages, newMessage]);
        setMessage("");
      } catch (error) {
        console.error("Error sending message:", error);
      }
    }
  };

  return (
    <div>
      {selectedChatroom ? (
        <>
          <h2 className="text-2xl font-semibold mb-4">
            {selectedChatroom.name}
          </h2>
          <div className="mb-4 h-64 overflow-y-auto p-4 border border-gray-200 rounded-lg">
            {/* Messages Section */}
            {messages?.length > 0 ? (
              messages.map((msg) => {
                const isCurrentUser = msg.sender.username === "shivam";

                return (
                  <div
                    key={msg.messageID}
                    className={`mb-4 flex ${
                      isCurrentUser ? "justify-end" : "justify-start"
                    }`}
                  >
                    <div
                      className={`p-3 max-w-sm rounded-lg ${
                        isCurrentUser
                          ? "bg-blue-500 text-white"
                          : "bg-gray-200 text-black"
                      }`}
                    >
                      <div className="text-xs font-medium">
                        {msg.sender.username} || {" "}
                        {new Date(msg.sentAt).toLocaleTimeString([], {
                          hour: "2-digit",
                          minute: "2-digit",
                        })}
                      </div>
                      <div className="mt-1">{msg.content}</div>
                    </div>
                  </div>
                );
              })
            ) : (
              <div className="text-center text-gray-500">No messages yet</div>
            )}
          </div>

          {/* Input Box */}
          <form onSubmit={handleSendMessage} className="flex mt-4">
            <input
              type="text"
              value={message}
              onChange={handleMessageChange}
              placeholder="Type a message..."
              className="w-full p-3 border border-gray-300 rounded-lg"
            />
            <button
              type="submit"
              className="ml-2 bg-blue-500 text-white px-4 py-2 rounded-lg"
            >
              Send
            </button>
          </form>
        </>
      ) : (
        <div className="text-center text-gray-500">
          Select a chatroom to start chatting
        </div>
      )}
    </div>
  );
};

export default ChatWindow;
