using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Base.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YuvaTravel.Data.Dtos.Home;

namespace YuvaTravel.Data.Dtos.RecruitAgents
{
    public class RecruitAgentEdit : RecruitAgentDto
    {
        [Display(Name = "ID")]
        public Guid RecruitAgentId { get; set; }

    }
}
