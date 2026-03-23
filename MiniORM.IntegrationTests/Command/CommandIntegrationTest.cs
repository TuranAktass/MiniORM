using MiniORM.IntegrationTests.Models;

namespace MiniORM.IntegrationTests.Command;

public class CommandIntegrationTest
{
    [Fact]
    public void Insert_PersistsUserInDatabase()
    {
        var context = TestDbContextFactory.Create();
        var guid = Guid.NewGuid();
        var email = $"{guid}@test.com";

        var user = new User
        {
            FirstName = $"Test-{guid}",
            LastName = $"Test-{guid}",
            Email = email,
            IsActive = true
        };

        var insertResult = context.Insert(user);

        var result = context.Queryable<User>()
            .Where(x => x.Email == email)
            .ToList();

        Assert.Equal(1, insertResult);
        Assert.Single(result);

        var insertedUser = result.Single();
        Assert.Equal(user.FirstName, insertedUser.FirstName);
        Assert.Equal(user.LastName, insertedUser.LastName);
        Assert.Equal(user.Email, insertedUser.Email);
        Assert.Equal(user.IsActive, insertedUser.IsActive);
    }

    [Fact]
    public void Update_ChangesExistingUserInDatabase()
    {
        var context = TestDbContextFactory.Create();
        var guid = Guid.NewGuid();
        var email = $"{guid}@test.com";

        var user = new User
        {
            FirstName = $"Test-{guid}",
            LastName = $"Test-{guid}",
            Email = email,
            IsActive = true
        };

        var insertResult = context.Insert(user);

        var insertedUser = context.Queryable<User>()
            .Where(x => x.Email == email)
            .FirstOrDefault();

        Assert.NotNull(insertedUser);

        var updatedUser = new User
        {
            Id = insertedUser!.Id,
            FirstName = $"TestUpdated-{guid}",
            LastName = $"TestUpdated-{guid}",
            Email = email,
            IsActive = false
        };

        var updateResult = context.Update(updatedUser);

        var result = context.Queryable<User>()
            .Where(x => x.Id == insertedUser.Id)
            .FirstOrDefault();

        Assert.Equal(1, insertResult);
        Assert.Equal(1, updateResult);
        Assert.NotNull(result);

        Assert.Equal(updatedUser.Id, result!.Id);
        Assert.Equal(updatedUser.FirstName, result.FirstName);
        Assert.Equal(updatedUser.LastName, result.LastName);
        Assert.Equal(updatedUser.Email, result.Email);
        Assert.Equal(updatedUser.IsActive, result.IsActive);
    }
    
    [Fact]
    public void Delete_RemovesExistingUserFromDatabase()
    {
        var context = TestDbContextFactory.Create();
        var guid = Guid.NewGuid();
        var email = $"{guid}@test.com";

        var user = new User
        {
            FirstName = $"Test-{guid}",
            LastName = $"Test-{guid}",
            Email = email,
            IsActive = true
        };

        var insertResult = context.Insert(user);

        var insertedUser = context.Queryable<User>()
            .Where(x => x.Email == email)
            .FirstOrDefault();

        Assert.Equal(1, insertResult);
        Assert.NotNull(insertedUser);

        var deleteResult = context.Delete(new User
        {
            Id = insertedUser!.Id
        });

        var resultAfterDelete = context.Queryable<User>()
            .Where(x => x.Id == insertedUser.Id)
            .ToList();

        Assert.Equal(1, deleteResult);
        Assert.Empty(resultAfterDelete);
    }
}