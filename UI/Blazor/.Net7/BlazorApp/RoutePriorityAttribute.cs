namespace BlazorApp
{
    public class RoutePriorityAttribute : Attribute
    {
        public int Priority { get; set; }
        public RoutePriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
