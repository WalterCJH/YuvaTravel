using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YuvaTravel.Data.Dtos.UserGroups
{
    public class FuncGroupDto
    {
        public string Name { get; set; }

        [Display(Name = "功能群組勾選")]
        public bool FuncGroupCheckAll { get; set; }

        public List<FuncProgramDto> FuncPrograms { get; set; } = new List<FuncProgramDto>();
    }
}
