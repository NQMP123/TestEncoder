using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MODMANHHDC.Canvas
{
    public enum ScrollType
    {
        Horizontal = 4,
        Vertical = 6,
        Both = Horizontal | Vertical
    }
    public class mScroll
    {
        private ScrollType scrollType;

        private Vector2Int pointStart = Vector2Int.zero;

        private bool IsPointerHold = false;

        private Action eventDown;

        private Action eventMove;

        private Action eventEnd;

        private Vector2Int _delta;

        public Vector2Int Delta => _delta;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scrollType">Ki?u kéo, n?u ki?u kéo là Horizontal thì là kéo ngang, d? li?u tr? v? là (X, 0), Vertical s? là (0, Y). N?u c? 2 thì s? là (X, Y)</param>
        /// <param name="eventDown">S? ki?n khi ng??i ch?i click chu?t</param>
        /// <param name="eventMove">S? ki?n khi ng??i ch?i kéo</param>
        /// <param name="eventEnd">S? ki?n khi ng??i ch?i nh? chu?t</param>
        public mScroll(ScrollType scrollType, Action eventDown = null, Action eventMove = null, Action eventEnd = null)
        {
            this.scrollType = scrollType;
            this.eventDown = eventDown;
            this.eventMove = eventMove;
            this.eventEnd = eventEnd;
        }

        private Vector2Int _flattenVectorByDirection(Vector2Int vector)
        {
            if ((scrollType & ScrollType.Horizontal) == ScrollType.Horizontal && (scrollType & ScrollType.Vertical) == ScrollType.Vertical)
            {
                return vector;
            }
            else if ((scrollType & ScrollType.Horizontal) == ScrollType.Horizontal)
            {
                return new Vector2Int(vector.x, 0);
            }
            else if ((scrollType & ScrollType.Vertical) == ScrollType.Vertical)
            {
                return new Vector2Int(0, vector.y);
            }
            return Vector2Int.zero;
        }

        /// <summary>
        /// L?y t?a ?? hi?n t?i c?ng/tr? v?i delta tr? v?.
        /// ?? m??t h?n có th? s? d?ng thêm công th?c nh?: Mathf.Round(X/Y * 10_000) * 0.0001f
        /// </summary>
        /// <param name="rectView">T?a ?? và kích th??c c?a view</param>
        /// <returns>Delta Location While Drag</returns>
        public Vector2Int UpdateKey(RectInt rectView)
        {
            if (!IsPointerHold && GameCanvas.isPointerHoldIn(rectView.x, rectView.y, rectView.width, rectView.height))
            {
                IsPointerHold = true;
                pointStart = new Vector2Int(GameCanvas.px, GameCanvas.py);
                eventDown?.Invoke();
            }
            if (IsPointerHold)
            {
                if (GameCanvas.isPointerDown)
                {
                    _delta = new Vector2Int(GameCanvas.px, GameCanvas.py) - pointStart;

                    if (_delta.magnitude > 0)
                    {
                        eventMove?.Invoke();
                    }

                    pointStart = new Vector2Int(GameCanvas.px, GameCanvas.py);
                 //   GameCanvas.clearAllPointerEvent();
                    return _flattenVectorByDirection(_delta);
                }
                if (GameCanvas.isPointerJustRelease)
                {
                    IsPointerHold = false;
                    eventEnd?.Invoke();
                    GameCanvas.isPointerJustRelease = false;
                }
            }
            return Vector2Int.zero;
        }
    }
}
