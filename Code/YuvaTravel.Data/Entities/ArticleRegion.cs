using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class ArticleRegion
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }

        public Guid RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        public UserLog UserLog { get; set; }

        public ArticleRegion()
        {
            UserLog = new UserLog();
        }
    }
}
