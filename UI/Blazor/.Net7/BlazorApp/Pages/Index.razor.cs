using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace BlazorApp.Pages
{
    public partial class Index
    {

        private Dictionary<string,List<string>> pageUrls = new Dictionary<string, List<string>>();

        protected override Task OnInitializedAsync()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var pageTypes = types.Where(type => type.BaseType == typeof(ComponentBase));

            foreach (var pageType in pageTypes)
            {
                var routeAttribute = pageType.GetCustomAttribute<RouteAttribute>();

                if (routeAttribute != null)
                {
                    string[] segments = routeAttribute.Template.Split('/');
                    string group = segments[segments.Length - 2];
                   
                    if (!pageUrls.ContainsKey(group))
                    {
                        pageUrls.Add(group, new List<string>());
                    }
                    else
                    {
                        pageUrls[group].Add(routeAttribute.Template);
                    }


                }
            }

            return Task.CompletedTask;
        }
    }
}
