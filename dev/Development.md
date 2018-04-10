## Environment

GRA code should successfully compile using the free [Visual Studio Community](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) edition. External dependencies are managed with NuGet and should be automatically downloaded upon the first build.

By default, GRA is configured to use LocalDB as a database for development and testing. While not packaged with Visual Studio Community, it can easily be added by selecting the notification flag in the title bar of Visual Studio and choosing to install the "MS SQL Server Update for database tooling". Alternately, the [SQL Server Data Tools (SSDT)](https://msdn.microsoft.com/en-us/library/mt204009.aspx) package can be installed manually by downloading it from Microsoft.

If you have any trouble getting the package to build, please submit an [issue](https://github.com/MCLD/greatreadingadventure/issues/new) with details.

### Running locally

Once the solution has built, you can run the SRP project in order to go through the initial configuration in a browser. If  the default database settings are left during configuration (you may need to enter text for the database user names and logins so that you can complete the configuration but they won't be saved or used in a LocalDB configuration), the software should deploy to a LocalDB named `SRP` under the `(localdb)\ProjectsV12` instance (you can see this by selecting *SQL Server Object Explorer* from the *View* menu in Visual Studio.

### Design/Architecture

#### Back-end software

* [ASP.NET Web Forms](http://www.asp.net/web-forms) backed by [C#](https://msdn.microsoft.com/en-us/vstudio/hh341490.aspx) [code-behind](https://support.microsoft.com/en-us/kb/303247)
* [ASP.NET User Controls](https://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx)

The use of ASP.NET Web Forms and User Controls allows some amount of customization without needing to recompile the software. ASCX (User Control) and ASPX (Web Form) files can be edited to provide additional pages or to modify the layout.

Some **[ASP.NET Conventions](ASPNETConventions.md)** are used in the GRA.

#### Front-end user interface

* [Bootstrap](http://getbootstrap.com/) - **[Bootstrap conventions](BootstrapConventions.md)** used in the GRA.
* [jQuery](https://jquery.com/)

## Manual

See the [manual](Manual.md) wiki page for details on contributing to the manual.

## Test

Please review the current [test plan](TestPlan.md).

## Release

In order to prepare a release, please review the [release engineering](ReleaseEngineering.md) Wiki article.
