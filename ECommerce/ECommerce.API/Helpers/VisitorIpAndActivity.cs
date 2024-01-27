using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace ECommerce.API.Helpers
{
    public class VisitorIpAndActivity : IAsyncActionFilter
    {
        private readonly ILogger<VisitedPath> logger;

        public VisitorIpAndActivity(ILogger<VisitedPath> logger)
        {
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var url = "http://localhost:5004";

            using (WebClient webclient = new WebClient())
            {
                try
                {

                    //use webclient to perform a post request to the server
                    // var result = await
                    //
                    this.logger.LogInformation("Entered OnActionExecutionAsync ");
                    webclient.Headers[HttpRequestHeader.ContentType] = "application/json";

                    var ipProfile = new IpProfile();
                    ipProfile.IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    var returnval = await webclient.UploadStringTaskAsync(new System.Uri(url + "/api/IpProfile"), "POST", JsonConvert.SerializeObject(ipProfile));
                    // await ipProfileRepoistory.AddIpProfileAsync(context.Connection.RemoteIpAddress.ToString());
                    this.logger.LogInformation("connected to the webtracker");
                    var ipprofile = JsonConvert.DeserializeObject<IpProfile>(returnval);

                    var visitorPath = new VisitedPath();
                    var httpVerb = context.HttpContext.Request.Method;
                    // Get request URL
                    var requestUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
                    visitorPath.Path = requestUrl;
                    visitorPath.Verb = httpVerb;
                    visitorPath.IpProfileId = ipprofile.Id;
                    visitorPath.Created = DateTime.UtcNow;
                    visitorPath.AppName = "OfficeShop";
                    // add http header to support application/json format
                    webclient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    await webclient.UploadStringTaskAsync(new System.Uri(url + "/api/VisitorPath"), "POST", JsonConvert.SerializeObject(visitorPath));

                }
                catch (Exception ex)
                {
                    this.logger.LogInformation(ex.Message + " " + ex.StackTrace);

                }
            }
        }
    }
}