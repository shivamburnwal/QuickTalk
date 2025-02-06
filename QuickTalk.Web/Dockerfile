# Use node image for building the frontend
FROM node:18 AS build
WORKDIR /app

# Copy package.json and yarn.lock from QuickTalk.Web directory (relative to Dockerfile location) and install dependencies
COPY package.json yarn.lock ./
RUN yarn install

# Copy all frontend files from the QuickTalk.Web directory into the container's working directory
COPY . ./

# Build the React app
RUN yarn build

# Use nginx to serve the React build
FROM nginx:alpine

# Copy the build to nginx's public folder
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
