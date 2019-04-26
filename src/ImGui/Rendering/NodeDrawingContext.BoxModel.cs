﻿using System;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal partial class NodeDrawingContext
    {
        private void DrawOutline(Rect borderBoxRect)
        {
            var style = ownerNode.RuleSet;
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (MathEx.AmostZero(outlineWidth)) return;
            var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
            if (MathEx.AmostZero(outlineColor.A)) return;
            dc.DrawRectangle(null, new Pen(outlineColor, outlineWidth), borderBoxRect);
        }

        private void DrawBorder(Rect borderBoxRect, Rect paddingBoxRect)
        {
            var style = ownerNode.RuleSet;
            // draw border between border-box and padding-box
            var borderImageSource = style.BorderImageSource;
            if (borderImageSource != null)
            {
                var rule = style.GetRule<string>(GUIStyleName.BorderImageSource);
                if (rule.Geometry == null)
                {
                    rule.Geometry = new ImageGeometry(borderImageSource);
                }

                Debug.Assert(rule.Geometry is ImageGeometry);
                //TODO this.DrawSlicedImage((ImageGeometry) rule.Geometry, borderBoxRect, style);
            }
            else
            {
                var border = style.Border;
                var borderColor = style.BorderColor;
                var borderRadius = style.BorderRadius;
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(GenerateTopBorderFigure(borderBoxRect, paddingBoxRect, border, borderRadius));
                geometry.Figures.Add(GenerateRightBorderFigure(borderBoxRect, paddingBoxRect, border, borderRadius));
                //TODO other borders

                dc.DrawGeometry(new Brush(borderColor.top), new Pen(Color.Black, 2), geometry);
            }
        }

        private static PathFigure GenerateTopBorderFigure(
            Rect borderBoxRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius)
        {
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            //start from top-left
            var bl = border.left;
            var bt = border.top;
            Point startPoint;
            Point endPoint;
            {
                var r = borderRadius.TopLeft;
                var arcCenter = borderBoxRect.TopLeft + new Vector(r, r);
                var paddingCorner = paddingRect.TopLeft;
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-135));
                if (bl < r && r < bt || bt < r && r < bl || r < bl && bl < bt || r < bt && bt < bl
                    || r == bt && bt == bl)
                {
                    startPoint = endPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(new Point(borderBoxRect.Left + r, borderBoxRect.Top),new Size(r, r), 0,false, SweepDirection.Clockwise, false));
                }
                else // if (bl < bt && bt < r || bt<bl && bl <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - bl;
                    var ellipseYRadius = r - bt;
                    startPoint = endPoint = paddingCorner + new Vector(ellipseXRadius, 0);
                    var halfEllipsePoint =
                        MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-135));
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0,
                        false,
                        SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(new Point(borderBoxRect.Left + r, borderBoxRect.Top),
                        new Size(r, r), 0,
                        false, SweepDirection.Clockwise, false));
                }
            }

            figure.StartPoint = startPoint;

            //top upper line: connect left to right
            figure.Segments.Add(new LineSegment(borderBoxRect.TopRight + new Vector(-borderRadius.TopRight, 0), false));

            //to top-right
            var br = border.right;
            {
                var r = borderRadius.TopRight;
                var arcCenter = borderBoxRect.TopRight + new Vector(-r, r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-45));
                var paddingCorner = paddingRect.TopRight;

                if (br < r && r < bt || bt < r && r < br || r < br && br < bt || r < bt && bt < br
                    || r == bt && bt == br)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,
                        false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else //(br < bt && bt < r || bt<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bt;
                    var halfEllipsePoint =
                        MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45));
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,
                        false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(
                        new Point(borderBoxRect.Right - r, borderBoxRect.Top + border.top),
                        new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                }
            }

            //top lower line: connect left to right
            figure.Segments.Add(new LineSegment(endPoint, false));

            return figure;
        }

        private static PathFigure GenerateRightBorderFigure(
            Rect borderBoxRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius)
        {
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            var br = border.right;
            var bb = border.bottom;
            Point startPoint;
            Point endPoint;
            //top-right
            {
                var r = borderRadius.TopRight;
                var arcCenter = borderBoxRect.TopRight + new Vector(-r, r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-45));
                var paddingCorner = paddingRect.TopRight;
                if (br < r && r < bb || bb < r && r < br || r < br && br < bb || r < bb && bb < br
                    || r == bb && bb == br)
                {
                    startPoint = endPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(new Point(borderBoxRect.Right - br, borderBoxRect.Top + r), new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                else //(br < bt && bt < r || bt<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45));
                    startPoint = endPoint = new Point(paddingCorner.X + ellipseXRadius, paddingCorner.Y);
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(
                        new Point(borderBoxRect.Right, borderBoxRect.Top + r),
                        new Size(r, r), 0, false, SweepDirection.Clockwise, false));
                }
            }

            figure.StartPoint = startPoint;

            figure.Segments.Add(new LineSegment(new Point(borderBoxRect.Right, borderBoxRect.Bottom - borderRadius.BottomRight), false));

            //bottom-right
            {
                var r = borderRadius.TopRight;
                var arcCenter = borderBoxRect.BottomRight + new Vector(-r, -r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(45));
                var paddingCorner = paddingRect.BottomRight;

                if (br < r && r < bb || bb < r && r < br || r < br && br < bb || r < bb && bb < br
                    || r == bb && bb == br)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,
                        false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else //(br < bb && bt < r || bb<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint =
                        MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45));
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,
                        false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(
                        new Point(borderBoxRect.Right - r, borderBoxRect.Top + border.top),
                        new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                }
            }


            //close the figure
            figure.Segments.Add(new LineSegment(endPoint, false));

            return figure;
        }

        private void DrawBackground(Rect paddingBoxRect)
        {
            var style = ownerNode.RuleSet;
            // draw background in padding-box
            var gradient = (Gradient) style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.BackgroundColor;
                DrawRectangle(new Brush(bgColor), null, paddingBoxRect);
                //TODO apply border-radius clip, which is determined by how border corners are rendered
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                //TODO this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}