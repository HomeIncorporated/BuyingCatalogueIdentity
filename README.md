# Buying Catalogue Identity - Authentication & authorization service for the NHS Digital Buying Catalogue

## IMPORTANT NOTES!
**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Requirements

- .NET Core Version 3.1
- Docker
- Nodejs Version 12.16.1

> Before you begin please install **.NET Core 3.1**, **Nodejs 12.16.1** & **Docker** on your machine.

## Overview
This application uses **.NET core** to provide an identity service currently implemented with IdentityServer 4 wrapping ASP.NET Identity. It is used to provide token-based authentication and API access control in the Buying Catalogue associated services.

### Project structure
This repository uses **.NET Core**, **Nodejs** and **Docker**.

It contains one endpoint

- account/login
  - Returns a HTML view.

The application is broken down into the following project libraries:

- API 
  - Defines and exposes the available Buying Catalogue Identity endpoints.
- API.UnitTests
  - Contains all of the unit tests for the API project.
- API.IntegrationTests
  - Contains all of the integration tests for the API project.

## Running the Application

To start up the web application, run the following command from the root directory of the repository.

```
docker-compose up -d --build
```

This will start the application in a docker container. You can verify that the service has launched correctly by navigating to the following url via any web browser.

```
http://localhost:8070/account/login
```

### To stop the application 
To stop the application, run the following command from the root directory of the repository, which will stop the service within the docker container.

```
docker-compose down -v
```

### Running the Integration Tests
TO BE COMPLETED