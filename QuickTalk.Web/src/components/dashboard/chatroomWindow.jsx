import React, { useState, useEffect } from "react";
import { getChatroom, getMessage, sendMessage } from "../../services/api";
import { createSendMessageRequestDto } from "../../dto/MessageDTOs";
import { useAuth } from "../../context/AuthContext";

const ChatWindow = ({ selectedChatroom }) => {
  const { user } = useAuth();

  const [chatroom, setChatroom] = useState(null);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState([]);

  const handleMessageChange = (e) => {
    setMessage(e.target.value);
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (message.trim() !== "") {
      const newMessage = createSendMessageRequestDto(
        message,
        selectedChatroom.chatroomID
      );

      try {
        var response = await sendMessage(newMessage);
        if (response?.data?.messageId) {
          const messageId = response.data.messageId;

          const fetchedMessage = await getMessage(messageId);

          if (fetchedMessage?.data) {
            setMessages((prevMessages) => [
              ...prevMessages,
              fetchedMessage.data,
            ]);
          }
        }

        setMessage("");
      } catch (error) {
        console.error("Error sending message:", error);
      }
    }
  };

  useEffect(() => {
    const fetchChatroom = async () => {
      if (selectedChatroom) {
        try {
          const response = await getChatroom(selectedChatroom.chatroomID);
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

  return (
    <div>
      {selectedChatroom ? (
        <>
          <h2 className="text-2xl font-semibold mb-4">
            {selectedChatroom.name}
          </h2>
          <div className="mb-4 overflow-auto p-4 border border-gray-200 rounded-lg scrollbar-hidden">
            {/* Messages Section */}
            <div className="flex flex-col h-90">
              {messages?.length > 0 ? (
                messages.map((msg) => {
                  const senderID = msg.sender?.userID || "unknownId";
                  const senderUsername = msg.sender?.username || "unknown";
                  const isCurrentUser = String(senderID) === String(user?.id);

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
                          {!isCurrentUser && <span>{senderUsername}</span>}
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
                <p>No messages yet</p>
              )}
            </div>
          </div>

          {/* Input Box */}
          <form onSubmit={handleSendMessage} className="flex mt-4">
            <div className="flex-1">
              <input
                type="text"
                value={message}
                onChange={handleMessageChange}
                placeholder="Type a message..."
                className="w-full p-3 border border-gray-300 rounded-lg"
              />
            </div>
            <button
              type="submit"
              className="ml-2 bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg"
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
