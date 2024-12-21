 
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.DataTransformationServices.Design.Project;
using Microsoft.SqlServer.Dts.Runtime;

namespace Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

internal class ExecutePackageMainWnd : DTSBaseTaskUI
{
    internal ExecutePackageMainWnd(TaskHost taskHost, object connections, IObjectModelProjectManager obProjectManager, Project project)
        : base(Localized.Title, ExecPkgUIIcons.Task32x32, Localized.TitleDesc, (object)taskHost, connections)
    {
        if ((DtsObject)(object)taskHost == (DtsObject)null)
        {
            throw new ArgumentNullException(Localized.TaskHost, Localized.CannotInitializeToNullTaskException);
        }

        if (!(taskHost.InnerObject is ExecutePackageTask))
        {
            throw new ArgumentException(Localized.MustInitializeToOwnTaskException, Localized.TaskHost);
        }

        if (connections == null)
        {
            throw new ApplicationException(Localized.CouldNotAccessIDtsConnectionServiceException);
        }

        GeneralView generalView = new GeneralView();
        PackageView packageView = new PackageView(project);
        ParametersView parametersView = new ParametersView(obProjectManager, project);
        ((DTSBaseTaskUI)this).DTSTaskUIHost.AddView(Localized.General, (IDTSTaskUIView)(object)generalView, (TreeNode)null);
        ((DTSBaseTaskUI)this).DTSTaskUIHost.AddView(Localized.Package, (IDTSTaskUIView)(object)packageView, (TreeNode)null);
        ((DTSBaseTaskUI)this).DTSTaskUIHost.AddView(Localized.Parameters, (IDTSTaskUIView)(object)parametersView, (TreeNode)null);
    }

    protected override void Dispose(bool disposing)
    {
        ((DTSBaseTaskUI)this).Dispose(disposing);
    }

    protected override void OnCancel(object sender, EventArgs e)
    {
        ((Form)this).Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
    }
}
