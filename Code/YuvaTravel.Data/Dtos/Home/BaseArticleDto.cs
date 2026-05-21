using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuvaTravel.Data.Entities;

namespace YuvaTravel.Data.Dtos.Home
{
    public class BaseArticleDto
    {
        public BaseArticleDto()
        {
            Guid = Guid.NewGuid();
        }
        public Guid Guid { get; set; }
        public bool IsTop { get; set; }
        public string C { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string ImageMobilePath { get; set; }
        public string ImageAlt { get; set; }
        public string ImageTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? OnlineTime { get; set; }
        public bool ShowPublishDate { get; set; }
        public string PublishDate
        {
            get
            {
                return OnlineTime?.ToString("yyyy/MM/dd");
            }
        }
        public int? ReadTimeMin { get; set; }
        public bool ShowViews { get; set; }
        public int Views { get; set; }
        public int ViewCounts
        {
            get
            {
                if (Views > 2000)
                {
                    return Views;
                }

                var dtNow = DateTime.Now;
                var totalDay = (dtNow - OnlineTime.Value).TotalDays;
                if (totalDay <= 35)
                {
                    int countViews = Convert.ToInt32(Math.Ceiling(totalDay / 3.5 * 100)) + Views % 100;
                    if (Views > countViews)
                        return Views;
                    else
                        return countViews;
                }

                return 1000 + Views;
            }
        }
        public List<TagDto> Tags { get; set; }
        public List<ArticleSDCSDto> ArticleSDCSs { get; set; }
        public string ShowArticleSDCS
        {
            get
            {
                string value = "";

                if (ArticleSDCSs != null && ArticleSDCSs.Count() > 0)
                {
                    value += @"
    <script type=""application/ld+json"">
        {
          ""@context"": ""https://schema.org"",
          ""@type"": ""FAQPage"",
          ""mainEntity"": [
              ";

                    string str = "";

                    foreach (var item in ArticleSDCSs)
                    {
                        str += @"{
            ""@type"": ""Question"",
            ""name"": """ + item.Name + @""",
            ""acceptedAnswer"": {
              ""@type"": ""Answer"",
              ""text"": """ + item.Text + @"""
            }},";
                    }

                    value += str.TrimEnd(',') + @"]
        }
    </script>";
                }

                return value;
            }
        }


        #region 精選活動

        [Display(Name = "國家")]
        public string EventLocation { get; set; }

        [Display(Name = "星星數")]
        public int? EventStarNum { get; set; }

        [Display(Name = "評論數")]
        public string EventCount { get; set; }

        [Display(Name = "價格")]
        public int? EventPrice { get; set; }

        [Display(Name = "優惠方式")]
        public string EventDiscount { get; set; }

        [Display(Name = "優惠折扣")]
        public string EventDiscountValue { get; set; }

        #endregion

    }
}
