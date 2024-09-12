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

        public void Add(DrawElement element)
        {
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);

                Console.WriteLine("SelectedManager.Add: " + element.GetType().Name);

                // 触发事件
                OnSelectionChanged();
            }
        }

        public void AddRange(List<DrawElement> elements)
        {
            bool changed = false;
            foreach (var element in elements)
            {
                if (!_selectedElements.Contains(element))
                {
                    _selectedElements.Add(element);

                    Console.WriteLine("SelectedManager.AddRange: " + element.GetType().Name);
                    changed = true;
                }
            }

            if (changed)
            {
                // 触发事件
                OnSelectionChanged();
            }
        }

        public bool Contains(DrawElement element)
        {
            return _selectedElements.Contains(element);
        }

        public List<DrawElement> GetSelected()
        {
            return _selectedElements;
        }

        public bool IsEditMode(DrawElement drawElement)
        {
            return _selectedElements.Count == 1 && _selectedElements.Contains(drawElement);
        }


        public void Clear()
        {
            if (_selectedElements.Count > 0) 
            {
                _selectedElements.Clear();

                Console.WriteLine("SelectedManager.Clear");

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
