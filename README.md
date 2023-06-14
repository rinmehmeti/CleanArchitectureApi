# Clean Architecture (.NET 7, CQRS, MediatR)

#### Database Configuration:
By default solution is configured to use an in-memory database by default. This ensures that all users will be able to run the solution without needing to set up additional infrastructure (e.g. SQL Server).

If you would like to use SQL Server, you will need to update *Api/appsettings.json* as follows: `"UseInMemoryDatabase": false`

Verify that the DefaultConnection connection string within appsettings.json points to a valid SQL Server instance.

When you run the application the database will be automatically created (if necessary) and the latest migrations will be applied.

#### Database Migrations
To use dotnet-ef for your migrations first ensure that "UseInMemoryDatabase" is disabled, as described within previous section. Then, add the following flags to your command (values assume you are executing from repository root)

--project src/Infrastructure
--startup-project src/Api
--output-dir Persistence/Migrations

For example, to add a new migration from the root folder:

`dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Api --output-dir Persistence\Migrations`

#### Overview
###### Domain
Contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

###### Application
Contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. This layer defines interfaces that are implemented by outside layers. For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

###### Infrastructure
Contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

###### Api
Depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only Startup.cs should reference Infrastructure.
