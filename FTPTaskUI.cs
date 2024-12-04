using System;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace Microsoft.SqlServer.Dts.Tasks.FtpTask;

public class FTPTaskUI : IDtsTaskUI
{
    private TaskHost _taskHost;

    private IDtsConnectionService _connectionService;

    public string ToolTip => SR.DTS_MSG_TOOLTIP_TEXT;

    public void Initialize(TaskHost taskWrapper, IServiceProvider serviceProvider)
    {
        if ((DtsObject)(object)taskWrapper == (DtsObject)null)
        {
            throw new ArgumentNullException(SR.DTS_MSG_TASK_WRAPPER, SR.DTS_E_INITIALIZATION_WITH_NULL_TASK);
        }

        if (!(taskWrapper.InnerObject is FtpTask))
        {
            throw new ArgumentException(SR.DTS_E_UI_INITIALIZATION_WITH_WRONG_TASK, SR.DTS_MSG_TASK_WRAPPER);
        }

        if (serviceProvider == null)
        {
            throw new ApplicationException(SR.ServiceProviderRequired);
        }

        ref IDtsConnectionService connectionService = ref _connectionService;
        object service = serviceProvider.GetService(typeof(IDtsConnectionService));
        connectionService = (IDtsConnectionService)((service is IDtsConnectionService) ? service : null);
        if (_connectionService == null)
        {
            throw new ApplicationException(SR.CantAccess("IDtsConnectionService"));
        }

        _taskHost = taskWrapper;
    }

    public ContainerControl GetView()
    {
        return (ContainerControl)(object)new FTPTaskMainWnd(_taskHost, _connectionService);
    }

    public void New(IWin32Window parentWindow)
    {
    }

    public void Delete(IWin32Window parentWindow)
    {
    }
}
