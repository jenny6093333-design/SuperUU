using System;

namespace SuperUU.Dto;

/// <summary>
/// 使用者登入紀錄表
/// </summary>
public partial class UserLoginLog
{
    /// <summary>
    /// 登入紀錄ID
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// 對應使用者ID(成功登入時必填)
    /// </summary>
    public ulong? UserId { get; set; }

    /// <summary>
    /// 嘗試登入帳號(保留當時輸入資料)
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 登入IP位址(支援IPv4/IPv6)
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// 瀏覽器/裝置資訊
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 登入結果 1=成功 0=失敗
    /// </summary>
    public sbyte LoginStatus { get; set; }

    /// <summary>
    /// 失敗原因(密碼錯誤/帳號不存在/鎖定等)
    /// </summary>
    public string? FailReason { get; set; }

    /// <summary>
    /// 登入時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
