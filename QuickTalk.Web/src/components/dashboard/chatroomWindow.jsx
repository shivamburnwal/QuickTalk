import React, { useState, useEffect, useRef } from "react";
import { getChatroom, getMessage, sendMessage } from "../../services/api";
import { createSendMessageRequestDto } from "../../dto/MessageDTOs";
import { useAuth } from "../../context/AuthContext";
import { format, isToday, isYesterday, parseISO } from "date-fns";

const ChatWindow = ({ selectedChatroom }) => {
  const { user } = useAuth();
  const messagesEndRef = useRef(null);

  const [chatroom, setChatroom] = useState(null);
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState([]);

  const groupMessagesByDate = (messages) => {
    const grouped = {};
    messages.forEach((msg) => {
      const dateKey = format(parseISO(msg.sentAt), "yyyy-MM-dd");
      if (!grouped[dateKey]) grouped[dateKey] = [];
      grouped[dateKey].push(msg);
    });
    return grouped;
  };

  const groupedMessages = groupMessagesByDate(messages);

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
  }, [selectedChatroom]);

  {/* Separate useEffect for scrolling after messages update */}
  useEffect(() => {
    if (messages.length > 0) {
      setTimeout(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
      }, 100);
    }
  }, [messages]);

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
              { ...fetchedMessage.data.message },
            ]);
          }
        }

        setMessage("");
      } catch (error) {
        console.error("Error sending message:", error);
      }
    }
  };

  return (
    <div className="flex flex-col h-[65vh]">
      {selectedChatroom ? (
        <>
          <h2 className="text-xl font-semibold text-center shrink-0 mb-4">
            {selectedChatroom.name}
          </h2>

          {/* Messages Section */}
          <div className="flex-1 overflow-y-auto p-3 border border-gray-200 rounded-lg scrollbar-hidden">
            {Object.keys(groupedMessages).map((dateKey) => {
              const messagesOnDate = groupedMessages[dateKey];
              const formattedDate = isToday(parseISO(dateKey))
                ? "Today"
                : isYesterday(parseISO(dateKey))
                ? "Yesterday"
                : format(parseISO(dateKey), "dd MMM yyyy");

              return (
                <div key={dateKey}>
                  {/* Date Separator */}
                  <div className="text-center text-gray-500 my-4 text-xs font-medium">
                    {formattedDate}
                  </div>

                  {messagesOnDate.map((msg, index) => {
                    const senderID = msg.sender?.userID || "unknownId";
                    const senderUsername = msg.sender?.username || "unknown";
                    const isCurrentUser = String(senderID) === String(user?.id);

                    {/* Store old message data to update username print logic. */}
                    const previousMessage = messagesOnDate[index - 1];
                    const isSameSender = previousMessage?.sender?.userID === senderID;

                    return (
                      <div
                        key={msg.messageID}
                        className={`flex mb-1 ${
                          isCurrentUser ? "justify-end" : "justify-start"
                        }`}
                      >
                        <div
                          className={`px-3 py-1.5 max-w-xs rounded-md text-sm relative ${
                            isCurrentUser
                              ? "bg-green-200 text-black rounded-br-none"
                              : "bg-white text-black rounded-bl-none"
                          }`}
                        >
                          {/* Sender Name for Received Messages */}
                          {!isCurrentUser && !isSameSender && (
                            <div className="text-xs font-medium text-gray-500">
                              {senderUsername}
                            </div>
                          )}

                          {/* Message Content and Time */}
                          <div className="flex items-end">
                            <span>{msg.content}</span>
                            <span className="text-[10px] text-gray-500 ml-8">
                              {format(parseISO(msg.sentAt), "hh:mm")}
                            </span>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              );
            })}

            {/* Scrolling reference to  the newest messages of chatroom. */}
            <div ref={messagesEndRef} />
          </div>

          {/* Input Box */}
          <form
            onSubmit={handleSendMessage}
            className="flex mt-3 shrink-0 pt-2"
          >
            <input
              type="text"
              value={message}
              onChange={handleMessageChange}
              placeholder="Type a message..."
              className="flex-1 p-2 text-sm border border-gray-300 rounded-lg"
            />
            <button
              type="submit"
              className="ml-2 bg-blue-500 hover:bg-blue-600 text-white px-3 py-1.5 rounded-lg text-sm"
            >
              Send
            </button>
          </form>
        </>
      ) : (
        <div className="flex items-center justify-center h-full text-gray-500">
          Select a chatroom to start chatting
        </div>
      )}
    </div>
  );
};

export default ChatWindow;
