using MiniORM.Mapping;

namespace MiniORM.Tests.TestModels;

[Table("Users")]
public class User
{
    [Key] [Column("Id")] public int Id { get; set; }

    [Column("FirstName")] public string FirstName { get; set; } = "";

    [Column("LastName")] public string? LastName { get; set; }

    [Column("Email")] public string Email { get; set; } = "";

    [Column("IsActive")] public bool IsActive { get; set; }

    [Column("PhoneNumber")] public string Phone { get; set; }


    public override string ToString()
    {
        return $"{Id} | {FirstName} {LastName} | {Email} | {IsActive}";
    }
}