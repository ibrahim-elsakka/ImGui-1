﻿using ImGui;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        Demo demo = new Demo();

        protected override void OnGUI()
        {
            //GUILayout.Label("Hello ImGui,");
            demo.OnGUI();
            //GUILayout.BeginVertical("H", GUILayout.ExpandWidth(true).ExpandHeight(true));
            //    GUILayout.Button("A", GUILayout.Width(200).Height(120));
            //    GUILayout.Button("B", GUILayout.Width(200).Height(120));
            //    GUILayout.Button("C", GUILayout.Width(200).Height(120));
            //    GUILayout.Button("D", GUILayout.Width(200).Height(120));
            //GUILayout.EndHorizontal();
        }
    }
}

