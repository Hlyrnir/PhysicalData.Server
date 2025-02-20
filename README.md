#### Description

Shows an example of a Minimal API using Passport.

Requirements:

- *appsettings.json* in PhysicalData.Api project folder

For realisation following concepts has been used.

- Clean architecture
- Domain driven design
- Message pipeline (Request -> Authorization -> Validation -> Handler -> Result -> Response) using [Mediator](https://github.com/martinothamar/Mediator)
- ORM using [Dapper](https://github.com/DapperLib/Dapper)