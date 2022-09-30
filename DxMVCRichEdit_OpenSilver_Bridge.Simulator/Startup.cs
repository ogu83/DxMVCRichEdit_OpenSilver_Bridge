using OpenSilver.Simulator;
using System;

namespace DxMVCRichEdit_OpenSilver_Bridge.Simulator
{
    internal static class Startup
    {
        [STAThread]
        static int Main(string[] args)
        {
            return SimulatorLauncher.Start(typeof(App));
        }
    }
}
