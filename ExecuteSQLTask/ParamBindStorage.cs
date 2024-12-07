using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using ADODB;
using Microsoft.NetEnterpriseServers;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Management.UI.Grid;

namespace Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;

internal class ParamBindStorage : GenericBindStorage
{
    private enum ColumnDataType
    {
        DTSVariable,
        ParamDirection,
        DataType,
        ParamName,
        ParamSize,
        NumColumns
    }

    private class GridRow : IDTSParameterBinding
    {
        private object m_parameterName;

        private string m_dtsVariableName;

        private ParameterDirections m_parameterDirection;

        private int m_dataType;

        private int m_ParameterSize = -1;

        public object ParameterName
        {
            get
            {
                return m_parameterName;
            }
            set
            {
                m_parameterName = value;
            }
        }

        public string DtsVariableName
        {
            get
            {
                return m_dtsVariableName;
            }
            set
            {
                m_dtsVariableName = value;
            }
        }

        public ParameterDirections ParameterDirection
        {
            get
            {
                return m_parameterDirection;
            }
            set
            {
                m_parameterDirection = value;
            }
        }

        public int DataType
        {
            get
            {
                return m_dataType;
            }
            set
            {
                m_dataType = value;
            }
        }

        public int ParameterSize
        {
            get
            {
                return m_ParameterSize;
            }
            set
            {
                m_ParameterSize = value;
            }
        }

        public GridRow(int defaultDataType, Variables variables)
        {
            ArrayList arrayList = new ArrayList();
            VariableEnumerator enumerator = variables.GetEnumerator();
            while (((DtsEnumerator)enumerator).MoveNext())
            {
                Variable current = enumerator.Current;
                arrayList.Add(current.QualifiedName);
            }

            if (arrayList != null && arrayList.Count > 0)
            {
                arrayList.Sort();
            }

            m_dtsVariableName = (string)arrayList[0];
            m_parameterDirection = ParameterDirections.Input;
            m_dataType = defaultDataType;
            m_parameterName = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.NewParameterName;
            m_ParameterSize = -1;
        }

        public GridRow(IDTSParameterBinding bind)
        {
            m_parameterName = bind.ParameterName;
            m_dtsVariableName = bind.DtsVariableName;
            m_parameterDirection = bind.ParameterDirection;
            m_dataType = bind.DataType;
            m_ParameterSize = bind.ParameterSize;
        }
    }

    private Variables variables;

    private ConnectionManager m_connection;

    public override void RefreshVariables(Variables variables)
    {
        if (m_variables != null)
        {
            m_variables.Clear();
        }

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

    public ParamBindStorage(Variables variables)
        : base(variables)
    {
        this.variables = variables;
    }

    public void SetConnection(ConnectionManager connection)
    {
        m_connection = connection;
    }

    public override void AddRow()
    {
        m_bindData.Add(new GridRow(GetDataTypeValueForInt32(), variables));
    }

    public void AddRow(IDTSParameterBinding bind)
    {
        m_bindData.Add(new GridRow(bind));
    }

    public void Clear()
    {
        if (m_bindData != null)
        {
            m_bindData.Clear();
        }
    }

    public override string GetCellDataAsString(long nRowIndex, int nColIndex)
    {
        if (nRowIndex < NumRows() && nColIndex < 5)
        {
            GridRow gridRow = m_bindData[(int)nRowIndex] as GridRow;
            switch ((ColumnDataType)nColIndex)
            {
                case ColumnDataType.ParamName:
                    if (gridRow.ParameterName == null)
                    {
                        return string.Empty;
                    }

                    return gridRow.ParameterName.ToString();
                case ColumnDataType.DTSVariable:
                    return gridRow.DtsVariableName;
                case ColumnDataType.ParamDirection:
                    return gridRow.ParameterDirection.ToString();
                case ColumnDataType.DataType:
                    return GetDataTypeStringForInt(gridRow.DataType);
                case ColumnDataType.ParamSize:
                    return gridRow.ParameterSize.ToString();
                default:
                    return "";
            }
        }

        return "";
    }

    public override int IsCellEditable(long nRowIndex, int nColIndex)
    {
        if (NumRows() == 0L)
        {
            return 0;
        }

        switch (nColIndex)
        {
            case 1:
                return 3;
            case 0:
            case 2:
                return 2;
            default:
                return 1;
        }
    }

    public override void FillControlWithData(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
    {
        switch (nColIndex)
        {
            case 1:
                {
                    IGridEmbeddedControl obj = ((control is ComboBox) ? control : null);
                    ((ComboBox)obj).Items.Add((object)ParameterDirections.Input.ToString());
                    ((ComboBox)obj).Items.Add((object)ParameterDirections.Output.ToString());
                    ((ComboBox)obj).Items.Add((object)ParameterDirections.ReturnValue.ToString());
                    control.SetCurSelectionAsString(GetCellDataAsString(nRowIndex, nColIndex));
                    break;
                }
            case 0:
                FillControlWithVariableList((ComboBox)(object)((control is ComboBox) ? control : null), nRowIndex, nColIndex, bDisplayAll: true);
                break;
            case 2:
                {
                    ComboBox val = (ComboBox)(object)((control is ComboBox) ? control : null);
                    Type enumTypeForConnection = GetEnumTypeForConnection();
                    if (enumTypeForConnection != null)
                    {
                        ObjectCollection items = val.Items;
                        object[] names = Enum.GetNames(enumTypeForConnection);
                        items.AddRange(names);
                    }

                    control.SetCurSelectionAsString(GetCellDataAsString(nRowIndex, nColIndex));
                    break;
                }
            default:
                control.AddDataAsString(GetCellDataAsString(nRowIndex, nColIndex));
                break;
        }
    }

    internal void SetCellDataFromString(long nRowIndex, int nColIndex, string data)
    {
        (m_bindData[(int)nRowIndex] as GridRow).DtsVariableName = data;
    }

    public override bool SetCellDataFromControl(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
    {
        //IL_00b2: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b7: Unknown result type (might be due to invalid IL or missing references)
        //IL_00c2: Unknown result type (might be due to invalid IL or missing references)
        //IL_00ca: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f4: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f9: Unknown result type (might be due to invalid IL or missing references)
        //IL_0104: Unknown result type (might be due to invalid IL or missing references)
        //IL_010c: Unknown result type (might be due to invalid IL or missing references)
        bool flag = false;
        GridRow gridRow = m_bindData[(int)nRowIndex] as GridRow;
        string curSelectionAsString = control.GetCurSelectionAsString();
        if (curSelectionAsString.Trim().Length == 0)
        {
            flag = true;
        }
        else
        {
            switch ((ColumnDataType)nColIndex)
            {
                case ColumnDataType.ParamName:
                    gridRow.ParameterName = curSelectionAsString;
                    break;
                case ColumnDataType.DTSVariable:
                    gridRow.DtsVariableName = curSelectionAsString;
                    break;
                case ColumnDataType.ParamDirection:
                    gridRow.ParameterDirection = (ParameterDirections)Enum.Parse(gridRow.ParameterDirection.GetType(), curSelectionAsString, ignoreCase: true);
                    break;
                case ColumnDataType.DataType:
                    {
                        if (GetDataTypeAsInt(curSelectionAsString, out var intValue2))
                        {
                            gridRow.DataType = intValue2;
                            break;
                        }

                        new ExceptionMessageBox(new Exception(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.InvalidDataType), (ExceptionMessageBoxButtons)0, (ExceptionMessageBoxSymbol)3, (ExceptionMessageBoxDefaultButton)0)
                        {
                            Caption = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLTask,
                            Buttons = (ExceptionMessageBoxButtons)0
                        }.Show((IWin32Window)null);
                        break;
                    }
                case ColumnDataType.ParamSize:
                    {
                        if (GetDataTypeAsInt(curSelectionAsString, out var intValue))
                        {
                            gridRow.ParameterSize = intValue;
                            break;
                        }

                        new ExceptionMessageBox(new Exception(Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.ParameterSizeShouldBeInteger), (ExceptionMessageBoxButtons)0, (ExceptionMessageBoxSymbol)3, (ExceptionMessageBoxDefaultButton)0)
                        {
                            Caption = Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.Localized.SQLTask,
                            Buttons = (ExceptionMessageBoxButtons)0
                        }.Show((IWin32Window)null);
                        break;
                    }
                default:
                    return false;
            }
        }

        if (flag)
        {
            control.SetCurSelectionAsString(GetCellDataAsString(nRowIndex, nColIndex));
        }

        return true;
    }

    public int GetDataTypeAsInt(long rowIndex)
    {
        return ((GridRow)m_bindData[(int)rowIndex]).DataType;
    }

    private bool GetDataTypeAsInt(string cellString, out int intValue)
    {
        Type enumTypeForConnection = GetEnumTypeForConnection();
        object obj = null;
        if (enumTypeForConnection != null)
        {
            try
            {
                obj = Enum.Parse(enumTypeForConnection, cellString, ignoreCase: true);
            }
            catch (Exception)
            {
            }
        }

        if (obj == null)
        {
            obj = cellString;
        }

        try
        {
            intValue = Convert.ToInt32(obj);
            return true;
        }
        catch (Exception)
        {
            intValue = 0;
            return false;
        }
    }

    private Type GetEnumTypeForConnection()
    {
        if ((DtsObject)(object)m_connection == (DtsObject)null)
        {
            return null;
        }

        switch (m_connection.CreationName)
        {
            case "ODBC":
                return ODBCDataTypes.SQL_BIGINT.GetType();
            case "OLEDB":
            case "EXCEL":
                return OleDBDataTypes.VARIANT_BOOL.GetType();
            case "ADO":
                return ((object)(DataTypeEnum)8192).GetType();
            case "ADO.NET:OLEDB":
            case "ADO.NET:SQL":
            case "ADO.NET":
                return DbType.AnsiString.GetType();
            case "SQLMOBILE":
                return DbType.AnsiString.GetType();
            default:
                if (m_connection.CreationName.StartsWith(ConnTypeStrings.AdoDotNet))
                {
                    return DbType.AnsiString.GetType();
                }

                return null;
        }
    }

    private string GetDataTypeStringForInt(int intValue)
    {
        object obj;
        if ((DtsObject)(object)m_connection == (DtsObject)null)
        {
            obj = intValue;
        }
        else
        {
            switch (m_connection.CreationName)
            {
                case "ODBC":
                    obj = (ODBCDataTypes)intValue;
                    break;
                case "OLEDB":
                case "EXCEL":
                    obj = (OleDBDataTypes)intValue;
                    break;
                case "ADO":
                    obj = (object)(DataTypeEnum)intValue;
                    break;
                case "ADO.NET:OLEDB":
                case "ADO.NET:SQL":
                case "ADO.NET":
                    obj = (DbType)intValue;
                    break;
                case "SQLMOBILE":
                    obj = (DbType)intValue;
                    break;
                default:
                    obj = ((!m_connection.CreationName.StartsWith(ConnTypeStrings.AdoDotNet)) ? ((object)intValue) : ((object)(DbType)intValue));
                    break;
            }
        }

        return obj.ToString();
    }

    private int GetDataTypeValueForInt32()
    {
        if ((DtsObject)(object)m_connection == (DtsObject)null)
        {
            return 0;
        }

        switch (m_connection.CreationName)
        {
            case "ODBC":
                return 4;
            case "OLEDB":
            case "EXCEL":
                return 3;
            case "ADO":
                return 3;
            case "ADO.NET:OLEDB":
            case "ADO.NET:SQL":
            case "ADO.NET":
                return 11;
            case "SQLMOBILE":
                return 11;
            default:
                if (m_connection.CreationName.StartsWith(ConnTypeStrings.AdoDotNet))
                {
                    return 11;
                }

                return 0;
        }
    }
}
