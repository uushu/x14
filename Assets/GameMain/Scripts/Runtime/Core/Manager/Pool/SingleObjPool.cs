using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class SingleObjPool<T> where T : MonoBehaviour
    {
        protected Dictionary<string, T> _entities;

        public Transform PoolRoot { get; private set; }

        private Transform _loadMask;

        public SingleObjPool(Transform poolRoot, Transform loadMask)
        {
            PoolRoot = poolRoot;
            _loadMask = loadMask;
            _entities = new();
        }

        public async UniTask<(T, bool)> GetFromPool(string location)
        {
            if (_entities.TryGetValue(location, out var entity))
            {
                if (entity != null)
                {
                    _entities.Remove(location);
                    entity.gameObject.SetActive(true);
                    return (entity, true);
                }
            }

            if (_loadMask != null)
            {
                _loadMask.gameObject.SetActive(true);
            }

            var prefab = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(location);

            if (_loadMask != null)
            {
                _loadMask.gameObject.SetActive(false);
            }

            entity = GameObject.Instantiate(prefab).GetComponent<T>();
            entity.name = location;
            entity.gameObject.SetActive(true);
            return (entity, false);
        }

        public virtual void PushToPool(T entity)
        {
            if (entity == null)
            {
                Debugger.LogError($"放入池子 entity=null _poolRoot={PoolRoot.transform.name}");
                return;
            }

            if (_entities.TryGetValue(entity.name, out var value))
            {
                if (value != null)
                { 
                    GameObject.Destroy(value.gameObject);
                    _entities.Remove(entity.name);
                    Debugger.LogError($"已经存在缓存 entity:{entity.name} 销毁使用新的缓存代替");
                }
            }

            entity.transform.SetParent(PoolRoot);
            entity.gameObject.SetActive(false);
            _entities.Add(entity.name, entity);
        }

        public void DestroyAllEntity()
        {
            foreach (var item in _entities)
            {
                GameObject.DestroyImmediate(item.Value.gameObject);
            }

            _entities.Clear();
        }
    }
}