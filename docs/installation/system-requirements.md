# System requirements

The GRA requires the following services to run. These services may all be hosted on the same machine or may be hosted on separate machines if desired. As an alternative to installing the GRA directly in a Web server environment, the GRA can be run from a [Docker container](https://www.docker.com/) using our [official Docker images](https://hub.docker.com/r/mcld/gra/) (also [available from the GitHub Container Registry](https://ghcr.io/mcld/gra)).

## Web site

### Web server requirements

Version 4 of the GRA runs in the [Microsoft .NET 5.0](https://dotnet.microsoft.com/download) runtime environment. Downloads of the runtime environment are available from Microsoft for the following operating systems:

- macOS
- Linux
- Windows

To host in a Windows environment you'll need the [ASP.NET Core Hosting Bundle](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-5.0), for other environments you will need to install the appropriate runtime.

ASP.NET Core applications can be run behind a reverse proxy or directly connected to the Internet using the built-in ASP.NET Core [Kestrel Web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-5.0).

- To host with IIS, [Windows Server 2012 R2 or later is supported](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-5.0). Note that currently GRA reporting utilizes Web Sockets and in a Windows environment that requires IIS 8 and Windows Server 2012.
- To host with [Apache](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-5.0) or [Nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0), a version of Linux which supports .NET 5.0 should be selected.

### Docker server requirements

Once [Docker is installed](https://docs.docker.com/install/) in your environment (configured for Linux containers) you are good to go!

## Database server requirements

The GRA version 4 supports the following database environments:

- [Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2012 or later using SQL Server authentication mode.

## Mail server requirements

The ability to send Internet email, such as a service which accepts email via SMTP.

- The GRA sends mail in certain instances (such as helping users recover their lost passwords) and requires the ability to connect to an SMTP server.
