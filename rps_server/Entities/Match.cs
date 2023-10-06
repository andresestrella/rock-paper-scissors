namespace rps_server.Entities;

using System.ComponentModel.DataAnnotations;

public class Match
{
    [Key]
    public int MatchId { get; set; }

    // Enumeration for the possible match results
    public enum MatchResult
    {
        Win,
        Lose,
        Draw
    }

    [Required]
    public MatchResult Result { get; set; }

    [Required]
    public DateTime Date { get; set; }

    // Foreign key to associate a User with a Match
    public int UserId { get; set; }

    // Navigation property for the User associated with this Match
    // EF discovers the relationship by convention
    public User User { get; set; } = null!;
}
