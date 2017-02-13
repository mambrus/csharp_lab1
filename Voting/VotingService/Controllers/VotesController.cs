namespace VotingService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Net.Http.Headers;
    using System.Web.Http;


    public class VotesController : ApiController
    {
        // Used for health checks.
        public static long _requestCount = 0L;

        // Holds the votes and counts. NOTE: THIS IS NOT THREAD SAFE FOR THE PURPOSES OF THE LAB ONLY.
        static Dictionary<string, int> _counts = new Dictionary<string, int>();

        // GET api/votes 
        [HttpGet]
        [Route("api/votes")]
        public HttpResponseMessage Get()
        {
            string activityID = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("VotesController.Get", activityID);

            Interlocked.Increment(ref _requestCount);

            List<KeyValuePair<string, int>> votes = new List<KeyValuePair<string, int>>(_counts.Count);
            foreach (KeyValuePair<string, int> kvp in _counts)
            {
                votes.Add(kvp);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, votes);
            response.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true, MustRevalidate = true };
            return response;
        }

        [HttpPost]
        [Route("api/{key}")]
        public HttpResponseMessage Post(string key)
        {
            string activityID = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("VotesController.Post", activityID);

            Interlocked.Increment(ref _requestCount);

            if (false == _counts.ContainsKey(key))
            {
                _counts.Add(key, 1);
            }
            else
            {
                _counts[key] = _counts[key] + 1;
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("api/{key}")]
        public HttpResponseMessage Delete(string key)
        {
            string activityID = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("VotesController.Detete", activityID);

            Interlocked.Increment(ref _requestCount);

            if (true == _counts.ContainsKey(key))
            {
                if (_counts.Remove(key))
                    return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("api/{file}")]
        public HttpResponseMessage GetFile(string file)
        {
            string response = null;
            string responseType = "text/html";

            string activityID = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("VotesController.GetFile", activityID);

            Interlocked.Increment(ref _requestCount);

            // Validate file name.
            if ("index.html" == file)
            {
                // This hardcoded path is only for the lab. Later in the lab when the version is changed, this
                // hardcoded path must be changed to use the UX. In part 2 of the lab, this will be calculated
                // using the connected service path.
                string path = string.Format(@"..\VotingServicePkg.Code.1.0.2\{0}", file);
                response = File.ReadAllText(path);
            }

            if (null != response)
                return Request.CreateResponse(HttpStatusCode.OK, response, responseType);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File");
        }
    }

}
