using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Windows.Forms;

namespace LogVariableTaskUI
{
    internal class LogVariableTaskUI : IDtsTaskUI
    {
        private TaskHost _taskHost;
        public LogVariableTaskUI()
        {
            
        }
        public void New(System.Windows.Forms.IWin32Window form)
        {

        }
        void IDtsTaskUI.Delete(System.Windows.Forms.IWin32Window parentWindow)
        {
            throw new NotImplementedException();
        }

        System.Windows.Forms.ContainerControl IDtsTaskUI.GetView()
        {
            throw new NotImplementedException();
        }

        void IDtsTaskUI.Initialize(TaskHost taskHost, IServiceProvider serviceProvider)
        {
            _taskHost = taskHost;
    }

    void IDtsTaskUI.New(System.Windows.Forms.IWin32Window parentWindow)
        {
            throw new NotImplementedException();
        }
    }
}
