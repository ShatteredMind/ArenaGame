using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace ObjectPool
{
    public class ObjectPool<T> 
    {

        private List<T> items;
        public List<T> Items
        {
            get { return items; }
        }
        private List<T> usedItems;
        public List<T> UsedItems
        {
            get { return usedItems; }
        }
        private Func<T> itemGenerator;

        public ObjectPool(Func<T> ItemGenerator)
        {
            if (ItemGenerator == null) throw new NullReferenceException("Item Generator can't be null!");
            items = new List<T>();
            usedItems = new List<T>();
            itemGenerator = ItemGenerator;
        }

        public void AddItem(T item)
        {
            items.Add(item);
        }

        public T GetItemProperties()
        {
            foreach (T item in items)
            {
                if (!usedItems.Contains(item)) 
                {
                   return item;
                }
            }
            return default(T);
        }

        public T GetItem()
        {
            foreach (T item in items)
            {
                if (!usedItems.Contains(item))
                {
                    usedItems.Add(item);
                    return item;
                }
            }
            return default(T);
        }

        public T GetUsedItemProperties()
        {
            foreach (T item in usedItems)
            {
               return item;
            }
            return default(T);
        }

        public void RemoveItem(T item)
        {
            usedItems.Remove(item);
            items.Remove(item);
        }
    }

}


