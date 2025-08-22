using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_user_connect_log")]
public class UserConnectLog
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("log_date")]
    public long LogDate { get; set; }

    [Column("ip")]
    public string? Ip { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
