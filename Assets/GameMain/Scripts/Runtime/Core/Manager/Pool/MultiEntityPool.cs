using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class MultiEntityPool<T> where T : ARecoverable
    {
        private Dictionary<string, EntityPool<T>> _pools;

        private Transform _poolRoot;

        public MultiEntityPool(Transform poolRoot)
        {
            _pools = new();
            _poolRoot = poolRoot;
        }

        public void PushToPool(T instance)
        {
            if (instance == null)
            {
                Debugger.LogError($"instance = null");
                return;
            }

            string location = instance.Location;
            if (string.IsNullOrEmpty(location))
            {
                Debugger.Log($"location = null 自动销毁");
                GameObject.DestroyImmediate(instance.gameObject);
                return;
            }

            if (_pools.TryGetValue(location, out var pool) == false)
            {
                Debugger.LogError($"不存在的池子 poolName:{location}");
                return;
            }

            pool.PushToPool(instance);
        }

        public void AddPoolComponent(T prefab)
        {
            if (!_pools.ContainsKey(prefab.name))
            {
                var pool = new EntityPool<T>(prefab.gameObject, _poolRoot);
                _pools.Add(prefab.name, pool);
            }
            else
            {
                Debugger.LogError($"已经添加的预制体:{prefab.gameObject.name}");
            }
        }

        public void DestroyAllEntity()
        {
            foreach (var p in _pools)
            {
                p.Value.DestroyAllEntity();
            }
        }

        public void DisableAllEntities()
        {
            foreach (var p in _pools)
            {
                p.Value.DisableAllEntities();
            }
        }

        public async UniTask<T> GetEntity(T prefab)
        {
            foreach (var pool in _pools)
            {
                if (pool.Key.Equals(prefab.name))
                {
                    var result = await pool.Value.GetFromPoolAsync();
                    return result.Item1;
                }
            }

            Debugger.LogError($"不存在的缓存:{prefab.gameObject.name}");
            return null;
        }

        public T GetEntitySync(string localtion)
        {
            if (_pools.TryGetValue(localtion, out var pool) == false)
            {
                pool = new EntityPool<T>(localtion, _poolRoot);
                _pools.Add(localtion, pool);
            }

            var entity = pool.GetFromPoolSync();
            return entity.Item1;
        }

        public async UniTask<T> GetEntityAsync(string localtion)
        {
            if (_pools.TryGetValue(localtion, out var pool) == false)
            {
                pool = new EntityPool<T>(localtion, _poolRoot);
                _pools.Add(localtion, pool);
            }

            var entity = await pool.GetFromPoolAsync();
            return entity.Item1;
        }
    }
}