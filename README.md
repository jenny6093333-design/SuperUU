# SuperUU

SuperUU 是一個使用 ASP.NET Core Razor Pages 建置的會員系統範例專案，提供註冊、登入、登出、忘記密碼與重設密碼等基本帳號流程，並以 MySQL 做資料儲存。

## 專案目標

- 建立完整的會員帳號流程
- 使用 Cookie Authentication 管理登入狀態
- 使用 EF Core 連接 MySQL 並操作使用者資料

## 技術堆疊

- .NET 8 (`Microsoft.NET.Sdk.Web`)
- ASP.NET Core Razor Pages
- Entity Framework Core 8
- Pomelo.EntityFrameworkCore.MySql 8
- Cookie Authentication
- Bootstrap + jQuery（前端樣式與基礎互動）

## 目前主要功能

- `註冊`：建立使用者，密碼以雜湊儲存，成功後自動登入
- `登入`：驗證帳號密碼，登入成功後寫入 Claims 與登入紀錄
- `登出`：清除 Cookie 身分
- `忘記密碼`：產生重設 token，資料庫儲存 token hash（有效 30 分鐘）
- `重設密碼`：驗證 token 後更新密碼，並將 token 標記為已使用
- `會員中心`：需登入才能存取（`[Authorize]`）
- `DB 測試頁`：測試資料庫連線是否正常

## 主要資料表（對應模型）

- `users`（`Dto/User.cs`）
- `user_login_logs`（`Dto/UserLoginLog.cs`）
- `user_password_resets`（`Dto/UserPasswordReset.cs`）

> 主要啟用的 DbContext 為 `Data/AppDbContext.cs`。

## 重要路徑

- 啟動與服務註冊：`Program.cs`
- 資料庫設定：`Data/AppDbContext.cs`
- 首頁：`Pages/Index.cshtml`
- 註冊：`Pages/Register.cshtml` / `Pages/Register.cshtml.cs`
- 登入：`Pages/Login.cshtml` / `Pages/Login.cshtml.cs`
- 忘記密碼：`Pages/ForgotPassword.cshtml` / `Pages/ForgotPassword.cshtml.cs`
- 重設密碼：`Pages/ResetPassword.cshtml` / `Pages/ResetPassword.cshtml.cs`
- 會員中心：`Pages/Dashboard.cshtml` / `Pages/Dashboard.cshtml.cs`
- 登出：`Pages/Logout.cshtml.cs`

## 本機開發啟動

1. 安裝 .NET 8 SDK
2. 設定資料庫連線（`appsettings.json` 的 `ConnectionStrings:DefaultConnection`）
3. 在專案根目錄執行：

```powershell
dotnet restore
dotnet run
```

4. 開啟瀏覽器進入：
   - `https://localhost:7051`
   - 或 `http://localhost:5035`

## 目前已知狀態與建議

- 部分頁面與錯誤訊息有中文亂碼（疑似檔案編碼不一致）：
  - `Pages/ResetPassword.cshtml`
  - `Pages/Dashboard.cshtml`
  - 部分 `*.cshtml.cs` 驗證訊息字串
- 連線字串目前存在於 `appsettings.json`，建議改用環境變數或 Secret Manager 管理。
- `Context/SuperUUContext.cs` 看起來是舊 scaffold 產物，且包含硬編碼連線字串；若未使用，建議整理或移除以避免混淆。

## 後續可優先處理

1. 先修正所有頁面與程式碼中文字亂碼（統一 UTF-8）
2. 將敏感設定（DB 帳密）移出版本控管
3. 補上登入失敗紀錄與帳號鎖定策略
4. 增加基本測試（登入、重設密碼流程）
