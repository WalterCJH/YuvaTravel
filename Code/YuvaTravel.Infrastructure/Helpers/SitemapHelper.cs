using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using YuvaTravel.Base.Constants;
using YuvaTravel.Infrastructure.Model;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class SitemapHelper
    {
        /// <summary>
        /// 製作Sitemap.xml內容
        /// </summary>
        /// <param name="host">首頁網址</param>
        /// <param name="articleCategoryNameEnList">文章類別頁代碼</param>
        /// <param name="articleIdList">文章代碼</param>
        /// <param name="articleOnlineTimeList">文章上線時間</param>
        /// <param name="tagList">標籤代碼</param>
        /// <returns></returns>
        public static IReadOnlyCollection<SitemapNode> GetSitemapNodes(string host, List<string> articleCategoryNameEnList, List<string> articleIdList, List<DateTime> articleOnlineTimeList, List<string> tagList, List<(string Code, DateTime? LastMod)> guideList = null)
        {
            List<SitemapNode> nodes = new List<SitemapNode>();

            var dtNow = DateTime.Now;

            // 首頁
            nodes.Add(new SitemapNode { Url = $"{host}/", LastModified = dtNow, Frequency = SitemapFrequency.Daily, Priority = 1.0 });

            // 固定頁
            nodes.Add(new SitemapNode { Url = $"{host}/category", LastModified = dtNow, Frequency = SitemapFrequency.Daily, Priority = 0.9 });
            nodes.Add(new SitemapNode { Url = $"{host}/tag", LastModified = dtNow, Frequency = SitemapFrequency.Weekly, Priority = 0.7 });
            nodes.Add(new SitemapNode { Url = $"{host}/guides", LastModified = dtNow, Frequency = SitemapFrequency.Weekly, Priority = 0.9 });
            nodes.Add(new SitemapNode { Url = $"{host}/faq", LastModified = dtNow, Frequency = SitemapFrequency.Monthly, Priority = 0.5 });

            // 文章分類
            foreach (var nameEn in articleCategoryNameEnList)
            {
                nodes.Add(new SitemapNode
                {
                    Url = $"{host}/category/{nameEn}",
                    LastModified = dtNow,
                    Frequency = SitemapFrequency.Weekly,
                    Priority = 0.8
                });
            }

            // 文章
            int i = 0;
            foreach (var id in articleIdList)
            {
                nodes.Add(new SitemapNode
                {
                    Url = $"{host}/article/{id}",
                    LastModified = articleOnlineTimeList[i++],
                    Frequency = SitemapFrequency.Monthly,
                    Priority = 0.7
                });
            }

            // 標籤
            foreach (var name in tagList)
            {
                nodes.Add(new SitemapNode
                {
                    Url = $"{host}/tag?t={name}",
                    LastModified = dtNow,
                    Frequency = SitemapFrequency.Weekly,
                    Priority = 0.5
                });
            }

            // 嚮導
            if (guideList != null)
            {
                foreach (var (code, lastMod) in guideList)
                {
                    nodes.Add(new SitemapNode
                    {
                        Url = $"{host}/guideprofile/{code}",
                        LastModified = lastMod ?? dtNow,
                        Frequency = SitemapFrequency.Monthly,
                        Priority = 0.7
                    });
                }
            }

            return nodes;
        }

        /// <summary>
        /// 產生Sitemap.xml內容
        /// </summary>
        /// <param name="sitemapNodes"></param>
        /// <returns></returns>
        public static string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "https://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace xhtml = "http://www.w3.org/1999/xhtml";

            //var root = new XElement(xmlns + "urlset", new XAttribute(XNamespace.Xmlns + "xhtml", xhtml));

            XElement root = new XElement(xmlns + "urlset");
            root.SetAttributeValue(XNamespace.Xmlns + "xhtml", xhtml);
            //root.SetAttributeValue("encoding", "UTF-16");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")),
                        //sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(new XDeclaration("1.0", "utf-16", "no"), root);

            return document.ToString();
        }

    }
}
