 

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.DataTransformationServices.Design.Project;
using Microsoft.DataTransformationServices.ScaleHelper;
using Microsoft.SqlServer.Diagnostics.STrace;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;

namespace Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

[HelpContext("sql13.dts.designer.executepackagetask.general.f1")]
public class ParametersView : UserControl, IDTSTaskUIView
{
    private Project m_project;

    private IObjectModelProjectManager m_obProjectManager;

    private ParameterAssignmentDataList m_parameterAssignmentList;

    private IDTSParameterAssignments m_parameterAssignments;

    private PackageView.PackageNode m_packageNode;

    private string m_childPackageName;

    private bool m_useProjectReference;

    private TaskHost m_dtrTaskHost;

    private IDTSTaskUIHost m_treeHost;

    private static readonly TraceContext s_tc = TraceContext.GetTraceContext("ParameterView", "ParameterView");

    private IContainer components;

    private TableLayoutPanel tableLayoutPanelParametersPage;

    private DataGridView parametersGridView;

    private Panel panelAddRemove;

    private TableLayoutPanel tableLayoutPanelAddRemove;

    private Button buttonAdd;

    private Button buttonRemove;

    private ToolTip toolTipParameterPage;

    private Label labelInstruction;

    private DataGridViewComboBoxColumn ColumnParameterName;

    private DataGridViewComboBoxColumn ColumnBindedVariableParameter;

    private DataGridViewTextBoxColumn ColumnFill;

    private TaskHost DtrTaskHost => m_dtrTaskHost;

    public ParametersView(IObjectModelProjectManager obProjectManager, Project project)
    {
        InitializeComponent();
        m_obProjectManager = obProjectManager;
        m_project = project;
        toolTipParameterPage.SetToolTip((Control)(object)buttonAdd, Localized.AddDescription);
        toolTipParameterPage.SetToolTip((Control)(object)buttonRemove, Localized.RemoveDescription);
        toolTipParameterPage.SetToolTip((Control)(object)parametersGridView, Localized.GridDescription);
    }

    public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
    {
        //IL_0092: Unknown result type (might be due to invalid IL or missing references)
        //IL_0098: Expected O, but got Unknown
        m_treeHost = treeHost;
        PackageView packageView = (PackageView)(object)m_treeHost.GetView(viewNode.TreeView.Nodes[1]);
        if (packageView != null)
        {
            m_packageNode = packageView.PackageNodeData;
        }

        if (taskHost == null)
        {
            throw new ArgumentNullException(Localized.TaskHost, Localized.CannotInitializeToNullTaskException);
        }

        m_dtrTaskHost = (TaskHost)((taskHost is TaskHost) ? taskHost : null);
        if ((DtsObject)(object)m_dtrTaskHost == (DtsObject)null || !(m_dtrTaskHost.InnerObject is ExecutePackageTask))
        {
            throw new ArgumentException(Localized.MustInitializeToOwnTaskException, Localized.TaskHost);
        }

        IDTSExecutePackage100 val = (IDTSExecutePackage100)m_dtrTaskHost.InnerObject;
        m_parameterAssignments = val.ParameterAssignments;
        m_parameterAssignmentList = new ParameterAssignmentDataList(m_parameterAssignments);
        m_parameterAssignmentList.InitList();
        PopulateParameterNames();
        PopulateVariables();
        parametersGridView.DataSource = m_parameterAssignmentList;
    }

    private void PopulateVariables()
    {
        VariableEnumerator enumerator = ((DtsContainer)m_dtrTaskHost).Variables.GetEnumerator();
        while (((DtsEnumerator)enumerator).MoveNext())
        {
            string qualifiedName = enumerator.Current.QualifiedName;
            if (!ColumnBindedVariableParameter.Items.Contains((object)qualifiedName))
            {
                ColumnBindedVariableParameter.Items.Add((object)qualifiedName);
            }
        }

        if (ColumnBindedVariableParameter.Items.Count == 0)
        {
            ColumnBindedVariableParameter.Items.Add((object)string.Empty);
        }
    }

    private void PopulateParameterNames()
    {
        //IL_00ab: Unknown result type (might be due to invalid IL or missing references)
        PackageItem val = null;
        Package val2 = null;
        Parameters val3 = null;
        if (m_packageNode != null)
        {
            m_childPackageName = m_packageNode.PackageNameFromProjectReference;
        }

        if (m_obProjectManager != null && (DtsObject)(object)m_project != (DtsObject)null)
        {
            m_obProjectManager.BeginProjectSnapshot();
        }

        if ((DtsObject)(object)m_project != (DtsObject)null && m_childPackageName != null)
        {
            val = m_project.PackageItems[m_childPackageName];
        }

        if (val != null)
        {
            val2 = val.Package;
        }

        if ((DtsObject)(object)val2 != (DtsObject)null)
        {
            val3 = val2.Parameters;
        }

        ColumnParameterName.Items.Clear();
        if (val3 != null)
        {
            foreach (Parameter item in (DTSReadOnlyCollectionBase)val3)
            {
                string name = item.Name;
                if (!ColumnParameterName.Items.Contains((object)name))
                {
                    ColumnParameterName.Items.Add((object)name);
                }
            }
        }

        if (m_obProjectManager != null && (DtsObject)(object)m_project != (DtsObject)null)
        {
            m_obProjectManager.EndProjectSnapshot();
        }

        int rowCount = parametersGridView.RowCount;
        for (int i = 0; i < rowCount; i++)
        {
            DataGridViewCell obj = parametersGridView["ColumnParameterName", i];
            DataGridViewComboBoxCell val4 = (DataGridViewComboBoxCell)(object)((obj is DataGridViewComboBoxCell) ? obj : null);
            string parameterName = m_parameterAssignmentList[i].ParameterName;
            if (val4 == null)
            {
                continue;
            }

            val4.Items.Clear();
            val4.Items.Add((object)parameterName);
            foreach (object item2 in ColumnParameterName.Items)
            {
                if (!val4.Items.Contains((object)(string)item2))
                {
                    val4.Items.Add((object)(string)item2);
                }
            }
        }
    }

    public virtual void OnValidate(ref bool bViewIsValid, ref string reason)
    {
        foreach (ParameterAssignmentData parameterAssignment in m_parameterAssignmentList)
        {
            if (parameterAssignment.ParameterName == null || parameterAssignment.ParameterName.Trim().Length == 0)
            {
                bViewIsValid = false;
                reason = Localized.ParameterNameEmpty;
            }

            if (parameterAssignment.BindedVariableOrParameterName == null || parameterAssignment.BindedVariableOrParameterName.Trim().Length == 0)
            {
                bViewIsValid = false;
                reason = Localized.BindedNameEmpty;
            }
        }

        if (!bViewIsValid)
        {
            m_treeHost.SelectView((IDTSTaskUIView)(object)this);
        }
    }

    public virtual void OnSelection()
    {
        if (m_packageNode != null)
        {
            m_useProjectReference = m_packageNode.UseProjectReference;
        }

        if ((DtsObject)(object)m_project == (DtsObject)null || !m_useProjectReference)
        {
            parametersGridView.ReadOnly = true;
            ((Control)buttonAdd).Enabled = false;
        }
        else
        {
            parametersGridView.ReadOnly = false;
            ((Control)buttonAdd).Enabled = true;
        }

        CheckRemoveButton();
        PopulateParameterNames();
        PopulateVariables();
    }

    public virtual void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
    {
    }

    public virtual void OnCommit(object taskHost)
    {
        m_parameterAssignmentList.CommitList();
    }

    private void buttonAdd_Click(object sender, EventArgs e)
    {
        int count = ColumnBindedVariableParameter.Items.Count;
        s_tc.Assert(count >= 1);
        string bindedVariableOrParameterName = (string)ColumnBindedVariableParameter.Items[0];
        ParameterAssignmentData item = new ParameterAssignmentData(GetNewParameterName(), bindedVariableOrParameterName);
        m_parameterAssignmentList.Add(item);
        int num = parametersGridView.RowCount - 1;
        parametersGridView.CurrentCell = parametersGridView[0, num];
        parametersGridView.BeginEdit(true);
    }

    private string GetNewParameterName()
    {
        int count = m_parameterAssignmentList.Count;
        int num = 0;
        string[] array = new string[count];
        foreach (object item in ColumnParameterName.Items)
        {
            bool flag = false;
            string text = (string)item;
            foreach (ParameterAssignmentData parameterAssignment in m_parameterAssignmentList)
            {
                if (string.CompareOrdinal(parameterAssignment.ParameterName, text) == 0)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                return text;
            }
        }

        foreach (ParameterAssignmentData parameterAssignment2 in m_parameterAssignmentList)
        {
            array[num++] = parameterAssignment2.ParameterName;
        }

        string newParameterName = Localized.NewParameterName;
        return GenerateNewParameterName(array, newParameterName);
    }

    private string GenerateNewParameterName(string[] usedNames, string baseName)
    {
        string text = baseName;
        int num = 0;
        while (true)
        {
            bool flag = false;
            for (int i = 0; i < usedNames.Length; i++)
            {
                if (string.CompareOrdinal(usedNames[i], text) == 0)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                break;
            }

            text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseName, ++num);
        }

        return text;
    }

    private void parametersGridView_SelectionChanged(object sender, EventArgs e)
    {
        CheckRemoveButton();
    }

    private void parametersGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
        if (parametersGridView.IsCurrentCellDirty)
        {
            parametersGridView.CommitEdit((DataGridViewDataErrorContexts)512);
        }
    }

    private void CheckRemoveButton()
    {
        if (((BaseCollection)parametersGridView.SelectedRows).Count == 0)
        {
            ((Control)buttonRemove).Enabled = false;
        }
        else
        {
            ((Control)buttonRemove).Enabled = true;
        }
    }

    private void parametersGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
        CheckRemoveButton();
        int rowCount = parametersGridView.RowCount;
        for (int i = 0; i < rowCount; i++)
        {
            DataGridViewCell obj = parametersGridView["ColumnParameterName", i];
            DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)(object)((obj is DataGridViewComboBoxCell) ? obj : null);
            DataGridViewCell obj2 = parametersGridView["ColumnBindedVariableParameter", i];
            DataGridViewComboBoxCell val2 = (DataGridViewComboBoxCell)(object)((obj2 is DataGridViewComboBoxCell) ? obj2 : null);
            string parameterName = m_parameterAssignmentList[i].ParameterName;
            string bindedVariableOrParameterName = m_parameterAssignmentList[i].BindedVariableOrParameterName;
            if (val != null)
            {
                val.Items.Clear();
                val.Items.Add((object)parameterName);
                foreach (object item in ColumnParameterName.Items)
                {
                    if (!val.Items.Contains((object)(string)item))
                    {
                        val.Items.Add((object)(string)item);
                    }
                }
            }

            if (val2 != null && !val2.Items.Contains((object)bindedVariableOrParameterName))
            {
                val2.Items.Add((object)bindedVariableOrParameterName);
            }
        }
    }

    private void buttonRemove_Click(object sender, EventArgs e)
    {
        int index = ((DataGridViewBand)parametersGridView.CurrentRow).Index;
        m_parameterAssignmentList.Remove(m_parameterAssignmentList[index]);
    }

    private void parametersGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0028: Expected O, but got Unknown
        if (((object)e.Control).GetType() == typeof(DataGridViewComboBoxEditingControl))
        {
            ComboBox val = (ComboBox)e.Control;
            val.DropDownStyle = (ComboBoxStyle)1;
            val.MaxDropDownItems = 8;
            val.DropDownHeight = val.ItemHeight * val.MaxDropDownItems;
        }
    }

    private void parametersGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        DataGridViewCell currentCell = parametersGridView.CurrentCell;
        DataGridViewComboBoxCell val = (DataGridViewComboBoxCell)(object)((currentCell is DataGridViewComboBoxCell) ? currentCell : null);
        if (val != null && !val.Items.Contains(e.FormattedValue))
        {
            val.Items.Add((object)(string)e.FormattedValue);
            if (parametersGridView.IsCurrentCellDirty)
            {
                parametersGridView.CommitEdit((DataGridViewDataErrorContexts)512);
            }

            ((DataGridViewCell)val).Value = (string)e.FormattedValue;
        }
    }

    private void parametersGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
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
        
        float displayScaleFactor = DpiUtil.GetDisplayScaleFactor();
        components = new Container();
        ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ParametersView));
        DataGridViewCellStyle val = new DataGridViewCellStyle();
        DataGridViewCellStyle val2 = new DataGridViewCellStyle();
        DataGridViewCellStyle val3 = new DataGridViewCellStyle();
        tableLayoutPanelParametersPage = new TableLayoutPanel();
        parametersGridView = new DataGridView();
        panelAddRemove = new Panel();
        tableLayoutPanelAddRemove = new TableLayoutPanel();
        buttonAdd = new Button();
        buttonRemove = new Button();
        labelInstruction = new Label();
        toolTipParameterPage = new ToolTip(components);
        ColumnParameterName = new DataGridViewComboBoxColumn();
        ColumnBindedVariableParameter = new DataGridViewComboBoxColumn();
        ColumnFill = new DataGridViewTextBoxColumn();
        ((Control)tableLayoutPanelParametersPage).SuspendLayout();
        ((ISupportInitialize)parametersGridView).BeginInit();
        ((Control)panelAddRemove).SuspendLayout();
        ((Control)tableLayoutPanelAddRemove).SuspendLayout();
        ((Control)this).SuspendLayout();
        ((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
        ((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
        componentResourceManager.ApplyResources(tableLayoutPanelParametersPage, "tableLayoutPanelParametersPage");
        tableLayoutPanelParametersPage.Controls.Add((Control)(object)parametersGridView, 1, 1);
        tableLayoutPanelParametersPage.Controls.Add((Control)(object)panelAddRemove, 1, 3);
        tableLayoutPanelParametersPage.Controls.Add((Control)(object)labelInstruction, 1, 0);
        ((Control)tableLayoutPanelParametersPage).Name = "tableLayoutPanelParametersPage";
        parametersGridView.AllowUserToAddRows = false;
        parametersGridView.AllowUserToDeleteRows = false;
        parametersGridView.AllowUserToResizeRows = false;
		
        parametersGridView.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)16;
        parametersGridView.AutoSizeRowsMode = (DataGridViewAutoSizeRowsMode)7;
        parametersGridView.BackgroundColor = SystemColors.Window;
        parametersGridView.BorderStyle = (BorderStyle)2;
		
		
        val.Alignment = (DataGridViewContentAlignment)16;
        val.BackColor = SystemColors.Control;
        val.Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
        val.ForeColor = SystemColors.WindowText;
        val.SelectionBackColor = SystemColors.Highlight;
        val.SelectionForeColor = SystemColors.HighlightText;
        val.WrapMode = (DataGridViewTriState)1;
        parametersGridView.ColumnHeadersDefaultCellStyle = val;
        parametersGridView.ColumnHeadersHeightSizeMode = (DataGridViewColumnHeadersHeightSizeMode)2;
        parametersGridView.Columns.AddRange((DataGridViewColumn[])(object)new DataGridViewColumn[3]
        {
            (DataGridViewColumn)ColumnParameterName,
            (DataGridViewColumn)ColumnBindedVariableParameter,
            (DataGridViewColumn)ColumnFill
        });
        val2.Alignment = (DataGridViewContentAlignment)16;
        val2.BackColor = SystemColors.Window;
        val2.Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
        val2.ForeColor = SystemColors.ControlText;
        val2.SelectionBackColor = SystemColors.Highlight;
        val2.SelectionForeColor = SystemColors.HighlightText;
        val2.WrapMode = (DataGridViewTriState)2;
        parametersGridView.DefaultCellStyle = val2;
        componentResourceManager.ApplyResources(parametersGridView, "parametersGridView");
        parametersGridView.EditMode = (DataGridViewEditMode)0;
        parametersGridView.MultiSelect = false;
        ((Control)parametersGridView).Name = "parametersGridView";
        val3.Alignment = (DataGridViewContentAlignment)16;
        val3.BackColor = SystemColors.Control;
        val3.Font = new Font("Microsoft Sans Serif", 8.25f, (FontStyle)0, (GraphicsUnit)3, (byte)0);
        val3.ForeColor = SystemColors.WindowText;
        val3.SelectionBackColor = SystemColors.Highlight;
        val3.SelectionForeColor = SystemColors.HighlightText;
        val3.WrapMode = (DataGridViewTriState)1;
        parametersGridView.RowHeadersDefaultCellStyle = val3;
        parametersGridView.RowHeadersVisible = false;
        parametersGridView.SelectionMode = (DataGridViewSelectionMode)1;
        parametersGridView.CellValidating += new DataGridViewCellValidatingEventHandler(parametersGridView_CellValidating);
        parametersGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(parametersGridView_EditingControlShowing);
        parametersGridView.CurrentCellDirtyStateChanged += parametersGridView_CurrentCellDirtyStateChanged;
        parametersGridView.DataError += new DataGridViewDataErrorEventHandler(parametersGridView_DataError);
        parametersGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(parametersGridView_DataBindingComplete);
        parametersGridView.SelectionChanged += parametersGridView_SelectionChanged;
        componentResourceManager.ApplyResources(panelAddRemove, "panelAddRemove");
        ((Control)panelAddRemove).Controls.Add((Control)(object)tableLayoutPanelAddRemove);
        ((Control)panelAddRemove).Name = "panelAddRemove";
        componentResourceManager.ApplyResources(tableLayoutPanelAddRemove, "tableLayoutPanelAddRemove");
        tableLayoutPanelAddRemove.Controls.Add((Control)(object)buttonAdd, 1, 1);
        tableLayoutPanelAddRemove.Controls.Add((Control)(object)buttonRemove, 2, 1);
        ((Control)tableLayoutPanelAddRemove).Name = "tableLayoutPanelAddRemove";
        componentResourceManager.ApplyResources(buttonAdd, "buttonAdd");
        ((Control)buttonAdd).Name = "buttonAdd";
        ((ButtonBase)buttonAdd).UseVisualStyleBackColor = true;
        ((Control)buttonAdd).Click += buttonAdd_Click;
        componentResourceManager.ApplyResources(buttonRemove, "buttonRemove");
        ((Control)buttonRemove).Name = "buttonRemove";
        ((ButtonBase)buttonRemove).UseVisualStyleBackColor = true;
        ((Control)buttonRemove).Click += buttonRemove_Click;
        componentResourceManager.ApplyResources(labelInstruction, "labelInstruction");
        ((Control)labelInstruction).Name = "labelInstruction";
        toolTipParameterPage.ShowAlways = true;
        ((DataGridViewColumn)ColumnParameterName).AutoSizeMode = (DataGridViewAutoSizeColumnMode)1;
        ((DataGridViewColumn)ColumnParameterName).DataPropertyName = "ParameterName";
        ColumnParameterName.DisplayStyle = (DataGridViewComboBoxDisplayStyle)0;
        ColumnParameterName.DropDownWidth = 160;
        ColumnParameterName.FlatStyle = (FlatStyle)0;
        componentResourceManager.ApplyResources(ColumnParameterName, "ColumnParameterName");
        ColumnParameterName.MaxDropDownItems = 5;
        ((DataGridViewColumn)ColumnParameterName).Name = "ColumnParameterName";
        ((DataGridViewBand)ColumnParameterName).Resizable = (DataGridViewTriState)1;
        ColumnParameterName.Sorted = true;
        ((DataGridViewColumn)ColumnParameterName).Width = (int)(160f * displayScaleFactor);
		
		
        ((DataGridViewColumn)ColumnBindedVariableParameter).AutoSizeMode = (DataGridViewAutoSizeColumnMode)1;
        ((DataGridViewColumn)ColumnBindedVariableParameter).DataPropertyName = "BindedVariableOrParameterName";
        ColumnBindedVariableParameter.DisplayStyle = (DataGridViewComboBoxDisplayStyle)0;
        ColumnBindedVariableParameter.DropDownWidth = 160;
        ColumnBindedVariableParameter.FlatStyle = (FlatStyle)0;
        componentResourceManager.ApplyResources(ColumnBindedVariableParameter, "ColumnBindedVariableParameter");
        ColumnBindedVariableParameter.MaxDropDownItems = 5;
        ((DataGridViewColumn)ColumnBindedVariableParameter).Name = "ColumnBindedVariableParameter";
        ((DataGridViewBand)ColumnBindedVariableParameter).Resizable = (DataGridViewTriState)1;
        ColumnBindedVariableParameter.Sorted = true;
        ((DataGridViewColumn)ColumnBindedVariableParameter).Width = (int)(200f * displayScaleFactor);
        ((DataGridViewColumn)ColumnFill).AutoSizeMode = (DataGridViewAutoSizeColumnMode)16;
        componentResourceManager.ApplyResources(ColumnFill, "ColumnFill");
        ((DataGridViewColumn)ColumnFill).Name = "ColumnFill";
        ((DataGridViewBand)ColumnFill).ReadOnly = true;
		
        componentResourceManager.ApplyResources(this, "$this");
        ((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
        ((Control)this).Controls.Add((Control)(object)tableLayoutPanelParametersPage);
        ((Control)this).Name = "ParametersView";
        ((Control)tableLayoutPanelParametersPage).ResumeLayout(false);
        ((Control)tableLayoutPanelParametersPage).PerformLayout();
        ((ISupportInitialize)parametersGridView).EndInit();
        ((Control)panelAddRemove).ResumeLayout(false);
        ((Control)tableLayoutPanelAddRemove).ResumeLayout(false);
        ((Control)tableLayoutPanelAddRemove).PerformLayout();
        ((Control)this).ResumeLayout(false);
    }
}
