import React, { useState } from "react";
import { FiEdit } from "react-icons/fi";
import { MdKeyboardBackspace } from "react-icons/md";
import { format, parseISO } from "date-fns";

const ChatroomsList = ({ chatrooms, onChatroomSelect, selectedChatroomId }) => {
  const [searchText, setSearchText] = useState("");
  const [groupName, setGroupName] = useState("");
  const [activeModal, setActiveModal] = useState(null);

  const filteredChatrooms = chatrooms
    .filter((chatroom) =>
      chatroom.name.toLowerCase().includes(searchText.toLowerCase())
    )
    .sort((a, b) => new Date(b.lastModified) - new Date(a.lastModified));

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-semibold">Chats</h2>
        <div className="relative">
          <div
            className="p-2 rounded-sm hover:bg-gray-200 cursor-pointer flex items-center justify-center"
            onClick={() => setActiveModal("newChat")}
          >
            <FiEdit className="text-xl" size={16} />
          </div>

          {/* New Chat Modal */}
          {activeModal === "newChat" && (
            <>
              <div
                className="fixed inset-0"
                onClick={() => setActiveModal(null)}
              />
              <div className="absolute top-full left-2 mt-2 p-4 rounded-lg w-64 bg-gray-100 border border-gray-300 shadow-lg z-10">
                <h2 className="text-sm font-semibold mb-4">New Chat</h2>
                <button
                  className="w-full py-2 rounded-md border border-gray-200 hover:bg-gray-200 cursor-pointer mb-2"
                  onClick={() => setActiveModal("createGroup")}
                >
                  Group
                </button>
                <button
                  className="w-full py-2 rounded-md border border-gray-200 hover:bg-gray-200 cursor-pointer"
                  onClick={() => setActiveModal("createDirect")}
                >
                  Direct
                </button>
              </div>
            </>
          )}

          {/* Create Group Modal */}
          {activeModal === "createGroup" && (
            <>
              <div
                className="fixed inset-0"
                onClick={() => setActiveModal(null)}
              />
              <div className="absolute top-full left-2 mt-2 p-4 rounded-lg w-64 bg-gray-100 border border-gray-300 shadow-lg z-10">
                <div className="flex justify-between items-center mb-4">
                  <MdKeyboardBackspace
                    size={18}
                    className="cursor-pointer"
                    onClick={() => setActiveModal("newChat")}
                  />
                  <h2 className="text-md font-semibold">Create Group</h2>
                </div>
                <input
                  type="text"
                  className="w-full p-1 pl-2 border border-gray-300 rounded-sm mb-4"
                  placeholder="Group Name"
                  value={groupName}
                  onChange={(e) => setGroupName(e.target.value)}
                />
                <button className="w-full bg-indigo-400 text-white py-2 rounded-md">
                  Create
                </button>
              </div>
            </>
          )}

          {/* Create Direct Modal */}
          {activeModal === "createDirect" && (
            <>
              <div
                className="fixed inset-0"
                onClick={() => setActiveModal(null)}
              />
              <div className="absolute top-full left-2 mt-2 p-4 rounded-lg w-64 bg-gray-100 border border-gray-300 shadow-lg z-10">
                <div className="flex justify-between items-center mb-4">
                  <MdKeyboardBackspace
                    size={18}
                    className="cursor-pointer"
                    onClick={() => setActiveModal("newChat")}
                  />
                  <h2 className="text-md font-semibold">Create Direct Chat</h2>
                </div>
                <input
                  type="text"
                  className="w-full p-1 pl-2 border border-gray-300 rounded-sm mb-4"
                  placeholder="User Name"
                />
                <button className="w-full bg-indigo-400 text-white py-2 rounded-md">
                  Start Chat
                </button>
              </div>
            </>
          )}
        </div>
      </div>
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
            key={chatroom.chatroomID}
            onClick={() => {
              onChatroomSelect(chatroom);
            }}
            className={`cursor-pointer p-2 rounded-lg mb-2 transition-all duration-200 flex flex-col justify-center h-16 ${
              selectedChatroomId === chatroom.chatroomID
                ? "bg-indigo-400 text-white"
                : "bg-gray-100 hover:bg-gray-200 hover:shadow-sm border border-gray-200"
            }`}
          >
            <div className="font-semibold">{chatroom.name}</div>
            <div className="flex justify-between items-center text-sm truncate">
              <span className="truncate max-w-[70%]">
                {chatroom.lastMessage}
              </span>
              <span className="text-[10px]">
                {format(parseISO(chatroom.lastModified), "hh:mm")}
              </span>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ChatroomsList;
