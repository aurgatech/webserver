# Project Overview

This project consists of two main parts:
1. An ASP.NET Core project (`app` folder) that provides web APIs.
2. A Node.js project (`wwwroot` folder) that provides the sign-up/sign-in UI.

## Prerequisites

- [Node.js](https://nodejs.org/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- [IIS](https://www.iis.net/) (for Windows hosting)
- [Nginx](https://nginx.org/) or [Apache](https://httpd.apache.org/) (for Linux hosting)

## Step 1: Build the Node.js Project

### Automatic Build and Copy for Windows, Linux, and macOS

If `npm` is installed, the following scripts can automatically build the Node.js project and copy the built files to the `app/wwwroot` directory for debugging:

- **Windows Users**: Run the batch script:
  ```bash
  .\wwwroot\win.bat
  ```
- **Linux Users**: Run the shell  script:
  ```bash
  ./wwwroot/linux.sh
  ```
- **macOS Users**: Run the shell  script:
  ```bash
  ./wwwroot/osx.sh
  ```
### Manual Build
- Navigate to the `wwwroot` folder:
- Follow the instructions in the `wwwroot/README.md` to build the Node.js project.
- After building, copy the dist folder to app/wwwroot.

## Step 2: Build the ASP.NET Core Project

### Configure the Project
Before building the project, you need to configure the ASP.NET Core project settings in the `appsettings.json` file located in the `app` folder.

### Sample `appsettings.json` Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=aurga.db"
  },
  "EmailServer": "smtp.yourdomain.com",
  "EmailAccount": "account@yourdomain.com",
  "EmailPassword": "smtp_password"
}
```
Configuration Details
`DefaultConnection`: Specifies the SQLite database file location. Update this to your desired database file path.
`EmailServer`: The SMTP server address. Replace smtp.yourdomain.com with your SMTP server's address.
`EmailAccount`: The email account used for sending emails. Replace account@yourdomain.com with your email account.
`EmailPassword`: The password for the email account. Replace smtp_password with your email account's password.

### Automatic Build and Copy for Windows, Linux, and macOS

- Navigate to the app folder:
- Build the ASP.NET Core project in Release configuration:
```bash
dotnet build -c Release
```
After building, the files are generated in `app/bin/Release/netx.0/`. Copy the `wwwroot/dist` folder to this location and rename it to `wwwroot`:

### Debug
Open `aurga.sln` with Visual Studio and debug.

## Step 3: Host the Web Server
### Windows (IIS)
- Open IIS Manager.
- Create a new website and point the physical path to `app/bin/Release/netx.0/`.
- Configure the bindings (e.g., port, hostname).
- Start the website.
### Linux (Nginx or Apache)
- [Apache](https://www.yogihosting.com/aspnet-core-host-apache-linux/)
- [Nginx](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-9.0&tabs=linux-ubuntu)
- [Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-9.0)