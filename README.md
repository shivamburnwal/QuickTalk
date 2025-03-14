# QuickTalk – Real-Time Chat Application

Welcome to **QuickTalk**! This project is a **real-time messaging platform** built with **React and .NET Core**, enabling seamless user communication. It supports **instant messaging, secure authentication, and low-latency interactions**, ensuring a smooth and engaging chatting experience. Whether you're looking to **connect with friends, collaborate with teams, or build a scalable messaging system**, QuickTalk has you covered.

## Features

- **Real-Time Messaging:** Instantly send and receive messages using **SignalR/WebSockets**, ensuring a seamless chat experience with minimal latency.
- **Secure Authentication:** Built with **JWT-based authentication**, QuickTalk ensures user data and conversations are safe from unauthorized access.
- **Scalable Backend:** Powered by **.NET Core Web API**, the application supports a robust and efficient architecture, capable of handling multiple users simultaneously.
- **Responsive Design:** Designed to work flawlessly across **desktop and mobile devices**, ensuring an optimal experience on all screen sizes.
- **Chatrooms & Conversations:** Supports **direct messaging** for private chats and **group chat functionality** for collaborative discussions.

## Screenshots

### Home Page

The **Home Page** is the gateway to QuickTalk. It provides users with an intuitive interface to **log in or register** for an account. From here, users can access the chat platform and start engaging in real-time conversations.

![Home Page](/Screenshots/HomePage.png)

### Register User

To start using QuickTalk, new users must create an account. The **Register Page** ensures a smooth onboarding process, guiding users to provide their credentials in a secure manner. The form includes **input validation** to ensure user data is properly formatted and meets security standards.

<p align="center">
  <img src="/Screenshots/Register.png" alt="Register Page" width="40%">
</p>

### Login User

Once registered, users can log in through the **Login Page**, which provides **secure authentication** using **JWT tokens**. This guarantees that only authorized users can access the chat system. The login process is designed to be fast and efficient, with **error handling** to assist users in case of incorrect credentials.

<p align="center">
  <img src="/Screenshots/Login.png" alt="Login Page" width="35%">
</p>

### Dashboard Window

The **Dashboard** serves as the central hub for QuickTalk. Here, users can **access their conversations**, **view active chats**, and **send real-time messages**. With a clean and responsive design, the dashboard ensures a smooth chatting experience. Users can also manage their profile settings and chat preferences.

![Dashboard](/Screenshots/UserDashboard.png)

## Getting Started

Follow these steps to set up and run QuickTalk locally on your machine:

1. **Clone this repository** to your local machine using Git.
2. **Navigate to the project directory** using your terminal.
3. **Install dependencies** and run the project.

#### Backend (ASP.NET Core API)

```bash
# Navigate to backend folder
cd QuickTalk.Api

# Run the API
dotnet run
```

#### Frontend (Using Yarn)

```bash
# Navigate to backend folder
cd QuickTalk.Web

# Install dependencies
yarn install

# Launch the app in development mode
yarn run dev
```

4. Access [https://localhost:5173](https://localhost:5173) in your preferred web browser to explore the app's functionality.

## Feedback

Feedback and contributions! Should you have any suggestions or encounter any issues while utilizing the app, kindly [raise an issue](https://github.com/shivamburnwal/QuickTalk/issues) on GitHub.

Happy movie browsing!
