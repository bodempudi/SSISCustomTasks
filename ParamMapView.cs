using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.NetEnterpriseServers;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Graph.Extended;
using Microsoft.SqlServer.Management.UI.Grid;

namespace Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;


internal class ParamMapView : UserControl, IDTSTaskUIView
{
    private Container components;

    private Button buttonParamAdd;

    private Button buttonParamRemove;

    private CustomGridControl bindParamDisplay;

    private ParamBindStorage m_paramBindStorage;

    private Connections m_connections;

    private IDTSTaskUIHost _treeHost;

    private TreeNode _viewNode;

    private GeneralView gView;

    private IDtsVariableService _variableService;

    private TableLayoutPanel tableLayoutPanel1;

    private TaskHost _dtrTaskHost;

    private IDtsVariableService VariableService => _variableService;

    private TaskHost DtrTaskHost => _dtrTaskHost;

    internal ParamMapView()
    {
        InitializeComponent();
        bindParamDisplay = new CustomGridControl();
        ((Control)bindParamDisplay).Name = "bindParamDisplay";
        ((GridControl)bindParamDisplay).AlwaysHighlightSelection = true;
        ((GridControl)bindParamDisplay).FocusEditorOnNavigation = true;
        ((Control)bindParamDisplay).Visible = true;
        ((Control)bindParamDisplay).TabIndex = 0;
        ((GridControl)bindParamDisplay).SelectionType = (GridSelectionType)0;
        ((Control)bindParamDisplay).AutoSize = true;
        ((Control)bindParamDisplay).Anchor = (AnchorStyles)15;
        tableLayoutPanel1.Controls.Add((Control)(object)bindParamDisplay, 0, 0);
        tableLayoutPanel1.SetColumnSpan((Control)(object)bindParamDisplay, tableLayoutPanel1.ColumnCount);
        ((Control)tableLayoutPanel1).PerformLayout();
        ((Control)this).PerformLayout();
        ((GridControl)bindParamDisplay).EmbeddedControlContentsChanged += new EmbeddedControlContentsChangedEventHandler(bindParamDisplay_EmbeddedControlContentsChanged);
        SetTheme(HighContrastSupport.Theme);
        HighContrastSupport.ThemeChanged += new ThemeChangeHandler(SetTheme);
    }

    private void bindParamDisplay_EmbeddedControlContentsChanged(object sender, EmbeddedControlContentsChangedEventArgs evtArgs)
    {
        if (evtArgs.EmbeddedControl.GetCurSelectionAsString().CompareTo(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NEW_VARIABLE) != 0)
        {
            return;
        }

        ((Control)this).Cursor = Cursors.WaitCursor;
        ParamBindStorage paramBindStorage = (ParamBindStorage)(object)((GridControl)bindParamDisplay).GridStorage;
        evtArgs.EmbeddedControl.ClearData();
        IDtsVariableService variableService = VariableService;
        if (variableService == null)
        {
            showExceptionMessageBox(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.VariableServiceIsNull);
            ((Control)this).Cursor = Cursors.Default;
            return;
        }

        Variable val = variableService.PromptAndCreateVariable((IWin32Window)(object)this, (DtsContainer)(object)DtrTaskHost);
        if ((DtsObject)(object)val == (DtsObject)null)
        {
            paramBindStorage.SetCellDataFromString(evtArgs.EmbeddedControl.RowIndex, evtArgs.EmbeddedControl.ColumnIndex, evtArgs.EmbeddedControl.GetCurSelectionAsString());
        }
        else
        {
            paramBindStorage.SetCellDataFromString(evtArgs.EmbeddedControl.RowIndex, evtArgs.EmbeddedControl.ColumnIndex, val.QualifiedName);
            ((GridControl)bindParamDisplay).UpdateGrid(true);
        }

        ((Control)this).Cursor = Cursors.Default;
    }

    private void SetTheme(DesignerTheme theme)
    {
        ((Control)buttonParamAdd).BackColor = HighContrastSupport.GeneralWindowBackColor;
        ((Control)buttonParamAdd).ForeColor = HighContrastSupport.GeneralWindowForeColor;
        ((Control)buttonParamRemove).BackColor = HighContrastSupport.GeneralWindowBackColor;
        ((Control)buttonParamRemove).ForeColor = HighContrastSupport.GeneralWindowForeColor;
        ((ButtonBase)buttonParamAdd).FlatStyle = (FlatStyle)3;
        ((ButtonBase)buttonParamRemove).FlatStyle = (FlatStyle)3;
    }

    private void InitParamGrid()
    {
        int columnWidth = ((Control)bindParamDisplay).Width / 5;
        GridColumnInfo val = new GridColumnInfo();
        val.ColumnType = 1;
        val.WidthType = (GridColumnWidthType)0;
        val.ColumnWidth = columnWidth;
        ((GridControl)bindParamDisplay).AddColumn(val);
        val = new GridColumnInfo();
        val.ColumnType = 1;
        val.WidthType = (GridColumnWidthType)0;
        val.ColumnWidth = columnWidth;
        ((GridControl)bindParamDisplay).AddColumn(val);
        val = new GridColumnInfo();
        val.ColumnType = 1;
        val.WidthType = (GridColumnWidthType)0;
        val.ColumnWidth = columnWidth;
        ((GridControl)bindParamDisplay).AddColumn(val);
        val = new GridColumnInfo();
        val.ColumnType = 1;
        val.WidthType = (GridColumnWidthType)0;
        val.ColumnWidth = columnWidth;
        ((GridControl)bindParamDisplay).AddColumn(val);
        val = new GridColumnInfo();
        val.ColumnType = 1;
        val.WidthType = (GridColumnWidthType)0;
        val.ColumnWidth = columnWidth;
        ((GridControl)bindParamDisplay).AddColumn(val);
        ((GridControl)bindParamDisplay).SetHeaderInfo(0, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DtsVariableName, (Bitmap)null);
        ((GridControl)bindParamDisplay).SetHeaderInfo(1, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.Direction, (Bitmap)null);
        ((GridControl)bindParamDisplay).SetHeaderInfo(2, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.DataType, (Bitmap)null);
        ((GridControl)bindParamDisplay).SetHeaderInfo(3, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ParameterName, (Bitmap)null);
        ((GridControl)bindParamDisplay).SetHeaderInfo(4, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ParameterSize, (Bitmap)null);
        ((GridControl)bindParamDisplay).GridStorage = (IGridStorage)(object)m_paramBindStorage;
        bindParamDisplay.HandleGotFocus();
        ((GridControl)bindParamDisplay).UpdateGrid();
    }

    private ArrayList GetSortedListOfSelectedRows(BlockOfCellsCollection cellsColl)
    { 
        ArrayList arrayList = new ArrayList();
        foreach (BlockOfCells item in (CollectionBase)(object)cellsColl)
        {
            BlockOfCells val = item;
            for (long num = val.Y; num <= val.Bottom; num++)
            {
                arrayList.Add(num);
            }
        }

        arrayList.Sort();
        return arrayList;
    }

    private void showExceptionMessageBox(string message)
    {    
        new ExceptionMessageBox(new Exception(message), (ExceptionMessageBoxButtons)0, (ExceptionMessageBoxSymbol)2)
        {
            Caption = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLTask
        }.Show((IWin32Window)null);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        ((ContainerControl)this).Dispose(disposing);
        HighContrastSupport.ThemeChanged -= new ThemeChangeHandler(SetTheme);
    }

    private void InitializeComponent()
    {
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ParamMapView));
        buttonParamAdd = new Button();
        buttonParamRemove = new Button();
        tableLayoutPanel1 = new TableLayoutPanel();
        ((Control)tableLayoutPanel1).SuspendLayout();
        ((Control)this).SuspendLayout();
        componentResourceManager.ApplyResources(buttonParamAdd, "buttonParamAdd");
        ((Control)buttonParamAdd).Name = "buttonParamAdd";
        ((Control)buttonParamAdd).Click += buttonParamAdd_Click;
        componentResourceManager.ApplyResources(buttonParamRemove, "buttonParamRemove");
        ((Control)buttonParamRemove).Name = "buttonParamRemove";
        ((Control)buttonParamRemove).Click += buttonParamRemove_Click;
        componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
        tableLayoutPanel1.Controls.Add((Control)(object)buttonParamRemove, 2, 1);
        tableLayoutPanel1.Controls.Add((Control)(object)buttonParamAdd, 1, 1);
        ((Control)tableLayoutPanel1).Name = "tableLayoutPanel1";
        componentResourceManager.ApplyResources(this, "$this");
        ((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
        ((Control)this).Name = "ParamMapView";
        ((Control)tableLayoutPanel1).ResumeLayout(false);
        ((Control)tableLayoutPanel1).PerformLayout();
        ((Control)this).ResumeLayout(false);
        ((Control)this).PerformLayout();
    }

    public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
    {
        _treeHost = treeHost;
        _viewNode = viewNode;
        if (taskHost == null)
        {
            throw new ArgumentNullException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TaskHost, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.CantInitializeToNull);
        }

        _dtrTaskHost = (TaskHost)taskHost;
        if (!(_dtrTaskHost.InnerObject is ExecuteSQLTask))
        {
            throw new ArgumentException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.MustInitializetoSQLTask, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.TaskHost);
        }

        ExecuteSQLTask executeSQLTask = (ExecuteSQLTask)_dtrTaskHost.InnerObject;
        m_paramBindStorage = new ParamBindStorage(((DtsContainer)_dtrTaskHost).Variables);
        ref IDtsVariableService variableService = ref _variableService;
        object service = ((DtsContainer)_dtrTaskHost).Site.GetService(typeof(IDtsVariableService));
        variableService = (IDtsVariableService)((service is IDtsVariableService) ? service : null);
        IDtsConnectionService val = (IDtsConnectionService)connections;
        m_connections = ((IDtsConnectionBaseService)val).GetConnections();
        try
        {
            m_paramBindStorage.SetConnection(m_connections[(object)executeSQLTask.Connection]);
        }
        catch
        {
            m_paramBindStorage.SetConnection(null);
        }

        InitParamGrid();
        foreach (IDTSParameterBinding parameterBinding in executeSQLTask.ParameterBindings)
        {
            m_paramBindStorage.AddRow(parameterBinding);
        }

        ((GridControl)bindParamDisplay).UpdateGrid(true);
    }

    public virtual void OnValidate(ref bool bViewIsValid, ref string reason)
    {
    }

    public virtual void OnSelection()
    {
        gView = (GeneralView)(object)_treeHost.GetView(_viewNode.TreeView.Nodes[0]);
        if (gView.GenNode.Connection.Trim().Length == 0 && m_paramBindStorage != null)
        {
            m_paramBindStorage.Clear();
            ((GridControl)bindParamDisplay).UpdateGrid(true);
        }

        if (((GridControl)bindParamDisplay).GridStorage.NumRows() > 0)
        {
            ((Control)buttonParamRemove).Enabled = true;
        }
        else
        {
            ((Control)buttonParamRemove).Enabled = false;
        }

        try
        {
            m_paramBindStorage.SetConnection(m_connections[(object)gView.GenNode.Connection]);
        }
        catch (Exception)
        {
            m_paramBindStorage.SetConnection(null);
        }
    }

    public virtual void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
    {
    }

    public virtual void OnCommit(object taskHost)
    {
        ExecuteSQLTask executeSQLTask = (ExecuteSQLTask)((TaskHost)taskHost).InnerObject;
        long num = m_paramBindStorage.NumRows();
        executeSQLTask.ParameterBindings.Clear();
        for (long num2 = 0L; num2 < num; num2++)
        {
            IDTSParameterBinding iDTSParameterBinding = executeSQLTask.ParameterBindings.Add();
            iDTSParameterBinding.DtsVariableName = m_paramBindStorage.GetCellDataAsString(num2, 0);
            iDTSParameterBinding.ParameterDirection = (ParameterDirections)Enum.Parse(iDTSParameterBinding.ParameterDirection.GetType(), m_paramBindStorage.GetCellDataAsString(num2, 1), ignoreCase: true);
            iDTSParameterBinding.DataType = m_paramBindStorage.GetDataTypeAsInt(num2);
            string cellDataAsString = m_paramBindStorage.GetCellDataAsString(num2, 3);
            try
            {
                iDTSParameterBinding.ParameterName = Convert.ToInt32(cellDataAsString);
            }
            catch (Exception)
            {
                iDTSParameterBinding.ParameterName = cellDataAsString;
            }

            string cellDataAsString2 = m_paramBindStorage.GetCellDataAsString(num2, 4);
            try
            {
                iDTSParameterBinding.ParameterSize = Convert.ToInt32(cellDataAsString2);
            }
            catch (Exception)
            {
                throw new ArgumentException(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ParameterSizeShouldBeInteger);
            }
        }
    }

    private void buttonParamAdd_Click(object sender, EventArgs e)
    {
        if (gView != null && (gView.GenNode.Connection == null || gView.GenNode.Connection.Trim().Length == 0))
        {
            new ExceptionMessageBox(new Exception(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ConnectionNotSpecified), (ExceptionMessageBoxButtons)0, (ExceptionMessageBoxSymbol)2)
            {
                Caption = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLTask
            }.Show((IWin32Window)null);
            return;
        }

        ParamBindStorage paramBindStorage = m_paramBindStorage;
        GridControl val = (GridControl)(object)bindParamDisplay;
        paramBindStorage.AddRow();
        bindParamDisplay.HandleGotFocus();
        val.UpdateGrid(true);
        if (((GridControl)bindParamDisplay).GridStorage.NumRows() > 0)
        {
            ((Control)buttonParamRemove).Enabled = true;
        }
    }

    private void buttonParamRemove_Click(object sender, EventArgs e)
    {
        GenericBindStorage paramBindStorage = m_paramBindStorage;
        GridControl val = (GridControl)(object)bindParamDisplay;
        ArrayList sortedListOfSelectedRows = GetSortedListOfSelectedRows(val.SelectedCells);
        for (int num = sortedListOfSelectedRows.Count - 1; num >= 0; num--)
        {
            paramBindStorage.RemoveRow((long)sortedListOfSelectedRows[num]);
        }

        val.UpdateGrid(true);
        ((Control)buttonParamRemove).Enabled = false;
        long num2 = (long)sortedListOfSelectedRows[sortedListOfSelectedRows.Count - 1];
        if (num2 > val.GridStorage.NumRows() - 1)
        {
            num2 = val.GridStorage.NumRows() - 1;
        }

        BlockOfCellsCollection val2 = new BlockOfCellsCollection();
        BlockOfCells val3 = new BlockOfCells(num2, 0);
        val3.Height = 1L;
        val3.Width = val.ColumnsNumber;
        if (num2 >= 0)
        {
            val2.Add(val3);
        }

        val.SelectedCells = val2;
        SelectionChangedEventArgs args = new SelectionChangedEventArgs(val.SelectedCells);
        GridDisplay_SelectionChanged(val, args);
    }

    private void GridDisplay_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        ((Control)buttonParamRemove).Enabled = ((CollectionBase)(object)args.SelectedBlocks).Count > 0;
    }
}
