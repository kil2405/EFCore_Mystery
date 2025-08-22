using Microsoft.EntityFrameworkCore;
using MysteryBox.Api.Models.Game;

namespace MysteryBox.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Game tables
    public DbSet<User> Users => Set<User>();
    public DbSet<UserItem> UserItems => Set<UserItem>();
    public DbSet<UserConnectLog> UserConnectLogs => Set<UserConnectLog>();
    public DbSet<ClickCountLog> ClickCountLogs => Set<ClickCountLog>();
    public DbSet<GoldAcqLog> GoldAcqLogs => Set<GoldAcqLog>();
    public DbSet<ItemAcqLog> ItemAcqLogs => Set<ItemAcqLog>();
    public DbSet<ItemReward> ItemRewards => Set<ItemReward>();
    public DbSet<RewardGold> RewardGolds => Set<RewardGold>();
    public DbSet<GlobalRanking> GlobalRankings => Set<GlobalRanking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite keys
        modelBuilder.Entity<UserItem>().HasKey(x => new { x.UserId, x.ItemId });
        modelBuilder.Entity<UserConnectLog>().HasKey(x => new { x.UserId, x.LogDate });
        modelBuilder.Entity<ClickCountLog>().HasKey(x => new { x.UserId, x.LogDate });
        modelBuilder.Entity<GoldAcqLog>().HasKey(x => new { x.UserId, x.LogDate });
        modelBuilder.Entity<ItemAcqLog>().HasKey(x => new { x.UserId, x.LogDate });
        modelBuilder.Entity<ItemReward>().HasKey(x => new { x.UserId, x.AppearNum });
        modelBuilder.Entity<GlobalRanking>().HasKey(x => x.Region);
        modelBuilder.Entity<RewardGold>().HasKey(x => x.UserId);

        // Indexes
        modelBuilder.Entity<User>().HasIndex(x => x.Nickname).IsUnique(false);
        modelBuilder.Entity<User>().HasIndex(x => x.DeviceId).IsUnique();

        // Table names map to original SQL
        modelBuilder.Entity<User>().ToTable("tb_user");
        modelBuilder.Entity<UserItem>().ToTable("tb_user_item");
        modelBuilder.Entity<UserConnectLog>().ToTable("tb_user_connect_log");
        modelBuilder.Entity<ClickCountLog>().ToTable("tb_click_count_log");
        modelBuilder.Entity<GoldAcqLog>().ToTable("tb_gold_acq_log");
        modelBuilder.Entity<ItemAcqLog>().ToTable("tb_item_acq_log");
        modelBuilder.Entity<ItemReward>().ToTable("tb_item_reward");
        modelBuilder.Entity<RewardGold>().ToTable("tb_reward_gold");
        modelBuilder.Entity<GlobalRanking>().ToTable("tb_global_ranking");
    }
}
