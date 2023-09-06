
using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;

public class RolConfiguration: IEntityTypeConfiguration<Rol>
{
    public void Configure (EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("rol");
        builder.HasKey(x => x.Id);

        
        builder.Property(x => x.Nombre)
        .HasColumnName("nombre")
        .HasColumnType("varchar")
        .HasMaxLength(50)
        .IsRequired();


    }
}
