using System.Collections;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault
{
    /// <summary>
    /// Simple ListViewItemComparer to sort by column 
    /// </summary>
    class ListViewItemComparer : IComparer
    {
        private readonly int _column = 0;
        private readonly SortOrder _sortOrder = SortOrder.Ascending;

        public ListViewItemComparer(int column, SortOrder sortOrder)
        {
            _column = column;
            _sortOrder = sortOrder;
        }

        public int Compare(object x, object y)
        {
            int c = string.Compare(((ListViewItem)x).SubItems[_column].Text, ((ListViewItem)y).SubItems[_column].Text);
            return (_sortOrder == SortOrder.Descending) ? -c : c;
        }
    }
}
