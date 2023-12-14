using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Modelos.Auth;
using PruebasBackendModelo.Modelo;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PruebasBackendDomain.Context
{
    public partial class PruebasBackendContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public PruebasBackendContext()
        {
        }

        public PruebasBackendContext(DbContextOptions<PruebasBackendContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Esquema> Esquema { get; set; }
        public virtual DbSet<Elemento> Elemento { get; set; }
        public virtual DbSet<TipoElemento> TipoElemento { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=192.168.30.243,55555;Database=BVData;User Id=Juanjo;Password=J12345678-j;Persist Security Info=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Elemento>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CoordX).HasColumnName("coordX");

                entity.Property(e => e.CoordY).HasColumnName("coordY");

                entity.Property(e => e.Fill)
                    .HasColumnName("fill")
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.Fonts)
                    .HasColumnName("fonts")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.IdEsquema).HasColumnName("idEsquema");

                entity.Property(e => e.IdTipoElemento).HasColumnName("idTipoElemento");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rellenado).HasColumnName("rellenado");

                entity.Property(e => e.SizeLetter).HasColumnName("sizeLetter");

                entity.Property(e => e.Stroke)
                    .HasColumnName("stroke")
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.StrokeWidth).HasColumnName("strokeWidth");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Width).HasColumnName("width");

                entity.Property(e => e.X1).HasColumnName("x1");

                entity.Property(e => e.X2).HasColumnName("x2");

                entity.Property(e => e.Y1).HasColumnName("y1");

                entity.Property(e => e.Y2).HasColumnName("y2");

                entity.Property(e => e.ScaleX).HasColumnName("scaleX");

                entity.Property(e => e.ScaleY).HasColumnName("scaleY");

                entity.Property(e => e.Angle).HasColumnName("angle");

                entity.Property(e => e.EstadoBorde).HasColumnName("estadoBorde");



                entity.HasOne(d => d.IdEsquemaNavigation)
                    .WithMany(p => p.Elemento)
                    .HasForeignKey(d => d.IdEsquema)
                    .HasConstraintName("FK_Elemento_Esquema");

                entity.HasOne(d => d.IdTipoElementoNavigation)
                    .WithMany(p => p.Elemento)
                    .HasForeignKey(d => d.IdTipoElemento)
                    .HasConstraintName("FK_Elemento_TipoElemento");
            });

            modelBuilder.Entity<TipoElemento>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(50);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasMaxLength(15);
            });

            modelBuilder.Entity<Esquema>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FechaAlta)
                    .HasColumnName("fechaAlta")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.Descripcion).HasColumnName("descripcion");


                //entity.HasOne<AspNetUsers>()
                //   .WithMany()
                //   .HasForeignKey(d => d.IdUsuario)
                //   .HasConstraintName("FK_Esquema_AspNetUsers");

            });

           

            OnModelCreatingPartial(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("AspNetUsers", "users");
            modelBuilder.Entity<AppRole>().ToTable("AspNetRoles", "users");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims", "users");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims", "users");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins", "users");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles", "users");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens", "users");

            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
