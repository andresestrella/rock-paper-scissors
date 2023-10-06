namespace rps_server.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    public string UserName { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; } = "placeholder until auth is implemented";

    public ICollection<Match> Matches { get; } = new List<Match>();
}
