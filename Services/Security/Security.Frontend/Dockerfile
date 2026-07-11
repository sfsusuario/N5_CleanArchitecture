# --- build stage -----------------------------------------------------------
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies first so this layer is cached unless package*.json changes.
COPY package.json package-lock.json* ./
RUN npm install

COPY . .

# Vite bakes VITE_* env vars into the JS bundle at build time (not runtime), so it must be
# passed as a build ARG — setting it as a container `environment:` var later would do nothing.
ARG VITE_API_BASE_URL=http://localhost:5080
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# --- runtime stage -----------------------------------------------------------
FROM nginx:1.27-alpine AS runtime

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

# docker build -t security_frontend --build-arg VITE_API_BASE_URL=http://localhost:5080 .
# docker run -p 3000:80 security_frontend
