using System;

namespace SuperUU.Dto;

/// <summary>
/// 使用者密碼重設紀錄
/// </summary>
public partial class UserPasswordReset
{
    /// <summary>
    /// 重設紀錄ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Token 雜湊值 (SHA256 Hex)
    /// </summary>
    public string TokenHash { get; set; } = null!;

    /// <summary>
    /// Token 失效時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token 使用時間
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// 申請 IP
    /// </summary>
    public string? RequestIp { get; set; }

    /// <summary>
    /// 申請 User Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
