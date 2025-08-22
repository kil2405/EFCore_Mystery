using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_gold_acq_log")]
public class GoldAcqLog
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("log_date")]
    public long LogDate { get; set; }

    [Column("add_value")]
    public int AddValue { get; set; }

    [Column("total_value")]
    public long TotalValue { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
