using System;
using System.Collections;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Simple ListViewItemSorter to sort by two columns (Strikeout and Column index)
    /// </summary>
    public class ListViewItemSorter : IComparer
    {
        public int Column;
        public SortOrder SortOrder;

        public ListViewItemSorter()
        {
            Column = 0;
            SortOrder = SortOrder.Ascending;
        }

        public ListViewItemSorter(int column, SortOrder sortOrder)
        {
            Column = column;
            SortOrder = sortOrder;
        }

        public void SwapSortOder()
        {
            SortOrder = (SortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
        }

        public int Compare(object x, object y)
        {
            ListViewItemBase sx = (ListViewItemBase)x;
            ListViewItemBase sy = (ListViewItemBase)y;

            ListViewSubItem a = sx.SubItems[Column];
            ListViewSubItem b = sy.SubItems[Column];
            if (sx.Strikeout != sy.Strikeout) return sx.Strikeout ? 1 : -1;

            int c = 0;
            if ((a.Tag != null) && (b.Tag != null) && (a.Tag is DateTime?) && (b.Tag is DateTime?) && (a.Tag as DateTime?).HasValue && (b.Tag as DateTime?).HasValue)
            {
                var adt = (a.Tag as DateTime?).Value;
                var bdt = (b.Tag as DateTime?).Value;
                c = DateTime.Compare(adt, bdt);
            }
            else
            {
                c = string.Compare(a.Text, b.Text);
            }
            return (SortOrder == SortOrder.Descending) ? -c : c;
        }
    }
}
