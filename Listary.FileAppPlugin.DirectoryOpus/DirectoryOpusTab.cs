using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Listary.FileAppPlugin.DirectoryOpus
{
    public class DirectoryOpusTab : IFileTab, IGetFolder, IOpenFolder
    {
        private IFileAppPluginHost _host;
        private DirectoryOpusWindow _parent;

        public DirectoryOpusTab(IFileAppPluginHost host, DirectoryOpusWindow parent)
        {
            _host = host;
            _parent = parent;
        }

        public async Task<string> GetCurrentFolder()
        {
            HWND focus = Win32Utils.GetFocus(new(_parent.Handle));
            if (focus != default)
            {
                if (Win32Utils.GetClassName(focus) is "dopus.filedisplay" or "dopus.iconfiledisplay")
                {
                    HWND tab = PInvoke.GetParent(focus);
                    if (tab != default && Win32Utils.GetClassName(tab) == "dopus.filedisplaycontainer")
                    {
                        HWND tabPathControl = PInvoke.FindWindow("dopus.ctl.treepath", default);
                        return tabPathControl != default ? Win32Utils.GetWindowText(tabPathControl) : Win32Utils.GetWindowText(tab);
                    }
                }
                else
                {
                    _host.Logger.LogDebug("Unknown focus window class");
                }
            }
            else
            {
                _host.Logger.LogDebug("Cannot get focus window");
            }

            HWND addressBar = GetAddressBar();
            if (addressBar != default)
            {
                return Win32Utils.GetWindowText(addressBar);
            }
            else
            {
                _host.Logger.LogError("GetAddressBar failed");
            }

            return default;
        }

        private HWND GetAddressBar()
        {
            HWND locationBar = Win32Utils.FindWindowExRecursively(new(_parent.Handle), default, "dopus.ctl.treepath", default);
            if (locationBar != default)
            {
                return Win32Utils.FindWindowExRecursively(locationBar, default, "Edit", default);
            }
            return default;
        }

        public async Task<bool> OpenFolder(string path)
        {
            HWND dopus = PInvoke.FindWindow("DOpus.ParentWindow", "Directory Opus");
            if (dopus == default)
            {
                _host.Logger.LogError("Cannot find DOpus.ParentWindow");
                return false;
            }

            string command = $"Go \"{path}\"\0";  // with a trailing \0
            byte[] bytes = new UnicodeEncoding().GetBytes(command);
            IntPtr result = _host.SendCopyData(dopus, default, (IntPtr)0x14, bytes, (uint)bytes.Length);
            return result != IntPtr.Zero;
        }
    }
}
