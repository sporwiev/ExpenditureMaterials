using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Helpers
{
    public class SubList<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
    {
        public T1 Item1 { get; set; } = item1;
        public T2 Item2 { get; set; } = item2;
        public T3 Item3 { get; set; } = item3;

        public T4 Item4 { get; set; } = item4;
    }
    public class List<T1, T2, T3, T4> : IList<BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>>
    {
        private List<BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>> _items = new List<BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>>();

        public BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> item)
        {
            _items.Add(item);
        }

        public void Add(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            _items.Add(new BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>(item1, item2, item3, item4));
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> item)
        {
            _items.Insert(index, item);
        }

        public bool Remove(BifServiceExpenditureMaterials.Helpers.SubList<T1, T2, T3, T4> item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
