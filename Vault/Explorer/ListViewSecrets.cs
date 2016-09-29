using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public partial class ListViewSecrets : ListView
    {
        public const int FirstCustomColumnIndex = 4;

        public int SortingColumn { get; set; }

        public ListViewSecrets()
        {
            InitializeComponent();
            ListViewItemSorter = new ListViewSecretsSorter(this);
            DoubleBuffered = true;
            _tags = new Dictionary<string, TagMenuItem>();
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

        private void RemoveTags(IDictionary<string, string> tags)
        {
            if (null == tags) return;
            foreach (string t in tags.Keys)
            {
                if (false == _tags.ContainsKey(t)) continue;
                _tags[t].Count--;
                if (_tags[t].Count < 0)
                {
                    _tags.Remove(t);
                }
            }
        }

        private void AddTags(IDictionary<string, string> tags)
        {
            if (null == tags) return;
            foreach (string t in tags.Keys)
            {
                var tag = _tags.ContainsKey(t) ? _tags[t] : new TagMenuItem(t, this);
                tag.Count++;
                _tags[t] = tag;
            }
        }

        public void AddOrReplaceItem(ListViewItemBase item)
        {
            if (null == item) return;
            if (Items.ContainsKey(item.Name)) // Overwrite flow
            {
                var lvi = Items[item.Name] as ListViewItemBase;
                RemoveTags(lvi.Tags);
                Items.RemoveByKey(item.Name); 
            }
            Items.Add(item);
            AddTags(item.Tags);
        }

        public void RemoveAllItems()
        {
            // Remove custom tag columns
            for (int i = Columns.Count - 1; i >= ListViewSecrets.FirstCustomColumnIndex; i--)
            {
                Columns.RemoveAt(i);
            }
            Items.Clear();
            _tags.Clear();
        }

        public Exception FindItemsWithText(string regexPattern)
        {
            try
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
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
            finally
            {
                EndUpdate();
            }
        }

        public void ToggleSelectedItemsToFromFavorites()
        {
            BeginUpdate();
            foreach (ListViewItemBase lvib in SelectedItems) lvib.Favorite = !lvib.Favorite;
            Sort();
            RefreshGroupsHeader();
            EndUpdate();
        }

        public void ExportToTsv(string filename)
        {
            StringBuilder sb = new StringBuilder();
            // Output column headers
            foreach (ColumnHeader col in Columns)
            {
                sb.AppendFormat("{0}\t", col.Text);
            }
            sb.Append("Status\t");
            sb.Append("Valid from time (UTC)\t");
            sb.Append("Valid until time (UTC)\t");
            sb.Append("Content Type");
            sb.AppendLine();
            // Take all items or in case of multiple selection only the selected ones
            IEnumerable<ListViewItem> items = (SelectedItems.Count <= 1) ? Items.Cast<ListViewItem>() : SelectedItems.Cast<ListViewItem>();
            foreach (ListViewItem item in items)
            {
                sb.AppendLine(item.ToString());
            }
            File.WriteAllText(filename, sb.ToString());
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            // Show uxMenuStripColumns menu in case user right-click on columns header bar
            if ((m.Msg == WM_CONTEXTMENU) && (m.WParam != this.Handle))
            {
                uxMenuStripColumns.Items.Clear();
                var sortedTags = from t in _tags.Keys orderby t select t;
                foreach (string k in sortedTags)
                {
                    uxMenuStripColumns.Items.Add(_tags[k]);
                }
                uxMenuStripColumns.Show(Control.MousePosition);
            }
        }

        private const int WM_CONTEXTMENU = 0x7B;
        private Dictionary<string, TagMenuItem> _tags;
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

            int col = Math.Min(_control.SortingColumn, _control.Columns.Count - 1);

            ListViewItem.ListViewSubItem a = sx.SubItems[col];
            ListViewItem.ListViewSubItem b = sy.SubItems[col];

            int c = 0;
            if ((a.Tag != null) && (b.Tag != null))
            {
                // Compare DateTime
                if ((a.Tag is DateTime?) && (b.Tag is DateTime?) && (a.Tag as DateTime?).HasValue && (b.Tag as DateTime?).HasValue)
                {
                    var adt = (a.Tag as DateTime?).Value;
                    var bdt = (b.Tag as DateTime?).Value;
                    c = DateTime.Compare(adt, bdt);
                }
                // Compare TimeSpan
                if ((a.Tag is TimeSpan?) && (b.Tag is TimeSpan?) && (a.Tag as TimeSpan?).HasValue && (b.Tag as TimeSpan?).HasValue)
                {
                    var ats = (a.Tag as TimeSpan?).Value;
                    var bts = (b.Tag as TimeSpan?).Value;
                    c = TimeSpan.Compare(ats, bts);
                }
            }
            else
            {
                c = string.Compare(a.Text, b.Text);
            }
            return (_control.Sorting == SortOrder.Descending) ? -c : c;
        }
    }

    public class TagMenuItem : ToolStripMenuItem, IComparable, IComparable<TagMenuItem>
    {
        public readonly ListViewSecrets ListView;
        public int Count;

        public TagMenuItem(string tag, ListViewSecrets listView) : base(tag)
        {     
            Name = tag;
            Count = 0;
            ListView = listView;
        }

        public override string Text
        {
            get
            {
                return $"{Name} ({Count})";
            }
            set
            {
                base.Text = value;
            }
        }

        public override int GetHashCode() => Name.GetHashCode();

        public override bool Equals(object obj) => Equals((TagMenuItem)obj);

        public bool Equals(TagMenuItem tag)
        {
            if (null == tag) return false;
            return (Name == tag.Name);
        }

        public int CompareTo(object obj) => CompareTo((TagMenuItem)obj);

        public int CompareTo(TagMenuItem other)
        {
            if (null == other) return -1;
            return string.Compare(Name, other.Name);
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
            if (Checked)
            {
                ListView.Columns.Add(new ColumnHeader() { Name = Name, Text = Name, Width = 200 });
            }
            else
            {
                ListView.Columns.RemoveByKey(Name);
            }
            foreach (var item in ListView.Items.Cast<ListViewItemBase>())
            {
                item.RepopulateSubItems();
            }
        }
    }
}
