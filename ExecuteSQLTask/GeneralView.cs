
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.DataTransformationServices.Design;
using Microsoft.NetEnterpriseServers;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Dts.Runtime.Localization;
using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Connections;
using Microsoft.SqlServer.Graph.Extended;

namespace Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;

[HelpContext("sql13.dts.designer.executesqltask.general.f1")]
internal class GeneralView : UserControl, IDTSTaskUIView
{
    [SortProperties(new string[]
    {
        "Name", "Description", "ConnectionType", "Connection", "SQLSourceType", "SourceVariable", "FileConnection", "SQLStatement", "IsQueryStoredProcedure", "BypassPrepare",
        "TimeOut", "CodePage", "TypeConversionMode", "ResultSet"
    })]
    internal class GeneralNode : ICustomTypeDescriptor
    {
        private string _name;

        private string _description;

        private string _connectionType = ConnTypeStrings.Oledb;

        private bool _isQueryStoredProcedure;

        private string _connName;

        private string _sqlStmt;

        private uint _timeout;

        private uint _codepage;

        private string _resultSetType;

        private string _sQLSourceType;

        private string _sourceVar;

        private string _fileConn;

        private bool _bypassPrepare;

        private string _typeConversionMode;

        internal TypeConversionMode typeConversionMode = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode.Allowed;

        private IDtsVariableService _variableService;

        internal IDtsConnectionService iDTSConnService;

        internal ResultSetType resultSetType = ResultSetType.ResultSetType_None;

        internal TaskHost SQLTaskHost;

        internal IDtsVariableService VariableService => _variableService;

        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "GENERAL")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "GENERAL_NAME_DOC")]
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
                    throw new ApplicationException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.HOST_NAME_CANT_EMPTY);
                }

                _name = value;
            }
        }

        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "GENERAL")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "GENERAL_DESC_DOC")]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(ConnectionTypes))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "ConnectionTypeDesc")]
        public string ConnectionType
        {
            get
            {
                return _connectionType;
            }
            set
            {
                if (value != _connectionType)
                {
                    _connName = "";
                }

                _connectionType = value;
            }
        }

        [TypeConverter(typeof(DbConnections))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT_CONNECTION_NAME")]
        public string Connection
        {
            get
            {
                return _connName;
            }
            set
            {
                _connName = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(SQLSourceTypeLocalized))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "SQLSourceTypeDesc")]
        public string SQLSourceType
        {
            get
            {
                return _sQLSourceType;
            }
            set
            {
                _sQLSourceType = value;
            }
        }

        [TypeConverter(typeof(GetVariables))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "SourceVariableDescription")]
        public string SourceVariable
        {
            get
            {
                return _sourceVar;
            }
            set
            {
                _sourceVar = value;
            }
        }

        [TypeConverter(typeof(FileConnections))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "SourceFileConnectionDesciption")]
        public string FileConnection
        {
            get
            {
                return _fileConn;
            }
            set
            {
                _fileConn = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [Editor(typeof(SQLStatementEditor), typeof(UITypeEditor))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT_SQL_STATEMENT")]
        public string SQLStatement
        {
            get
            {
                return _sqlStmt;
            }
            set
            {
                _sqlStmt = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [ReadOnly(true)]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "QueryIsStoredProcedureDesc")]
        public bool IsQueryStoredProcedure
        {
            get
            {
                return _isQueryStoredProcedure;
            }
            set
            {
                _isQueryStoredProcedure = value;
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [ReadOnly(true)]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "STATEMENT")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "BypassPrepare")]
        public bool BypassPrepare
        {
            get
            {
                return _bypassPrepare;
            }
            set
            {
                _bypassPrepare = value;
            }
        }

        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS_TIMEOUT")]
        public uint TimeOut
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS_CODEPAGE")]
        public uint CodePage
        {
            get
            {
                return _codepage;
            }
            set
            {
                _codepage = value;
            }
        }

        [TypeConverter(typeof(TypeConversionModeOptions))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OPTIONS_TYPECONVERSIONMODE")]
        public string TypeConversionMode
        {
            get
            {
                return _typeConversionMode;
            }
            set
            {
                _typeConversionMode = value;
                if (value.Equals(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TCM_None, StringComparison.Ordinal))
                {
                    typeConversionMode = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode.None;
                }
                else
                {
                    typeConversionMode = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode.Allowed;
                }
            }
        }

        [TypeConverter(typeof(ResultSetOptions))]
        [LocalizablePropertyCategory(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "RESULTSET")]
        [LocalizablePropertyDescription(typeof(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized), "OUTPUT_RESULTSET")]
        public string ResultSet
        {
            get
            {
                return _resultSetType;
            }
            set
            {
                _resultSetType = value;
                if (_resultSetType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_NONE) == 0)
                {
                    resultSetType = ResultSetType.ResultSetType_None;
                }
                else if (_resultSetType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_XML) == 0)
                {
                    resultSetType = ResultSetType.ResultSetType_XML;
                }
                else if (_resultSetType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_SINGLE_ROW) == 0)
                {
                    resultSetType = ResultSetType.ResultSetType_SingleRow;
                }
                else if (_resultSetType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_FULL_RESULT_SET) == 0)
                {
                    resultSetType = ResultSetType.ResultSetType_Rowset;
                }
            }
        }

        internal GeneralNode(TaskHost taskHost, IDtsConnectionService connService)
        {
            SQLTaskHost = taskHost;
            if (connService == null)
            {
                throw new ArgumentNullException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.CantAccess("IDtsConnectionService"));
            }

            iDTSConnService = connService;
            ref IDtsVariableService variableService = ref _variableService;
            object service = ((DtsContainer)taskHost).Site.GetService(typeof(IDtsVariableService));
            variableService = (IDtsVariableService)((service is IDtsVariableService) ? service : null);
            _name = ((DtsContainer)taskHost).Name;
            _description = ((DtsContainer)taskHost).Description;
            ExecuteSQLTask executeSQLTask = taskHost.InnerObject as ExecuteSQLTask;
            _connName = executeSQLTask.Connection;
            _isQueryStoredProcedure = executeSQLTask.IsStoredProcedure;
            _bypassPrepare = executeSQLTask.BypassPrepare;
            ConnectionEnumerator enumerator = ((IDtsConnectionBaseService)connService).GetConnections().GetEnumerator();
            while (((DtsEnumerator)enumerator).MoveNext())
            {
                ConnectionManager current = enumerator.Current;
                if (_connName.CompareTo(current.Name) == 0)
                {
                    if (current.CreationName.CompareTo(ConnTypeStrings.Ado) == 0)
                    {
                        _connectionType = ConnTypeStrings.Ado;
                    }
                    else if (current.CreationName.CompareTo(ConnTypeStrings.AdoDotNet) == 0 || current.CreationName.CompareTo(ConnTypeStrings.AdoDotNetOledb) == 0 || current.CreationName.CompareTo(ConnTypeStrings.AdoDotNetSql) == 0 || current.CreationName.StartsWith(ConnTypeStrings.AdoDotNet))
                    {
                        _connectionType = ConnTypeStrings.AdoDotNet;
                    }
                    else if (current.CreationName.CompareTo(ConnTypeStrings.SqlMobile) == 0)
                    {
                        _connectionType = ConnTypeStrings.SqlMobile;
                    }
                    else if (current.CreationName.CompareTo(ConnTypeStrings.Odbc) == 0)
                    {
                        _connectionType = ConnTypeStrings.Odbc;
                    }
                    else if (current.CreationName.CompareTo("OLEDB") == 0)
                    {
                        _connectionType = ConnTypeStrings.Oledb;
                    }
                    else if (current.CreationName.CompareTo("EXCEL") == 0)
                    {
                        _connectionType = ConnTypeStrings.Excel;
                    }
                }
            }

            switch (executeSQLTask.ResultSetType)
            {
                case ResultSetType.ResultSetType_None:
                    _resultSetType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_NONE;
                    break;
                case ResultSetType.ResultSetType_Rowset:
                    _resultSetType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_FULL_RESULT_SET;
                    break;
                case ResultSetType.ResultSetType_SingleRow:
                    _resultSetType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_SINGLE_ROW;
                    break;
                case ResultSetType.ResultSetType_XML:
                    _resultSetType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.RS_XML;
                    break;
            }

            resultSetType = executeSQLTask.ResultSetType;
            switch (executeSQLTask.TypeConversionMode)
            {
                case Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode.None:
                    _typeConversionMode = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TCM_None;
                    break;
                case Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode.Allowed:
                    _typeConversionMode = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TCM_Allowed;
                    break;
            }

            typeConversionMode = executeSQLTask.TypeConversionMode;
            if (executeSQLTask.SqlStatementSourceType == SqlStatementSourceType.DirectInput)
            {
                _sqlStmt = executeSQLTask.SqlStatementSource;
                _sQLSourceType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput;
            }
            else if (executeSQLTask.SqlStatementSourceType == SqlStatementSourceType.FileConnection)
            {
                _sQLSourceType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection;
                _fileConn = executeSQLTask.SqlStatementSource;
            }
            else if (executeSQLTask.SqlStatementSourceType == SqlStatementSourceType.Variable)
            {
                _sQLSourceType = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable;
                _sourceVar = executeSQLTask.SqlStatementSource;
            }

            _timeout = executeSQLTask.TimeOut;
            _codepage = executeSQLTask.CodePage;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] array = null;
            PropertyDescriptorCollection propertyDescriptorCollection = null;
            Attribute[] array2 = new Attribute[1];
            propertyDescriptorCollection = TypeDescriptor.GetProperties(this, attributes, noCustomTypeDesc: true);
            if (propertyDescriptorCollection == null)
            {
                return null;
            }

            array = new PropertyDescriptor[propertyDescriptorCollection.Count];
            propertyDescriptorCollection.CopyTo(array, 0);
            if (_connectionType.CompareTo(ConnTypeStrings.Ado) == 0 || _connectionType.CompareTo(ConnTypeStrings.AdoDotNet) == 0 || _connectionType.CompareTo(ConnTypeStrings.AdoDotNetOledb) == 0 || _connectionType.CompareTo(ConnTypeStrings.AdoDotNetSql) == 0)
            {
                for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                {
                    if (propertyDescriptorCollection[i].Name.CompareTo("IsQueryStoredProcedure") == 0)
                    {
                        array2[0] = new ReadOnlyAttribute(isReadOnly: false);
                        array[i] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[i], array2);
                    }
                }
            }

            if (_connectionType.CompareTo(ConnTypeStrings.Oledb) == 0)
            {
                for (int j = 0; j < propertyDescriptorCollection.Count; j++)
                {
                    if (propertyDescriptorCollection[j].Name.CompareTo("BypassPrepare") == 0)
                    {
                        array2[0] = new ReadOnlyAttribute(isReadOnly: false);
                        array[j] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[j], array2);
                    }
                }
            }

            if (_sQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
            {
                for (int k = 0; k < propertyDescriptorCollection.Count; k++)
                {
                    if (propertyDescriptorCollection[k].Name.CompareTo("SourceVariable") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[k] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[k], array2);
                    }
                    else if (propertyDescriptorCollection[k].Name.CompareTo("SQLStatement") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: true);
                        array[k] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[k], array2);
                    }
                    else if (propertyDescriptorCollection[k].Name.CompareTo("FileConnection") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[k] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[k], array2);
                    }
                }
            }
            else if (_sQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
            {
                for (int l = 0; l < propertyDescriptorCollection.Count; l++)
                {
                    if (propertyDescriptorCollection[l].Name.CompareTo("SourceVariable") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[l] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[l], array2);
                    }
                    else if (propertyDescriptorCollection[l].Name.CompareTo("SQLStatement") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[l] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[l], array2);
                    }
                    else if (propertyDescriptorCollection[l].Name.CompareTo("FileConnection") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: true);
                        array[l] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[l], array2);
                    }
                }
            }
            else
            {
                for (int m = 0; m < propertyDescriptorCollection.Count; m++)
                {
                    if (propertyDescriptorCollection[m].Name.CompareTo("SourceVariable") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: true);
                        array[m] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[m], array2);
                    }
                    else if (propertyDescriptorCollection[m].Name.CompareTo("SQLStatement") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[m] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[m], array2);
                    }
                    else if (propertyDescriptorCollection[m].Name.CompareTo("FileConnection") == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                        array[m] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[m], array2);
                    }
                }
            }

            for (int n = 0; n < propertyDescriptorCollection.Count; n++)
            {
                if (propertyDescriptorCollection[n].Name.CompareTo("CodePage") == 0)
                {
                    if (_connectionType.CompareTo(ConnTypeStrings.Ado) == 0 || _connectionType.CompareTo(ConnTypeStrings.Odbc) == 0)
                    {
                        array2[0] = new BrowsableAttribute(browsable: false);
                    }
                    else
                    {
                        array2[0] = new BrowsableAttribute(browsable: true);
                    }

                    array[n] = TypeDescriptor.CreateProperty(typeof(GeneralNode), propertyDescriptorCollection[n], array2);
                }
            }

            if (array == null)
            {
                return propertyDescriptorCollection;
            }

            return new PropertyDescriptorCollection(array);
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
    }

    private Container components;

    private LocalizablePropertyGrid propertyGridGeneral;

    private Button btnBuildQurey;

    private Button btnParseQuery;

    private Button btnBrowse;

    private OpenFileDialog openFileDialog;

    private GeneralNode _generalNode;

    private Connections _connections;

    private TaskHost _taskHost;

    private IDTSTaskUIHost _treeHost;

    private string _sqlStatement;

    private Variables _variables;

    internal GeneralNode GenNode => _generalNode;

    internal GeneralView()
    {
        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
        //IL_001e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0028: Expected O, but got Unknown
        InitializeComponent();
        SetTheme(HighContrastSupport.Theme);
        HighContrastSupport.ThemeChanged += new ThemeChangeHandler(SetTheme);
    }

    protected override void Dispose(bool disposing)
    {
        //IL_0024: Unknown result type (might be due to invalid IL or missing references)
        //IL_002e: Expected O, but got Unknown
        if (disposing && components != null)
        {
            components.Dispose();
        }

        ((ContainerControl)this).Dispose(disposing);
        HighContrastSupport.ThemeChanged -= new ThemeChangeHandler(SetTheme);
    }

    private void InitializeComponent()
    {
        //IL_0010: Unknown result type (might be due to invalid IL or missing references)
        //IL_001a: Expected O, but got Unknown
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0025: Expected O, but got Unknown
        //IL_0026: Unknown result type (might be due to invalid IL or missing references)
        //IL_0030: Expected O, but got Unknown
        //IL_0031: Unknown result type (might be due to invalid IL or missing references)
        //IL_003b: Expected O, but got Unknown
        //IL_003c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0046: Expected O, but got Unknown
        //IL_00aa: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b4: Expected O, but got Unknown
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GeneralView));
        propertyGridGeneral = new LocalizablePropertyGrid();
        btnBuildQurey = new Button();
        btnParseQuery = new Button();
        btnBrowse = new Button();
        openFileDialog = new OpenFileDialog();
        ((Control)this).SuspendLayout();
        componentResourceManager.ApplyResources(propertyGridGeneral, "propertyGridGeneral");
        ((PropertyGrid)propertyGridGeneral).CommandsVisibleIfAvailable = false;
        propertyGridGeneral.LocalizableSelectedObject = null;
        ((Control)propertyGridGeneral).Name = "propertyGridGeneral";
        ((PropertyGrid)propertyGridGeneral).PropertySort = (PropertySort)2;
        ((PropertyGrid)propertyGridGeneral).ToolbarVisible = false;
        ((PropertyGrid)propertyGridGeneral).PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGridGeneral_PropertyValueChanged);
        componentResourceManager.ApplyResources(btnBuildQurey, "btnBuildQurey");
        ((Control)btnBuildQurey).Name = "btnBuildQurey";
        ((Control)btnBuildQurey).Click += btnBuildQurey_Click;
        componentResourceManager.ApplyResources(btnParseQuery, "btnParseQuery");
        ((Control)btnParseQuery).Name = "btnParseQuery";
        ((Control)btnParseQuery).Click += btnParseQuery_Click;
        componentResourceManager.ApplyResources(btnBrowse, "btnBrowse");
        ((Control)btnBrowse).Name = "btnBrowse";
        ((Control)btnBrowse).Click += btnBrowse_Click;
        ((FileDialog)openFileDialog).CheckFileExists = false;
        ((FileDialog)openFileDialog).DefaultExt = "sql";
        componentResourceManager.ApplyResources(openFileDialog, "openFileDialog");
        ((FileDialog)openFileDialog).FilterIndex = 0;
        ((Control)this).Controls.Add((Control)(object)btnBrowse);
        ((Control)this).Controls.Add((Control)(object)btnParseQuery);
        ((Control)this).Controls.Add((Control)(object)btnBuildQurey);
        ((Control)this).Controls.Add((Control)(object)propertyGridGeneral);
        ((Control)this).Name = "GeneralView";
        componentResourceManager.ApplyResources(this, "$this");
        ((Control)this).ResumeLayout(false);
    }

    public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
    {
        //IL_001c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0026: Expected O, but got Unknown
        //IL_006f: Unknown result type (might be due to invalid IL or missing references)
        //IL_0075: Expected O, but got Unknown
        _treeHost = treeHost;
        if (taskHost == null)
        {
            throw new ArgumentNullException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TaskHost, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.CantInitializeToNull);
        }

        _taskHost = (TaskHost)taskHost;
        if (!(_taskHost.InnerObject is ExecuteSQLTask))
        {
            throw new ArgumentException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.MustInitializetoSQLTask, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TaskHost);
        }

        if (connections == null)
        {
            throw new ApplicationException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.CantAccess("IDtsConnectionService"));
        }

        _variables = ((DtsContainer)_taskHost).Variables;
        IDtsConnectionService val = (IDtsConnectionService)connections;
        _connections = ((IDtsConnectionBaseService)val).GetConnections();
        _generalNode = new GeneralNode(_taskHost, val);
        propertyGridGeneral.LocalizableSelectedObject = _generalNode;
        ((Control)btnBuildQurey).Enabled = !string.IsNullOrEmpty(_generalNode.Connection) && _generalNode.ConnectionType == ConnTypeStrings.Oledb;
        if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
        {
            ((Control)btnBrowse).Enabled = true;
        }
        else
        {
            ((Control)btnBrowse).Enabled = false;
        }
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
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_001c: Unknown result type (might be due to invalid IL or missing references)
        TaskHost val = (TaskHost)taskHost;
        ((DtsContainer)val).Name = _generalNode.Name.Trim();
        ((DtsContainer)val).Description = _generalNode.Description;
        ExecuteSQLTask executeSQLTask = (ExecuteSQLTask)val.InnerObject;
        executeSQLTask.Connection = _generalNode.Connection;
        executeSQLTask.IsStoredProcedure = false;
        executeSQLTask.BypassPrepare = _generalNode.BypassPrepare;
        if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
        {
            executeSQLTask.SqlStatementSourceType = SqlStatementSourceType.DirectInput;
            executeSQLTask.SqlStatementSource = _generalNode.SQLStatement;
        }
        else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
        {
            executeSQLTask.SqlStatementSourceType = SqlStatementSourceType.FileConnection;
            executeSQLTask.SqlStatementSource = _generalNode.FileConnection;
        }
        else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable) == 0)
        {
            executeSQLTask.SqlStatementSourceType = SqlStatementSourceType.Variable;
            executeSQLTask.SqlStatementSource = _generalNode.SourceVariable;
        }

        if (_generalNode.ConnectionType.CompareTo(ConnTypeStrings.Ado) == 0 || _generalNode.ConnectionType.CompareTo(ConnTypeStrings.AdoDotNet) == 0 || _generalNode.ConnectionType.CompareTo(ConnTypeStrings.AdoDotNetOledb) == 0 || _generalNode.ConnectionType.CompareTo(ConnTypeStrings.AdoDotNetSql) == 0)
        {
            executeSQLTask.IsStoredProcedure = _generalNode.IsQueryStoredProcedure;
        }

        executeSQLTask.TimeOut = _generalNode.TimeOut;
        executeSQLTask.CodePage = _generalNode.CodePage;
        executeSQLTask.TypeConversionMode = _generalNode.typeConversionMode;
        executeSQLTask.ResultSetType = _generalNode.resultSetType;
    }

    private void btnBuildQurey_Click(object sender, EventArgs e)
    {
        //IL_0279: Unknown result type (might be due to invalid IL or missing references)
        //IL_027f: Invalid comparison between Unknown and I4
        ((Control)this).Cursor = Cursors.WaitCursor;
        string text = null;
        try
        {
            if (_generalNode.Connection == null || _generalNode.Connection.Trim().Length == 0)
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NoConnectionSpecified, (ExceptionMessageBoxSymbol)0);
                return;
            }

            if (!isConnectionExists(_generalNode.Connection.Trim()))
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ConnNotFound(_generalNode.Connection), (ExceptionMessageBoxSymbol)3);
                return;
            }

            string connection = _generalNode.Connection;
            ConnectionManager val = null;
            try
            {
                val = _connections[(object)connection];
            }
            catch (Exception ex)
            {
                showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)0);
                return;
            }

            if (string.IsNullOrEmpty(val.ConnectionString))
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Connection + "'" + connection + "' " + Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NoConnectionString, (ExceptionMessageBoxSymbol)0);
                return;
            }

            if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
            {
                if (!isSourceSpecified(isFile: true, isVariable: false))
                {
                    return;
                }

                text = getFilePathFromFileConection(_generalNode.FileConnection);
                if (text == null || text.Trim().Length <= 0)
                {
                    return;
                }

                _sqlStatement = getStringFromFile(text);
            }
            else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable) == 0)
            {
                if (!isSourceSpecified(isFile: false, isVariable: true))
                {
                    return;
                }

                _sqlStatement = getStringFromVariable(_generalNode.SourceVariable);
            }
            else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
            {
                _sqlStatement = _generalNode.SQLStatement;
            }

            QueryBuilderHostForm queryBuilderHostForm = new QueryBuilderHostForm(((DtsContainer)_taskHost).Site, val.ConnectionString, _sqlStatement);
            try
            {
                try
                {
                    if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
                    {
                        if (text != null || text.Trim().Length > 0)
                        {
                            queryBuilderHostForm.FilePath = text;
                        }

                        queryBuilderHostForm.SQLSourceTypeValue = SQLSourceType.FileConnection;
                    }
                    else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable) == 0)
                    {
                        if (_variables != null)
                        {
                            queryBuilderHostForm.DtrVariables = _variables;
                        }

                        queryBuilderHostForm.VarName = _generalNode.SourceVariable;
                        queryBuilderHostForm.SQLSourceTypeValue = SQLSourceType.Variable;
                    }
                    else
                    {
                        queryBuilderHostForm.SQLSourceTypeValue = SQLSourceType.DirectInput;
                    }
                }
                catch (Exception ex2)
                {
                    showExceptionMessageBox(ex2.Message, (ExceptionMessageBoxSymbol)2);
                }

                ((Form)queryBuilderHostForm).StartPosition = (FormStartPosition)4;
                if ((int)((Form)queryBuilderHostForm).ShowDialog((IWin32Window)(object)this) == 1)
                {
                    updateSqlStringToSource(queryBuilderHostForm.FinalQuery, text);
                    ((Control)propertyGridGeneral).Refresh();
                }
            }
            finally
            {
                ((IDisposable)queryBuilderHostForm)?.Dispose();
            }
        }
        catch (Exception ex3)
        {
            showExceptionMessageBox(ex3.Message, (ExceptionMessageBoxSymbol)0);
        }

        ((Control)this).Cursor = Cursors.Default;
    }

    private void btnParseQuery_Click(object sender, EventArgs e)
    {
        if (_generalNode.BypassPrepare && _generalNode.ConnectionType.CompareTo(ConnTypeStrings.Oledb) == 0)
        {
            SQLStmtFrm.ShowExceptionMessageBox(new Exception(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ParseHasNoEffect), (ExceptionMessageBoxSymbol)1);
            return;
        }

        ((Control)this).Cursor = Cursors.WaitCursor;
        try
        {
            if (_generalNode.Connection != null && _generalNode.Connection.Trim().Length != 0)
            {
                if (!isConnectionExists(_generalNode.Connection.Trim()))
                {
                    showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ConnNotFound(_generalNode.Connection), (ExceptionMessageBoxSymbol)3);
                    return;
                }

                if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
                {
                    if (!isSourceSpecified(isFile: true, isVariable: false))
                    {
                        return;
                    }
                }
                else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable) == 0 && !isSourceSpecified(isFile: false, isVariable: true))
                {
                    return;
                }

                if (gotStringToParse())
                {
                    string ErrorDescription = null;
                    if (checkSqlQuery(ref ErrorDescription, _sqlStatement))
                    {
                        showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.QueryParsedCorrectly, (ExceptionMessageBoxSymbol)0);
                    }
                    else
                    {
                        showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.QueryFailedToParse(ErrorDescription), (ExceptionMessageBoxSymbol)3);
                    }
                }

                return;
            }

            showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NoConnectionSpecified, (ExceptionMessageBoxSymbol)0);
        }
        catch (Exception ex)
        {
            showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)0);
        }

        ((Control)this).Cursor = Cursors.Default;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Invalid comparison between Unknown and I4
        try
        {
            string text = null;
            string text2 = null;
            if ((int)((CommonDialog)openFileDialog).ShowDialog((IWin32Window)(object)this) == 1)
            {
                text = ((FileDialog)openFileDialog).FileName;
            }

            if (text != null && text.Trim().Length != 0)
            {
                text2 = getStringFromFile(text);
            }

            if (text2 != null)
            {
                _generalNode.SQLStatement = text2;
                ((Control)propertyGridGeneral).Refresh();
            }
        }
        catch (Exception ex)
        {
            showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)0);
        }
    }

    private void SetTheme(DesignerTheme theme)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_0026: Unknown result type (might be due to invalid IL or missing references)
        //IL_0036: Unknown result type (might be due to invalid IL or missing references)
        //IL_0046: Unknown result type (might be due to invalid IL or missing references)
        //IL_0056: Unknown result type (might be due to invalid IL or missing references)
        //IL_0066: Unknown result type (might be due to invalid IL or missing references)
        ((Control)btnBrowse).BackColor = HighContrastSupport.GeneralWindowBackColor;
        ((Control)btnBrowse).ForeColor = HighContrastSupport.GeneralWindowForeColor;
        ((Control)btnBuildQurey).BackColor = HighContrastSupport.GeneralWindowBackColor;
        ((Control)btnBuildQurey).ForeColor = HighContrastSupport.GeneralWindowForeColor;
        ((Control)btnParseQuery).BackColor = HighContrastSupport.GeneralWindowBackColor;
        ((Control)btnParseQuery).ForeColor = HighContrastSupport.GeneralWindowForeColor;
        ((PropertyGrid)propertyGridGeneral).LineColor = HighContrastSupport.ButtonBackColor;
        ((ButtonBase)btnBrowse).FlatStyle = (FlatStyle)3;
        ((ButtonBase)btnBuildQurey).FlatStyle = (FlatStyle)3;
        ((ButtonBase)btnParseQuery).FlatStyle = (FlatStyle)3;
    }

    private string getStringFromVariable(string varName)
    {
        string result = null;
        try
        {
            VariableEnumerator enumerator = _variables.GetEnumerator();
            while (((DtsEnumerator)enumerator).MoveNext())
            {
                Variable current = enumerator.Current;
                if (current.QualifiedName == varName)
                {
                    result = current.Value.ToString();
                }
            }

            return result;
        }
        catch
        {
            return null;
        }
    }

    private bool isSourceSpecified(bool isFile, bool isVariable)
    {
        if (isFile)
        {
            if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
            {
                if (_generalNode.FileConnection == null)
                {
                    showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NoFileConnection, (ExceptionMessageBoxSymbol)2);
                    return false;
                }

                if (!isConnectionExists(_generalNode.FileConnection.Trim()))
                {
                    showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ConnNotFound(_generalNode.FileConnection), (ExceptionMessageBoxSymbol)3);
                    return false;
                }
            }
        }
        else if (isVariable)
        {
            if (_generalNode.SourceVariable == null)
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NoVariableSpecified, (ExceptionMessageBoxSymbol)2);
                return false;
            }

            if (!isVariableExists(_generalNode.SourceVariable.Trim()))
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.VarNotFound(_generalNode.SourceVariable), (ExceptionMessageBoxSymbol)3);
                return false;
            }
        }

        return true;
    }

    private bool isVariableExists(string varName)
    {
        bool result = false;
        VariableEnumerator enumerator = _variables.GetEnumerator();
        while (((DtsEnumerator)enumerator).MoveNext())
        {
            if (enumerator.Current.QualifiedName == varName)
            {
                result = true;
            }
        }

        return result;
    }

    private bool isConnectionExists(string connName)
    {
        bool result = false;
        ConnectionEnumerator enumerator = _connections.GetEnumerator();
        while (((DtsEnumerator)enumerator).MoveNext())
        {
            if (enumerator.Current.Name == connName)
            {
                result = true;
            }
        }

        return result;
    }

    private string getStringFromFile(string filePath)
    {
        string text = null;
        if (filePath == null || filePath.Length <= 0)
        {
            return null;
        }

        try
        {
            FileStream fileStream = File.OpenRead(filePath);
            StreamReader streamReader = new StreamReader(fileStream);
            text = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();
            return text;
        }
        catch (Exception ex)
        {
            showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)3);
            return null;
        }
    }

    private IDTSSQLTaskConnection GetSqlConnection()
    {
        //IL_00f1: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f7: Expected O, but got Unknown
        //IL_00f9: Unknown result type (might be due to invalid IL or missing references)
        //IL_00ff: Expected O, but got Unknown
        //IL_0101: Unknown result type (might be due to invalid IL or missing references)
        //IL_0107: Expected O, but got Unknown
        string connection = _generalNode.Connection;
        ConnectionManager val = null;
        try
        {
            val = _connections[(object)connection];
        }
        catch
        {
            return null;
        }

        IDTSSQLTaskConnection val2 = null;
        if (val.CreationName.StartsWith("ADO.NET", StringComparison.OrdinalIgnoreCase))
        {
            val2 = (IDTSSQLTaskConnection)(object)((!val.CreationName.StartsWith("ADO.NET:System.Data.SqlClient.SqlConnection", StringComparison.OrdinalIgnoreCase) && !val.CreationName.StartsWith("ADO.NET:SQL", StringComparison.OrdinalIgnoreCase)) ? ((!val.CreationName.StartsWith("ADO.NET:System.Data.OleDb.OleDbConnection", StringComparison.OrdinalIgnoreCase) && !val.CreationName.StartsWith("ADO.NET:OLEDB", StringComparison.OrdinalIgnoreCase)) ? ((ConnectionManaged)new ConnectionManagedGeneric()) : ((ConnectionManaged)new ConnectionManagedOleDb())) : new ConnectionManagedSQL());
        }
        else
        {
            switch (val.CreationName)
            {
                case "ODBC":
                    val2 = (IDTSSQLTaskConnection)new SQLTaskConnectionODBCClass();
                    break;
                case "OLEDB":
                case "EXCEL":
                    val2 = (IDTSSQLTaskConnection)new SQLTaskConnectionOleDbClass();
                    break;
                case "ADO":
                    val2 = (IDTSSQLTaskConnection)new SQLTaskConnectionADOClass();
                    break;
                case "SQLMOBILE":
                    val2 = (IDTSSQLTaskConnection)(object)new ConnectionManagedSQL();
                    break;
            }
        }

        if (val2 != null)
        {
            object obj2 = val.AcquireConnection((object)null);
            if (obj2 != null)
            {
                val2.SetDBConnection(obj2);
            }
        }

        return val2;
    }

    private bool gotStringToParse()
    {
        string text = null;
        if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
        {
            text = getFilePathFromFileConection(_generalNode.FileConnection);
            if (text == null || text.Trim().Length <= 0)
            {
                return false;
            }

            _sqlStatement = getStringFromFile(text);
        }
        else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Variable) == 0)
        {
            _sqlStatement = getStringFromVariable(_generalNode.SourceVariable);
        }
        else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
        {
            _sqlStatement = _generalNode.SQLStatement;
        }

        return true;
    }

    private bool checkSqlQuery(ref string ErrorDescription, string sqlStatement)
    {
        if (sqlStatement == null || sqlStatement.Trim().Length == 0)
        {
            ErrorDescription = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLQueryIsEmpty;
            return false;
        }

        IDTSSQLTaskConnection sqlConnection = GetSqlConnection();
        if (sqlConnection == null)
        {
            ErrorDescription = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SpecifiedConnNotFound;
            return false;
        }

        IDTSSQLTaskConnectionOleDb val = (IDTSSQLTaskConnectionOleDb)(object)((sqlConnection is IDTSSQLTaskConnectionOleDb) ? sqlConnection : null);
        StringCollection statements = new Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ExecuteBatch().GetStatements(sqlStatement);
        string connectionString = _connections[(object)_generalNode.Connection].ConnectionString;
        if (sqlConnection is ConnectionManagedSQL || DesignUtils.IsSqlServer(connectionString))
        {
            try
            {
                if (val != null)
                {
                    val.SetMaxRows(1);
                }

                sqlConnection.PrepareSQLStatement("Set ParseOnly On", false);
                sqlConnection.ExecuteStatement(1, false, 30u);
                StringEnumerator enumerator = statements.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        sqlConnection.PrepareSQLStatement(current, false);
                        sqlConnection.ExecuteStatement(1, false, 30u);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
                return false;
            }
            finally
            {
                sqlConnection.PrepareSQLStatement("Set ParseOnly Off", false);
                sqlConnection.ExecuteStatement(1, false, 30u);
                sqlConnection.Close();
            }
        }
        else
        {
            try
            {
                StringEnumerator enumerator = statements.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current2 = enumerator.Current;
                        sqlConnection.PrepareSQLStatement(current2, false);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable disposable2)
                    {
                        disposable2.Dispose();
                    }
                }
            }
            catch (Exception ex2)
            {
                ErrorDescription = ex2.Message;
                return false;
            }
        }

        return true;
    }

    private void updateSqlStringToSource(string sqlStmt, string filePath)
    {
        if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
        {
            _generalNode.SQLStatement = sqlStmt;
        }
        else if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileConnection) == 0)
        {
            setSqlStringToFileConnection(sqlStmt, filePath);
        }
        else
        {
            setSqlStringToVariable(sqlStmt);
        }
    }

    private void setSqlStringToFileConnection(string sqlString, string filePath)
    {
        try
        {
            StreamWriter streamWriter = File.CreateText(filePath);
            streamWriter.WriteLine(sqlString);
            streamWriter.Close();
        }
        catch (Exception ex)
        {
            showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)3);
        }
    }

    private void setSqlStringToVariable(string sqlStmt)
    {
        try
        {
            _variables[(object)_generalNode.SourceVariable].Value = sqlStmt;
        }
        catch (Exception ex)
        {
            showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)3);
        }
    }

    private string getFilePathFromFileConection(string connName)
    {
        ConnectionManager val = null;
        if (connName.Trim().Length == 0)
        {
            return null;
        }

        try
        {
            ConnectionEnumerator enumerator = _connections.GetEnumerator();
            while (((DtsEnumerator)enumerator).MoveNext())
            {
                if (!(enumerator.Current.Name == connName))
                {
                    continue;
                }

                object obj = null;
                val = _connections[(object)connName];
                if (val.ConnectionString == null || val.ConnectionString.Trim().Length == 0)
                {
                    showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileTypeConnection, (ExceptionMessageBoxSymbol)2);
                    return null;
                }

                try
                {
                    obj = val.AcquireConnection((object)null);
                    if (obj == null)
                    {
                        showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.CannotAcquireConnMgr(connName), (ExceptionMessageBoxSymbol)2);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    showExceptionMessageBox(ex.Message, (ExceptionMessageBoxSymbol)3);
                    return null;
                }

                if (obj.ToString() != null)
                {
                    if (!File.Exists(obj.ToString()))
                    {
                        showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.FileDoesNotExist(obj.ToString()), (ExceptionMessageBoxSymbol)3);
                        return null;
                    }

                    return obj.ToString();
                }
            }
        }
        catch (Exception ex2)
        {
            showExceptionMessageBox(ex2.Message, (ExceptionMessageBoxSymbol)0);
            return null;
        }

        return null;
    }

    private void showExceptionMessageBox(string message, ExceptionMessageBoxSymbol symbol)
    {
        //IL_0007: Unknown result type (might be due to invalid IL or missing references)
        //IL_0009: Unknown result type (might be due to invalid IL or missing references)
        //IL_000e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0019: Unknown result type (might be due to invalid IL or missing references)
        //IL_0021: Unknown result type (might be due to invalid IL or missing references)
        new ExceptionMessageBox(new Exception(message), (ExceptionMessageBoxButtons)0, symbol, (ExceptionMessageBoxDefaultButton)0)
        {
            Caption = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLTask,
            Buttons = (ExceptionMessageBoxButtons)0
        }.Show((IWin32Window)null);
        ((Control)this).Cursor = Cursors.Default;
    }

    private void propertyGridGeneral_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        //IL_00ad: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b3: Expected O, but got Unknown
        //IL_01dd: Unknown result type (might be due to invalid IL or missing references)
        //IL_01e4: Expected O, but got Unknown
        if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("Connection") == 0)
        {
            string text = null;
            if (e.ChangedItem.Value.Equals(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NEW_CON))
            {
                ((Control)this).Cursor = Cursors.WaitCursor;
                GenNode.Connection = null;
                text = ((_generalNode.ConnectionType.CompareTo(ConnTypeStrings.Oledb) != 0) ? _generalNode.ConnectionType : "OLEDB");
                ArrayList arrayList = ((IDtsConnectionBaseService)GenNode.iDTSConnService).CreateConnection(text);
                ((Control)this).Cursor = Cursors.Default;
                if (arrayList != null && arrayList.Count > 0)
                {
                    ConnectionManager val = (ConnectionManager)arrayList[0];
                    GenNode.Connection = val.Name;
                }
                else if (e.OldValue == null)
                {
                    GenNode.Connection = null;
                }
                else
                {
                    GenNode.Connection = (string)e.OldValue;
                }
            }
        }

        if (e.ChangedItem.PropertyDescriptor.Name == "ConnectionType" || e.ChangedItem.PropertyDescriptor.Name == "Connection")
        {
            ((Control)btnBuildQurey).Enabled = !string.IsNullOrEmpty(_generalNode.Connection) && _generalNode.ConnectionType == ConnTypeStrings.Oledb;
        }
        else if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("FileConnection") == 0)
        {
            if (e.ChangedItem.Value.Equals(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NEW_CON))
            {
                ((Control)this).Cursor = Cursors.WaitCursor;
                GenNode.FileConnection = null;
                ArrayList arrayList2 = ((IDtsConnectionBaseService)GenNode.iDTSConnService).CreateConnection("FILE");
                if (arrayList2 != null && arrayList2.Count > 0)
                {
                    ConnectionManager val2 = (ConnectionManager)arrayList2[0];
                    GenNode.FileConnection = val2.Name;
                }
                else if (e.OldValue != null)
                {
                    GenNode.FileConnection = (string)e.OldValue;
                }
                else
                {
                    GenNode.FileConnection = null;
                }

                ((Control)this).Cursor = Cursors.Default;
            }
        }
        else if (e.ChangedItem.PropertyDescriptor.Name.CompareTo("SourceVariable") == 0 && e.ChangedItem.Value.Equals(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NEW_VARIABLE))
        {
            ((Control)this).Cursor = Cursors.WaitCursor;
            GenNode.SourceVariable = null;
            IDtsVariableService variableService = GenNode.VariableService;
            if (variableService == null)
            {
                showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.VariableServiceIsNull, (ExceptionMessageBoxSymbol)2);
                return;
            }

            Variable val3 = variableService.PromptAndCreateVariable((IWin32Window)(object)this, (DtsContainer)(object)GenNode.SQLTaskHost);
            if ((DtsObject)(object)val3 == (DtsObject)null)
            {
                GenNode.SourceVariable = (string)e.OldValue;
            }
            else
            {
                GenNode.SourceVariable = val3.QualifiedName;
            }

            ((Control)this).Cursor = Cursors.Default;
        }

        if (_generalNode.SQLSourceType.CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DirectInput) == 0)
        {
            ((Control)btnBrowse).Enabled = true;
        }
        else
        {
            ((Control)btnBrowse).Enabled = false;
        }
    }
}
 
