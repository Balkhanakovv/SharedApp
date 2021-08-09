using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SharedApp
{
    public partial class sharefiledbContext : DbContext
    {
        public sharefiledbContext()
        {
        }

        public sharefiledbContext(DbContextOptions<sharefiledbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccessRightsTbl> AccessRightsTbls { get; set; }
        public virtual DbSet<BanListTbl> BanListTbls { get; set; }
        public virtual DbSet<BansTbl> BansTbls { get; set; }
        public virtual DbSet<CountriesTbl> CountriesTbls { get; set; }
        public virtual DbSet<DocumentsTbl> DocumentsTbls { get; set; }
        public virtual DbSet<FileTypeTbl> FileTypeTbls { get; set; }
        public virtual DbSet<FilesTbl> FilesTbls { get; set; }
        public virtual DbSet<TrafficPlanTbl> TrafficPlanTbls { get; set; }
        public virtual DbSet<TransactionsTbl> TransactionsTbls { get; set; }
        public virtual DbSet<UserLevelsTbl> UserLevelsTbls { get; set; }
        public virtual DbSet<UsersTbl> UsersTbls { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-8USLB37\\SQLEXPRESS;Database=sharefiledb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessRightsTbl>(entity =>
            {
                entity.HasKey(e => e.RightId)
                    .HasName("PK_Right");

                entity.ToTable("AccessRights_tbl");

                entity.HasIndex(e => e.AccessRight, "UQ__AccessRi__B98F020F65731EB2")
                    .IsUnique();

                entity.Property(e => e.AccessRight)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BanListTbl>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BanList_tbl");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.HasOne(d => d.Ban)
                    .WithMany()
                    .HasForeignKey(d => d.BanId)
                    .HasConstraintName("FK__BanList_t__BanId__3E52440B");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__BanList_t__UserI__3D5E1FD2");
            });

            modelBuilder.Entity<BansTbl>(entity =>
            {
                entity.HasKey(e => e.BanId)
                    .HasName("PK_Ban");

                entity.ToTable("Bans_tbl");

                entity.HasIndex(e => e.BanName, "UQ__Bans_tbl__0B6236FE8648E6DE")
                    .IsUnique();

                entity.Property(e => e.BanName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CountriesTbl>(entity =>
            {
                entity.HasKey(e => e.CountryId)
                    .HasName("PK_Country");

                entity.ToTable("Countries_tbl");

                entity.HasIndex(e => e.CountryName, "UQ__Countrie__E056F2015C0226EF")
                    .IsUnique();

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DocumentsTbl>(entity =>
            {
                entity.HasKey(e => e.DocumentId)
                    .HasName("PK_DOCUMENT");

                entity.ToTable("Documents_tbl");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.HasOne(d => d.File)
                    .WithMany(p => p.DocumentsTbls)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK__Documents__FileI__37A5467C");

                entity.HasOne(d => d.Right)
                    .WithMany(p => p.DocumentsTbls)
                    .HasForeignKey(d => d.RightId)
                    .HasConstraintName("FK__Documents__Right__38996AB5");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DocumentsTbls)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Documents__UserI__36B12243");
            });

            modelBuilder.Entity<FileTypeTbl>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_Type");

                entity.ToTable("FileType_tbl");

                entity.HasIndex(e => e.TypeName, "UQ__FileType__D4E7DFA8EC3AB273")
                    .IsUnique();

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FilesTbl>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PK_File");

                entity.ToTable("Files_tbl");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.FileBin).IsRequired();

                entity.Property(e => e.FileNam)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.FilesTbls)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__Files_tbl__TypeI__33D4B598");
            });

            modelBuilder.Entity<TrafficPlanTbl>(entity =>
            {
                entity.HasKey(e => e.PlanId)
                    .HasName("PK_Plan");

                entity.ToTable("TrafficPlan_tbl");

                entity.HasIndex(e => e.TrafficPlan, "UQ__TrafficP__D21AEA0264F474A2")
                    .IsUnique();
            });

            modelBuilder.Entity<TransactionsTbl>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK_Transaction");

                entity.ToTable("Transactions_tbl");

                entity.Property(e => e.Receiver).HasMaxLength(50);

                entity.Property(e => e.Sender).HasMaxLength(50);

                entity.Property(e => e.TransactionTime).HasColumnType("datetime");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.TransactionsTbls)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK__Transacti__Docum__4316F928");

                entity.HasOne(d => d.ReceiverNavigation)
                    .WithMany(p => p.TransactionsTblReceiverNavigations)
                    .HasForeignKey(d => d.Receiver)
                    .HasConstraintName("FK__Transacti__Recei__4222D4EF");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.TransactionsTblSenderNavigations)
                    .HasForeignKey(d => d.Sender)
                    .HasConstraintName("FK__Transacti__Sende__412EB0B6");
            });

            modelBuilder.Entity<UserLevelsTbl>(entity =>
            {
                entity.HasKey(e => e.LevelId)
                    .HasName("PK_Level");

                entity.ToTable("UserLevels_tbl");

                entity.Property(e => e.LevelName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UsersTbl>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users_tb__1788CC4CBD8A3FB6");

                entity.ToTable("Users_tbl");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.Passwd)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.UsersTbls)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__Users_tbl__Count__2A4B4B5E");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.UsersTbls)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK__Users_tbl__Level__45F365D3");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.UsersTbls)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK__Users_tbl__PlanI__2B3F6F97");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
