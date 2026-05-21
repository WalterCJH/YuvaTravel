namespace YuvaTravel.Infrastructure.Storage
{
    /// <summary>Cloudflare R2 設定,綁定 appsettings.json 的 Storage:R2 區塊</summary>
    public class R2Options
    {
        /// <summary>啟用 R2 (false 時 fallback 回本機 wwwroot)</summary>
        public bool Enabled { get; set; }

        /// <summary>R2 account id (URL 中間那段)</summary>
        public string AccountId { get; set; } = "";

        /// <summary>R2 API token Access Key</summary>
        public string AccessKeyId { get; set; } = "";

        /// <summary>R2 API token Secret</summary>
        public string SecretAccessKey { get; set; } = "";

        /// <summary>Bucket 名稱,例 'yuvatravel'</summary>
        public string BucketName { get; set; } = "";

        /// <summary>R2 S3-相容 endpoint,例 https://{accountId}.r2.cloudflarestorage.com</summary>
        public string Endpoint { get; set; } = "";

        /// <summary>公開讀取的網址 (自訂域名或 r2.dev),不含結尾 /</summary>
        public string PublicUrl { get; set; } = "";

        /// <summary>WebP 編碼品質 (0–100),預設 80</summary>
        public int WebpQuality { get; set; } = 80;
    }
}
