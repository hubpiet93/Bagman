to create migration

```shell
cd ../../..
dotnet ef migrations add InitialCreate --project 'src/Bagman.Infrastructure' --startup-project 'src/Bagman.Api' --output-dir 'Data/Migrations'
```

to update database

```shell
cd ../../..
dotnet ef database update --project 'src/Bagman.Infrastructure' --startup-project 'src/Bagman.Api'
```