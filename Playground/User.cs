using MiniORM.Mapping;

namespace Playground;

[Table("Users")]
public class User
{
    [Key] [Column("Id")] public int Id { get; set; }

    [Column("FirstName")] public string FirstName { get; set; } = "";

    [Column("LastName")] public string? LastName { get; set; }

    [Column("Email")] public string Email { get; set; } = "";

    [Column("IsActive")] public bool IsActive { get; set; }


    public override string ToString()
    {
        return $"{Id} | {FirstName} {LastName} | {Email} | {IsActive}";
    }
}