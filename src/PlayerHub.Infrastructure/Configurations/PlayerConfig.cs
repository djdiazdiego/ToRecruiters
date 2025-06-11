using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayerHub.Domain;

namespace PlayerHub.Infrastructure.Configurations
{
    internal sealed class PlayerConfig : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            builder.HasMany(x => x.Skills)
                .WithMany()
                .UsingEntity(
                    "PlayerSkill",
                    l => l.HasOne(typeof(Skill))
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .HasPrincipalKey(nameof(Skill.Id)),
                    r => r.HasOne(typeof(Player))
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .HasPrincipalKey(nameof(Player.Id)),
                    x => x.HasKey("PlayerId", "SkillId"));

            builder.ToTable("Player");
        }
    }
}
