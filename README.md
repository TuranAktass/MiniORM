# MiniORM

A lightweight ORM built in C# to explore how ORMs work internally.

---

## Features

### Mapping

* Attribute-based mapping

    * `[Table]`
    * `[Column]`
    * `[Key]`

### CRUD Operations

* Insert
* Update
* Delete
* GetById

### Query API (IQueryable-like)

* `Where`
* `OrderBy` / `OrderByDescending`
* `ThenBy`
* `Take`
* `Skip`
* `FirstOrDefault`
* `Any`
* `Count`
* `Select` (Projection)

---

## Example Usage

### Define Entity

```csharp
[Table("Users")]
public class User
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("FirstName")]
    public string FirstName { get; set; } = "";

    [Column("LastName")]
    public string? LastName { get; set; }

    [Column("Email")]
    public string Email { get; set; } = "";

    [Column("IsActive")]
    public bool IsActive { get; set; }
}
```

---

### Create Context

```csharp
var context = new OrmContext(
    connectionString,
    cs => new DbExecutor(cs));
```

---

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

**Generated SQL:**

```sql
INSERT INTO Users (FirstName, LastName, Email, IsActive)
VALUES (@p0, @p1, @p2, @p3)
```

---

###  Update

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

**Generated SQL:**

```sql
UPDATE Users
SET FirstName = @p0,
    LastName = @p1,
    Email = @p2,
    IsActive = @p3
WHERE Id = @p4
```

---

### Query

```csharp
var users = context.Queryable<User>()
    .Where(x => x.IsActive)
    .OrderBy(x => x.FirstName)
    .Take(10)
    .ToList();
```

**Generated SQL:**

```sql
SELECT *
FROM Users
WHERE IsActive = 1
ORDER BY FirstName ASC
LIMIT 10
```

---

### Projection (Select)

```csharp
var users = context.Queryable<User>()
    .Select(x => new { x.Id, x.FirstName })
    .Where(x => x.IsActive)
    .ToList();
```

**Generated SQL:**

```sql
SELECT Id, FirstName
FROM Users
WHERE IsActive = 1
```

---

## Limitations (for now)

* Limited projection support (anonymous types only)
* No relation handling (joins, navigation properties)
* No change tracking
* Basic SQL generation

---

##  TODO

* [ ] Projection materialization (mapping to DTOs)
* [ ] Joins / relationships
* [ ] Change tracking
* [ ] Query optimization
* [ ] Caching / compiled queries
