﻿using ImGui.Common.Primitive;
using Xunit;

namespace ImGui.UnitTest
{
    public class LabelFacts
    {
        public class Label
        {
            [Fact]
            public void ShowOneFixedLabel()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                form.OnGUIAction = () =>
                {
                    GUI.Label(new Rect(0, 0, 100, 30), "Some Text");
                };

                Application.Run(form);
            }
        }
    }
}