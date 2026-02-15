using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SuperUU.Dto;

namespace SuperUU.Context;

public partial class SuperUUContext : DbContext
{
    public SuperUUContext()
    {
    }

    public SuperUUContext(DbContextOptions<SuperUUContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=18.177.57.10;port=3306;database=superUU;user=npa_user;password=S!lver2024@DB;allowpublickeyretrieval=True;sslmode=None", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users", tb => tb.HasComment("使用者帳號資料表"));

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("使用者唯一ID")
                .HasColumnName("id");
            entity.Property(e => e.Birthday)
                .HasComment("生日")
                .HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("建立時間")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("電子郵件")
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasComment("姓名")
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasComment("性別")
                .HasColumnType("enum('male','female','other')")
                .HasColumnName("gender");
            entity.Property(e => e.LastLoginAt)
                .HasComment("最後登入時間")
                .HasColumnType("datetime")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LoginFailCount)
                .HasComment("登入失敗次數")
                .HasColumnName("login_fail_count");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasComment("加密後的密碼(例如bcrypt)")
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasComment("手機號碼")
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'user'")
                .HasComment("使用者角色")
                .HasColumnType("enum('user','admin')")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("帳號狀態 1=正常 0=停用")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("更新時間")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasComment("登入帳號")
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserLoginLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_login_logs", tb => tb.HasComment("使用者登入紀錄表"));

            entity.HasIndex(e => e.CreatedAt, "idx_created_at");

            entity.HasIndex(e => e.UserId, "idx_user_id");

            entity.HasIndex(e => e.Username, "idx_username");

            entity.Property(e => e.Id)
                .HasComment("登入紀錄ID")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("登入時間")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FailReason)
                .HasMaxLength(100)
                .HasComment("失敗原因(密碼錯誤/帳號不存在/鎖定等)")
                .HasColumnName("fail_reason");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasComment("登入IP位址(支援IPv4/IPv6)")
                .HasColumnName("ip_address");
            entity.Property(e => e.LoginStatus)
                .HasComment("登入結果 1=成功 0=失敗")
                .HasColumnName("login_status");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasComment("瀏覽器/裝置資訊")
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId)
                .HasComment("對應使用者ID(成功登入時必填)")
                .HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasComment("嘗試登入帳號(保留當時輸入資料)")
                .HasColumnName("username");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
