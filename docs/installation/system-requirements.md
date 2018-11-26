# System requirements

The GRA requires the following services to run. These services may all be hosted on the same machine or may be hosted on separate machines if desired. As an alternative to installing the GRA directly in a Web server environment, the GRA can be run from a [Docker container](https://www.docker.com/) using our [official Docker  images](https://hub.docker.com/r/mcld/gra/).

## Web site

### Web server requirements
Version 4 of the GRA runs in the [Microsoft .NET Core 2.1](https://www.microsoft.com/net/download/all) runtime environment. Downloads of the runtime environment are available from Microsoft for the following operating systems:
  * [macOS](https://docs.microsoft.com/en-us/dotnet/core/macos-prerequisites?tabs=netcore2x) (10.12 "Sierra" and later versions)
  * [Linux](https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore21) (Red Hat Enterprise Linux, CentOS, Oracle Linux, Fedora, Debian, Ubuntu, Linux Mint, openSUSE, SUSE Enterprise Linux, Alpine Linux)
  * [Windows](https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites?tabs=netcore21) (x64 and x86)

As of this writing the latest release of the .NET Core 2.1 environment is [Runtime v2.1.6](https://www.microsoft.com/net/download/dotnet-core/2.1). To host in a Windows environment you'll need the [Runtime & Hosting Bundle](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.1#install-the-net-core-hosting-bundle), for other environments you will need to install the appropriate runtime. You can safely use any 2.1.x version of the runtime as long as it's v2.1.6 or newer.

ASP.NET Core applications can be run behind a reverse proxy or directly connected to the Internet using the built-in ASP.NET Core [Kestrel Web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.1).
  * To host with IIS, [Windows Server 2008 R2 or later is supported](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.1). Note that currently GRA reporting utilizes Web Sockets and in a Windows environment that requires IIS 8 and Windows Server 2012.
  * To host with [Apache](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-2.1) or [Nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.1), a version of Linux which supports .NET Core 2.1 should be selected (see above).

### Docker server requirements
Once [Docker is installed](https://docs.docker.com/install/) in your environment (configured for Linux containers) you are good to go!

## Database server requirements
The GRA version 4 supports the following database environments:
  * [Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2008 or later using SQL Server authentication mode.

## Mail server requirements
The ability to send Internet email, such as a service which accepts email via SMTP.
  * The GRA sends mail in certain instances (such as helping users recover their lost passwords) and requires the ability to connect to an SMTP server.
