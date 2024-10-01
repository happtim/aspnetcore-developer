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

        //定义鼠标悬停变更委托类型
        public delegate void HoverChangedEventHandler(object sender, DrawingElement element);

        //定义鼠标悬停变更事件
        public event HoverChangedEventHandler HoverChanged;

        //定义控制点悬停变更委托类型
        public delegate void HoverControlPointIndexChangedEventHandler(object sender, int index);

        //定义控制点悬停变更事件

        public event HoverControlPointIndexChangedEventHandler HoverControlPointIndexChanged;

        private DrawingElement? _hoverElement;

        private int _hoverControlPointIndex;

        private List<DrawingElement> _selectedElements;

        public ObservableCollection<DrawingElement> SelectedElements { get; protected set; }

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

        public bool IsHover(DrawingElement element)
        {
            return _hoverElement == element;
        }

        public bool IsEditMode()
        {
            return _selectedElements.Count == 1;
        }

        public bool IsEditMode(DrawingElement element)
        {
            return _selectedElements.Count == 1 && _selectedElements.Contains(element);
        }

        public bool GetEditModeElement(out DrawingElement element)
        {
            if (_selectedElements.Count == 1)
            {
                element = _selectedElements.First();
                return true;
            }
            else
            {
                element = null;
                return false;
            }
        }

        public void SetHover(DrawingElement element)
        {
            if (_hoverElement != element)
            {
                _hoverElement = element;

                OnHoverChanged(element);
            }
        }

        public void SetHoverControlPointIndex(int index)
        {

            if (_hoverControlPointIndex != index)
            {
                _hoverControlPointIndex = index;

                OnHoverControlPointIndexChanged(index);
            }
        }

        public int GetHoverControlPointIndex()
        {
            return _hoverControlPointIndex;
        }


        // 触发选择事件的方法  
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        //触发悬停事件的方法
        protected virtual void OnHoverChanged(DrawingElement element)
        {
            HoverChanged?.Invoke(this, element);
        }

        //触发悬停控制点事件的方法
        protected virtual void OnHoverControlPointIndexChanged(int index)
        {
            HoverControlPointIndexChanged?.Invoke(this, index);
        }

    }
}
