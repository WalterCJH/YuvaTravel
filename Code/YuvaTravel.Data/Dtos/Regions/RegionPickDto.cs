using System;

namespace YuvaTravel.Data.Dtos.Regions
{
    public class RegionPickDto
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public Guid RegionId { get; set; }
    }
}
