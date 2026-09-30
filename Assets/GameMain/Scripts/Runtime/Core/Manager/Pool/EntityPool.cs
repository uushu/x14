using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain
{
    public class EntityPool<T> where T : ARecoverable
    {
        //未显示
        protected List<T> _disableEntities;

        //已显示
        protected List<T> _enableEntities;

        public Transform PoolRoot { get; private set; }
        public GameObject Prefab { get; private set; }

        private Transform _loadMask;

        private string _location;

        public EntityPool(string name, Transform poolRoot, Transform loadMask = null)
        {
            PoolRoot = poolRoot;
            _location = name;
            _disableEntities = new();
            _enableEntities = new();
            _loadMask = loadMask;
        }

        public EntityPool(GameObject prefab, Transform poolRoot, Transform loadMask = null)
        {
            Prefab = prefab;
            PoolRoot = poolRoot;
            _location = prefab.name;
            _disableEntities = new();
            _enableEntities = new();
            _loadMask = loadMask;
        }

        private T CreateEntitySync()
        {
            if (Prefab == null)
            {
                Debugger.LogError($"对象池,{PoolRoot.name} Prefab=null");
                return null;
            }

            var instance = Object.Instantiate(Prefab, PoolRoot, false);
            var component = instance.GetComponent<T>();
            if (component == null) Debugger.LogError($"{instance.name} 不存在的类型 {typeof(T)}");
            component.Location = _location;
            component.name = _location;
            return component;
        }

        private async UniTask<T> CreateEntityAsync()
        {
            if (Prefab == null)
            {
                if (_loadMask != null)
                {
                    _loadMask.gameObject.SetActive(true);
                }

                Prefab = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(_location);

                if (_loadMask != null)
                {
                    _loadMask.gameObject.SetActive(false);
                }
            }

            return CreateEntitySync();
        }

        public async UniTask<(T, bool)> GetFromPoolAsync()
        {
            T entity = default;
            bool isFromPool = false;

            if (_disableEntities.Count > 0)
            {
                entity = _disableEntities[0];
                _disableEntities.RemoveAt(0);
                entity.gameObject.SetActive(true);
                entity.OnGetFromPool();
                isFromPool = true;
            }
            else
            {
                entity = await CreateEntityAsync();
                entity.Location = _location;
                isFromPool = false;
            }

            _enableEntities.Add(entity);
            entity.name = _location;
            return (entity, isFromPool);
        }

        public (T, bool) GetFromPoolSync()
        {
            T entity = default;
            bool isFromPool = false;

            if (_disableEntities.Count > 0)
            {
                entity = _disableEntities.Last();
                entity.gameObject.SetActive(true);
                _disableEntities.RemoveAt(_disableEntities.Count - 1);
                entity.OnGetFromPool();
                isFromPool = true;
            }
            else
            {
                entity = CreateEntitySync();
                isFromPool = false;
            }

            _enableEntities.Add(entity);
            entity.name = _location;
            return (entity, isFromPool);
        }

        public void PushToPool(T entity)
        {
            if (entity == null)
            {
                Debugger.LogError($"{PoolRoot.name} entity=null _poolRoot={PoolRoot.transform.name}");
                return;
            }

            if (_disableEntities.Contains(entity) == false)
            {
                _disableEntities.Add(entity);
            }
            else
            {
                Debugger.LogError($"{PoolRoot.name} 池子中已经存在:{entity.name}");
            }

            _enableEntities.Remove(entity);
            entity.transform.SetParent(PoolRoot);
            entity.gameObject.SetActive(false);
            entity.OnPushToPool();
        }

        public void DisableAllEntities()
        {
            for (int i = _enableEntities.Count - 1; i >= 0; i--)
            {
                var entity = _enableEntities[i];
                PushToPool(entity);
            }

            _enableEntities.Clear();
        }

        public void DestroyAllEntity()
        {
            foreach (var entity in _disableEntities)
            {
                if (entity != null)
                {
                    GameObject.DestroyImmediate(entity);
                }
            }

            foreach (var entity in _enableEntities)
            {
                if (entity != null)
                {
                    GameObject.DestroyImmediate(entity);
                }
            }

            _disableEntities.Clear();
            _enableEntities.Clear();
        }

        private void Test()
        {
            var hashSet = new HashSet<T>();

            foreach (var entity in _disableEntities)
            {
                if (hashSet.Contains(entity))
                {
                    Debug.LogError($"_disableEntities 已经存在:{entity.name}");
                    continue;
                }

                hashSet.Add(entity);
            }

            foreach (var entity in _enableEntities)
            {
                if (hashSet.Contains(entity))
                {
                    Debug.LogError($"_enableEntities 已经存在:{entity.name}");
                    continue;
                }

                hashSet.Add(entity);
            }
        }
    }
}