using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;

namespace ExecuteCatalogPackageTaskComplexUI
{
    public partial class GeneralView : System.Windows.Forms.UserControl, IDTSTaskUIView
    {
        private GeneralNode generalNode = null;
        private System.Windows.Forms.PropertyGrid generalPropertyGrid;
        private ExecuteCatalogPackageTask.ExecuteCatalogPackageTask theTask = null;
        private System.ComponentModel.Container components = null;

        public GeneralView()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // generalPropertyGrid
            this.generalPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            this.generalPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                                                                                    | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                    | System.Windows.Forms.AnchorStyles.Left)
                                                                                    | System.Windows.Forms.AnchorStyles.Right)));
            this.generalPropertyGrid.Location = new System.Drawing.Point(3, 0);
            this.generalPropertyGrid.Name = "generalPropertyGrid";
            this.generalPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.generalPropertyGrid.Size = new System.Drawing.Size(387, 360);
            this.generalPropertyGrid.TabIndex = 0;
            this.generalPropertyGrid.ToolbarVisible = false;

            // GeneralView
            this.Controls.Add(this.generalPropertyGrid);
            this.Name = "GeneralView";
            this.Size = new System.Drawing.Size(390, 360);
            this.ResumeLayout(false);
        }

        public virtual void OnInitialize(IDTSTaskUIHost treeHost
                                       , System.Windows.Forms.TreeNode viewNode
                                       , object taskHost
                                       , object connections)
        {
            if (taskHost == null)
            {
                throw new ArgumentNullException("Attempting to initialize the ExecuteCatalogPackageTask UI with a null TaskHost.");
            }

            if (!(((TaskHost)taskHost).InnerObject is ExecuteCatalogPackageTask.ExecuteCatalogPackageTask))
            {
                throw new ArgumentException("Attempting to initialize the ExecuteCatalogPackageTask UI with a task that is not an ExecuteCatalogPackageTask.");
            }

            theTask = ((TaskHost)taskHost).InnerObject as ExecuteCatalogPackageTask.ExecuteCatalogPackageTask;

            this.generalNode = new GeneralNode(taskHost as TaskHost);

            generalPropertyGrid.SelectedObject = this.generalNode;

            generalNode.Name = ((TaskHost)taskHost).Name;
        }

        public void OnValidate(ref bool bViewIsValid, ref string reason)
        {
            // throw new NotImplementedException();
        }

        public virtual void OnCommit(object taskHost)
        {
            TaskHost th = (TaskHost)taskHost;

            th.Name = generalNode.Name.Trim();
            th.Description = generalNode.Description.Trim();

            theTask.TaskName = generalNode.Name.Trim();
            theTask.TaskDescription = generalNode.Description.Trim();
        }

        public void OnSelection()
        {
            // throw new NotImplementedException();
        }

        public void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
        {
            // throw new NotImplementedException();
        }
    }

    internal class GeneralNode
    {
        internal TaskHost taskHost = null;
        private ExecuteCatalogPackageTask.ExecuteCatalogPackageTask task = null;

        public GeneralNode(TaskHost taskHost)
        {
            this.taskHost = taskHost;
            this.task = taskHost.InnerObject as ExecuteCatalogPackageTask.ExecuteCatalogPackageTask;
        }

        [
        Category("General"),
        Description("Specifies the name of the task.")
        ]
        public string Name {
            get { return task.TaskName; }
            set {
                if ((value == null) || (value.Trim().Length == 0))
                {
                    throw new ApplicationException("Task name cannot be empty");
                }

                task.TaskName = value;
            }
        }

        [
        Category("General"),
        Description("Specifies the description for this task.")
        ]
        public string Description {
            get { return task.TaskDescription; }
            set { task.TaskDescription = value; }
        }
    }
}
