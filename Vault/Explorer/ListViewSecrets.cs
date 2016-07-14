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
using System.Text.RegularExpressions;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class ListViewSecrets : ListView
    {
        public int SortingColumn { get; set; }

        public ListViewSecrets()
        {
            InitializeComponent();
            ListViewItemSorter = new ListViewSecretsSorter(this);
            DoubleBuffered = true;
        }

        public ListViewItemBase FirstSelectedItem => SelectedItems.Count > 0 ? SelectedItems[0] as ListViewItemBase : null;

        public int SearchResultsCount => Groups[ListViewItemBase.SearchResultsGroup].Items.Count;

        public void RefreshGroupsHeader()
        {
            foreach (var g in Groups.Cast<ListViewGroup>())
            {
                g.Header = $"{g.Name} ({g.Items.Count})";
            }
        }

        public void AddOrReplaceItem(ListViewItemBase item)
        {
            Guard.ArgumentNotNull(item, nameof(item));
            Items.RemoveByKey(item.Name); // Overwrite flow
            Items.Add(item);
        }

        public void RemoveAllItems()
        {
            Items.Clear();
        }

        public void FindItemsWithText(string regexPattern)
        {
            ListViewItemBase selectItem = null;
            BeginUpdate();
            Regex regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            foreach (ListViewItemBase lvib in Items)
            {
                bool contains = lvib.Contains(regex);
                lvib.SearchResult = contains;
                if ((selectItem == null) && contains)
                {
                    selectItem = lvib;
                }
            }
            Sort();
            selectItem?.RefreshAndSelect();
            RefreshGroupsHeader();
            EndUpdate();
        }

        public void ToggleSelectedItemsToFromFavorites()
        {
            BeginUpdate();
            foreach (ListViewItemBase lvib in SelectedItems) lvib.Favorite = !lvib.Favorite;
            Sort();
            RefreshGroupsHeader();
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
