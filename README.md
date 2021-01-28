# Bill Tracker - Backend

## 1. Introduction
This project contains whole Backend part the is required for BillTracker application, including auth, user management and expenses calculations.

## 2. Prerequisites
Everything by default is dockerized, so you do not have to download all unnecessary packages to run the app. In order to contribute, developer can choose how to 
run the app: either locally or in Docker.

1. Docker
    - [Windows with Linux Containers](https://docs.docker.com/docker-for-windows/install/)
    - [Linux](https://docs.docker.com/engine/install/ubuntu/)
    - [Mac](https://docs.docker.com/docker-for-mac/install/)
2. For contributors:
    - For Windows users - [Visual Studio 2019 (at least 16.8.4)](https://visualstudio.microsoft.com/pl/vs/)
    - [.NET 5 (currently v5.0.2)](https://dotnet.microsoft.com/download/dotnet/5.0)
    - [Visual Studio Code](https://code.visualstudio.com/)

## 3. Docker
For local development purposes we use Docker to set up PostgreSQL database and PGAdmin4.

## 4. Build & Run

### Only infrastructure:
Infrastructure elements:
    - PostreSQL
    - PGAdmin4

```bash
docker-compose up -d
```

Endpoint of locally run backend app: http://localhost:5000

### Whole app:
If you want to run the whole infrastracture with application or restart app after introducing some changes.

```bash
docker-compose -f docker-compose.test.yml up -d --build
```

Endpoint of dockerized backend app: http://localhost:5100
