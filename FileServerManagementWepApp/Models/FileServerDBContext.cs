using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class FileServerDBContext : DbContext
    {
        public FileServerDBContext()
        {
        }

        public FileServerDBContext(DbContextOptions<FileServerDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAccess> TblAccesses { get; set; }
        public virtual DbSet<TblFile> TblFiles { get; set; }
        public virtual DbSet<TblFileType> TblFileTypes { get; set; }
        public virtual DbSet<TblLog> TblLogs { get; set; }
        public virtual DbSet<TblServer> TblServers { get; set; }
        public virtual DbSet<TblSubSystem> TblSubSystems { get; set; }
        public virtual DbSet<TblSystem> TblSystems { get; set; }
        public virtual DbSet<TblUser> TblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=FileServerDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<TblAccess>(entity =>
            {
                entity.ToTable("TblAccess");

                entity.Property(e => e.FileTypeId).HasColumnName("FileType_Id");

                entity.Property(e => e.ServerId).HasColumnName("Server_Id");

                entity.Property(e => e.SubSystemId).HasColumnName("SubSystem_Id");

                entity.HasOne(d => d.FileType)
                    .WithMany(p => p.TblAccesses)
                    .HasForeignKey(d => d.FileTypeId)
                    .HasConstraintName("FK_TblAccess_TblFileType");

                entity.HasOne(d => d.Server)
                    .WithMany(p => p.TblAccesses)
                    .HasForeignKey(d => d.ServerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TblAccess_TblServer");

                entity.HasOne(d => d.SubSystem)
                    .WithMany(p => p.TblAccesses)
                    .HasForeignKey(d => d.SubSystemId)
                    .HasConstraintName("FK_TblAccess_TblSubSystem");
            });

            modelBuilder.Entity<TblFile>(entity =>
            {
                entity.ToTable("TblFile");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.FileTypeId).HasColumnName("FileType_Id");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.ServerId).HasColumnName("Server_Id");

                entity.Property(e => e.SubSystemId).HasColumnName("SubSystem_Id");

                entity.Property(e => e.SystemId).HasColumnName("System_Id");

                entity.HasOne(d => d.FileType)
                    .WithMany(p => p.TblFiles)
                    .HasForeignKey(d => d.FileTypeId)
                    .HasConstraintName("FK_TblFile_TblFileType");

                entity.HasOne(d => d.Server)
                    .WithMany(p => p.TblFiles)
                    .HasForeignKey(d => d.ServerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TblFile_TblServer");

                entity.HasOne(d => d.SubSystem)
                    .WithMany(p => p.TblFiles)
                    .HasForeignKey(d => d.SubSystemId)
                    .HasConstraintName("FK_TblFile_TblSubSystem");

                entity.HasOne(d => d.System)
                    .WithMany(p => p.TblFiles)
                    .HasForeignKey(d => d.SystemId)
                    .HasConstraintName("FK_TblFile_TblSystem");
            });

            modelBuilder.Entity<TblFileType>(entity =>
            {
                entity.ToTable("TblFileType");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<TblLog>(entity =>
            {
                entity.ToTable("TblLog");

                entity.Property(e => e.Action).HasMaxLength(50);

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.FileId).HasColumnName("File_Id");

                entity.Property(e => e.ServerId).HasColumnName("Server_Id");

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_TblLog_TblUser");
            });

            modelBuilder.Entity<TblServer>(entity =>
            {
                entity.ToTable("TblServer");

                entity.Property(e => e.Address).HasMaxLength(2000);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.ServerPassword).HasMaxLength(150);

                entity.Property(e => e.ServerUsername).HasMaxLength(150);
            });

            modelBuilder.Entity<TblSubSystem>(entity =>
            {
                entity.ToTable("TblSubSystem");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.System)
                    .WithMany(p => p.TblSubSystems)
                    .HasForeignKey(d => d.SystemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TblSubSystem_TblSystem");
            });

            modelBuilder.Entity<TblSystem>(entity =>
            {
                entity.ToTable("TblSystem");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.ToTable("TblUser");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.Password).HasMaxLength(150);

                entity.Property(e => e.Token).HasMaxLength(150);

                entity.Property(e => e.Username).HasMaxLength(150);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
