using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogVariableTask
{
   [
    DtsTask
    (
        TaskType="DTS150"
        ,Description ="This task will be used for adding variable values to SSIS Execution Log"
        ,DisplayName = "LogVariableTask"
    )
   ]
    public class LogVariableTask : Microsoft.SqlServer.Dts.Runtime.Task
    {
        public LogVariableTask()
        {
            //public key token is 53a54c1d0a7a6530
        }
        public override void InitializeTask(Connections connections, VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log, EventInfos eventInfos, LogEntryInfos logEntryInfos, ObjectReferenceTracker refTracker)
        {
            base.InitializeTask(connections, variableDispenser, events, log, eventInfos, logEntryInfos, refTracker);
        }
        public override DTSExecResult Validate(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log)
        {
            return DTSExecResult.Success;
        }
        public override DTSExecResult Execute(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log, object transaction)
        {
            return base.Execute(connections, variableDispenser, componentEvents, log, transaction);
        }
    }
}
