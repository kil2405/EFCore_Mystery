using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MysteryBox.Api.Models.Game;

[Table("tb_user")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(64)]
    [Column("device_id")]
    public string DeviceId { get; set; } = default!;

    [MaxLength(40)]
    [Column("nickname")]
    public string? Nickname { get; set; }

    [MaxLength(8)]
    [Column("region")]
    public string Region { get; set; } = "KR";

    [Column("gold")]
    public long Gold { get; set; } = 0;

    [Column("click_count")]
    public long ClickCount { get; set; } = 0;

    [Column("terms_agree")]
    public bool TermsAgree { get; set; } = false;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
