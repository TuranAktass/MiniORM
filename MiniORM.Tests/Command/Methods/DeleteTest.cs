using MiniORM.Query;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests.Command.Methods;

public class DeleteTest
{
    [Fact]
    public void Delete_ReturnsCorrectSql()
    {
        var sql = Delete.BuildDeleteSql<User>();

        Assert.Equal("DELETE FROM Users WHERE Id = @Id",
            sql);
    }
}