# Requirements checklist

## Configuration information you'll need

### Configuration step 1: database configuration

- Database server name or IP address
- Database/catalog name
- Database owner user login (the user in the `db_owner` role)
- Database owner user password

### Configuration step 2: mail server configuration

- The administrator's email address - you may want to set up a role address ahead of time so that system emails don't appear to come from your personal address
- Mail server - SMTP server to handle emails
- Mail server port (optional) - by default 25 will be used
- Mail server login (optional) - if you need to authenticate to send email
- Mail server password (optional)

### Configuration step 3: select an initial program configuration

Your final decision is which initial program configuration to choose:

- You can opt to set up with a single reading program that **tracks by books read**
- You can opt to set up with four age-specific reading programs that **track by minutes read**

Once you set up in either configuration you can add or remove programs as you see fit.

**For more information on these options, please review the [planning section](../../introduction/planning) of this manual.**

## Hosting in a Docker environment

Setting up to run the Web site using Docker is much simpler as the environment is entirely contained in the Docker image.

- Ensure you have [Docker installed](https://docs.docker.com/install/) properly using Linux containers. You can verify your install with:
  - The Docker Hello World sample (`docker run --rm hello-world`)
  - The Microsoft ASP.NET Core sample (`docker run --rm microsoft/dotnet-samples`)
- Ensure you have access to a **Microsoft SQL Server version 2012 or newer**
- Ensure that you'll be able to authenticate in **SQL Server authentication mode**
- Confirm that you'll be able to **create a database**
- Ensure that you have a **mail server with an accessible SMTP port or the ability to deliver mail from a service running on the Web server**

## Hosting in a Windows environment

- Ensure you have a Windows server running **Windows Server 2012 or newer**
- Note that for reporting to work you must be running **Windows Server 2012/IIS 8 or later with Web Sockets enabled**
- Ensure your server has the **ASP.NET Core Hosting Bundle** installed
- Confirm that you can **create a new Web site in IIS** on this server
- Confirm that you will be permitted to configure it so that **Web site files can be writable by the Windows user who owns the IIS process (typically the `IIS_IUSRS` group or `DefaultAppPool` user)**.
- Ensure you have access to a **Microsoft SQL Server version 2012 or newer**
- Ensure that you'll be able to authenticate in **SQL Server authentication mode**
- Confirm that you'll be able to **create a database**
- Ensure that you have a **mail server with an accessible SMTP port or the ability to deliver mail from a service running on the Web server**
- The **[latest release of the GRA software](https://github.com/MCLD/greatreadingadventure/releases/latest)** downloaded from GitHub.
