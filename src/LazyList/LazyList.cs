using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChairWars.DataStructures
{
    public interface IsAlive
    {
        bool Destroyed { get; set; }
    }

    public class LazyList<T> : IList<T> where T : IsAlive
    {
        private List<T> _List;
        private int alive = -1;
        public int Alive { get { return alive + 1; } }

        public IList<T> List 
        { 
            get 
            {
                return _List; 
            } 
            private set 
            {
                _List = new List<T>(value);
            }
        }

        public LazyList()
        {
            _List = new List<T>();
        }

        public LazyList(IList<T> list, bool removeDead = true)
        {

             _List = new List<T>(list.Where(f => !f.Destroyed));

            alive = _List.Count - 1;
        }

        public void ForEach(Action<T> func)
        {
            var size = Alive;
            for (int i = 0; i < size; i++)
            {
                func(_List[i]);
            }
        }

        public void ForAll(Action<T> func)
        {
            _List.ForEach(func);
        }

        public void ClearDead()
        {
            if (alive == 0)
            {
                _List.Clear();
                alive = _List.Count - 1;
                return;
            }
            var alivePlusOne = alive + 1;
            for (int i = _List.Count - 1; i > alivePlusOne; i--)
            {
                _List.RemoveAt(i);
            }
            alive = _List.Count - 1;
        }

        public int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (index > alive)
            {
                if (Alive != Count)
                {
                    _List[++alive] = item; // Add to end of Alive list
                }
                else
                {
                    _List.Add(item); // No dead so just add an item to end.
                }
                alive++;
            }
            else
            {
                _List[index] = item; // Overwrite
            }
        }

        public void RemoveAt(int index)
        {

            if (index < 0 || index > alive || index >= _List.Count)
                return;

            if (alive != index)
            {

                T temp = _List[index];
                _List[index] = _List[alive];
                _List[alive] = temp;
                //LLHelper.Swap<T>(_List, alive, index);
            }

            _List[alive].Destroyed = true;
            alive--;

            
            // If at end make index as kill
            // decrement index
            // else
            // swap(List[index], List[LastCount]);
            // decrement our index


        }


        

        public T this[int index]
        {
            get
            {
                return _List[index];
            }
            set
            {
                Insert(index, value);
            }
        }

        public void Add(T item)
        {
            List.Insert(++alive, item);
        }

        public void Clear()
        {
            List.Clear();
            alive = -1;
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public bool AliveContains(T Item)
        {
           // return List.Where(f => !f.Kill).Contains(item);
            for (int i = 0; i <= alive; i++)
            {
                if (_List[i].Equals(Item))
                    return true;
            }
            return false;
           // return List.Where(
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _List.Count; }
        }

        public bool IsReadOnly
        {
            get { return List.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            RemoveAt(index);
            if (index > 0) 
                return true;

            return false;

        }

        public IEnumerator<T> GetEnumerator()
        {
            return _List.Where(f => !f.Destroyed).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _List.Where( f => !f.Destroyed).GetEnumerator();
        }
    }

    public static class LLHelper
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp;
            temp = a;
            a = b;
            b = temp;
        }

        public static void Swap<T>(this IList<T> list, int idx1, int idx2)
        {
            T temp = list[idx1];
            list[idx1] = list[idx2];
            list[idx2] = temp;
        }
    }
}
