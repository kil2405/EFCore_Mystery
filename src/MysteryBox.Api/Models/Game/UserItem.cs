using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_user_item")]
public class UserItem
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("count")]
    public int Count { get; set; } = 0;

    [Column("is_equip")]
    public bool IsEquip { get; set; } = false;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; } = default!;
}
