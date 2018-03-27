﻿using System;
using System.Collections.Generic;
using ImGui.Layout;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    internal partial class Node
    {
        /// <summary>
        /// identifier number of the node
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// string identifier of the node
        /// </summary>
        public string StrId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Dirty
        {
            get;
            set;
        }

        /// <summary>
        /// border-box, the layout result
        /// </summary>
        public Rect Rect;

        #region Layout
        /// <summary>
        /// Make this node a group
        /// </summary>
        public void AttachLayoutGroup(bool isVertical, LayoutOptions? options = null)
        {
            this.Group_Init(this.Id, isVertical, options);
            //TODO delete entry?
        }

        /// <summary>
        /// Make this node an entry
        /// </summary>
        public void AttachLayoutEntry(Size contentSize, LayoutOptions? options = null)
        {
            this.Entry_Init(Id, contentSize, options);
        }

        /// <summary>
        /// Layout the sub-tree rooted at this node
        /// </summary>
        public void Layout()
        {
            this.Group_CalcWidth(this.ContentWidth);
            this.Group_CalcHeight(this.ContentHeight);
            this.Group_SetX(this.Rect.X);
            this.Group_SetY(this.Rect.Y);
        }

        private void CalcWidth(double unitPartWidth = -1)
        {
            if (this.Children == null)
            {
                this.Entry_CalcWidth(unitPartWidth);
            }
            else
            {
                this.Group_CalcWidth(unitPartWidth);
            }
        }

        private void CalcHeight(double unitPartHeight = -1)
        {
            if (this.Children == null)
            {
                this.Entry_CalcHeight(unitPartHeight);
            }
            else
            {
                this.Group_CalcHeight(unitPartHeight);
            }
        }

        private void SetX(double x)
        {
            if (this.Children == null)
            {
                this.Entry_SetX(x);
            }
            else
            {
                this.Group_SetX(x);
            }
        }

        
        private void SetY(double y)
        {
            if (this.Children == null)
            {
                this.Entry_SetY(y);
            }
            else
            {
                this.Group_SetY(y);
            }
        }
        #endregion

        #region Hierarchy
        public Node Parent { get; set; }

        public List<Node> Children { get; set; }

        public Node GetNodeById(int id)
        {
            foreach (var node in this.Children)
            {
                if (node.Id == id)
                {
                    return node;
                }
                else
                {
                    return node.GetNodeById(id);
                }
            }
            return null;
        }
        #endregion
        
        #region Draw

        internal Primitive Primitive { get; set; }
        internal bool IsFill { get; set; }
        internal Brush Brush { get; set; }
        internal StrokeStyle StrokeStyle { get; set; }

        public void Draw(IPrimitiveRenderer renderer)
        {
            switch (this.Primitive)
            {
                case PathPrimitive p:
                {
                    if (this.IsFill)
                    {
                        renderer.Fill(p, this.Brush);
                    }
                    else
                    {
                        renderer.Stroke(p, this.Brush, this.StrokeStyle);
                    }
                }
                break;
                case TextPrimitive t:
                {
                    var style = GUIStyle.Default;//FIXME TEMP
                    renderer.DrawText(t, style.FontFamily, style.FontSize, style.FontColor, style.FontStyle, style.FontWeight);
                    break;
                }
                case ImagePrimitive i:
                {
                    renderer.DrawImage(i, this.Brush);
                    break;
                }
            }
        }

        #endregion

    }
}
