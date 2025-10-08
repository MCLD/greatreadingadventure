# GRA Developer Documentation

## Environment

GRA code should successfully compile using either the free [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio Community](https://www.visualstudio.com/vs/community/) edition. External dependencies are managed with NuGet and should be automatically downloaded upon the first build. The current `main` branch requires [C# 11](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11) to compile, we are currently using Visual Studio 2022.

Framework history:

- Current work (versions 4.5.0 and 4.6.x) use [.NET 8.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- Versions 4.4.x use [.NET 7.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-7)
- Version 4.3.17 uses [.NET 6.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- Versions 4.2 and 4.2.1 use [.NET 5.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-5)
- Version 4.1.1 used ASP.NET Core v2.2
- Version 4.1.0 used ASP.NET Core v2.1
- Version 4.0.0 used ASP.NET Core v1.1

By default, GRA is configured to use LocalDB as a database for development and testing. While not packaged with Visual Studio Community, it can easily be added by selecting the notification flag in the title bar of Visual Studio and choosing to install the "MS SQL Server Update for database tooling". Alternately, the [SQL Server Data Tools (SSDT)](https://msdn.microsoft.com/en-us/library/hh272686.aspx) package can be installed manually by downloading it from Microsoft. You could also add a SQLite migration and configure the GRA to use SQLite.

If you have any trouble getting the package to build, please submit an [issue](https://github.com/MCLD/greatreadingadventure/issues/new) with details.

## Project conventions

The `main` branch contains work in progress.

- Try to follow the [HTML](http://codeguide.co/#html) and [CSS](http://codeguide.co/#css) code guides.
- Wrap lines at 100 characters whenever possible.
- [`IDisposable`](https://msdn.microsoft.com/en-us/library/system.idisposable.aspx) calls should be called with [`using`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement).
- [`async`](https://docs.microsoft.com/en-us/dotnet/standard/async-in-depth) methods should have "Async" at the end of the method name unless they are Controllers.
- Avoid "[magic numbers](<https://en.wikipedia.org/wiki/Magic_number_(programming)>)," place them in constants.
- Avoid adding settings to the `appsettings.json` configuration file, preferences generally belong somewhere that they can be set in the application.
- Review the [project organization](project-organization.md).
- Follow the [initial developer setup](initial-developer-setup.md) guidance.
- Examine guidance for [adding an entity](adding-an-entity.md) if necessary.
- We try to adhere to [domain-driven design](https://en.wikipedia.org/wiki/Domain-driven_design) with varying levels of success.

## Design/Architecture

### Back-end software

- Development is done in the [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) language.
- Version 4 of the GRA is developed using the [Microsoft .NET Core](https://www.microsoft.com/net/core) platform using [ASP.NET MVC](https://www.asp.net/mvc).
- Data storage uses the [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) data access technology.

### Front-end user interface

- [Bootstrap](http://getbootstrap.com/)
- [jQuery](https://jquery.com/)
- [FontAwesome](https://fontawesome.com/)

#### Front-end best practices

In Mission Control we try to be consistent with buttons and iconography:

- Add/Save/Search buttons: classed `btn-outline-primary`
- Back/Back to List/Clear Search buttons: classed `btn-outline-secondary`
- Add icon buttons: classed `btn-outline-success`, interior span classed `fas fa-plus fa-fw`
- Edit icon buttons: classed `btn-outline-primary`, interior span classed `fas fa-pencil-alt fa-fw`
- Delete icon buttons: classed `btn-outline-danger`, interior span classed `fas fa-times fa-fw`
- Up arrow icon buttons: classed `btn-outline-secondary`, interior span classed `fas fa-arrow-up fa-fw`
- Down arrow icon buttons: classed `btn-outline-secondayr`, interior span classed `fas fa-arrow-down fa-fw`
- Yes/Active/Positive indicator icon: classed `far fa-check-circle fa-fw text-success`
- No/Inactive/Negative indicator icon: classed `far fa-times-circle fa-fw text-danger`
