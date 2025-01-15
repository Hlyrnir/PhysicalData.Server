#### Description

Requirements:

- [Passport](https://github.com/Hlyrnir/Passport) in same the folder as this repository
- *appsettings.json* in project PhysicalData.Api

For realisation following concepts has been used.

- Clean architecture
- Domain driven design
- Message pipeline (Request -> Authorization -> Validation -> Handler -> Result -> Response) using [Mediator](https://github.com/martinothamar/Mediator)
- ORM using [Dapper](https://github.com/DapperLib/Dapper)