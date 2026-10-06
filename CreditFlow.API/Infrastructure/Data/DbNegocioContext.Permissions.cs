using CreditFlow.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditFlow.API.Infrastructure.Data;

// Mapeo de los permisos de menú por rol. Va en un archivo parcial para no tocar el DbContext generado por
// scaffold. La tabla se crea con Infrastructure/Data/Scripts/2026-10-06_RolMenuPermiso.sql.
public partial class DbNegocioContext
{
    public virtual DbSet<RolMenuPermiso> RolMenuPermisos { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolMenuPermiso>(entity =>
        {
            entity.ToTable("RolMenuPermiso");

            entity.HasKey(e => e.IdRolMenuPermiso);

            entity.HasIndex(e => new { e.IdRol, e.CClaveMenu }, "UQ_RolMenuPermiso_Rol_Clave").IsUnique();

            entity.Property(e => e.IdRolMenuPermiso).HasColumnName("idRolMenuPermiso");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.CClaveMenu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cClaveMenu");
            entity.Property(e => e.BVer).HasColumnName("bVer");
            entity.Property(e => e.BCrear).HasColumnName("bCrear");
            entity.Property(e => e.BEditar).HasColumnName("bEditar");
            entity.Property(e => e.BEliminar).HasColumnName("bEliminar");
            entity.Property(e => e.DFechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dFechaModificacion");

            entity.HasOne(d => d.IdRolNavigation).WithMany()
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RolMenuPermiso_Roles");
        });
    }
}
