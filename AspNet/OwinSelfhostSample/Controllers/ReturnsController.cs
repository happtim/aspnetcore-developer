using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace OwinSelfhostSample.Controllers
{
    /// <summary>
    /// web api 返回类型
    /// 1. void
    /// 2. HttpResponseMessage 
    /// 3. IHttpActionResult defines a httpResponseMessageFactory have some advantages.
    /// 4. Some Other Types web api use media formatter to serialize
    /// </summary>
    public class ReturnsController : ApiController
    {
        //return 1 type
        // GET  returns/void
        [HttpGet]
        public void Void(int id)
        {
            //return new string[] { "value1", "value2" };
        }


        // HttpResponseMessage
        //GET returns/responseMessage
        [HttpGet]
        public HttpResponseMessage ResponseMessage(int id) {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "王伟德");
            //response.Content = new StringContent("hello", Encoding.Unicode);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromMinutes(20)
            };
            return response;

        }

        // IHttpActionResult
        //GET returns/actionResult
        [HttpGet]
        public IHttpActionResult ActionResult(int id) {

            return Ok(new string[] { "value1", "value2" });
        }

        // Some Other Types 
        //GET returns/otherType
        [HttpGet]
        public IEnumerable<string> OtherType()
        {
            return new string[] { "value1", "value2","value3" };
        }

    }
}
