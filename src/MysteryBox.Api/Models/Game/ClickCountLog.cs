using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_click_count_log")]
public class ClickCountLog
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("log_date")]
    public long LogDate { get; set; }

    [Column("click_count")]
    public int ClickCount { get; set; }

    [Column("total_count")]
    public long TotalCount { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
