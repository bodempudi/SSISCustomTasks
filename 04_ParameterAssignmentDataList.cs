 

using System.ComponentModel;

namespace Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

internal class ParameterAssignmentDataList : BindingList<ParameterAssignmentData>
{
    private IDTSParameterAssignments m_parameterAssignments;

    public ParameterAssignmentDataList(IDTSParameterAssignments parameterAssignments)
    {
        base.AllowNew = true;
        base.AllowEdit = true;
        m_parameterAssignments = parameterAssignments;
    }

    public void InitList()
    {
        int count = m_parameterAssignments.Count;
        for (int i = 0; i < count; i++)
        {
            ParameterAssignmentData item = new ParameterAssignmentData(m_parameterAssignments[(object)i]);
            Add(item);
        }
    }

    public void CommitList()
    {
        for (int num = m_parameterAssignments.Count - 1; num >= 0; num--)
        {
            m_parameterAssignments.Remove((object)num);
        }

        int num2 = 0;
        foreach (ParameterAssignmentData item in base.Items)
        {
            m_parameterAssignments.Add();
            m_parameterAssignments[(object)num2].ParameterName = item.ParameterName;
            m_parameterAssignments[(object)num2].BindedVariableOrParameterName = item.BindedVariableOrParameterName;
            num2++;
        }
    }
}
