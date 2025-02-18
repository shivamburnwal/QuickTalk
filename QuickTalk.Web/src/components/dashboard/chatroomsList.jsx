import React from 'react';

const ChatroomsList = ({ chatrooms, onChatroomSelect, selectedChatroomId }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold mb-4">Chatrooms</h2>
      <ul>
        {chatrooms.map((chatroom) => (
          <li
            key={chatroom.id}
            onClick={() => onChatroomSelect(chatroom)}
            className={`cursor-pointer p-2 rounded-lg mb-2 hover:bg-blue-100 ${
              selectedChatroomId === chatroom.id ? 'bg-blue-200' : ''
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
