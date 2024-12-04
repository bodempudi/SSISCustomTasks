 
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.NetEnterpriseServers;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Dts.Runtime.Localization;

namespace Microsoft.SqlServer.Dts.Tasks.FtpTask;

[HelpContext("sql13.dts.designer.ftptask.filetransfer.f1")]
internal class FTPTaskOperationsView : UserControl, IDTSTaskUIView
{
    [SortProperties(new string[] { "Operation", "IsTransferAscii", "IsLocalPathVariable", "LocalVariable", "LocalPath", "IsRemotePathVariable", "RemoteVariable", "RemotePath", "OverwriteFileAtDest" })]
    internal class TaskOperationNode : ICustomTypeDescriptor
    {
        private string _operationType = SR.SEND_FILES;

        private bool _isTransferAscii;

        private string _sourcePath;

        private bool _isSrcPathVar;

        private string _destnPath;

        private bool _isDestnPathVar;

        private bool _overWriteDest;

        private TaskHost _taskHost;

        private IDtsVariableService _variableService;

        internal IDtsConnectionService iDtsConnService;

        internal string OperationType => _operationType;

        internal TaskHost Taskhost => _taskHost;

        internal IDtsVariableService VariableService => _variableService;

        [LocalizablePropertyDescription(typeof(SR), "OPERATION_DESC")]
        [LocalizablePropertyCategory(typeof(SR), "OPERATION")]
        [TypeConverter(typeof(OperationTypes))]
        [RefreshProperties(RefreshProperties.All)]
        public string Operation
        {
            get
            {
                return _operationType;
            }
            set
            {
                _operationType = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "OPERATION")]
        [LocalizablePropertyDescription(typeof(SR), "TRANSFER_ASCII_DESC")]
        public bool IsTransferAscii
        {
            get
            {
                return _isTransferAscii;
            }
            set
            {
                _isTransferAscii = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "LOCAL_PARAMETERS")]
        [LocalizablePropertyDescription(typeof(SR), "LOCAL_PATH_DESC")]
        [TypeConverter(typeof(FileConnections))]
        public string LocalPath
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "LOCAL_PARAMETERS")]
        [LocalizablePropertyDescription(typeof(SR), "LOCAL_VARIABLE_DESC")]
        [TypeConverter(typeof(GetVariables))]
        public string LocalVariable
        {
            get
            {
                return _sourcePath;
            }
            set
            {
                _sourcePath = value;
            }
        }

        [LocalizablePropertyDescription(typeof(SR), "IS_LOCAL_PATH_VAR_DESC")]
        [RefreshProperties(RefreshProperties.All)]
        [LocalizablePropertyCategory(typeof(SR), "LOCAL_PARAMETERS")]
        public bool IsLocalPathVariable
        {
            get
            {
                return _isSrcPathVar;
            }
            set
            {
                _isSrcPathVar = value;
                _sourcePath = "";
            }
        }

        [LocalizablePropertyDescription(typeof(SR), "IS_REMOTE_PATH_VAR_DESC")]
        [LocalizablePropertyCategory(typeof(SR), "REMOTE_PARAMETERS")]
        [RefreshProperties(RefreshProperties.All)]
        public bool IsRemotePathVariable
        {
            get
            {
                return _isDestnPathVar;
            }
            set
            {
                _isDestnPathVar = value;
                _destnPath = "";
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "REMOTE_PARAMETERS")]
        [LocalizablePropertyDescription(typeof(SR), "REMOTE_PATH_DESC")]
        [RefreshProperties(RefreshProperties.All)]
        [Editor(typeof(FtpBrowserEditor), typeof(UITypeEditor))]
        public string RemotePath
        {
            get
            {
                return _destnPath;
            }
            set
            {
                _destnPath = value;
            }
        }

        [TypeConverter(typeof(GetVariables))]
        [LocalizablePropertyDescription(typeof(SR), "REMOTE_VARIABLE_DESC")]
        [LocalizablePropertyCategory(typeof(SR), "REMOTE_PARAMETERS")]
        public string RemoteVariable
        {
            get
            {
                return _destnPath;
            }
            set
            {
                _destnPath = value;
            }
        }

        [LocalizablePropertyCategory(typeof(SR), "REMOTE_PARAMETERS")]
        [LocalizablePropertyDescription(typeof(SR), "OVER_WRITE_DEST_DESC")]
        public bool OverwriteFileAtDest
        {
            get
            {
                return _overWriteDest;
            }
            set
            {
                _overWriteDest = value;
            }
        }

        public TaskOperationNode(TaskHost taskHost, IDtsConnectionService connectionService, FtpTask ftpTask)
        {
            _taskHost = taskHost;
            ref IDtsVariableService variableService = ref _variableService;
            object service = ((DtsContainer)taskHost).Site.GetService(typeof(IDtsVariableService));
            variableService = (IDtsVariableService)((service is IDtsVariableService) ? service : null);
            iDtsConnService = connectionService;
            for (int i = 0; i < OperationInformation.operationMap.Length; i++)
            {
                if (OperationInformation.operationMap[i].op == ftpTask.Operation)
                {
                    _operationType = OperationInformation.operationMap[i].displayName;
                    break;
                }
            }

            _isSrcPathVar = ftpTask.IsLocalPathVariable;
            _isDestnPathVar = ftpTask.IsRemotePathVariable;
            _sourcePath = ftpTask.LocalPath;
            _destnPath = ftpTask.RemotePath;
            _overWriteDest = ftpTask.OverwriteDestination;
            _isTransferAscii = ftpTask.IsTransferTypeASCII;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            //IL_0343: Unknown result type (might be due to invalid IL or missing references)
            //IL_0349: Expected O, but got Unknown
            //IL_03ee: Unknown result type (might be due to invalid IL or missing references)
            //IL_03f4: Expected O, but got Unknown
            //IL_03a6: Unknown result type (might be due to invalid IL or missing references)
            //IL_03ac: Expected O, but got Unknown
            //IL_052f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0535: Expected O, but got Unknown
            //IL_04e7: Unknown result type (might be due to invalid IL or missing references)
            //IL_04ed: Expected O, but got Unknown
            //IL_0670: Unknown result type (might be due to invalid IL or missing references)
            //IL_0676: Expected O, but got Unknown
            //IL_0628: Unknown result type (might be due to invalid IL or missing references)
            //IL_062e: Expected O, but got Unknown
            //IL_0484: Unknown result type (might be due to invalid IL or missing references)
            //IL_048a: Expected O, but got Unknown
            //IL_043c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0442: Expected O, but got Unknown
            //IL_071b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0721: Expected O, but got Unknown
            //IL_06d3: Unknown result type (might be due to invalid IL or missing references)
            //IL_06d9: Expected O, but got Unknown
            //IL_05c5: Unknown result type (might be due to invalid IL or missing references)
            //IL_05cb: Expected O, but got Unknown
            //IL_057d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0583: Expected O, but got Unknown
            //IL_07c6: Unknown result type (might be due to invalid IL or missing references)
            //IL_07cc: Expected O, but got Unknown
            //IL_077e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0784: Expected O, but got Unknown
            //IL_0871: Unknown result type (might be due to invalid IL or missing references)
            //IL_0877: Expected O, but got Unknown
            //IL_0829: Unknown result type (might be due to invalid IL or missing references)
            //IL_082f: Expected O, but got Unknown
            //IL_091c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0922: Expected O, but got Unknown
            //IL_08d4: Unknown result type (might be due to invalid IL or missing references)
            //IL_08da: Expected O, but got Unknown
            //IL_09c7: Unknown result type (might be due to invalid IL or missing references)
            //IL_09cd: Expected O, but got Unknown
            //IL_097f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0985: Expected O, but got Unknown
            bool flag = true;
            bool flag2 = true;
            Attribute[] array = new Attribute[1];
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, attributes, noCustomTypeDesc: true);
            PropertyDescriptor[] array2 = new PropertyDescriptor[properties.Count];
            PropertyDescriptor[] array3 = new PropertyDescriptor[properties.Count];
            properties.CopyTo(array2, 0);
            properties.CopyTo(array3, 0);
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < array2.Length; i++)
            {
                hashtable.Add(array2[i].Name, i);
            }

            if (_operationType.CompareTo(SR.CREATE_LOCAL_DIRECTORY) == 0 || _operationType.CompareTo(SR.REMOVE_LOCAL_DIRECTORY) == 0 || _operationType.CompareTo(SR.DELETE_LOCAL_FILES) == 0)
            {
                array[0] = new BrowsableAttribute(browsable: false);
                array3[(int)hashtable[FTPTaskPropertyNames.IS_REMOTE_PATH_VAR]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.IS_REMOTE_PATH_VAR], array);
                array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                array3[(int)hashtable[FTPTaskPropertyNames.OVERWRITE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.OVERWRITE], array);
                flag = false;
            }

            if (_operationType.CompareTo(SR.CREATE_LOCAL_DIRECTORY) == 0)
            {
                array[0] = new BrowsableAttribute(browsable: true);
                array3[(int)hashtable[FTPTaskPropertyNames.OVERWRITE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.OVERWRITE], array);
            }

            if (_operationType.CompareTo(SR.CREATE_REMOTE_DIRECTORY) == 0 || _operationType.CompareTo(SR.REMOVE_REMOTE_DIRECTORY) == 0 || _operationType.CompareTo(SR.DELETE_REMOTE_FILES) == 0)
            {
                array[0] = new BrowsableAttribute(browsable: false);
                array3[(int)hashtable[FTPTaskPropertyNames.IS_LOCAL_PATH_VAR]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.IS_LOCAL_PATH_VAR], array);
                array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                array3[(int)hashtable[FTPTaskPropertyNames.OVERWRITE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.OVERWRITE], array);
                flag2 = false;
            }

            if (flag)
            {
                if (_isDestnPathVar)
                {
                    hideVarOrPath(properties, array3, hashtable, src: false, browseVar: true, browsePath: false);
                }
                else
                {
                    hideVarOrPath(properties, array3, hashtable, src: false, browseVar: false, browsePath: true);
                }
            }

            if (flag2)
            {
                if (_isSrcPathVar)
                {
                    hideVarOrPath(properties, array3, hashtable, src: true, browseVar: true, browsePath: false);
                }
                else
                {
                    hideVarOrPath(properties, array3, hashtable, src: true, browseVar: false, browsePath: true);
                }
            }

            if (_operationType.CompareTo(SR.RECEIVE_FILES) == 0 || _operationType.CompareTo(SR.CREATE_LOCAL_DIRECTORY) == 0)
            {
                array[0] = (Attribute)new LocalizablePropertyCategoryAttribute(typeof(SR), "LOCAL_PARAMETERS");
                array3[(int)hashtable[FTPTaskPropertyNames.OVERWRITE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.OVERWRITE], array);
            }

            if (_operationType.CompareTo(SR.SEND_FILES) == 0)
            {
                if (_isSrcPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "SEND_FILES_LOCVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "SEND_FILES_LOCPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                }

                if (_isDestnPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "SEND_FILES_REMVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "SEND_FILES_REMPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.RECEIVE_FILES) == 0)
            {
                if (_isSrcPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "RECEIVE_FILES_LOCVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "RECEIVE_FILES_LOCPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                }

                if (_isDestnPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "RECEIVE_FILES_REMVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "RECEIVE_FILES_REMPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.CREATE_LOCAL_DIRECTORY) == 0)
            {
                if (_isSrcPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "CREATE_LOCDIR_LOCVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "CREATE_LOCDIR_LOCPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.CREATE_REMOTE_DIRECTORY) == 0)
            {
                if (_isDestnPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "CREATE_REMDIR_REMVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "CREATE_REMDIR_REMPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.REMOVE_LOCAL_DIRECTORY) == 0)
            {
                if (_isSrcPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "REMOVE_LOCDIR_LOCVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "REMOVE_LOCDIR_LOCPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.REMOVE_REMOTE_DIRECTORY) == 0)
            {
                if (_isDestnPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "REMOVE_REMDIR_REMVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "REMOVE_REMDIR_REMPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.DELETE_LOCAL_FILES) == 0)
            {
                if (_isSrcPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "DEL_LOCFILES_LOCVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "DEL_LOCFILES_LOCPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.LOCAL_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.LOCAL_PATH], array);
                }
            }

            if (_operationType.CompareTo(SR.DELETE_REMOTE_FILES) == 0)
            {
                if (_isDestnPathVar)
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "DEL_REMFILES_REMVAR_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_VARIABLE]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_VARIABLE], array);
                }
                else
                {
                    array[0] = (Attribute)new LocalizablePropertyDescriptionAttribute(typeof(SR), "DEL_REMFILES_REMPATH_DESC");
                    array3[(int)hashtable[FTPTaskPropertyNames.REMOTE_PATH]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), properties[FTPTaskPropertyNames.REMOTE_PATH], array);
                }
            }

            return new PropertyDescriptorCollection(array3);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
        }

        private void hideVarOrPath(PropertyDescriptorCollection baseProps, PropertyDescriptor[] newProps, Hashtable ht, bool src, bool browseVar, bool browsePath)
        {
            Attribute[] array = new Attribute[1];
            string text = "";
            string text2 = "";
            if (src)
            {
                text = FTPTaskPropertyNames.LOCAL_VARIABLE;
                text2 = FTPTaskPropertyNames.LOCAL_PATH;
            }
            else
            {
                text = FTPTaskPropertyNames.REMOTE_VARIABLE;
                text2 = FTPTaskPropertyNames.REMOTE_PATH;
            }

            array[0] = new BrowsableAttribute(browseVar);
            newProps[(int)ht[text]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), baseProps[text], array);
            array[0] = new BrowsableAttribute(browsePath);
            newProps[(int)ht[text2]] = TypeDescriptor.CreateProperty(typeof(TaskOperationNode), baseProps[text2], array);
        }
    }

    private IDTSTaskUIHost _treeHost;

    internal TreeNode _viewNode;

    internal TaskOperationNode _taskOperationNode;

    private FtpTask _ftpTask;

    private LocalizablePropertyGrid propertyGridTaskOperation;

    private Container components;

    private TaskOperationNode TaskNode => _taskOperationNode;

    public FTPTaskOperationsView()
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
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FTPTaskOperationsView));
        propertyGridTaskOperation = new LocalizablePropertyGrid();
        ((Control)this).SuspendLayout();
        componentResourceManager.ApplyResources(propertyGridTaskOperation, "propertyGridTaskOperation");
        propertyGridTaskOperation.LocalizableSelectedObject = null;
        ((Control)propertyGridTaskOperation).Name = "propertyGridTaskOperation";
        ((PropertyGrid)propertyGridTaskOperation).PropertySort = (PropertySort)2;
        ((PropertyGrid)propertyGridTaskOperation).ToolbarVisible = false;
        ((PropertyGrid)propertyGridTaskOperation).PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGridTaskOperation_PropertyValueChanged);
        ((Control)this).Controls.Add((Control)(object)propertyGridTaskOperation);
        ((Control)this).Name = "FTPTaskOperationsView";
        componentResourceManager.ApplyResources(this, "$this");
        ((Control)this).ResumeLayout(false);
    }

    public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
    {
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0045: Unknown result type (might be due to invalid IL or missing references)
        //IL_005b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0061: Expected O, but got Unknown
        //IL_0063: Unknown result type (might be due to invalid IL or missing references)
        //IL_0074: Expected O, but got Unknown
        _treeHost = treeHost;
        _viewNode = viewNode;
        if (taskHost == null)
        {
            throw new ArgumentNullException(SR.DTS_MSG_TASKHOST, SR.DTS_E_INITIALIZATION_WITH_NULL_TASK);
        }

        if (!(((TaskHost)taskHost).InnerObject is FtpTask))
        {
            throw new ArgumentException(SR.DTS_E_UI_INITIALIZATION_WITH_WRONG_TASK, SR.DTS_MSG_TASKHOST);
        }

        _ftpTask = ((TaskHost)taskHost).InnerObject as FtpTask;
        IDtsConnectionService connectionService = (IDtsConnectionService)connections;
        _taskOperationNode = new TaskOperationNode((TaskHost)taskHost, connectionService, _ftpTask);
        propertyGridTaskOperation.LocalizableSelectedObject = _taskOperationNode;
    }

    public virtual void OnValidate(ref bool bViewIsValid, ref string reason)
    {
    }

    public virtual void OnSelection()
    {
        if (_treeHost.GetView(_viewNode.TreeView.Nodes[0]) is FTPTaskGeneralView fTPTaskGeneralView && fTPTaskGeneralView._generalNode.FtpConnection != null)
        {
            FTPTaskGeneralView.ftpConnection = fTPTaskGeneralView._generalNode.FtpConnection.Trim();
        }
    }

    public virtual void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
    {
    }

    public virtual void OnCommit(object taskHost)
    {
        for (int i = 0; i < OperationInformation.operationMap.Length; i++)
        {
            if (OperationInformation.operationMap[i].displayName == _taskOperationNode.OperationType)
            {
                _ftpTask.Operation = OperationInformation.operationMap[i].op;
                break;
            }
        }

        _ftpTask.IsLocalPathVariable = _taskOperationNode.IsLocalPathVariable;
        _ftpTask.IsRemotePathVariable = _taskOperationNode.IsRemotePathVariable;
        if (_taskOperationNode.IsRemotePathVariable)
        {
            _ftpTask.RemotePath = _taskOperationNode.RemoteVariable;
        }
        else
        {
            _ftpTask.RemotePath = _taskOperationNode.RemotePath;
        }

        if (_taskOperationNode.IsLocalPathVariable)
        {
            _ftpTask.LocalPath = _taskOperationNode.LocalVariable;
        }
        else
        {
            _ftpTask.LocalPath = _taskOperationNode.LocalPath;
        }

        _ftpTask.OverwriteDestination = _taskOperationNode.OverwriteFileAtDest;
        _ftpTask.IsTransferTypeASCII = _taskOperationNode.IsTransferAscii;
        FTPTaskGeneralView.ftpConnection = null;
    }

    private void propertyGridTaskOperation_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        //IL_0076: Unknown result type (might be due to invalid IL or missing references)
        //IL_007c: Expected O, but got Unknown
        //IL_0165: Unknown result type (might be due to invalid IL or missing references)
        //IL_016b: Expected O, but got Unknown
        if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("LocalPath") == 0)
        {
            if (e.ChangedItem.Value.Equals(SR.NEW_CONNECTION))
            {
                ArrayList arrayList = null;
                ((Control)this).Cursor = Cursors.WaitCursor;
                if (TaskNode.LocalPath != null && !(TaskNode.LocalPath == ""))
                {
                    TaskNode.LocalPath = null;
                }

                FileConnectionManagerUIArgs val = new FileConnectionManagerUIArgs();
                if (TaskNode.OperationType.CompareTo(SR.CREATE_LOCAL_DIRECTORY) == 0)
                {
                    val.SupportedUsageTypes = new DTSFileConnectionUsageType[1] { (DTSFileConnectionUsageType)3 };
                }
                else if (TaskNode.OperationType.CompareTo(SR.REMOVE_LOCAL_DIRECTORY) == 0 || TaskNode.OperationType.CompareTo(SR.RECEIVE_FILES) == 0)
                {
                    val.SupportedUsageTypes = new DTSFileConnectionUsageType[1] { (DTSFileConnectionUsageType)2 };
                }
                else if (TaskNode.OperationType.CompareTo(SR.DELETE_LOCAL_FILES) == 0 || TaskNode.OperationType.CompareTo(SR.SEND_FILES) == 0)
                {
                    DTSFileConnectionUsageType[] supportedUsageTypes = (DTSFileConnectionUsageType[])(object)new DTSFileConnectionUsageType[1];
                    val.SupportedUsageTypes = supportedUsageTypes;
                }

                arrayList = ((IDtsConnectionBaseService)TaskNode.iDtsConnService).CreateConnection("FILE", (ConnectionManagerUIArgs)(object)val);
                ((Control)this).Cursor = Cursors.Default;
                if (arrayList != null && arrayList.Count > 0)
                {
                    ConnectionManager val2 = (ConnectionManager)arrayList[0];
                    TaskNode.LocalPath = val2.Name;
                }
                else if (e.OldValue == null)
                {
                    TaskNode.LocalPath = null;
                }
                else
                {
                    TaskNode.LocalPath = (string)e.OldValue;
                }
            }
        }
        else if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("LocalVariable") == 0)
        {
            if (!e.ChangedItem.Value.Equals(SR.NEW_VARIABLE))
            {
                return;
            }

            ((Control)this).Cursor = Cursors.WaitCursor;
            TaskNode.LocalVariable = null;
            IDtsVariableService variableService = TaskNode.VariableService;
            if (variableService == null)
            {
                showExceptionMessageBox(SR.VariableServiceIsNull);
                ((Control)this).Cursor = Cursors.Default;
                return;
            }

            Variable val3 = variableService.PromptAndCreateVariable((IWin32Window)(object)this, (DtsContainer)(object)TaskNode.Taskhost);
            if ((DtsObject)(object)val3 == (DtsObject)null)
            {
                TaskNode.LocalVariable = (string)e.OldValue;
            }
            else if (val3.Value.GetType() != typeof(string))
            {
                TaskNode.LocalVariable = (string)e.OldValue;
                showExceptionMessageBox(SR.LocalVarIsNotString(val3.QualifiedName));
            }
            else
            {
                TaskNode.LocalVariable = val3.QualifiedName;
            }

            ((Control)this).Cursor = Cursors.Default;
        }
        else
        {
            if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("RemoteVariable") != 0 || !e.ChangedItem.Value.Equals(SR.NEW_VARIABLE))
            {
                return;
            }

            ((Control)this).Cursor = Cursors.WaitCursor;
            TaskNode.RemoteVariable = null;
            IDtsVariableService variableService2 = TaskNode.VariableService;
            if (variableService2 == null)
            {
                showExceptionMessageBox(SR.VariableServiceIsNull);
                ((Control)this).Cursor = Cursors.Default;
                return;
            }

            Variable val4 = variableService2.PromptAndCreateVariable((IWin32Window)(object)this, (DtsContainer)(object)TaskNode.Taskhost);
            if ((DtsObject)(object)val4 == (DtsObject)null)
            {
                TaskNode.RemoteVariable = (string)e.OldValue;
            }
            else if (val4.Value.GetType() != typeof(string))
            {
                TaskNode.RemoteVariable = (string)e.OldValue;
                showExceptionMessageBox(SR.RemoteVarIsNotString(val4.QualifiedName));
            }
            else
            {
                TaskNode.RemoteVariable = val4.QualifiedName;
            }

            ((Control)this).Cursor = Cursors.Default;
        }
    }

    private void showExceptionMessageBox(string message)
    {
        //IL_000a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0010: Expected O, but got Unknown
        //IL_001d: Unknown result type (might be due to invalid IL or missing references)
        Exception ex = new Exception(message);
        ExceptionMessageBox val = new ExceptionMessageBox(ex, (ExceptionMessageBoxButtons)0, (ExceptionMessageBoxSymbol)2);
        val.Caption = SR.WARNING;
        val.Show((IWin32Window)null);
    }
}
