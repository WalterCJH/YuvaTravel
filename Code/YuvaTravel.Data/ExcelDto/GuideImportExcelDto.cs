namespace YuvaTravel.Data.ExcelDto
{
    public class GuideImportExcelDto
    {
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public int YearsOfExperience { get; set; }
        public string Languages { get; set; }
        public string Bio { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string ServiceFormat { get; set; }
        public int? StartingPrice { get; set; }
        public string PortraitUrl { get; set; }
        public string CoverUrl { get; set; }
        public int DisplaySeq { get; set; }
    }
}
