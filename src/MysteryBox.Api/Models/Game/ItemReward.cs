using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_item_reward")]
public class ItemReward
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("appear_num")]
    public int AppearNum { get; set; }

    [Column("count")]
    public long Count { get; set; }

    [Column("is_reward")]
    public bool IsReward { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
