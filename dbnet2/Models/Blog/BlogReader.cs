using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace dbnet2.Models.Blog
{
    public class BlogReader
    {
        protected Uri FeedUrl { get; set; }

        public BlogReader(Uri feedUrl)
        {
            FeedUrl = feedUrl;
        }

        public List<BlogItemSummary> GetSummaryOfLatest(int amount)
        {
            var blogEntries = new List<BlogItemSummary>();

            XmlReader reader = null;
            SyndicationFeed feed = null;
            
            try
            {
                using (reader = XmlReader.Create(FeedUrl.ToString()))
                {
                    feed = SyndicationFeed.Load(reader);
                    reader.Close();
                }
            }
            catch (System.Net.WebException)
            {
                return blogEntries;
            }

            foreach (var item in feed.Items.OrderByDescending(x => x.PublishDate).Take(amount))
            {
                var blogItem = new BlogItemSummary
                {
                    Subject = item.Title.Text,
                    Summary = item.Summary.Text,
                    PublishDate = item.PublishDate,
                    Link = item.Links.First().Uri
                };
                blogEntries.Add(blogItem);
            }

            return blogEntries;
        }
    }
}