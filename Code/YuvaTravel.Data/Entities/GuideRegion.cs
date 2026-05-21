using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YuvaTravel.Data.Columns;

namespace YuvaTravel.Data.Entities
{
    public class GuideRegion
    {
        [Key]
        public Guid Id { get; set; }

        public Guid GuideId { get; set; }
        [ForeignKey("GuideId")]
        public virtual Guide Guide { get; set; }

        public Guid RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        public UserLog UserLog { get; set; }

        public GuideRegion()
        {
            UserLog = new UserLog();
        }
    }
}
