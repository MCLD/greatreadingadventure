# GRA Developer Documentation - Adding an entity

Adding an entity and the related infrastructure is a multi-step affair. We'll assume the type you are adding below is `Chronoton` for illustrative purposes.

1. Create a domain model in `Domain/GRA.Domain.Model`.
   - Generally this object should inherit `Abstract.BaseDomainEntity`.
2. Create a data model in `Infrastructure/GRA.Data/Model`.
   - Generally this object should inherit `Abstract.BaseDbEntity`.
   - Inheriting `Abstract.BaseDbEntity` is appropriate if the primary key is an identity `int` column (see `GRA.Data.Model.UserRole` for an example of a type with a composite key).
3. Add the type to the `GRA.Data/Context.cs` file in the appropriate place (alphabetically).
   - If any composite keys or indexing is necessary, add them in the appropriate places.
4. Create an interface for the repository in `Domain/GRA.Domain.Repository`.
   - Remember to make it `public`.
   - Inherits `IRepository<Model.Chronoton>`.
   - Named `IChronotonRepository`.
5. Create the repository in `GRA.Data/Repository` directory.
   - Inherits `AuditingRepository<Model.Chronoton, Domain.Model.Chronoton>`.
   - Implements `IChronotonRepository`.
   - If you right-click on the repository type to allow Visual Studio to generate a constructor, ensure you modify the `ILogger` to add the generic (e.g. `ILogger<ChronotonRepository>`). Also remember to reformat to < 100 characters per line.
6. Create the service in `GRA.Domain.Service`.
   - Keep in mind you might not name this service `ChronotonService` as it should serve the domain needs.
   - It should inherit `Abstract.BaseService<ChronotonService>` if it is a public service or `Abstract.BaseUserService<ChronotonService>` if a user must be authenticated to access it.
7. Add the service and repository (in the appropriate alphabetical location) to the DI configuration in `GRA.Web/Startup.cs`.
