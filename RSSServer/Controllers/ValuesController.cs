using RSSServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace RSSServer.Models
{
    public class ValuesController : ApiController
    {
        private static readonly IUrlRepository _repo = new UrlRepository();
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";
        // GET /api/values
        public IEnumerable<Url> Get()
        {
            return _repo.GetAll();
        }

        // GET /api/values/5
        public Url Get(int id)
        {
            return _repo.Get(id);
        }

    }
}
