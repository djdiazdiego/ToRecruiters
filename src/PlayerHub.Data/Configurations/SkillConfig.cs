using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayerHub.Domain;

namespace PlayerHub.Data.Configurations
{
    internal sealed class SkillConfig : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Value);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(40);

            builder.ToTable("Skill");
        }
    }
}
