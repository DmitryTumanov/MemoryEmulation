using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace MemoryEmulation.DataContext.MVVMBasics
{
    public class VmCollection<T> : List<T>, INotifyCollectionChanged
    {
        private readonly Dispatcher _dispatcher;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public VmCollection(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected virtual void OnChanged(NotifyCollectionChangedEventArgs eee)
        {
            var x = CollectionChanged;
            _dispatcher.Invoke(() => x?.Invoke(this, eee));
        }

        public void FireChange()
        {
            OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void FireChange(T item)
        {
            OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item, IndexOf(item)));
        }

        public new void Add(T item)
        {
            base.Add(item);
            OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
        }

        protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(() => OnCollectionChangedMultiItemEvent(e));
        }

        private void OnCollectionChangedMultiItemEvent(NotifyCollectionChangedEventArgs e)
        {
            var handlers = CollectionChanged;

            if (handlers == null)
            {
                return;
            }

            foreach (var eventHandler in handlers.GetInvocationList())
            {
                var handler = (NotifyCollectionChangedEventHandler)eventHandler;
                if (handler.Target is CollectionView collectionView)
                {
                    collectionView.Refresh();
                }
                else
                {
                    handler(this, e);
                }
            }
        }
    }
}
