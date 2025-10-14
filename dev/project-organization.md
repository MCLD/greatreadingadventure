# GRA Developer Documentation - Project organization

This document describes the layout of the project as viewed in the solution file.

- `GRA` - utilities for use throughout the solution
- `GRA.Controllers` - controllers for the MVC application - they are kept out of the `GRA.Web` project to keep all program logic removed from the UI and to make them easily testable
- Domain
  - `GRA.Domain.Model` - domain models which are passed between the UI and data repositories
  - `GRA.Domain.Report` - reporting implementations following our report API
  - `GRA.Domain.Repository` - interfaces describing the data repositories
  - `GRA.Domain.Service` - services for performing logic and calling data repositories
- Infrastructure
  - `GRA.Data` - main Entity Framework data project
    - `Config` - [Mapster configuration](https://github.com/MapsterMapper/Mapster/wiki/Configuration)
    - `Model` - model files that match the database schema, used by EF for [database CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete)
    - `Repository` - repositories for communicating with the database and returning domain model objects
  - `GRA.Data.SQLite` - database provider for using SQLite
  - `GRA.Data.SqlServer` - database provider for using Microsoft SQL Server
- `GRA.Web` - MVC Web front-end and dependency injection composition root
  - `wwwroot` - static files for serving via the Web
  - `Areas/MissionControl` - Razor files for Mission Control (site administration)
  - `Views` - Razor files
  - `Startup.cs` - the main configuration location for the project
