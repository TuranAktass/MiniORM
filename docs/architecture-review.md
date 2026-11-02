# MiniORM — Architecture Review

_Scope: `MiniORM/`, `MiniORM.Tests/`, `MiniORM.IntegrationTests/`, `Playground/`, `README.md`. This review comes from reading the code only. No build or test run was done, so statements about test results are inferred from the code._

---

## 1. Overview

MiniORM is a small learning ORM for .NET 10 (`MiniORM/MiniORM.csproj`). It works only with SQLite (`Microsoft.Data.Sqlite.Core`, a preview build `11.0.0-preview.2…`). It offers:

- **Attribute mapping**: `[Table]`, `[Column]`, `[Key]` (`MiniORM/Mapping/*.cs`). If `[Key]` is missing, a property named `Id` is used as the key (`MiniORM/Helpers/EntityMetaDataHelper.cs:20-33`).
- **CRUD**: `Insert`, `Update`, `Delete`, `GetById` on `OrmContext` (`MiniORM/Core/OrmContext.cs`).
- **A query API that looks like LINQ**: `QuerySet<T>` with `Where`, `OrderBy`/`OrderByDescending`/`ThenBy`, `Skip`/`Take`, `Select`, `ToList`, `FirstOrDefault`, `Any`, `Count` (`MiniORM/Query/QuerySet.cs`).
- **Raw SQL**: `OrmContext.Query<T>` and `OrmContext.Execute`.
- **Logging hooks**: `IOrmLogger`, with console and null implementations (`MiniORM/Logging/`).

The README (`README.md`) lists these limitations: no joins, no change tracking, and projections only into the entity type. The design is clear and easy to follow, which fits its teaching goal. Several correctness gaps exist, and the test suites do not catch them (see §3 and §4).

---

## 2. Architecture

### 2.1 Components

| Layer | Files | Responsibility |
|---|---|---|
| Entry point | `MiniORM/Core/OrmContext.cs` | Facade. Holds the connection string, the logger and a `DbExecutor` factory. Creates `QuerySet<T>` and runs CRUD. |
| Metadata | `MiniORM/Helpers/EntityMetaDataHelper.cs`, `MiniORM/Mapping/*.cs` | Reflection lookup of table name, column name, key property, and insertable/updatable properties. Nothing is cached. |
| Query API | `MiniORM/Query/QuerySet.cs` | Mutable fluent builder. Collects clauses into a `QueryModel`. |
| Expression parsing | `MiniORM/Query/ExpressionParser/SqlExpressionParser.cs`, `OrderExpressionParser.cs`, `SelectExpressionParser.cs`, `ExpressionParsingHelperMethods.cs` | Turns `Expression<Func<T,…>>` trees into SQL fragments and named parameters. |
| Query state | `MiniORM/Query/Model/QueryModel.cs`, `ProjectionModel.cs`, `MiniORM/Query/Context/QueryContext.cs`, `ExpressionParser/SqlResult.cs` | Holds the clause lists, limit/offset and the parameter dictionary, plus a parameter-index counter. |
| SQL building | `MiniORM/Query/SqlBuilder/SelectQueryBuilder.cs`; `MiniORM/Query/Methods/{Insert,Update,Delete,GetById}.cs` | Joins strings into SQLite-dialect SQL. |
| Execution | `MiniORM/Infrastructure/DbExecutor.cs` | Opens a connection (a new `SqliteConnection` per call, or an injected `DbConnection`), binds parameters, and runs reader, scalar or non-query commands. Logs each command. |
| Materialization | `MiniORM/Materialization/ObjectMaterializer.cs` | Reflection-based mapping from a row to `T`, matching columns by **property name**. |

### 2.2 Data flow: query path

```
ctx.Queryable<User>()                         OrmContext.cs:30
  └─ new QuerySet<T>(cs, executorFactory, logger)
       QueryModel.TableName = [Table] name     QuerySet.cs:29-32
  .Where(x => x.IsActive && x.Age > n)        QuerySet.cs:43
       └─ new SqlExpressionParser(sharedQueryContext).Parse(body)
            ├─ NormalizeBooleanExpression (top-level bool member → "= true")   SqlExpressionParser.cs:165
            ├─ ParseBinary / ParseMember / ParseConstant / ParseMethodCall / ParseUnary
            │     columns   → EntityMetaDataHelper.GetColumnName(property)
            │     values    → closures compiled with Expression.Lambda(...).Compile() (:126)
            │                 → "@pN", stored in QueryContext.Parameters
            └─ "(IsActive = @p0)" appended to QueryModel.WhereClauses
  .OrderBy(x => x.Name)                       → "Name ASC" in OrderByClauses (OrderExpressionParser.cs)
  .Select(x => new { x.Id, x.Name })          → "Id AS Id, Name AS Name" in SelectClause (SelectExpressionParser.cs, QueryModel.cs:15)
  .Take(10)/.Skip(5)                          → Limit/Offset
  .ToList()                                   QuerySet.cs:101
       └─ SelectQueryBuilder.Build(model)     "SELECT … FROM … WHERE a AND b ORDER BY … LIMIT n OFFSET m"
       └─ executorFactory(cs, logger).Query<T>(sql, parameters)   DbExecutor.cs:27
            ├─ AddParameters: IDictionary → "@"+key; any other object → one parameter per public property
            ├─ ExecuteReader
            └─ ObjectMaterializer.Materialize<T>(reader) per row
```

`FirstOrDefault`, `Any` and `Count` copy the model with `QueryModel.Clone()` and change `Limit`/`SelectClause` (`QuerySet.cs:90-130`). `ToList()` does not copy; it builds straight from the shared model.

### 2.3 Data flow: command path

`OrmContext.Insert/Update/Delete` (`OrmContext.cs:48-70`) build SQL from metadata (`Query/Methods/*.cs`). The parameters are named `@{PropertyName}`, and the **entity object itself** is passed as the parameter bag. `DbExecutor.AddParameters` (`DbExecutor.cs:167-178`) then turns every public property into a parameter. `GetById` uses a `Dictionary` keyed by the key property name (`OrmContext.cs:81-86`).

### 2.4 Design observations

- The pipeline is cleanly split into four stages: parse, model, build, execute. That makes each stage easy to unit-test on its own, and the unit tests do use this.
- SQL is built by joining strings. There is no intermediate SQL AST and no dialect abstraction. `LIMIT/OFFSET` and `SqliteConnection` are hard-coded (`SelectQueryBuilder.cs:23-31`, `DbExecutor.cs:190`).
- `QuerySet<T>` is mutable and returns `this`, so it is not composable the way `IQueryable` is (see §4.1).
- The metadata layer is fully reflection-driven and has no cache. The same goes for the materializer.

---

## 3. Test coverage

### 3.1 Unit tests (`MiniORM.Tests/`)

| Area | Tests | What is covered |
|---|---|---|
| `SqlExpressionParser` | `Query/ExpressionParser/ExpressionParserTests.cs` | `>` with a constant; `== null` → `IS NULL`; bare bool member → `= @p0(true)`; `!bool` → `= @p0(false)`; parameter index increases across two parses. |
| `OrderByExpressionParser` | `Query/ExpressionParser/OrderExpressionParserTests.cs` | String member; int member wrapped in `Convert`. The "ThenBy" test is the same as the OrderBy test because there is no separate ThenBy parser. |
| `SelectExpressionParser` | `Query/ExpressionParser/SelectExpressionParserTests.cs` | Anonymous-type projection; `[Column("PhoneNumber")]` → alias `Phone`. |
| `SelectQueryBuilder` | `Query/SqlBuilder/SelectQueryBuilderTests.cs` | WHERE + ORDER BY + LIMIT + OFFSET, and a projection. The ThenBy cases hand-write `"FirstName, LastName ASC"` as one clause, which is not what `QuerySet.ThenBy` produces. |
| CRUD SQL | `Command/Methods/{Insert,Update,Delete}Test.cs` | Exact SQL strings. |

**Likely broken tests:** `MiniORM.Tests/TestModels/User.cs:18` adds `[Column("PhoneNumber")] public string Phone`. `EntityMetaDataHelper.GetInsertableProperties` and `GetUpdatableProperties` include every read/write property. So `BuildInsertSql<User>()` should output `…, IsActive, PhoneNumber) VALUES (…, @IsActive, @Phone)`. `InsertTest.cs:14` and `UpdateTest.cs:14` expect strings without `PhoneNumber`, so those two tests appear to fail.

### 3.2 Integration tests (`MiniORM.IntegrationTests/`)

- `Query/QuerySetIntegrationTests.cs`: `Where(bool)`, `OrderBy`, `FirstOrDefault`, `Any`, `Count`, `Contains`/`StartsWith`/`EndsWith`, `OrderBy+ThenBy`, and a `Select` projection (which only checks `NotEmpty`).
- `Command/CommandIntegrationTest.cs`: round-trips for Insert, Update and Delete.

**The fixture is probably broken.** `TestDbContextFactory.Create()` (`MiniORM.IntegrationTests/TestDbContextFactory.cs:13-20`) opens a `Data Source=:memory:` connection and creates the schema on it. It then passes only `connection.ConnectionString` to `OrmContext`, with the default executor factory. `DbExecutor` opens a **new** `SqliteConnection` for every call (`DbExecutor.cs:189-190`), and every new `:memory:` connection is a separate, empty database. So `SeedData` should fail with `no such table: Users`, and every integration test should fail. The `DbExecutor(DbConnection, …)` constructor (`DbExecutor.cs:21`) and the `executorFactory` parameter (`OrmContext.cs:18`) already exist to fix this, but the fixture doesn't use them.

### 3.3 Playground

`Playground/Tests/*.cs` are manual console scripts that print results and assert nothing. `Program.cs` only runs `ProjectionTests`. They are useful for exploration but add no automated coverage.

### 3.4 Notable gaps

- `ObjectMaterializer`: no tests for type conversion (enum, Guid, nullable, bool from INTEGER), for `[Column]` names that differ from property names, or for null handling.
- `DbExecutor`: no tests for parameter binding (dictionary vs. object), logging, or errors.
- `Skip` without `Take`, `Count` combined with `Take`/`Skip`, and `OrderByDescending`: none are covered.
- Repeated `Where` calls and reusing one `QuerySet` instance: not covered.
- `Where` with captured variables and nested members, `!=`, `||`, nullable or enum comparisons (which create `Convert` nodes), and unsupported-expression error paths: none are covered.
- `GetById` and the raw `Query`/`Execute` APIs: not covered.
- Projection content: the integration test does not check that only the projected columns come back.

---

## 4. Risks

### 4.1 Correctness

1. **The materializer ignores `[Column]`.** `ObjectMaterializer.cs:21-22` matches reader columns to `PropertyInfo.Name`, not to `EntityMetaDataHelper.GetColumnName`. With `SELECT *`, the `PhoneNumber` column is never mapped to `Phone` (see `Playground/User.cs:18`, `Playground/DbOperations.cs:80`). It only works through a projection alias (`PhoneNumber AS Phone`). Any entity whose column names differ from its property names silently loses data on `ToList`, `FirstOrDefault` and `GetById`.
2. **`Skip` without `Take` produces invalid SQLite.** `SelectQueryBuilder.cs:28-31` writes `OFFSET n` with no `LIMIT`, which is a syntax error in SQLite. It needs `LIMIT -1 OFFSET n`.
3. **`Count()` ignores `Take`/`Skip`.** `QuerySet.cs:120-130` keeps `Limit`/`Offset`, so the result is `SELECT COUNT(*) … LIMIT 10`. The LIMIT applies to the single aggregate row, not to the rows being counted. LINQ semantics would count at most 10 rows.
4. **`QuerySet<T>` is mutable and shared.** Every builder method changes `_queryModel` and returns `this` (`QuerySet.cs:35-88`). Code like `var q = ctx.Queryable<User>(); var a = q.Where(A); var b = q.Where(B);` gives `b` both filters. A second `OrderBy` appends instead of replacing, and there is no `ThenByDescending`.
5. **Method calls are matched by name only.** `SqlExpressionParser.cs:92-116` dispatches on `Method.Name`. `ids.Contains(x.Id)` (a `List<T>.Contains` instance call) is treated as a string `LIKE` on the list value. `Enumerable.Contains(ids, x.Id)` has `Object == null`, and `methodCall.Object!` then throws a `NullReferenceException`. `IN (...)` is not supported.
6. **`Convert` nodes are not supported.** `ParseUnary` (`SqlExpressionParser.cs:130-147`) only handles `Not`. Comparing a nullable property (`x.Age == 5` where `Age` is `int?`) or an enum produces `Convert` nodes, which throw `NotSupportedException`.
7. **Null-valued variables don't become `IS NULL`.** `IsNullConstant` (`SqlExpressionParser.cs:175`) only detects literal `null`. `string? s = null; Where(x => x.LastName == s)` produces `LastName = @p0` with `@p0 = NULL`, which never matches.
8. **LIKE semantics.** Values passed to `Contains`/`StartsWith`/`EndsWith` don't have `%`, `_` or `\` escaped (`SqlExpressionParser.cs:96-112`). SQLite `LIKE` is also case-insensitive for ASCII, while .NET `string.Contains` is case-sensitive. The integration test `Contains("A")` (`QuerySetIntegrationTests.cs:71`) passes partly because of this.
9. **Nested boolean members aren't normalized.** Only the top-level bool member is rewritten (`SqlExpressionParser.cs:165-173`). `x.IsActive && …` emits a bare `IsActive`. That works in SQLite but would break with other dialects.
10. **`Select` with `MemberInit` silently drops bindings.** Bindings that aren't member accesses are skipped (`SelectExpressionParser.cs:66-79`). In `Playground/Tests/ProjectionTests.cs:9-13` (`new User { FirstName = "Ali" }`), the projection list is empty, so the query falls back to `SELECT *` with no error. Projections into anonymous types still materialize into `T` (`QuerySet.Select` returns `QuerySet<T>`). This only works while the alias names match `T`'s property names.
11. **Key handling is inconsistent.** `GetInsertableProperties` (`EntityMetaDataHelper.cs:35-42`) only excludes properties that have `[Key]`. A key found by the `Id` convention is included in the INSERT, which conflicts with autoincrement. Every public read/write property is mapped, and there is no `[NotMapped]`. `Insert` returns the number of affected rows, not the generated key.
12. **Executor bookkeeping.** `Execute` logs "completed" *before* `ExecuteNonQuery` runs (`DbExecutor.cs:131-134`). `ExecuteScalar` skips the completion log when the result is null (`:93-94`).

### 4.2 Security (SQL injection)

- **Values are parameterized.** Expression values, LIKE patterns and CRUD values all go through `DbParameter`s (`SqlExpressionParser.cs:157-163`, `DbExecutor.cs:148-179`). That is the main defence, and it is applied consistently.
- **Identifiers are not quoted.** Table and column names from attributes, or from type/property names, are put straight into SQL (`Insert.cs:25`, `Update.cs:21-24`, `Delete.cs:16-18`, `GetById.cs:15`, `SelectQueryBuilder.cs:11`). These are set by the developer, so the risk is low. Reserved words or odd names will still break queries, and there is no validation.
- **The raw SQL APIs invite string interpolation.** `OrmContext.Query/Execute(string sql, …)` accept any string. The sample `Playground/DbOperations.cs:15` (`$"DELETE FROM {tableName}"`) shows this pattern. A `FormattableString` overload, or clear guidance in the docs, would steer users toward parameters.
- **Sensitive data in logs.** `ConsoleOrmLogger.LogCommand` prints every parameter value (`ConsoleORMLogger.cs:9-12, 26-34`), including emails and anything else bound from the entity. There is no redaction option.
- **Supply chain / stability.** The library depends on a **preview** package (`Microsoft.Data.Sqlite.Core 11.0.0-preview…`, `MiniORM/MiniORM.csproj:10`).

### 4.3 Performance

- **Reflection on every row.** `ObjectMaterializer.Materialize` calls `GetProperties()` and a linear `FirstOrDefault` per column for **each row** (`ObjectMaterializer.cs:12-22`), and then `PropertyInfo.SetValue`. The cost is O(rows × columns × properties) reflection with nothing cached.
- **Metadata is not cached.** `EntityMetaDataHelper` re-reads attributes on every call, and the CRUD SQL strings are rebuilt on every call.
- **Expressions are compiled per value.** `Expression.Lambda(...).Compile().DynamicInvoke()` (`SqlExpressionParser.cs:126`, `ExpressionParsingHelperMethods.cs:29`) is expensive. Walking the closure's `FieldInfo` would be much cheaper.
- **`Any()` materializes a whole entity** (`SELECT * … LIMIT 1`, `QuerySet.cs:109-118`) instead of using `SELECT EXISTS(…)` / `SELECT 1`.
- **A new connection per operation** (`DbExecutor.cs:181-191`). There are no transactions and no async APIs, so a batch of inserts means N separate connection opens and N implicit transactions.
- The SQL builder concatenates strings with `+=`. That is fine at this scale.

### 4.4 Maintainability

- Naming is inconsistent: `OrderExpressionParser.cs` contains `OrderByExpressionParser`; `NullORMLogger` sits next to `ConsoleOrmLogger`; `GetById` is `internal class` while `Insert` is `static class` and `Update`/`Delete` are non-static `class`.
- `QueryContext.Parameters` is a public mutable *field* of type `Dictionary<string, object>`. `QueryModel.Parameters` is `Dictionary<string, object?>`, and `QuerySet.Where` assigns one to the other by reference (`QuerySet.cs:49`).
- Error handling is uneven: `throw new Exception(...)` (`SqlExpressionParser.cs:116`, `EntityMetaDataHelper.cs:30`) sits alongside `NotSupportedException`.
- There is dead or commented-out code: `ExpressionParsingHelperMethods.cs:32-36`, `SelectExpressionParser.cs:19-34`, and unused `using`s in `OrmContext.cs` and `SqlExpressionParser.cs`.
- The SQLite dialect is spread across the builder and the executor, so adding another provider would touch many files.
- Three almost identical `User` models exist (`MiniORM.Tests/TestModels/User.cs`, `MiniORM.IntegrationTests/Models/User.cs`, `Playground/User.cs`), and they have already drifted apart (`Phone` exists in only two of them).
- The README examples don't match the actual output. The README shows `@p0…` for INSERT/UPDATE and `IsActive = 1`, but the code emits `@FirstName…` and `(IsActive = @p0)` (`README.md:89-160`).

---

## 5. Recommendations (top 5)

1. **Fix the integration fixture so the suite actually runs.** In `MiniORM.IntegrationTests/TestDbContextFactory.cs`, pass `executorFactory: (_, log) => new DbExecutor(connection, log)` so that every operation shares the open `:memory:` connection. Return the connection (or make the fixture `IDisposable`) so it gets disposed. Also either remove `Phone` from `MiniORM.Tests/TestModels/User.cs`, or update `InsertTest.cs`/`UpdateTest.cs` to expect `PhoneNumber`, so the unit suite passes.

2. **Make materialization use the mapping, and cache it.** In `MiniORM/Materialization/ObjectMaterializer.cs`, build a per-type cache (`ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>`) keyed by **column name** (`EntityMetaDataHelper.GetColumnName`), with property name as a fallback for projection aliases. Look up columns once per reader, not once per row. Cache attribute lookups in `MiniORM/Helpers/EntityMetaDataHelper.cs` the same way. Add unit tests for `Phone`/`PhoneNumber`, enums, nullables and bool-from-INTEGER.

3. **Fix the SQL-generation semantics in `SelectQueryBuilder` and `QuerySet`.**
   - `MiniORM/Query/SqlBuilder/SelectQueryBuilder.cs`: when only `Offset` is set, emit `LIMIT -1 OFFSET n`.
   - `MiniORM/Query/QuerySet.cs`:
     - `Count()`: wrap the query when `Limit`/`Offset` are set (`SELECT COUNT(*) FROM (SELECT … LIMIT … OFFSET …)`).
     - `Any()`: use `SELECT EXISTS(SELECT 1 …)` through `ExecuteScalar`.
     - Make the builder methods immutable (`Clone()` the model and return a new `QuerySet<T>`) so query reuse is safe.
     - Make a second `OrderBy` replace the ordering instead of appending, and add `ThenByDescending`.

   Add unit tests for each case in `MiniORM.Tests/Query/SqlBuilder/SelectQueryBuilderTests.cs`.

4. **Harden `SqlExpressionParser`.** In `MiniORM/Query/ExpressionParser/SqlExpressionParser.cs`:
   - Dispatch on `MethodInfo` (`typeof(string).GetMethod("Contains", …)`) rather than on the method name.
   - Translate `Enumerable.Contains` / `List<T>.Contains` into `IN (@p0, @p1, …)`.
   - Unwrap `ExpressionType.Convert` in `ParseUnary`.
   - When an evaluated value is `null`, emit `IS NULL` / `IS NOT NULL`.
   - Escape `%`, `_` and `\` in LIKE values, using `ESCAPE '\'`.
   - Replace `throw new Exception` with `NotSupportedException`.
   - Replace `Compile().DynamicInvoke()` with direct `FieldInfo`/`PropertyInfo` evaluation of closures.

   Cover each case in `MiniORM.Tests/Query/ExpressionParser/ExpressionParserTests.cs`.

5. **Tighten the mapping and execution infrastructure.**
   - `MiniORM/Helpers/EntityMetaDataHelper.cs`: make `GetInsertableProperties` exclude whatever `GetPrimaryKeyProperty` returns, and add a `[NotMapped]` attribute under `MiniORM/Mapping/`.
   - `MiniORM/Infrastructure/DbExecutor.cs`: log completion *after* `ExecuteNonQuery` and on null scalar results, and use `Stopwatch` instead of `DateTime.UtcNow`.
   - `MiniORM/Logging/ConsoleORMLogger.cs`: add an option to redact parameter values.
   - `MiniORM/MiniORM.csproj`: move off the preview `Microsoft.Data.Sqlite.Core` to a stable release.
