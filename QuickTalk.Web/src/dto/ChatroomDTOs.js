export const createChatroomDto = (chatroomID, name, roomType, users, messages) => ({
    chatroomID: chatroomID,
    name: name,
    roomType: roomType,
    users: users, // Array of UserDTO objects
    messages: messages, // Array of MessageDTO objects
});
  
export const createGroupChatRoomDto = (senderID, name, description, userIDs) => ({
    senderID: senderID,
    name: name,
    description: description,
    userIDs: userIDs, // Array of user IDs
});

export const updateGroupChatDto = (groupName, description) => ({
    groupName: groupName,
    description: description,
});

export const createUserChatroomsViewDto = (chatroomID, name, roomType) => ({
    chatroomID: chatroomID,
    name: name,
    roomType: roomType,
});
