using BlazorWebassembly.Pages.skiasharp.Draws;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class SelectedManager
    {
        // 定义选择变更委托类型  
        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

        // 定义选择便变更事件  
        public event SelectionChangedEventHandler SelectionChanged;

        //定义鼠标悬停变更委托类型
        public delegate void HoverChangedEventHandler(object sender, DrawElement element);

        //定义鼠标悬停变更事件
        public event HoverChangedEventHandler HoverChanged;

        public DrawElement HoverElement { get; private set; }

        private List<DrawElement> _selectedElements;

        public SelectedManager()
        {
            _selectedElements = new List<DrawElement>();

        }

        public void Set(DrawElement element)
        {
            if (!_selectedElements.Contains(element)) 
            {
                _selectedElements.Add(element);

                // 触发事件
                OnSelectionChanged();
            }
        }

        public List<DrawElement> Get()
        {
            return _selectedElements;
        }

        public void Clear()
        {
            if (_selectedElements.Count > 0) 
            {
                _selectedElements.Clear();

                // 触发事件
                OnSelectionChanged();
            }
        }

        public void SetHover(DrawElement element)
        {
            if (HoverElement != element)
            {
                HoverElement = element;

                OnHoverChanged(element);
            }
        }

        // 触发选择事件的方法  
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        //触发悬停事件的方法
        protected virtual void OnHoverChanged(DrawElement element)
        {
            HoverChanged?.Invoke(this, element);
        }

    }
}
