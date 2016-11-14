# GRA 4 Developer Documentation - Project organization

- `GRA` - constants for used throughout the solution
- `GRA.Controllers` - controllers for the MVC application - they are kept out of the `GRA.Web` project to keep all program logic removed from the UI and to make them easily testable
- `GRA.Data` - main data project
  - `Model` - model files that match the database schema, used by EF for [database CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete)
  - `Profiles` - [AutoMapper profiles](https://github.com/AutoMapper/AutoMapper/wiki/Configuration#profile-instances)
  - `Repository.cs` - master repository for performing database operations and returning `GRA.Domain.Model` objects
    - **Note: this repository may need to be broken up into additional class files as the complexity increases**
- `GRA.Data.SQLite` - database provider for using SQLite
- `GRA.Data.SqlServer` - database provider for using Microsoft SQL Server
- `GRA.Domain` - domain logic for the software
  - `Model` - domain models which are passed to `GRA.Controllers` for displaying and manipulating data
  - `Service.cs` - service used to read and write `GRA.Domain.Model` objects
- `GRA.Web` - MVC Web front-end and dependency injection composition root
  - `wwwroot` - static files for serving via the Web
  - `Views` - Razor files
  - `Startup.cs` - the main configuration location for the project
