using System.Collections.Generic;

namespace YuvaTravel.Data.Dtos.Home
{
    public class MetaDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string OgType { get; set; }
        public string OgImage { get; set; }
        public string OgUrl { get; set; }
        public string OgImageAlt { get; set; }
        public string Canonical { get; set; }
        public string Author { get; set; }
        public bool NoIndex { get; set; }

        /// <summary>JSON-LD 結構化資料區塊 (Raw 輸出)</summary>
        public List<string> JsonLdBlocks { get; set; } = new List<string>();
    }
}
