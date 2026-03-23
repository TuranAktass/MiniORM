using MiniORM.Query;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests.Command.Methods;

public class InsertTest
{
    [Fact]
    public void Insert_ReturnsCorrectSql()
    {
        var sql = Insert.BuildInsertSql<User>();

        Assert.Equal(
            "INSERT INTO Users (FirstName, LastName, Email, IsActive) VALUES (@FirstName, @LastName, @Email, @IsActive)",
            sql);
    }
}