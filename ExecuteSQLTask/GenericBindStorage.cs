
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Management.UI.Grid;

namespace Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;

internal abstract class GenericBindStorage : IGridStorage
{
    private Variables variables;

    protected ArrayList m_bindData = new ArrayList();

    protected Hashtable m_variables;

    protected Variables m_VariablesLocalCopy;

    public GenericBindStorage(Variables variables)
    {
        this.variables = variables;
        m_variables = new Hashtable(((DTSReadOnlyCollectionBase)variables).Count);
        m_VariablesLocalCopy = variables;
        VariableEnumerator enumerator = variables.GetEnumerator();
        while (((DtsEnumerator)enumerator).MoveNext())
        {
            string qualifiedName = enumerator.Current.QualifiedName;
            if (!m_variables.ContainsKey(qualifiedName))
            {
                m_variables.Add(qualifiedName, null);
            }
        }
    }

    public abstract void AddRow();

    public abstract string GetCellDataAsString(long nRowIndex, int nColIndex);

    public abstract int IsCellEditable(long nRowIndex, int nColIndex);

    public abstract bool SetCellDataFromControl(long nRowIndex, int nColIndex, IGridEmbeddedControl control);

    public abstract void FillControlWithData(long nRowIndex, int nColIndex, IGridEmbeddedControl control);

    public abstract void RefreshVariables(Variables variables);

    public void RemoveRow(long index)
    {
        m_bindData.RemoveAt((int)index);
    }

    public long NumRows()
    {
        return m_bindData.Count;
    }

    public long EnsureRowsInBuf(long FirstRowIndex, long LastRowIndex)
    {
        return NumRows();
    }

    public GridCheckBoxState GetCellDataForCheckBox(long nRowIndex, int nColIndex)
    {
        return (GridCheckBoxState)4;
    }

    public Bitmap GetCellDataAsBitmap(long nRowIndex, int nColIndex)
    {
        return null;
    }

    public void GetCellDataForButton(long nRowIndex, int nColIndex, out ButtonCellState state, out Bitmap image, out string buttonLabel)
    {
        state = (ButtonCellState)1;
        image = null;
        buttonLabel = null;
    }

    protected void FillControlWithVariableList(ComboBox comboBox, long nRowIndex, int nColIndex, bool bDisplayAll)
    {
        RefreshVariables(variables);
        comboBox.Items.Add((object)Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NEW_VARIABLE);
        if (bDisplayAll)
        {
            foreach (string key in m_variables.Keys)
            {
                comboBox.Items.Add((object)key);
                comboBox.Sorted = true;
            }
        }
        else
        {
            foreach (string key2 in m_variables.Keys)
            {
                if (!m_VariablesLocalCopy[(object)key2].ReadOnly)
                {
                    comboBox.Items.Add((object)key2);
                    comboBox.Sorted = true;
                }
            }
        }

        ((Control)comboBox).Text = GetCellDataAsString(nRowIndex, nColIndex);
    }
}
