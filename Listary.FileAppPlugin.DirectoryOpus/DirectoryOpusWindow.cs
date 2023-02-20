using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listary.FileAppPlugin.DirectoryOpus
{
    public class DirectoryOpusWindow : IFileWindow
    {
        private IFileAppPluginHost _host;

        public IntPtr Handle { get; }

        public DirectoryOpusWindow(IFileAppPluginHost host, IntPtr hWnd)
        {
            _host = host;
            Handle = hWnd;
        }

        public async Task<IFileTab> GetCurrentTab()
        {
            return new DirectoryOpusTab(_host, this);
        }
    }
}
