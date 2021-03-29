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
For local development purposes the whole infrastructure and WebAPI are contenerized using Docker.

## 4. Build & Run

``` bash
./run.sh [--full-app --tests]
```

Optional parameters:
* --full-app - Runs BillTracker WebAPI (for Frontend devs)
    * _Endpoint of the WebAPI (also Swagger when using browser): http://localhost:5000_
* --tests - Runs all possible tests

## 5. BillTracker elements

Infrastructure elements:
- PostgreSQL
- pgAdmin4
- [Azurite (Azure Storage emulator)](https://docs.microsoft.com/pl-pl/azure/storage/common/storage-use-azurite)

### pgAdmin4
"pgAdmin is the most popular and feature rich Open Source administration and development platform for PostgreSQL, the most advanced Open Source database in the world."

The pgAdmin4 tool can be open in the browser under the link http://localhost:5050. Below are the credentials for the local instance:
- Email: pgadmin4@pgadmin.org
- Password: admin

!!! Warnings Below !!!

_1. Sometimes an error is shown with some message about "CSRF token". Ignore it, and hit the "Login" button until you are logged in._

_2. In case the popup with database password is shown, put "123456", check "Save password" and confirm. This is done once and each time after docker volumes are removed._
