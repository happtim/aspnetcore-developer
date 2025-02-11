﻿using BlazorWebassembly.Pages.skiasharp.Commands.Edits;
using SkiaSharp;

namespace BlazorWebassembly.Pages.skiasharp.Draws
{
    public abstract class DrawingElement
    {
        public abstract void Draw(SKCanvas canvas);
        public abstract void DrawHighlight(SKCanvas canvas);

        public abstract bool IsHit(SKPoint point);
        public abstract bool IsContainedIn(SKRect rect);
        public abstract void Move(float dx, float dy);
        public abstract void DrawControlPoints(SKCanvas canvas);
        public abstract int GetControlPointIndex(SKPoint point);
        public abstract bool SetHoverControlPointIndex(int index,CursorManager? cursorManager = null);
        public abstract void UpdateControlPoint(int index, SKPoint newPosition);
        public abstract IEditOperation? GetEditOperation(int controlPointIndex);

        public abstract DrawingElement Clone();
    }
}
