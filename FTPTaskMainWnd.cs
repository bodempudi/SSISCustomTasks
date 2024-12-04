using System;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;

namespace Microsoft.SqlServer.Dts.Tasks.FtpTask;

internal class FTPTaskMainWnd : DTSBaseTaskUI
{
    public FTPTaskMainWnd(TaskHost taskHost, object connections)
        : base(SR.TITLE, FTPTaskUIIcons.Task32x32, SR.DESC, (object)taskHost, connections)
    {
        FTPTaskGeneralView fTPTaskGeneralView = new FTPTaskGeneralView();
        FTPTaskOperationsView fTPTaskOperationsView = new FTPTaskOperationsView();
        ((DTSBaseTaskUI)this).DTSTaskUIHost.AddView(SR.GENERAL, (IDTSTaskUIView)(object)fTPTaskGeneralView, (TreeNode)null);
        ((DTSBaseTaskUI)this).DTSTaskUIHost.AddView(SR.TASK_OPERATIONS, (IDTSTaskUIView)(object)fTPTaskOperationsView, (TreeNode)null);
    }

    protected override void Dispose(bool disposing)
    {
        ((DTSBaseTaskUI)this).Dispose(disposing);
    }

    protected override void OnCancel(object sender, EventArgs e)
    {
        ((Form)this).Close();
    }
}
