
using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure (EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
        .HasColumnName("username")
        .HasColumnType("varchar")
        .HasMaxLength(50)
        .IsRequired();
        
        builder.Property(x => x.Password)
        .HasColumnName("password")
        .HasColumnType("varchar")
        .HasMaxLength(50)
        .IsRequired();

        
        builder.Property(x => x.UserEmail)
        .HasColumnName("email")
        .HasColumnType("varchar")
        .HasMaxLength(50)
        .IsRequired();

        builder.HasMany(r => r.Rols)
        .WithMany(u => u.Users)
        .UsingEntity<UserRol>(
            j => j 
                .HasOne(pt => pt.Rol)
                .WithMany(t => t.UsersRols)
                .HasForeignKey(ut => ut.IdRolFk),

            j => j
                .HasOne(et => et.User)
                .WithMany(t => t.UsersRols)
                .HasForeignKey(el => el.IdUserFk),
            j => 
            {
                j.ToTable("userRol");
                j.HasKey(t => new { t.IdUserFk, t.IdRolFk});
            }
        );
    }
}
