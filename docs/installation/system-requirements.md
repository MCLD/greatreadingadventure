# System requirements

The GRA requires the following services to run. These services may all be hosted on the same machine or may be hosted on separate machines for performance reasons.

## Runtime environment
Version 4 of the GRA runs in the [Microsoft .NET Core 1.1](https://www.microsoft.com/net/download/all) runtime environment. Downloads of the runtime environment are available from Microsoft for the following operating systems:
  * macOS
  * Linux (CentOS, Debian, Fedora, openSUSE, Ubuntu)
  * Windows (x64 and x86)

As of this writing the latest release of the .NET Core 1.1 environment is [Runtime v1.1.7](https://www.microsoft.com/net/download/dotnet-core/runtime-1.1.7). Please ensure you use a v1.1.x Hosting Bundle!

## Web server
Microsoft [recommends that ASP.NET Core 1.1 applications are run behind an Apache, nginx, or IIS reverse proxy](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-1.1&tabs=aspnetcore1x#when-to-use-kestrel-with-a-reverse-proxy).
  * To host with IIS, [Windows Server 2008 R2 or later is supported](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-1.1&tabs=aspnetcore1x).
  * Note that currently GRA reporting utilizes Web Sockets and requires IIS 8 and Windows Server 2012.
  * To host with [Apache](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-1.1&tabs=aspnetcore1x) or [Nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-1.1&tabs=aspnetcore1x), a version of Linux which supports .NET Core 1.1 should be selected (see above).

## Database server
The GRA version 4 supports the following database environments:
  * [Microsoft SQL Server](http://www.microsoft.com/en-us/server-cloud/products/sql-server/) 2008 or later using SQL Server authentication mode

## Mail server
The ability to send Internet email, such as a service which accepts email via SMTP.
  * The GRA sends mail in certain instances (such as helping users recover their lost passwords) and requires the ability to connect to an SMTP server.
