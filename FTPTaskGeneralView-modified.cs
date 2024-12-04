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

internal class FTPTaskGeneralView : UserControl, IDTSTaskUIView
{
    [SortProperties(new string[] { "Name", "Description" })]
    internal class GeneralNode
    {
        private string _name;

        private string _desc;

        private TaskHost _taskHost;
 
        private object _connections;

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

         

        internal object DTSConnections => _connections;

        public GeneralNode(object taskHost, object connections)
        {
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
         
        if (taskHost == null)
        {
            throw new ArgumentNullException(SR.DTS_MSG_TASKHOST, SR.DTS_E_INITIALIZATION_WITH_NULL_TASK);
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
        TaskHost val = (TaskHost)taskHost;
        ((DtsContainer)val).Name = GeneralProperties.Name.Trim();
        ((DtsContainer)val).Description = GeneralProperties.Description;
    }

     
}
