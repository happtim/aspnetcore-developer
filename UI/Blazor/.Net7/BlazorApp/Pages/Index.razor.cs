using Microsoft.AspNetCore.Components;
using System.Reflection;

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

            pageTypes = pageTypes.Where(type => type.GetCustomAttributes<RouteAttribute>().Count() > 0)
                .OrderBy(type => type.GetCustomAttribute<RoutePriorityAttribute>()?.Priority ?? 999)
                .ToList();

            foreach (var pageType in pageTypes)
            {
                var routeAttributes = pageType.GetCustomAttributes<RouteAttribute>();

                foreach (var routeAttribute in routeAttributes)
                {
                    string[] segments = routeAttribute.Template.Split('/');
                    string group = segments[1];

                    if (!pageUrls.ContainsKey(group))
                    {
                        pageUrls.Add(group, new List<string>());
                    }

                    var template = routeAttribute.Template;

                    //"/lifecycles/set-params-async/{Param?}"
                    if (routeAttribute.Template.EndsWith("{Param?}"))
                    {
                        template = template.Replace("{Param?}", "123");
                    }
                    //"/lifecycles/on-params-set/{StartDate:datetime}"
                    if (routeAttribute.Template.EndsWith("{StartDate:datetime}")) 
                    {
                        template = template.Replace("{StartDate:datetime}", new DateTime(2008,08,08).ToString("yyyy-MM-dd"));
                    }

                    pageUrls[group].Add(template);
                }
            }

            return Task.CompletedTask;
        }
    }
}
