using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class ListViewSecrets : ListView
    {
        public int SortingColumn { get; set; }

        public int StrikedoutSecrets { get; private set; }

        public ListViewSecrets()
        {
            InitializeComponent();
            ListViewItemSorter = new ListViewSecretsSorter(this);
        }

        public ListViewItemBase FirstSelectedItem => SelectedItems.Count > 0 ? SelectedItems[0] as ListViewItemBase : null;

        public void Replace(ListViewItemBase oldItem, ListViewItemBase newItem)
        {
            Items.Remove(oldItem);
            Items.Add(newItem);
            newItem.RefreshAndSelect();
        }

        public void RemoveAllItems()
        {
            Items.Clear();
            StrikedoutSecrets = 0;
        }

        public void FindItemsWithText(string text)
        {
            StrikedoutSecrets = 0;
            ListViewItemBase selectItem = null;
            BeginUpdate();
            foreach (ListViewItemBase lvib in Items)
            {
                bool contains = lvib.Contains(text);
                lvib.Strikeout = !contains;
                StrikedoutSecrets += contains ? 0 : 1;
                if ((selectItem == null) && contains)
                {
                    selectItem = lvib;
                }
            }
            Sort();
            selectItem?.RefreshAndSelect();
            EndUpdate();
        }

        private void ListViewSecrets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            BeginUpdate();
            if (SortingColumn == e.Column) // Swap sort order
            {
                Sorting = (Sorting == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                SortingColumn = e.Column;
                Sorting = SortOrder.Ascending;
            }
            EndUpdate();
        }
    }

    /// <summary>
    /// Simple ListViewItemSorter to sort by two columns (Strikeout and Column index)
    /// </summary>
    public class ListViewSecretsSorter : IComparer
    {
        private readonly ListViewSecrets _control;

        public ListViewSecretsSorter(ListViewSecrets control)
        {
            _control = control;
        }

        public int Compare(object x, object y)
        {
            ListViewItemBase sx = (ListViewItemBase)x;
            ListViewItemBase sy = (ListViewItemBase)y;

            ListViewItem.ListViewSubItem a = sx.SubItems[_control.SortingColumn];
            ListViewItem.ListViewSubItem b = sy.SubItems[_control.SortingColumn];
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
            return (_control.Sorting == SortOrder.Descending) ? -c : c;
        }
    }
}
