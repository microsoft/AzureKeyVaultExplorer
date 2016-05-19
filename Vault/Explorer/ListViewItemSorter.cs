using System.Collections;
using System.Windows.Forms;

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
            SecretListViewItem a = (SecretListViewItem)x;
            SecretListViewItem b = (SecretListViewItem)y;
            if (a.Strikeout != b.Strikeout) return a.Strikeout ? 1 : -1;
            int c = string.Compare(a.SubItems[Column].Text, b.SubItems[Column].Text);
            return (SortOrder == SortOrder.Descending) ? -c : c;
        }
    }
}
