#region Assembly Microsoft.SqlServer.FTPTaskUI, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// C:\Users\LENOVO\Downloads\New folder\Microsoft.SqlServer.FTPTaskUI\v4.0_13.0.0.0__89845dcd8080cc91\Microsoft.SqlServer.FtpTaskUI.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Dts.Runtime.Localization;

namespace Microsoft.SqlServer.Dts.Tasks.FtpTask;

[HelpContext("sql13.dts.designer.ftptask.general.f1")]
internal class FTPTaskGeneralView : UserControl, IDTSTaskUIView
{
    [SortProperties(new string[] { "Name", "Description", "FtpConnection", "StopOnFailure" })]
    internal class GeneralNode
    {
        private string _name;

        private string _desc;

        private TaskHost _taskHost;

        private FtpTask _ftpTask;

        private object _connections;

        private bool _stopOnFailure;

        private string _ftpConnection;

        [LocalizablePropertyDescription(typeof(SR), "GENERAL_NAME_DOC")]
        [LocalizablePropertyCategory(typeof(SR), "GENERAL")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    throw new ApplicationException(SR.HOST_NAME_CANT_EMPTY);
                }

                _name = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "GENERAL")]
        [LocalizablePropertyDescription(typeof(SR), "GENERAL_DESC_DOC")]
        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }

        [LocalizablePropertyDescription(typeof(SR), "CONNECTION_DESC")]
        [LocalizablePropertyCategory(typeof(SR), "CONNECTION")]
        [TypeConverter(typeof(FtpConnections))]
        public string FtpConnection
        {
            get
            {
                return _ftpConnection;
            }
            set
            {
                _ftpConnection = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "CONNECTION")]
        [LocalizablePropertyDescription(typeof(SR), "STOP_ON_FAILURE_DESC")]
        public bool StopOnFailure
        {
            get
            {
                return _stopOnFailure;
            }
            set
            {
                _stopOnFailure = value;
            }
        }

        internal object DTSConnections => _connections;

        public GeneralNode(object taskHost, object connections)
        {
            //IL_003d: Unknown result type (might be due to invalid IL or missing references)
            _taskHost = (TaskHost)((taskHost is TaskHost) ? taskHost : null);
            _connections = connections;
            _name = ((DtsContainer)_taskHost).Name;
            _desc = ((DtsContainer)_taskHost).Description;
            _ftpTask = ((TaskHost)taskHost).InnerObject as FtpTask;
            _stopOnFailure = _ftpTask.StopOnOperationFailure;
            _ftpConnection = _ftpTask.Connection;
        }
    }

    private Container components;

    private LocalizablePropertyGrid propertyGridGeneral;

    internal GeneralNode _generalNode;

    internal static string ftpConnection;

    private GeneralNode GeneralProperties => _generalNode;

    public FTPTaskGeneralView()
    {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        ((ContainerControl)this).Dispose(disposing);
    }

    private void InitializeComponent()
    {
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Expected O, but got Unknown
        //IL_0073: Unknown result type (might be due to invalid IL or missing references)
        //IL_007d: Expected O, but got Unknown
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FTPTaskGeneralView));
        propertyGridGeneral = new LocalizablePropertyGrid();
        ((Control)this).SuspendLayout();
        componentResourceManager.ApplyResources(propertyGridGeneral, "propertyGridGeneral");
        propertyGridGeneral.LocalizableSelectedObject = null;
        ((Control)propertyGridGeneral).Name = "propertyGridGeneral";
        ((PropertyGrid)propertyGridGeneral).PropertySort = (PropertySort)2;
        ((PropertyGrid)propertyGridGeneral).ToolbarVisible = false;
        ((PropertyGrid)propertyGridGeneral).PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGridGeneral_PropertyValueChanged);
        ((Control)this).Controls.Add((Control)(object)propertyGridGeneral);
        ((Control)this).Name = "FTPTaskGeneralView";
        componentResourceManager.ApplyResources(this, "$this");
        ((Control)this).ResumeLayout(false);
    }

    public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
    {
        //IL_0014: Unknown result type (might be due to invalid IL or missing references)
        if (taskHost == null)
        {
            throw new ArgumentNullException(SR.DTS_MSG_TASKHOST, SR.DTS_E_INITIALIZATION_WITH_NULL_TASK);
        }

        if (!(((TaskHost)taskHost).InnerObject is FtpTask))
        {
            throw new ArgumentException(SR.DTS_E_UI_INITIALIZATION_WITH_WRONG_TASK, SR.DTS_MSG_TASKHOST);
        }

        _generalNode = new GeneralNode(taskHost, connections);
        propertyGridGeneral.LocalizableSelectedObject = _generalNode;
    }

    public virtual void OnValidate(ref bool bViewIsValid, ref string reason)
    {
    }

    public virtual void OnSelection()
    {
    }

    public virtual void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
    {
    }

    public virtual void OnCommit(object taskHost)
    {
        //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //IL_0007: Expected O, but got Unknown
        //IL_002f: Unknown result type (might be due to invalid IL or missing references)
        TaskHost val = (TaskHost)taskHost;
        ((DtsContainer)val).Name = GeneralProperties.Name.Trim();
        ((DtsContainer)val).Description = GeneralProperties.Description;
        FtpTask ftpTask = ((TaskHost)taskHost).InnerObject as FtpTask;
        ftpTask.Connection = GeneralProperties.FtpConnection;
        ftpTask.StopOnOperationFailure = GeneralProperties.StopOnFailure;
    }

    private void propertyGridGeneral_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        //IL_005d: Unknown result type (might be due to invalid IL or missing references)
        //IL_008b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0091: Expected O, but got Unknown
        if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("FtpConnection") == 0 && e.ChangedItem.Value.Equals(SR.NEW_CONNECTION))
        {
            ArrayList arrayList = null;
            ((Control)this).Cursor = Cursors.WaitCursor;
            GeneralProperties.FtpConnection = null;
            arrayList = ((IDtsConnectionBaseService)(IDtsConnectionService)GeneralProperties.DTSConnections).CreateConnection("FTP");
            ((Control)this).Cursor = Cursors.Default;
            if (arrayList != null && arrayList.Count > 0)
            {
                ConnectionManager val = (ConnectionManager)arrayList[0];
                GeneralProperties.FtpConnection = val.Name;
            }
            else if (e.OldValue == null)
            {
                GeneralProperties.FtpConnection = null;
            }
            else
            {
                GeneralProperties.FtpConnection = (string)e.OldValue;
            }
        }
    }
}
#if false // Decompilation log
'11' items in cache
------------------
Resolve: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
------------------
Resolve: 'System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
------------------
Resolve: 'Microsoft.DataTransformationServices.Controls, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Could not find by name: 'Microsoft.DataTransformationServices.Controls, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
------------------
Resolve: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll'
------------------
Resolve: 'Microsoft.SqlServer.Dts.Design, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Could not find by name: 'Microsoft.SqlServer.Dts.Design, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
------------------
Resolve: 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Could not find by name: 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
------------------
Resolve: 'Microsoft.SqlServer.ManagedDTS, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Could not find by name: 'Microsoft.SqlServer.ManagedDTS, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
------------------
Resolve: 'Microsoft.SqlServer.FtpTask, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Found single assembly: 'Microsoft.SqlServer.FtpTask, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Load from: 'C:\Users\LENOVO\Downloads\New folder\Microsoft.SqlServer.FtpTask\v4.0_13.0.0.0__89845dcd8080cc91\Microsoft.SqlServer.FtpTask.dll'
------------------
Resolve: 'Microsoft.NetEnterpriseServers.ExceptionMessageBox, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
Could not find by name: 'Microsoft.NetEnterpriseServers.ExceptionMessageBox, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91'
------------------
Resolve: 'mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
WARN: Version mismatch. Expected: '2.0.0.0', Got: '4.0.0.0'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
------------------
Resolve: 'System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Could not find by name: 'System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
#endif
