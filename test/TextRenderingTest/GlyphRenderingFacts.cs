﻿using ImGui;
using ImGui.Rendering;
using ImGui.OSAbstraction.Text;
using System;
using System.Collections.Generic;
using Xunit;
using Typography.OpenFont;
using System.IO;
using System.Linq;
using ImGui.UnitTest;

namespace TextRenderingTest
{
    public class GlyphRenderingFacts
    {
        public GlyphRenderingFacts()
        {
            Application.InitSysDependencies();
            Directory.CreateDirectory(OutputDir);
        }

        private static readonly string OutputDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + Path.DirectorySeparatorChar + nameof(GlyphRenderingFacts);

        //glyph settings
        string fontFileName = "DroidSans.ttf";
        const char character = 'e';
        const int fontPointSize = 650;

        //drawing settings
        const double polygonLineWidth = 4;
        Color polygonColor = Color.Black;
        const double quadraticLineWidth = 2;
        Color quadraticSegmentColor = Color.Rgb(0, 122, 104);
        Color startPointColor = Color.Argb(200, 0, 104, 43);

        [Fact]
        public void ShowGlyphAsDirectedContours_Builtin()
        {
            //load the glyph
            Typeface typeFace;
            using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
            {
                var reader = new OpenFontReader();
                typeFace = reader.Read(fs);
            }
            Glyph glyph = typeFace.Lookup(character);

            //read polygons and quadratic bezier segments
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the AABB
            Rect aabb = new Rect(polygons[0][0], polygons[0][1]);
            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                foreach (var p in polygon)
                {
                    aabb.Union(p);
                }
            }
            foreach (var segment in quadraticBezierSegments)
            {
                aabb.Union(segment.Item1);
                aabb.Union(segment.Item2);
                aabb.Union(segment.Item3);
            }

            //polygons
            Geometry polygonGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var polygon in polygons)
                {
                    var startPoint = polygon[0];
                    d.MoveTo(startPoint);
                    var previousPoint = startPoint;
                    foreach (var p in polygon.Skip(1))
                    {
                        d.LineTo(p);
                        //DrawArrow(d, lastPoint, point);
                        previousPoint = p;
                    }
                    d.LineTo(startPoint);
                    //DrawArrow(d, lastPoint, point);
                    d.Stroke();
                }
                polygonGeometry = d.ToGeometry();
            }
            var polygonPen = new Pen(polygonColor, polygonLineWidth);

            //quadratic bezier segments
            Geometry quadraticGeometry;
            {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                foreach (var qs in quadraticBezierSegments)
                {
                    d.MoveTo(qs.Item1);
                    d.QuadraticCurveTo(qs.Item2, qs.Item3);
                    d.Stroke();
                }
                quadraticGeometry = d.ToGeometry();
            }
            var quadraticPen = new Pen(quadraticSegmentColor, quadraticLineWidth);

            //start points
            Geometry startPointGeometry;
            {
                var d = new PathGeometryBuilder();
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygon = polygons[i];
                    var startPoint = polygon[0];
                    d.Circle(startPoint, 10);
                    d.Fill();
                }
                startPointGeometry = d.ToGeometry();
            }
            var startPointBrush = new Brush(startPointColor);

            //create a DrawingVisual to hold geometries
            DrawingVisual drawingVisual = new DrawingVisual(0);

            //create geometries and save to DrawingVisual's content
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawGeometry(null, polygonPen, polygonGeometry);
            drawingContext.DrawGeometry(null, quadraticPen, quadraticGeometry);
            drawingContext.DrawGeometry(startPointBrush, null, startPointGeometry);
            drawingContext.Close();

            //draw the drawingVisual to image
            int width = 2048, height = 2048;
            Util.DrawDrawingVisualToImage(out var imageBytes, width, height, drawingVisual);

            //save and show the image
            var path = $"{OutputDir}{Path.DirectorySeparatorChar}{nameof(ShowGlyphAsDirectedContours_Builtin)}_{fontFileName}_{character}.png";
            Util.ShowImageNotOpenFolder(imageBytes, width, height, path);
        }

        [Fact]
        public void ShowGlyphAsDirectedContours_Cairo()
        {
            //load the glyph
            Typeface typeFace;
            using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
            {
                var reader = new OpenFontReader();
                typeFace = reader.Read(fs);
            }
            Glyph glyph = typeFace.Lookup(character);

            //read polygons and quadratic bezier segments
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the AABB
            Rect aabb = new Rect(polygons[0][0], polygons[0][1]);
            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                foreach (var p in polygon)
                {
                    aabb.Union(p);
                }
            }

            foreach (var segment in quadraticBezierSegments)
            {
                aabb.Union(segment.Item1);
                aabb.Union(segment.Item2);
                aabb.Union(segment.Item3);
            }

            // draw to an image
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32,
                MathEx.RoundToInt(2048), MathEx.RoundToInt(2048)))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                //set surface back ground to white (1,1,1,1)
                g.SetSourceColor(CairoEx.ColorWhite);
                g.Paint();

                //polygons
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygon = polygons[i];
                    var startPoint = polygon[0];
                    g.MoveTo(startPoint.X, startPoint.Y);
                    var previousPoint = startPoint;
                    foreach (var point in polygon)
                    {
                        g.LineTo(point.X, point.Y);
                        g.DrawArrow(previousPoint, point);
                        previousPoint = point;
                    }
                    g.LineTo(startPoint.X, startPoint.Y);
                    g.DrawArrow(previousPoint, startPoint);
                }
                g.LineWidth = polygonLineWidth;
                g.SetSourceColor(polygonColor.ToCairoColor());
                g.Stroke();

                //quadratic bezier segments
                foreach (var segment in quadraticBezierSegments)
                {
                    var p0 = segment.Item1;
                    var c = segment.Item2;
                    var p1 = segment.Item3;
                    g.MoveTo(p0.X, p0.Y);
                    g.QuadraticTo(c.X, c.Y, p1.X, p1.Y);
                }
                g.LineWidth = quadraticLineWidth;
                g.SetSourceColor(quadraticSegmentColor.ToCairoColor());
                g.Stroke();

                //start points
                g.SetSourceColor(startPointColor.ToCairoColor());
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygon = polygons[i];
                    var startPoint = polygon[0];
                    g.Arc(startPoint.x, startPoint.y, 10, 0, System.Math.PI * 2);
                    g.Fill();
                }

                //show the image of contours
                var path = $"{OutputDir}{Path.DirectorySeparatorChar}{fontFileName}_{character}.png";
                surface.WriteToPng(path);
                Util.OpenImage(path);
            }
        }

        [Fact]
        public void ShowGlyphAsOverlappingPolygon_Builtin()
        {
            //load the glyph
            Typeface typeFace;
            using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
            {
                var reader = new OpenFontReader();
                typeFace = reader.Read(fs);
            }
            Glyph glyph = typeFace.Lookup(character);

            //read polygons and quadratic bezier segments
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the AABB
            Rect aabb = new Rect(polygons[0][0], polygons[0][1]);
            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                foreach (var p in polygon)
                {
                    aabb.Union(p);
                }
            }
            foreach (var segment in quadraticBezierSegments)
            {
                aabb.Union(segment.Item1);
                aabb.Union(segment.Item2);
                aabb.Union(segment.Item3);
            }

            //polygons
            var polygonBrush = new Brush(Color.Argb(128, 10, 10, 10));
            Geometry polygonGeometry;
            {
                var d = new PathGeometryBuilder();
                foreach (var polygon in polygons)
                {
                    if (polygon.Count == 0)
                    {
                        continue;
                    }

                    d.MoveTo(polygon[0]);
                    foreach (var point in polygon.Skip(1))
                    {
                        d.LineTo(point);
                    }
                    d.Fill();
                }
                polygonGeometry = d.ToGeometry();
            }
            
            //polygon points
            Geometry pointsGeometry;
            {
                var d = new PathGeometryBuilder();
                foreach (var polygon in polygons)
                {
                    foreach (var point in polygon)
                    {
                        d.Circle(point, 5);
                        d.Fill();
                    }
                }
                pointsGeometry = d.ToGeometry();
            }
            var pointsBrush = new Brush(Color.Argb(200, 0, 0, 0));

            //create a DrawingVisual to hold geometries
            DrawingVisual drawingVisual = new DrawingVisual(0);

            //create geometries and save to DrawingVisual's content
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawGeometry(pointsBrush, null, pointsGeometry);
            drawingContext.DrawGeometry(polygonBrush, null, polygonGeometry);
            drawingContext.Close();

            //draw the drawingVisual to image
            int width = 2048, height = 2048;
            Util.DrawDrawingVisualToImage(out var imageBytes, width, height, drawingVisual);

            //save and show the image
            var path = $"{OutputDir}{Path.DirectorySeparatorChar}{nameof(ShowGlyphAsOverlappingPolygon_Builtin)}_{fontFileName}_{character}.png";
            Util.ShowImageNotOpenFolder(imageBytes, width, height, path);
        }

        [Fact]
        public void ShowGlyphAsTextMesh_Builtin()
        {
            //load the glyph
            Typeface typeFace;
            using (var fs = Utility.ReadFile(Utility.FontDir + fontFileName))
            {
                var reader = new OpenFontReader();
                typeFace = reader.Read(fs);
            }
            Glyph glyph = typeFace.Lookup(character);

            //read polygons and quadratic bezier segments
            GlyphLoader.Read(glyph, out var polygons, out var quadraticBezierSegments);

            //calculate the AABB
            Rect aabb = new Rect(polygons[0][0], polygons[0][1]);
            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                foreach (var p in polygon)
                {
                    aabb.Union(p);
                }
            }
            foreach (var segment in quadraticBezierSegments)
            {
                aabb.Union(segment.Item1);
                aabb.Union(segment.Item2);
                aabb.Union(segment.Item3);
            }

            //construct text mesh
            TextMesh textMesh = new TextMesh();
            Color polygonFillColor = Color.Argb(128, 10, 10, 10);
            textMesh.AddTriangles(polygons, polygonFillColor, Vector.Zero, Vector.Zero, 1, false);
            var command = textMesh.Commands[^1];
            command.ElemCount = textMesh.IndexBuffer.Count ;
            textMesh.Commands[^1] = command;
            
#if false
            int width = 2048, height = 2048;
            Util.DrawTextMeshToImage_Realtime(width, height, textMesh);

#else
            //draw the drawingVisual to image
            int width = 250, height = 250;
            Util.DrawTextMeshToImage(out var imageBytes, width, height, textMesh);

            //save and show the image
            var path =
 $"{OutputDir}{Path.DirectorySeparatorChar}{nameof(ShowGlyphAsTextMesh_Builtin)}_{fontFileName}_{character.GetHashCode()}.png";
            Util.ShowImageNotOpenFolder(imageBytes, width, height, path);
#endif
        }
    }
}
