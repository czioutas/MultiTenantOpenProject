# MultiTenantOpenProject

A .net based API with a focus on multi-tenancy

## Requirements

You will need:

- [ ] Docker
- [ ] Dotnet 7 https://dotnet.microsoft.com/download/dotnet/7.0

## Running the API 🏃‍♂️

### Running with Local Infrastructure

(working directory: MultiTenantOpenProject/API)

- `docker-compose -f infra.yml up`
- `DROP_DB_SEED_DATA=1 ASPNETCORE_ENVIRONMENT=Development dotnet run --project API/API.csproj`

### Running with VStudio

- Open the sln (solution) API/MultiTenantOpenProject.sln

## Monitoring 🕵️‍♀️

### Metrics

We use prometheus library to export metrics
[http://localhost:7000/metrics](http://localhost:7000/metrics)

## Packages

The package manager for C# is called Nuget. Nuget is the equivalent to NPM or
Composer. In most cases we don't deal with Nuget directly but we use the `dotnet`
cli. Restoring packages is done using `dotnet restore` although `dotnet build`
by default restores the packages unless specified otherwise.

Package requirements definition is done on `.csproj` files.

## Code Format 👗

### DotnetFormat 📏

The code is formatted using Dotnet format tool. The trigger point is API.csproj

```
  <Target Name="Dotnet Format" AfterTargets="PostBuildEvent" Condition=" '$(Configuration)' == 'Debug' ">
    <Exec Command="dotnet dotnet-format ..\API.sln"/>
  </Target>
```

### EditorConfig 🐭

See [EditorConfig](https://editorconfig.org/)

## Database Migrations

`dotnet ef --startup-project .\src migrations add "1" --context ApplicationDbContext -o Migrations --configuration Release`

## New Endpoint? Editing Endpoint? Check the things bellow

- Controller
  - actions
  - xml docu
  - throws
- Service
  - concrete
  - interface
- Repository
  - concrete
  - interface
- Contract
  - is it use-case specific?
- Contract Validator
  - is there a validator?
  - is it registered?

## Migrations

Navigate to the MultiTenantOpenProject folder and execute the following:

Generate: `dotnet ef --startup-project .\API\ migrations add "init" --context ApplicationDbContext --configuration Release`
Apply: `Migrations are applied by default on startup`

## Versioning

We release using GitVerion (https://github.com/GitTools/GitVersion) https://gitversion.net/docs/reference/intro-to-semver

- we push from PR & Master
- we use Mainline mode (which would switch later after launch) https://gitversion.net/docs/reference/versioning-modes/mainline-development
- and lastly we increment versions based on commits:

```
major-version-bump-message: 'type:\s?(breaking|major)'
minor-version-bump-message: 'type:\s?(feature|minor)'
patch-version-bump-message: 'type:\s?(fix|patch)'
```

For example, a commit message for a feature should look like this `type: feature ` following the description of the changes

## Extra files

- `.config/dotnet-tools.json` Dotnet tools are command line tools that can be
  used. This file configures which ones we want.
- `.vscode` Visual Studio code config files
- `.dockerignore` Ignore file for docker
- `.editorconfig` Editorconfig configuration file
- `.gitattributes` Specifies how git behaves for the codebase
- `.gitignore` Ignore config file for git
- `GitVersion.yml` Config file for using semver
- `infra.yml` Basic yaml for docker-compose to start any infrastructure needed
  like the database
- `NuGet.config` NuGet is the dotnet package manager
- `MultiTenantOpenProject.sln` sln file is a bundling file that combines all projects into one.
