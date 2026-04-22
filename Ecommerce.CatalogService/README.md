To use migrations please verify the dotnet-ef tool is installed
```
dotnet tool install --global dotnet-ef
```

Then 

to create initial migration:
```
dotnet ef migrations add InitialCreate -c EcommerceCatalogDbContext --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```


to apply migrations:
```
dotnet ef database update -c InitialCreate --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```