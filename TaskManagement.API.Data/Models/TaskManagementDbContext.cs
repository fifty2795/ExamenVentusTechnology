using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.API.Data.Models;

public partial class TaskManagementDbContext : DbContext
{
    public TaskManagementDbContext()
    {
    }

    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditoriaEstatusTarea> AuditoriaEstatusTareas { get; set; }

    public virtual DbSet<CatEstatus> CatEstatuses { get; set; }

    public virtual DbSet<CatPrioridade> CatPrioridades { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ALEJANDRO\\SQLEXPRESS;Database=TaskManagementDb;Trusted_Connection=True; TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditoriaEstatusTarea>(entity =>
        {
            entity.HasKey(e => e.IdAuditoria);

            entity.ToTable("AuditoriaEstatusTarea");

            entity.HasIndex(e => new { e.IdTarea, e.FechaCambio }, "IX_AuditoriaEstatusTarea_IdTarea_FechaCambio").IsDescending(false, true);

            entity.Property(e => e.FechaCambio)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UsuarioBaseDatos)
                .HasMaxLength(128)
                .HasDefaultValueSql("(suser_sname())");

            entity.HasOne(d => d.IdEstatusAnteriorNavigation).WithMany(p => p.AuditoriaEstatusTareaIdEstatusAnteriorNavigations)
                .HasForeignKey(d => d.IdEstatusAnterior)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditoriaEstatusTarea_EstatusAnterior");

            entity.HasOne(d => d.IdEstatusNuevoNavigation).WithMany(p => p.AuditoriaEstatusTareaIdEstatusNuevoNavigations)
                .HasForeignKey(d => d.IdEstatusNuevo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditoriaEstatusTarea_EstatusNuevo");

            entity.HasOne(d => d.IdTareaNavigation).WithMany(p => p.AuditoriaEstatusTareas)
                .HasForeignKey(d => d.IdTarea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditoriaEstatusTarea_Tareas");
        });

        modelBuilder.Entity<CatEstatus>(entity =>
        {
            entity.HasKey(e => e.IdEstatus);

            entity.ToTable("CatEstatus");

            entity.HasIndex(e => e.Nombre, "UQ_CatEstatus_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<CatPrioridade>(entity =>
        {
            entity.HasKey(e => e.IdPrioridad);

            entity.HasIndex(e => e.Nombre, "UQ_CatPrioridades_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.IdTarea);

            entity.ToTable(tb => tb.HasTrigger("trg_Tareas_AuditoriaEstatus"));

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IdEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tareas_CatEstatus");

            entity.HasOne(d => d.IdPrioridadNavigation).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IdPrioridad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tareas_CatPrioridades");

            entity.HasOne(d => d.IdUsuarioResponsableNavigation).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IdUsuarioResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tareas_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.HasIndex(e => e.CorreoElectronico, "UQ_Usuarios_CorreoElectronico").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.NombreCompleto).HasMaxLength(150);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasDefaultValue("");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
