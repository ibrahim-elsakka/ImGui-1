﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CSharpGL;
using System.Linq;
using System.Reflection;

namespace ImGui
{
    internal static partial class Utility
    {

        /// <summary>
        /// Get rect of the context box
        /// </summary>
        /// <param name="rect">rect of the entire box</param>
        /// <param name="style">style</param>
        /// <returns>rect of the context box</returns>
        public static Rect GetContentRect(Rect rect, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);
            return contentBoxRect;
        }

        // Detects the current OS (Windows, Linux, MacOS)
        internal static class CurrentOS
        {
            public static Platform Platform {get; private set;}

            static CurrentOS()
            {
#if __ANDROID__
                var envars = System.Environment.GetEnvironmentVariables();
                IsAndroid = envars.Contains("ANDROID_PROPERTY_WORKSPACE");
                if (IsAndroid)
                {
                    Platform = Platform.Android;
                    return;
                }
#else
                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (IsWindows)
                {
                    Platform = Platform.Windows;
                    return;
                }
                IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                if (IsMac)
                {
                    Platform = Platform.Mac;
                    return;
                }
                IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                if (IsLinux)
                {
                    Platform = Platform.Linux;
                    return;
                }
#endif
                IsUnknown = true;                
            }

            public static bool IsWindows { get; private set; }
            public static bool IsMac { get; private set; }
            public static bool IsLinux { get; private set; }

            public static bool IsAndroid { get; private set; }

            public static bool IsUnknown { get; private set; }

            public static bool Is64BitProcess
            {
                get { return (IntPtr.Size == 8); }
            }

            public static bool Is32BitProcess
            {
                get { return (IntPtr.Size == 4); }
            }
        }
        
        public static void GetId(string t, out string text, out string id)
        {
            if (t.Contains("###"))
            {
                var tmp0 = t.IndexOf("###");
                text = t.Substring(0, tmp0);
                id = t;
            }
            else if (t.Contains("##"))
            {
                var tmp0 = t.IndexOf("##");
                text = t.Substring(0, tmp0);
                id = t.Substring(tmp0 + 1);
            }
            else
            {
                text = id = t;
            }
        }

        [Conditional("DEBUG")]
        public static void SaveToObjFile(string path, IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# MTE");
            sb.AppendLine();

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.x, position.y, 0);
                sb.AppendLine();
            }

            //for (int i = 0; i < vertexes.Count; i++)
            //{
            //    var uv = vertexes[i].uv;
            //    sb.AppendFormat("vt {0} {1}", uv.x, uv.y);
            //    sb.AppendLine();
            //}

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString());
        }

        [Conditional("DEBUG")]
        public static void SaveToObjFile(IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# MTE");
            sb.AppendLine();

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.x, position.y, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < vertexes.Count; i++)
            {
                var uv = vertexes[i].uv;
                sb.AppendFormat("vt {0} {1}", uv.x, uv.y);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText("D:\\imgui_test.obj", sb.ToString());
        }

        public static void SaveToObjFile2(string filePath, float[] positions, int[] indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# Triangle");
            sb.AppendLine();

            for (int i = 0; i < positions.Length; i += 2)
            {
                sb.AppendFormat("v {0} {1} 0", positions[i], positions[i + 1]);
                sb.AppendLine();
            }

            for (int i = 0; i < positions.Length; i++)
            {
                sb.AppendFormat("vt {0} {1}", 0, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Length; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 1] + 1,
                    indexes[i + 2] + 1);
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

#if false //to be moved to unit test
#endif
        /// <summary>
        /// convert pt to dip
        /// </summary>
        /// <remarks>dpi defaults to 96</remarks>
        internal static float PointToDip(int pt)
        {
            var dip = pt * 96.0f / 72.0f;
            return dip;
        }

        /// <summary>
        /// convert dip to pt
        /// </summary>
        /// <remarks>dpi defaults to 96</remarks>
        internal static float DipToPoint(int dip)
        {
            var pt = dip * 72.0f / 96.0f;
            return pt;
        }

        public static void CheckGLError(
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = null)
        {
            var error = GL.GetError();
            string errorStr = "GL_NO_ERROR";
            switch (error)
            {
                case GL.GL_NO_ERROR:
                    errorStr = "GL_NO_ERROR";
                    break;
                case GL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case GL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case GL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case GL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case GL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case GL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case GL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case GL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
            }

            if (error != GL.GL_NO_ERROR)
            {
                Debug.WriteLine("{0}({1}): glError: 0x{2:X} ({3}) in {4}",
                    fileName, lineNumber, error, errorStr, memberName);
            }
        }

        public static void CheckGLESError(
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = null)
        {
            var error = CSharpGLES.GL.GetError();
            string errorStr = "GL_NO_ERROR";
            switch (error)
            {
                case GL.GL_NO_ERROR:
                    errorStr = "GL_NO_ERROR";
                    break;
                case GL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case GL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case GL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case GL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case GL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case GL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case GL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case GL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
            }

            if (error != GL.GL_NO_ERROR)
            {
                Debug.WriteLine("{0}({1}): glError: 0x{2:X} ({3}) in {4}",
                    fileName, lineNumber, error, errorStr, memberName);
            }
        }

        public static T Create<T>(Platform platform)
        {
            var IFuncType = typeof(T);
#if DEBUG
            if (!IFuncType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException("T must be an interface");
            }
#endif
            var asm = typeof(Utility).GetTypeInfo().Assembly;
            var asmTypes = asm.GetTypes();
            var funcType = asmTypes.FirstOrDefault(t =>
            {
                var typeInfo = t.GetTypeInfo();
                if (typeInfo.IsInterface)
                {
                    return false;
                }
                if (!IFuncType.GetTypeInfo().IsAssignableFrom(t))
                {
                    return false;
                }
                var platformAttr = (PlatformAttribute)typeInfo.GetCustomAttribute(typeof(PlatformAttribute), false);
                if (platformAttr == null || ((platformAttr.Value & platform) != platform))
                {
                    return false;
                }
                return true;
            });
            if (funcType == null) { throw new InvalidOperationException(); }
            var func = (T)Activator.CreateInstance(funcType);
            return func;
        }

        public static System.IO.Stream ReadFile(string filePath)
        {
            Stream stream = null;
            if (CurrentOS.IsAndroid)
            {
#if __ANDROID__
                var assets = (Android.Content.Res.AssetManager)Application.AssetManager;
                var s = assets.Open(filePath, Android.Content.Res.Access.Buffer);
                using (var ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    stream = new MemoryStream(ms.ToArray());
                }
#endif
            }
            else
            {
                var s = new FileStream(filePath, FileMode.Open);
                stream = s;
                //using (var ms = new MemoryStream())
                //{
                //    s.CopyTo(ms);
                //    stream = new MemoryStream(ms.ToArray());
                //}
            }
            return stream;
        }

        //private static string CompositeMS_base64_str = @"AAEAAAALAIAAAwAwT1MvMoa5eREAAAC8AAAAVmNtYXAhqDLeAAABFAAAAUpjdnQgAb8ESgAAAmAAAAAQZ2x5ZgQ7LD4AAAJwAAACYmhlYWTaMry2AAAE1AAAADZoaGVhC+EIFAAABQwAAAAkaG10eDu6EDAAAAUwAAAASGxvY2EF6AVnAAAFeAAAACZtYXhwAFgAOwAABaAAAAAgbmFtZYJgMvwAAAXAAAACynBvc3QA0wFMAAAIjAAAAEYAAQPrAfQABQAAAMgAyAAAAMgAyADIAAAAyAAxAQIAAAIABgMAAAAAAAAAAAADAAAAAAAAAAAAAAAAZ3d3AABAACAA/wXl/sIAWgXlAT4AAAABAAAAAAAAAAAAAwAAAAMAAAAcAAEAAAAAAEQAAwABAAAAHAAEACgAAAAGAAQAAQACACAATP//AAAAIAA/////4//FAAEAAAAAAAAAAAEGAAABAAAAAAAAAAEDAAAAAQAAAAAAAAAAAAAAAAAAAAEAAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBQYHCAkKCwwNDg8QEQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIQJ5ADABhgAAABUBbgA2AAIAIQAAAW4CmgADAAcALrEBAC88sgcEAO0ysQYF3DyyAwIA7TIAsQMALzyyBQQA7TKyBwYB/DyyAQIA7TIzESERJSERISEBTf7UAQv+9QKa/WYhAlgAAgAAAAAIZwXlAAMABwAAATUzFQE1MxUIPSr3mSoFuyoq+kUqKgABAAABqgGGAdoAAwAnALACL7ADM7EAAu2xAQLtAbAAL7ADM7EBA+2xAgPtMDGxBQEQzDAxESEVIQGG/noB2jAAAQAAAAAANgFuAAMAJwCxAgQ/sAMzsQAGP7ABMwGwAC+wAzOxAQftsQIH7TAxsQUBEMwwMREzESM2NgFu/pIA/////gIKAYYELxAvAAb//gIKYAAQBwAFAAABMf//AAACUQG+BBAQpwAGAAACi0PhvB5D4UPhEAcABQAAAb3//wAAAi4CBgOIEKcABgABAnQwAKzcUyMwABAHAAUAAAGS//8CXP7CA74CJRAvAAYCXAAAYAAQBwAGA4j+wv//Ayr//wToAb4QpwAGAyoAOUPhvB5D4UPhEAcABgSEAAD//wMuAAAFMwFuEKcABgMuAEYwAKzcUyMwABAHAAYEiwAA//8B9AAAAioC/hBnAAYCKgAAwABAABAHAAYB9AGQ//8B9AAAA/gBbhCnAAYB9AAwMADIk1MjIAAQBwAGAggAAP//AfQAAAP4AW4QpwAGA/cAMNAAyJOs3CAAEAcABgKGAAD//wGBAAACqwKKEKcABgGBAYYtQdK+LUEtQRAHAAYBhgAA//8AAAF/Ab4DPhCnAAYAAAG5Q+G8HkPhQ+EQBwAFAAAA6wAAAAEAAAAAAACO5ysnXw889QATA+gAAAAAuVO7lQAAAAC5U7uV//7+wghnBeUAAAAIAAIAAAAAAAAAAQAAAyD/OABaCJj//gAxCGcAAQAAAAAAAAAAAAAAAAAAABIBsAAhAAAAAAFNAAAEQgAACJgAAAHGAAAAuAAAAbr//gHzAAAGDwAABDYCXAWVAyoGDwMuAmgB9AROAfQEPAH0AukBgQHzAAAAAAArACsAKwArAD4AXgB+AIwAnQCuALwAzQDeAO0A/gEPASABMQAAAAEAAAASAAgAAgAAAAAAAgAAAAEAAQAAAEAALgACAAIAAAAVAQIAAQAAAAAAAABDAAAAAQAAAAAAAQALAEQAAQAAAAAAAgAHAFAAAQAAAAAAAwAeAFgAAQAAAAAABAAMAHcAAQAAAAAABQAHAIQAAQAAAAAABgALAIwAAAADAAAAAACGAJgAAAADAAAAAQAWASAAAAADAAAAAgAOATgAAAADAAAAAwA8AUgAAAADAAAABAAYAYYAAAADAAAABQAOAaAAAAADAAAABgAWAbAAAwABBAkAAACGAJgAAwABBAkAAQAWASAAAwABBAkAAgAOATgAAwABBAkAAwA8AUgAAwABBAkABAAYAYYAAwABBAkABQAOAaAAAwABBAkABgAWAbBDcmVhdGVkIGJ5IEdlb3JnZSBXaWxsaWFtcyB3aXRoIFBmYUVkaXQgMS4wIChodHRwOi8vcGZhZWRpdC5zZi5uZXQpAENvbXBvc2l0ZU1TAFJlZ3VsYXIAZ3d3IDogQ29tcG9zaXRlIE1TIDogMTEtNi0yMDcyAENvbXBvc2l0ZSBNUwAwMDEuMDAwAENvbXBvc2l0ZU1TAABDAHIAZQBhAHQAZQBkACAAYgB5ACAARwBlAG8AcgBnAGUAIABXAGkAbABsAGkAYQBtAHMAIAB3AGkAdABoACAAUABmAGEARQBkAGkAdAAgADEALgAwACAAKABoAHQAdABwADoALwAvAHAAZgBhAGUAZABpAHQALgBzAGYALgBuAGUAdAApAAAAQwBvAG0AcABvAHMAaQB0AGUATQBTAAAAUgBlAGcAdQBsAGEAcgAAAGcAdwB3ACAAOgAgAEMAbwBtAHAAbwBzAGkAdABlACAATQBTACAAOgAgADEAMQAtADYALQAyADAANwAyAAAAQwBvAG0AcABvAHMAaQB0AGUAIABNAFMAAAAwADAAMQAuADAAMAAwAAAAQwBvAG0AcABvAHMAaQB0AGUATQBTAAAAAAACAAAAAAAA/5wAMgAAAAAAAAAAAAAAAAAAAAAAAAAAABIAAAABAAIAAwAiACMAJAAlACYAJwAoACkAKgArACwALQAuAC8AAA==";
    }


    public class Profile
    {
        private static string name;
        private static Stopwatch watch = new Stopwatch();

        [Conditional("DEBUG")]
        public static void Start(string name)
        {
            Profile.name = name;
            watch.Start();
        }

        [Conditional("DEBUG")]
        public static void End()
        {
            watch.Stop();
            Debug.WriteLine("[Profile]{0} {1} ms", name, watch.ElapsedMilliseconds.ToString());
            watch.Reset();
        }
    }
}