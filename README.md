# SSISCustomTasks

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe" -u LogVariableTask


bin\NETFX 4.8 Tools\gacutil.exe" -if "C:\Program Files\Microsoft SQL Server\150\DTS\Tasks\LogVariableTask.dll"

C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Windows.Forms.dll


managed dts
dts.desin
runtime

create strong key

change dll output path to sqlserver dts path
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 ➥ Tools\gacutil.exe" -if "E:\Program Files (x86)\Microsoft SQL ➥ Server\150\DTS\Tasks\ExecuteCatalogPackageTask.dll"


Microsoft.SqlServer.Dts.Design.dll
Microsoft.SqlServer.ManagedDts.dll

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\sn.exe" -k key.snk
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\sn.exe" -p key.snk public.out
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\sn.exe" -t public.out

The first line creates the public/private key pair and puts them in a file named key.snk.

The second line extracts the public part of the key pair to a file named public.out.

The third line reads the public key from the public.out file.

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\gacutil.exe" -u LogVariableTask

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\gacutil.exe" -if LogVariableTask

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\gacutil.exe" -u LogVariableTaskUI

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools\gacutil.exe" -if LogVariableTaskUI

UITypeName= "LogVariableTaskUI.LogVariableTaskUI,LogVariableTaskUI,Version=1.0.0.0,Culture=Neutral,PublicKeyToken=04f7a693b0d868a9"
,TaskContact = "Venkat Bodempudi" 


---------------------------------------------
project implementation steps
1. Always run visual studio as an administrator
2. Add library
	Extensions - Microsoft.SqlServer.ManagedDts
3. Right click on class library and signin using key generated
4. Task Class code
	using Microsoft.SqlServer.Dts.Runtime;
	[DtsTask(
        TaskType = "DTS<version>"--optional
      , DisplayName = "Execute Catalog Package Task"
      , Description = "A task to execute packages stored in the SSIS Catalog."
        )]
	public class ExecuteCatalogPackageTask : Microsoft.SqlServer.Dts.Runtime.Task
5. Override validate task - just return SUCCESS
6. Override initilize task
7. Override Execute method
8. Add New Project for UI.
9. Add new dlls, Extensions, Scroll until you find Microsoft.SqlServer.Dts.Design and Microsoft.SqlServer.ManagedDTS.
	using Microsoft.SqlServer.Dts.Runtime;
	using Microsoft.SqlServer.Dts.Runtime.Design;
10. public class ExecuteCatalogPackageTaskUI : IDtsTaskUI
		private TaskHost taskhost;
		add System.Windows.Forms assembly to the class
		
		public void New(System.Windows.Forms.IWin32Window form) { }
		
		Add a form to the UI Project
		Implement Getview method in UI Class as we have an interface.
		
		public void Initialize(Microsoft.SqlServer.Dts.Runtime.TaskHost taskHost//task ui class
                     , System.IServiceProvider serviceProvider)
		{
		   taskHostValue = taskHost;
		}
		
		public void Delete(System.Windows.Forms.IWin32Window form) { }
		
11. this is for form class
	using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;
	private TaskHost taskHost; in form class
	
	public ExecuteCatalogPackageTaskUIForm (TaskHost taskHostValue)
{
  InitializeComponent();
  taskHost = taskHostValue;
  txtInstance.Text = ➥ taskHost.Properties["ServerName"].GetValue(taskHost).ToString();
  txtFolder.Text = ➥ taskHost.Properties["PackageFolder"].GetValue(taskHost).ToString();
  txtProject.Text = ➥ taskHost.Properties["PackageProject"].GetValue(taskHost).ToString();
  txtPackage.Text = ➥ taskHost.Properties["PackageName"].GetValue(taskHost).ToString();
}
12. Complete the signin process
13. Do this in task class - , UITypeName= "ExecuteCatalogPackageTaskUI.ExecuteCatalogPackageTaskUI ➥
",ExecuteCatalogPackageTaskUI,Version=1.0.0.0,Culture=Neutral ➥
,PublicKeyToken=<Your public key>"
, TaskContact = "ExecuteCatalogPackageTask; Building Custom Tasks for ➥
 SQL Server Integration Services, 2019 Edition; © 2020 Andy Leonard; ➥ https://dilmsuite.com/ExecuteCatalogPackageTaskBookCode"
Listing 9-4Updating the DtsTask decoration

14. 
