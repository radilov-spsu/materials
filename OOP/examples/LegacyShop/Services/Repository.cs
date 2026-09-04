using System;
using System.Collections.Generic;
using LegacyShop.Model;

namespace LegacyShop.Services
{
    /// <summary>
    /// Универсальный контракт хранилища. Сделан заранее, чтобы потом легко
    /// переехать на базу данных: интерфейс останется, поменяется реализация.
    /// </summary>
    public interface IRepository<T> where T : ShopEntity
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(string id);
        T GetById(string id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate);
        int Count();
        void Clear();
    }

    public class InMemoryRepository<T> : IRepository<T> where T : ShopEntity
    {
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();

        public void Add(T entity)
        {
            _items[entity.Id] = entity;
        }

        public void Update(T entity)
        {
            _items[entity.Id] = entity;
        }

        public void Remove(string id)
        {
            _items.Remove(id);
        }

        public T GetById(string id)
        {
            T value;
            if (_items.TryGetValue(id, out value))
            {
                return value;
            }
            return null;
        }

        public IEnumerable<T> GetAll()
        {
            return _items.Values;
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            List<T> result = new List<T>();
            foreach (T item in _items.Values)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public int Count()
        {
            return _items.Count;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
