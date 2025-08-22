using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_item_acq_log")]
public class ItemAcqLog
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("log_date")]
    public long LogDate { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("add_count")]
    public int AddCount { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
