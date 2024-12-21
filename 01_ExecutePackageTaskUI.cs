 

using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Design.Project;
using Microsoft.DataWarehouse.VsIntegration.Shell.Project;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

public class ExecutePackageTaskUI : IDtsTaskUI
{
    private TaskHost m_task;

    private IDtsConnectionService m_connService;

    private IObjectModelProjectManager m_obProjectManager;

    private Project m_project;

    private static readonly TraceContext s_tc = TraceContext.GetTraceContext("ExecutePackageTaskUI", "ExecutePackageTaskUI");

    public void Initialize(TaskHost taskHost, IServiceProvider serviceProvider)
    {
        m_obProjectManager = null;
        if (serviceProvider != null)
        {
            object service = serviceProvider.GetService(typeof(IFileProjectHierarchy));
            IFileProjectHierarchy val = (IFileProjectHierarchy)((service is IFileProjectHierarchy) ? service : null);
            if (val != null)
            {
                IFileProjectManager projectManager = val.ProjectManager;
                if (projectManager != null)
                {
                    m_obProjectManager = (IObjectModelProjectManager)(object)((projectManager is IObjectModelProjectManager) ? projectManager : null);
                }
            }
        }

        if (m_obProjectManager != null)
        {
            m_project = m_obProjectManager.ObjectModelProject;
        }

        if ((DtsObject)(object)taskHost == (DtsObject)null)
        {
            throw new ArgumentNullException(Localized.TaskHost, Localized.CannotInitializeToNullTaskException);
        }

        if (!(taskHost.InnerObject is ExecutePackageTask))
        {
            throw new ArgumentException(Localized.MustInitializeToOwnTaskException, Localized.TaskHost);
        }

        m_task = taskHost;
        ref IDtsConnectionService connService = ref m_connService;
        object service2 = serviceProvider.GetService(typeof(IDtsConnectionService));
        connService = (IDtsConnectionService)((service2 is IDtsConnectionService) ? service2 : null);
        if (m_connService == null)
        {
            throw new ApplicationException(Localized.CouldNotAccessIDtsConnectionServiceException);
        }

        if ((IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost)) == null)
        {
            throw new ApplicationException(Localized.CouldNotAccessIDesignerHostException);
        }
    }

    public ContainerControl GetView()
    {
        return (ContainerControl)(object)new ExecutePackageMainWnd(m_task, m_connService, m_obProjectManager, m_project);
    }

    public void New(IWin32Window parentWindow)
    {
    }

    public void Delete(IWin32Window parentWindow)
    {
    }
}
