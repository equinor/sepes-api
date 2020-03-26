FROM node:10-alpine as builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
ENV PORT=8080
ENV NODE_ENV=production
RUN npm run build
EXPOSE 8080
CMD ["npm", "start"]