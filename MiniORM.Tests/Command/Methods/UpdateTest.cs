using MiniORM.Query;
using MiniORM.Tests.TestModels;

namespace MiniORM.Tests.Command.Methods;

public class UpdateTest
{
    [Fact]
    public void Update_ReturnsCorrectSql()
    {
        var sql = Update.BuildUpdateSql<User>();

        Assert.Equal(
            "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, IsActive = @IsActive WHERE Id = @Id",
            sql);
    }
}