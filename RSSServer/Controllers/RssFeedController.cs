using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel.Syndication;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Xml;

namespace RSSServer.Models
{
    public class RssFeedController : ApiController
    {
        public XmlElement createFakeRSS(List<FitInfoModel> ListInfo)
        {
            // create fake rss feed
            HtmlDocument doc = new HtmlDocument();
            XmlDocument rssDoc = new XmlDocument();
            rssDoc.LoadXml("<?xml version=\"1.0\" encoding=\"" + doc.Encoding.BodyName + "\"?><rss version=\"0.91\"/>");

            // add channel element and other information
            XmlElement channel = rssDoc.CreateElement("channel");
            rssDoc.FirstChild.NextSibling.AppendChild(channel);

            XmlElement temp = rssDoc.CreateElement("title");
            temp.InnerText = "ASP.Net articles scrap RSS feed";
            channel.AppendChild(temp);

            temp = rssDoc.CreateElement("link");
            temp.InnerText = "";
            channel.AppendChild(temp);

            XmlElement item;
            // browse each article
            foreach (var info in ListInfo)
            {
                // get what's interesting for RSS
                //string link = href.Attributes["href"].Value;
                //string title = href.InnerText;
                //string description = null;
                //HtmlNode descNode = href.SelectSingleNode("../div/text()");
                //if (descNode != null)
                //    description = descNode.InnerText;

                // create XML elements
                item = rssDoc.CreateElement("item");
                channel.AppendChild(item);

                temp = rssDoc.CreateElement("title");
                temp.InnerText = info.Title;
                item.AppendChild(temp);

                temp = rssDoc.CreateElement("publish");
                temp.InnerText = info.CreateAt.ToString();
                item.AppendChild(temp);                
            }
            rssDoc.Save("rss.xml");
            return channel;
        }


        //public HttpResponseMessage getRss()
        //{
        //    List<FitInfoModel> ListOfInfo = new List<FitInfoModel>();

        //    ListOfInfo = Html2Rss();
        //    return CreateResponse(HttpStatusCode.OK, ListOfInfo);
        //}
        
        [HttpGet]
        public Rss20FeedFormatter Get()
        {
            var feed = new SyndicationFeed("My Blog", "Fit Events", new Uri("http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97"));
            feed.Authors.Add(new SyndicationPerson("htluan2811@gmail.com"));
            feed.Categories.Add(new SyndicationCategory("Titles"));
            feed.Description = new TextSyndicationContent("Infomation");


            List<FitInfoModel> ListOfInfo = new List<FitInfoModel>();

            ListOfInfo = Html2Rss();

            List<SyndicationItem> RssItems = new List<SyndicationItem>();

            foreach (var item in ListOfInfo)
            {
                SyndicationItem rss = new SyndicationItem(
               item.Title,
               item.CreateAt.ToString(),
               new Uri("http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97"),
               "",
               DateTime.Now);

               RssItems.Add(rss);
            }
            feed.Items = RssItems;
            //var ret = feed.GetRss20Formatter();
            var ret = feed.GetRss20Formatter();
            return new Rss20FeedFormatter(feed);
        }

        public string getWebContent(string strLink)
        {
            string strContent = "";
            try
            {
                WebRequest objWebRequest = WebRequest.Create(strLink);
                objWebRequest.Credentials = CredentialCache.DefaultCredentials;
                WebResponse objWebResponse = objWebRequest.GetResponse();
                Stream receiveStream = objWebResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                strContent = readStream.ReadToEnd();
                objWebResponse.Close();
                readStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strContent;
        }

        public string getTitle(string Content)
        {

            string pattern = "<H1 class=Title>[^<]+";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.Value.Substring(16, m.Value.Length - 16);
            return "";
        }

        public string getDescription(string Content)
        {
            string pattern = "<H2 class=Lead>[^<]+";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.Value.Substring(15, m.Value.Length - 15);
            return "";
        }

        public string getContent(string Content)
        {
            string pattern = "<P class=Normal>[^~]+";
            Regex Title = new Regex(pattern);
            Match m = Title.Match(Content);
            if (m.Success)
                return m.Value.Substring(16, m.Value.Length - 16).Replace("/Files", "http://vnexpress.net/Files").Replace("/gl","http://vnexpress.net/gl");
            return "";
        }

        public List<FitInfoModel> Html2Rss()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlWeb hw = new HtmlWeb();
            doc = hw.Load("http://www.fit.hcmus.edu.vn/vn/Default.aspx?tabid=97");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[@id=\"dnn_ctr785_ViewNews_ucShowPost_tblShowListOne\"]/tr[1]/td[1]/table");

            List<FitInfoModel> ListInfo = new List<FitInfoModel>();
            foreach (var item in nodes)
            {
                FitInfoModel Info = new FitInfoModel();
                
                // get Title
                Info.Title = item.SelectSingleNode("tr[1]/td[2]/a").InnerText; 

                // get Publish Date
                Info.CreateAt = DateTime.ParseExact(item.SelectSingleNode("tr[1]/td[2]/span").InnerText.Trim().Replace("(", "").Replace(")", ""), "dd/MM/yyyy", null);
                
                ListInfo.Add(Info);
            }

            return ListInfo.ToList();

        }

        #region Helper
        public HttpResponseMessage CreateResponse<T>(HttpStatusCode StatusCode, T Data)
        {
            return Request.CreateResponse(StatusCode, Data);
        }
        public HttpResponseMessage CreateResponse(HttpStatusCode httpStatusCode)
        {
            return Request.CreateResponse(httpStatusCode);
        }
        #endregion
    }
}
