export const createUserDto = (userID, username, displayName = null) => ({
    userID: userID,
    username: username,
    displayName: displayName,
});
  
export const createUserProfileDto = (userID, username, email, displayName = null, avatarUrl = null) => ({
    userID: userID,
    username: username,
    email: email,
    displayName: displayName,
    avatarUrl: avatarUrl,
});
  
export const createUserUpdateDto = (username = null, email = null, avatarUrl = null) => ({
    username: username,
    email: email,
    avatarUrl: avatarUrl,
});
  
export const createUserChatroomDto = (chatroomID, name, roomType, isPrivate) => ({
    chatroomID: chatroomID,
    name: name,
    roomType: roomType,
    isPrivate: isPrivate,
});
  