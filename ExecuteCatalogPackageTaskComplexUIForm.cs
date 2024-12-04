using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DataTransformationServices.Controls;
using System.Drawing;
using Microsoft.SqlServer.Dts.Runtime;

namespace ExecuteCatalogPackageTaskComplexUI
{
    public partial class ExecuteCatalogPackageTaskComplexUIForm : DTSBaseTaskUI
    {
        private const string Title = "Execute Catalog Package Task Editor";
        private const string Description = "This task executes an SSIS package in an SSIS Catalog.";
        public static Icon taskIcon = new Icon(typeof(ExecuteCatalogPackageTask.ExecuteCatalogPackageTask), "ALCStrike.ico");

        public ExecuteCatalogPackageTaskComplexUIForm(TaskHost taskHost, object connections) :
         base(Title, taskIcon, Description, taskHost, connections)
        {
            // Add General view
            GeneralView generalView = new GeneralView();
            this.DTSTaskUIHost.AddView("General", generalView, null);
            // Add Settings view
            SettingsView settingsView = new SettingsView();
            this.DTSTaskUIHost.AddView("Settings", settingsView, null);
        }
    }
}
