using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_global_ranking")]
public class GlobalRanking
{
    [Key]
    [MaxLength(8)]
    [Column("region")]
    public string Region { get; set; } = "KR";

    [Column("count")]
    public long Count { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
