using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listary.FileAppPlugin.DirectoryOpus
{
    public class DirectoryOpusPlugin : IFileAppPlugin
    {
        private IFileAppPluginHost _host;

        public bool IsOpenedFolderProvider => true;

        public bool IsQuickSwitchTarget => false;

        public bool IsSharedAcrossApplications => false;

        public SearchBarType SearchBarType => SearchBarType.Floating;

        public async Task<bool> Initialize(IFileAppPluginHost host)
        {
            _host = host;
            return true;
        }
        
        public IFileWindow BindFileWindow(IntPtr hWnd)
        {
            if (Win32Utils.GetClassName(new(hWnd)) == "dopus.lister")
            {
                return new DirectoryOpusWindow(_host, hWnd);
            }
            return default;
        }
    }
}
