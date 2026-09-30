using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain
{
    public class ObjPool
    {
        //在池子中的实例
        protected List<GameObject> _disableEntities;

        //出池子的实例
        protected List<GameObject> _enableEntities;

        public Transform PoolRoot { get; private set; }
        public GameObject Prefab { get; private set; }

        private Transform _loadMask;

        private string _location;

        public ObjPool(string name, Transform poolRoot, Transform loadMask = null)
        {
            PoolRoot = poolRoot;
            _location = name;
            _disableEntities = new();
            _enableEntities = new();
            _loadMask = loadMask;
        }

        public ObjPool(GameObject prefab, Transform poolRoot, Transform loadMask = null)
        {
            Prefab = prefab;
            PoolRoot = poolRoot;
            _location = prefab.name;
            _disableEntities = new();
            _enableEntities = new();
            _loadMask = loadMask;
        }

        private GameObject CreateEntitySync()
        {
            if (Prefab == null)
            {
                Debugger.LogError("同步使用池子错误 Prefab=null");
                return null;
            }

            var instance = Object.Instantiate(Prefab, PoolRoot, false);
            instance.name = _location;
            return instance;
        }

        private async UniTask<GameObject> CreateEntityAsync()
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

        public async UniTask<(GameObject, bool)> GetFromPoolAsync()
        {
            GameObject entity = default;
            bool isFromPool = false;

            if (_disableEntities.Count > 0)
            {
                entity = _disableEntities[0];
                _disableEntities.RemoveAt(0);
                entity.gameObject.SetActive(true);
                isFromPool = true;
            }
            else
            {
                entity = await CreateEntityAsync();
                isFromPool = false;
            }

            _enableEntities.Add(entity);
            entity.name = _location;
            return (entity, isFromPool);
        }

        public (GameObject, bool) GetFromPoolSync()
        {
            GameObject entity = default;
            bool isFromPool = false;

            if (_disableEntities.Count > 0)
            {
                entity = _disableEntities.Last();
                entity.gameObject.SetActive(true);
                _disableEntities.RemoveAt(_disableEntities.Count - 1);
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

        public void PushToPool(GameObject entity)
        {
            if (entity == null)
            {
                Debugger.LogError($"放入池子 entity=null _poolRoot={PoolRoot.transform.name}");
                return;
            }

            if (_disableEntities.Contains(entity) == false)
            {
                _disableEntities.Add(entity);
            }
            else
            {
                Debugger.LogError($"池子中已经存在:{entity.name}");
            }

            entity.transform.SetParent(PoolRoot);
            entity.gameObject.SetActive(false);

            _enableEntities.Remove(entity);
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

    }
}