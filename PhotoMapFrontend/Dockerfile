FROM node:20-alpine

WORKDIR /app
COPY . .

RUN npm install
RUN npm run build

# Раздача билдов через Vite dev (если нужно SSR/hmr)
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host"]
