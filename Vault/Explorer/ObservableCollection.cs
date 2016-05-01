using System.ComponentModel;

namespace VaultExplorer
{
    /// <summary>
    /// Simple wrapper on top of ObservableCollection, so we can register for:
    /// protected event PropertyChangedEventHandler PropertyChanged;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public ObservableCollection() : base() { }

        public void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            PropertyChanged += propertyChanged;
        }
    }
}
