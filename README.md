# Bill Tracker - Backend

## 1. Introduction
This project contains all logic of Bill Tracker application, including auth, user management and expenses calculations.

## 2. Prerequisites
1. [.NET 5 (currently v5.0.2)](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Docker
    - [Windows with Linux Containers](https://docs.docker.com/docker-for-windows/install/)
    - [Linux](https://docs.docker.com/engine/install/ubuntu/)
    - [Mac](https://docs.docker.com/docker-for-mac/install/)
3. Nice to have:
    - For Windows users - [Visual Studio 2019 (at least 16.8.4)](https://visualstudio.microsoft.com/pl/vs/)
    - [Visual Studio Code](https://code.visualstudio.com/)

## 3. Docker
For local development purposes we use Docker to set up PostgreSQL database and PGAdmin4.

## 3. Build & Run

### Only infrastructure:
Infrastructure elements:
    - PostreSQL
    - PGAdmin4

```bash
docker-compose up -d
```

### Whole app:
If you want to run the whole infrastracture with application or restart app after introducing some changes.

```bash
docker-compose -f docker-compose.test.yml up -d --build
```