using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.Home
{
    public class ArticleGridDto
    {
        public List<BaseArticleDto> Articles { get; set; }

        public ArticleGridDto()
        {
            Articles = new List<BaseArticleDto>();
        }
    }
}
