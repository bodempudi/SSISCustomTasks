using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Windows.Forms;

namespace ExecuteCatalogPackageTaskComplexUI
{
    public class ExecuteCatalogPackageTaskComplexUI : IDtsTaskUI
    {
        // Public key token: a68173515d1ee3e3

        private TaskHost taskHost = null;
        private IDtsConnectionService connectionService = null;

        public void Initialize(TaskHost taskHost, IServiceProvider serviceProvider)
        {
            this.taskHost = taskHost;
            this.connectionService = serviceProvider.GetService(typeof(IDtsConnectionService)) as IDtsConnectionService;
        }

        public ContainerControl GetView()
        {
            return new ExecuteCatalogPackageTaskComplexUIForm(taskHost, connectionService);
        }

        public void New(IWin32Window parentWindow)
        {
            // throw new NotImplementedException();
        }

        public void Delete(IWin32Window parentWindow)
        {
            // throw new NotImplementedException();
        }

    }
}
