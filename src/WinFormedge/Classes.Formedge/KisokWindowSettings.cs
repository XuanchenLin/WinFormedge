// This file is part of the WinFormedge project.
// Copyright (c) 2025 Xuanchen Lin all rights reserved.
// This project is licensed under the MIT License.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormedge;
public class KisokWindowSettings : WindowSettings
{

    public override bool Fullscreen { get; set; }

    protected internal override bool HasSystemTitlebar => false;

    protected internal override WindowProc? WndProc { get; set; }

    protected internal override WindowProc? DefWndProc { get; set; }

    public Screen TargetScreen { get; set; } = Screen.PrimaryScreen!;

    protected internal override Form CreateHostWindow()
    {
        var form = new KisokWindow(this)
        {
            // Set properties for the KisokWindow
            FormBorderStyle = FormBorderStyle.None,
            StartPosition = FormStartPosition.Manual,
            WindowState = FormWindowState.Maximized,
            ShowInTaskbar = false,
            Bounds = TargetScreen.Bounds
        };

        return form;
    }

    class KisokWindow : Form
    {
        private Screen _screen;

        public KisokWindow(KisokWindowSettings settings)
        {
            Settings = settings;
            // Initialize the window with the settings
            _screen = settings.TargetScreen;
        }

        public KisokWindowSettings Settings { get; }

        protected override void WndProc(ref Message m)
        {
            if(Settings.WndProc?.Invoke(ref m) ?? false)
            {
                return;
            }

            base.WndProc(ref m);
        }

        protected override void DefWndProc(ref Message m)
        {
            if (Settings.DefWndProc?.Invoke(ref m) ?? false)
            {
                return;
            }


            base.DefWndProc(ref m);
        }
    }
}

public static class KisokWindowSettingsExtension
{
    public static KisokWindowSettings KisokWindow(this HostWindowBuilder builder)
    {
        return new KisokWindowSettings();
    }

}