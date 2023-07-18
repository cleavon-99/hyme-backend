# Hyme API
[Visit swagger documentation](https://api.hyme.network/swagger)

### How to create database migration?
```
dotnet ef migrations add MigrationName --project Hyme.Infrastructure --startup-project Hyme.API
```
### How to remove migration?
```
dotnet ef migrations remove --project Hyme.Infrastructure --startup-project Hyme.API
```
### How to deploy database?
Make sure to change the right connection string before the command
```
dotnet ef database udpate --project Hume.Infrastructure --startup-project Hyme.API
```

#### If this commands is failing you should update your dotnet tool by using this command
```
dotnet tool update --global dotnet-ef
```
