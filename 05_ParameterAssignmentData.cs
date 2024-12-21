 

namespace Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

internal class ParameterAssignmentData
{
    private string m_parameterName;

    private string m_bindedVariableOrParameterName;

    public string ParameterName
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

    public string BindedVariableOrParameterName
    {
        get
        {
            return m_bindedVariableOrParameterName;
        }
        set
        {
            m_bindedVariableOrParameterName = value;
        }
    }

    public ParameterAssignmentData(IDTSParameterAssignment assignment)
    {
        ParameterName = assignment.ParameterName;
        BindedVariableOrParameterName = assignment.BindedVariableOrParameterName;
    }

    public ParameterAssignmentData(string parameterName, string bindedVariableOrParameterName)
    {
        ParameterName = parameterName;
        BindedVariableOrParameterName = bindedVariableOrParameterName;
    }
}
