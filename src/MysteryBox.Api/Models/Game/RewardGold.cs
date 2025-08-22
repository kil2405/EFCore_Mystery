using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_reward_gold")]
public class RewardGold
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("gold")]
    public int Gold { get; set; }

    [Column("reward_time")]
    public long RewardTime { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
