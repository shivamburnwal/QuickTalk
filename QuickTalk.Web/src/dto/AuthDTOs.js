
export const createLoginDto = (email, password) => ({
    username: email,
    password: password,
});

export const createRegisterDto = (username, email, password) => ({
    username: username,
    email: email,
    password: password,
});