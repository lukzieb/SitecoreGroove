using Sitecore.Mvc.Pipelines.Request.RequestBegin;
using System.Net;

namespace SitecoreGroove.Feature.ConditionalRequests.Pipelines
{
    public class ConditionalGetProcessor : RequestBeginProcessor
    {
        public override void Process(RequestBeginArgs args)
        {
            if (args.PageContext != null && args.RequestContext.HttpContext.Request.HttpMethod == WebRequestMethods.Http.Get)
            {
                args.RequestContext.HttpContext.Response.Filter = new EtagStream(args.RequestContext.HttpContext);
            }
        }
    }
}