# MiniORM 

A lightweight ORM built in C# to explore how ORMs work internally.

## Features

### Mapping
- Attribute-based mapping
    - `[Table]`
    - `[Column]`
    - `[Key]`

### CRUD
- Insert
- Update
- Delete
- GetById

### Query API (IQueryable-like)
- `Where`
- `OrderBy` / `OrderByDescending`
- `Take`
- `Skip`
- `FirstOrDefault`
- `Any`
- `Count`

## Example Usage

### Define Entity

```csharp
[Table("Users")]
public class User
{
    [Key] [Column("Id")] public int Id { get; set; }

    [Column("FirstName")] public string FirstName { get; set; } = "";

    [Column("LastName")] public string? LastName { get; set; }

    [Column("Email")] public string Email { get; set; } = "";

    [Column("IsActive")] public bool IsActive { get; set; }

}
```

### Create Context 
```csharp 
var context = new OrmContext(
    connectionString,
    cs => new DbExecutor(cs));
```
### Insert
```csharp
context.Insert(new User
{
    FirstName = "Ali",
    LastName = "Yılmaz",
    Email = "ali@test.com",
    IsActive = true
}); 
```
### Update
```csharp
context.Update(new User
{
    Id = 1,
    FirstName = "Ali",
    LastName = "Yılmaz",
    Email = "ali@test.com",
    IsActive = true
}); 
```

### Insert
```csharp
context.Insert(new User
{
    Id = 1,
    FirstName = "Ali",
    LastName = "Yılmaz",
    Email = "ali@test.com",
    IsActive = true
}); 
```

### Query 

```csharp 
var users = context.Queryable<User>()
    .Where(x => x.IsActive)
    .OrderBy(x => x.FirstName)
    .Take(10)
    .ToList();
```