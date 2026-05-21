using System.Collections.Generic;
using System.Linq;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Base
{
    public class NavBarDto
    {
        public string FAQ_Url { get; set; } = "/FAQ";
        public string CustomerService_Url { get; set; } = "/CustomerService";
        public List<CategoryDto> AllCategories { get; set; } = new List<CategoryDto>();

        public NavBarDto(string host, string path, string query, List<ArticleCategory> articleCategories, string goRegister)
        {
            foreach (var articleCategory in articleCategories.OrderBy(p => p.DisplaySeq).ToList())
            {
                var categoryDto = new CategoryDto();
                AllCategories.Add(categoryDto);
                categoryDto.Code = articleCategory.Code;
                categoryDto.Name = articleCategory.Name;
                categoryDto.Url = articleCategory.Url;
                categoryDto.DisplaySeq = articleCategory.DisplaySeq;
            }
        }
    }

}
