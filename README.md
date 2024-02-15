### Sample Data Layer Package

NOTE: This package is configured to use Postgres, but other changes might be required.

Include this package as a dependency to your projects in order to use the repository class, migrations and the context itself.

### IMPORTANT PARAMETERS / CHANGES YOU NEED

Make sure to remove any appsettings / credentials if you're going to use this as a public package. If you're planning on keeping it as a private package for yourself or your organization, you can maybe use a key vault for the connection strings.

If you're going to remove the appsettings from the Sample.Data project, make sure you add the connection string in the project's appsettings and when running the migrations make sure you specify the DbContext project path and the Start up project path, where the appsettings are.

I tried keeping the Sample API simple, but I would have the endpoints/logic and DTOs in a separate class library. If you run the API you are able to create and fetch products with the IRepository, which uses the Sample.Data DbContext.

I also recommend having a look through the code and reading the comments for other utilities.

### Installation / Testing

To test the sample API make sure you run a Postgres database through the postgres:latest image docker container. Do not forget to change the appsettings connection strings.

More information on parameters: https://hub.docker.com/_/postgres

```shell
docker image pull postgres:latest

docker container run postgres:latest --name postgres-container -e POSTGRES_PASSWORD=mysecretpassword -d postgres
```

You should then be able to connect via Azure Data Studio or Data Grip or even the localhost GUI to your database.

### Using the Sample.Data package externally

In our Sample.Api example I simply added the local Sample.Data project as a dependency, in separate repositories you need:

- nuget.config for the credentials to your NuGet package registry and for your project to recognize the missing package (I included one as example in the root directory, but without the .config extension, make sure to replace the _ to .)
- add package reference to your project
- CI/CD pipeline to build the Sample.Data project, pack it and deploy the package to the Github Package Registry OR run dotnet pack command

### How to deploy it as a NuGet package

- add the nuget.config to the root of your repository/solution, edit it with your personal access token to read packages, change the name of the github package registry to your username/organization name, and your username
- add <PackageReference Include="Sample.Data" Version="1.0.0" /> to your .csproj file
- edit the github actions workflow file @.github/workflows/nuget-package-deploy - rename it to nuget-package-deploy.yml in order for Github to consider it as an Action Workflow.

### Editing your NuGet package

If in the future you edit your Sample.Data project you need to change the version number in the Sample.Data.csproj file. By adding/editing:

<Version>1.0.0</Version>

#### Commands (for the use case you can remove the optional parameters)

If the DbContext class and the class/Service Collection where the DbContext is instantiated are in different class libraries you would need to add to the commands the optional parameters below.

```shell
--project <path to project that contains DbContext> --startup-project <path to project that contains DbContext initialization>
```

Creates a migration file (converts changes on the entities/model into SQL code)

```shell 
dotnet ef migrations add <MigrationNameCase> 
```

Removes last migration.

```shell 
dotnet ef migrations remove
```

Updates remote database (from the appsetting's connection string) for a specific Migration, by the name. You can also not use the name and it will update with all migrations.

```shell 
dotnet ef database update <MigrationCaseName>
```

Drops the database. (DATA will be lost, be careful with this)

```shell 
dotnet ef database drop --dry-run
```

After running and reading the output of the command above run this one below.

```shell 
dotnet ef database drop
```

----

#### Migration Commands that might be helpful

```shell
dotnet ef migrations ...
```

* add                        Adds a new migration. 

* bundle                     Creates an executable to update the database.

* has-pending-model-changes  Checks if any changes have been made to the model since the last migration.

* list                       Lists available migrations.

* remove                     Removes the last migration.

* script                     Generates a SQL script from migrations.

#### DbContext Commands that might be helpful

```shell
dotnet ef dbcontext ...
```

* info      Gets information about a DbContext type.
* list      Lists available DbContext types.
* optimize  Generates a compiled version of the model used by the DbContext.
* scaffold  Scaffolds a DbContext and entity types for a database.
* script    Generates a SQL script from the DbContext. Bypasses any migrations.
