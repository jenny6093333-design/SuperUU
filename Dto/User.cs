using System;
using System.Collections.Generic;

namespace SuperUU.Dto;

/// <summary>
/// 使用者帳號資料表
/// </summary>
public partial class User
{
    /// <summary>
    /// 使用者唯一ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 登入帳號
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 加密後的密碼(例如bcrypt)
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 性別
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateOnly? Birthday { get; set; }

    /// <summary>
    /// 帳號狀態 1=正常 0=停用
    /// </summary>
    public sbyte Status { get; set; }

    /// <summary>
    /// 使用者角色
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 登入失敗次數
    /// </summary>
    public int LoginFailCount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<UserLoginLog> UserLoginLogs { get; set; } = new List<UserLoginLog>();
}
