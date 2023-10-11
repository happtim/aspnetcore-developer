using OwinSelfhostSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OwinSelfhostSample.Controllers
{
    public class RoutesController : ApiController
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        //route template like a url.zhe framework tries to match the segment in the url path to the template.
        // GET api/routes/get         ==> template == "api/{controller}/{id}"
        // GET api/routes/get/abc/123 ==> tempalte == "api/{controller}/{action}/{id}"
       [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //GET api/routes/template/Groceries/1
        [HttpGet]
        public IEnumerable<Product> Template(string category,int id)
        {
            if (category == "all")
                return products;
            return products.Where((p) => p.Category == category && p.Id == id);
        }

        //var routeData = RequestContext.RouteData;
        //routeData.Route 匹配的routetemplate对象
        //routeData.Values 匹配的参数值
        //  routeData.Values['controller'] == routes
        //  routeData.Values['action'] == dictionary
        //  routeData.Valuse['category'] == Groceries
        //  routeData.Values['id'] == 1

        //GET api/routes/template/Groceries/1
        [HttpGet]
        public IEnumerable<Product> Dictionary(string category,int id)
        {
            if (category == "all")
                return products;
            return products.Where((p) => p.Category == category && p.Id == id);
        }
    }
}
