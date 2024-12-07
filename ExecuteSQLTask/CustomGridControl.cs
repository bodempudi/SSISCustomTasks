using System.Collections;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.UI.Grid;

namespace Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;

internal class CustomGridControl : GridControl
{
    public CustomGridControl()
    {
        ((Control)this).SetAutoSizeMode((AutoSizeMode)0);
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((int)keyData != 9)
        {
            if ((int)keyData == 65545 && ((GridControl)this).SelectedCells != null && ((CollectionBase)(object)((GridControl)this).SelectedCells).Count >= 1)
            {
                if (base.m_gridStorage.NumRows() <= 0)
                {
                    ((Control)this).Parent.Parent.Select();
                    return true;
                }

                if (((GridControl)this).SelectedCells[0].X == 0)
                {
                    ((Control)this).Parent.Parent.Select();
                    return true;
                }
            }
        }
        else if (((GridControl)this).SelectedCells != null && ((CollectionBase)(object)((GridControl)this).SelectedCells).Count >= 1)
        {
            if (base.m_gridStorage.NumRows() <= 0)
            {
                if (((Control)this).Name == "bindParamDisplay")
                {
                    ((Control)this).Parent.Controls["buttonParamAdd"].Select();
                }
                else
                {
                    ((Control)this).Parent.Controls["buttonResultAdd"].Select();
                }

                return true;
            }

            if (((GridControl)this).SelectedCells[0].X == ((CollectionBase)(object)((GridControl)this).GridColumnsInfo).Count - 1)
            {
                if (((Control)this).Name == "bindParamDisplay")
                {
                    ((Control)this).Parent.Controls["buttonParamAdd"].Select();
                }
                else
                {
                    ((Control)this).Parent.Controls["buttonResultAdd"].Select();
                }

                return true;
            }
        }

        return ((GridControl)this).ProcessDialogKey(keyData);
    }

    public void SelectRowAfterDeletion(int deletedRow)
    {
        int num = base.m_selMgr.CurrentColumn;
        if (num < 0)
        {
            num = 0;
        }

        long num2 = ((deletedRow >= base.m_gridStorage.NumRows()) ? (deletedRow - 1) : deletedRow);
        BlockOfCells val = new BlockOfCells(num2, num);
        base.m_selMgr.SelectedBlocks.Add(val);
        base.m_selMgr.UpdateCurrentBlock(num2, num);
    }

    internal void HandleGotFocus()
    {
        long num = 0L;
        int num2 = 0;
        if (base.m_selMgr != null)
        {
            if (base.m_selMgr.LastUpdatedRow >= 1)
            {
                num = base.m_selMgr.LastUpdatedRow;
            }

            if (base.m_selMgr.LastUpdatedColumn >= 1)
            {
                num2 = base.m_selMgr.LastUpdatedColumn;
            }

            BlockOfCells val = new BlockOfCells(num, num2);
            base.m_selMgr.SelectedBlocks.Add(val);
            base.m_selMgr.UpdateCurrentBlock(num, num2);
            ((GridControl)this).UpdateGrid(true);
        }
    }
}
