import React, { useState } from "react";

const ChatroomsList = ({ chatrooms, onChatroomSelect, selectedChatroomId }) => {
  const [searchText, setSearchText] = useState("");

  const filteredChatrooms = chatrooms.filter((chatroom) =>
    chatroom.name.toLowerCase().includes(searchText.toLowerCase())
  );

  return (
    <div>
      <h2 className="text-xl font-semibold mb-4">Chatrooms</h2>
      <div className="relative w-full mb-4">
        <div className="absolute inset-y-0 left-2 flex items-center text-gray-500">
          🔍
        </div>
        <input
          type="text"
          placeholder="Search..."
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          className="w-full p-1 pl-8 pr-2 border border-gray-300 rounded-sm"
        />
      </div>
      <ul>
        {filteredChatrooms.map((chatroom) => (
          <li
            key={chatroom.id}
            onClick={() => {
              onChatroomSelect(chatroom);
            }}
            className={`cursor-pointer p-2 rounded-sm mb-2 ${
              selectedChatroomId === chatroom.chatroomID
                ? "bg-indigo-400 text-white"
                : "bg-gray-100 hover:bg-gray-200"
            }`}
          >
            {chatroom.name}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ChatroomsList;
