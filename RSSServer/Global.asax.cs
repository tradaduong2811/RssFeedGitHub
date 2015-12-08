using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Net.Http.Formatting;

namespace RSSServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //GlobalConfiguration.Configuration.Formatters.Add(new Models.SyndicationFeedFormatter());
            
        }
    }
}
