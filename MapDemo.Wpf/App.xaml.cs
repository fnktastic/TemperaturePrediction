using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.DemoBase;

namespace WpfDemo {
    public partial class App : Application {
        static App() {
            DemoBaseControl.SetApplicationTheme();
        }
#if DEBUG
        public bool IsDebug { get { return true; } }
#endif
    }
}
