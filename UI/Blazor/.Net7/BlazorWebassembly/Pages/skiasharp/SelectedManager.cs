using BlazorWebassembly.Pages.skiasharp.Draws;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;

namespace BlazorWebassembly.Pages.skiasharp
{
    public class SelectedManager
    {
        // 定义选择变更委托类型  
        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

        // 定义选择便变更事件  
        public event SelectionChangedEventHandler SelectionChanged;

        //定义点击元素变更委托类型
        public delegate void ClickedElementChangedEventHandler(object sender, EventArgs e);

        //定义点击元素变更事件
        public event ClickedElementChangedEventHandler ClickedElementChanged;


        private List<DrawingElement> _selectedElements;

        public DrawingElement? ClickedElement { get; protected set; }

        public SelectedManager()
        {
            _selectedElements = new List<DrawingElement>();
        }

        public void Add(DrawingElement element)
        {
            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);

                // 触发事件
                OnSelectionChanged();
            }
        }

        public void AddRange(List<DrawingElement> elements)
        {
            bool changed = false;
            foreach (var element in elements)
            {
                if (!_selectedElements.Contains(element))
                {
                    _selectedElements.Add(element);

                    changed = true;
                }
            }

            if (changed)
            {
                // 触发事件
                OnSelectionChanged();
            }
        }

        public bool Contains(DrawingElement element)
        {
            return _selectedElements.Contains(element);
        }

        public bool Remove(DrawingElement element)
        {
            bool result = false;

            if (_selectedElements.Contains(element))
            {
                result = _selectedElements.Remove(element);

                // 触发事件
                OnSelectionChanged();
            }

            return result;
        }

        public IEnumerable<DrawingElement> GetSelectedElements()
        {
            return _selectedElements;
        }

        public int Count()
        {
            return _selectedElements.Count;
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

        public void SetClickedElement(DrawingElement? element)
        {
            if(ClickedElement != element)
            {
                ClickedElement = element;

                // 触发事件
                OnClickedElementChanged();
            }
        }

        // 触发选择事件的方法  
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        // 触发点击元素变更事件的方法
        protected virtual void OnClickedElementChanged()
        {
            ClickedElementChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
