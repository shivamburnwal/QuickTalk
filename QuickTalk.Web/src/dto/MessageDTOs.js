export const createSendMessageRequestDto = (senderUsername, content, chatRoomID) => ({
    senderUsername: senderUsername,
    content: content,
    chatRoomID: chatRoomID,
});
  
export const createMessageDto = (messageID, content, sentAt, sender) => ({
    messageID: messageID,
    content: content,
    sentAt: sentAt,
    sender: sender, // UserDTO object for the sender
});