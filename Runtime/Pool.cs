using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
    internal sealed class Pool
    {
        private GameObject _source;
        private Poolable _prototype;
        private Stack<Poolable> _instances;
        private List<GameObject> _allInstances;
        private readonly Quaternion _rotation;
        private readonly Vector3 _scale;
        private readonly bool _prototypeIsNotSource;

        private static readonly Dictionary<GameObject, Pool> _prefabLookup = new Dictionary<GameObject, Pool>(64);
        private static readonly Dictionary<GameObject, Pool> _instanceLookup = new Dictionary<GameObject, Pool>(512);

        private const int InitialSize = 128;

        public Pool(GameObject prefab)
        {
            _source = prefab;
            _prototype = prefab.GetComponent<Poolable>();

            if (_prototype == null)
            {
                _prototype = Object.Instantiate(prefab).AddComponent<Poolable>();
                Object.DontDestroyOnLoad(_prototype);
                _prototype.gameObject.SetActive(false);
                _prototypeIsNotSource = true;
            }
            
            _instances = new Stack<Poolable>(InitialSize);
            _allInstances = new List<GameObject>(InitialSize);
            _prefabLookup.Add(_source, this);

            var transform = prefab.transform;
            _rotation = transform.rotation;
            _scale = transform.localScale;
        }

        public static Pool GetPoolByPrefab(GameObject prefab, bool create = true)
        {
            var hasPool = _prefabLookup.TryGetValue(prefab, out var pool);

            if (!hasPool && create)
                pool = new Pool(prefab);

            return pool;
        }

        public static bool GetPoolByInstance(GameObject instance, out Pool pool)
        {
            return _instanceLookup.TryGetValue(instance, out pool);
        }

        public static void Remove(GameObject instance)
        {
            _instanceLookup.Remove(instance);
        }

        public void Populate(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var instance = CreateInstance();
                instance.gameObject.SetActive(false);
                _instances.Push(instance);
            }
        }

        public void Clear(bool destroyActive)
        {
            _prefabLookup.Remove(_source);
            
            foreach (var instance in _allInstances)
            {
                if (instance == null)
                    continue;

                _instanceLookup.Remove(instance);
                
                if (!destroyActive && instance.activeInHierarchy)
                    continue;
                
                Object.Destroy(instance);
            }
            
            if (_prototypeIsNotSource)
                Object.Destroy(_prototype.gameObject);

            _source = null;
            _prototype = null;
            _instances = null;
            _allInstances = null;
        }

        public GameObject Reuse()
        {
            var instance = GetInstance();

            return instance.gameObject;
        }

        public GameObject Reuse(Transform parent)
        {
            var instance = GetInstance();

            instance.transform.SetParent(parent);

            return instance.gameObject;
        }

        public GameObject Reuse(Transform parent, bool worldPositionStays)
        {
            var instance = GetInstance();

            instance.transform.SetParent(parent, worldPositionStays);

            return instance.gameObject;
        }

        public GameObject Reuse(Vector3 position, Quaternion rotation)
        {
            var instance = GetInstance();

            instance.transform.SetPositionAndRotation(position, rotation);

            return instance.gameObject;
        }

        public GameObject Reuse(Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = GetInstance();
            var instanceTransform = instance.transform;

            instanceTransform.SetPositionAndRotation(position, rotation);
            instanceTransform.SetParent(parent);

            return instance.gameObject;
        }

        public void Release(GameObject instance)
        {
            var poolable = instance.GetComponent<Poolable>();
            poolable.OnRelease();

            instance.SetActive(false);

            var instanceTransform = instance.transform;
            instanceTransform.SetParent(null);
            instanceTransform.rotation = _rotation;
            instanceTransform.localScale = _scale;

            _instances.Push(poolable);
        }

        private Poolable GetInstance()
        {
            var count = _instances.Count;

            if (count != 0)
            {
                var instance = _instances.Pop();

                if (instance == null)
                {
                    count--;

                    while (count != 0)
                    {
                        instance = _instances.Pop();

                        if (instance != null)
                        {
                            instance.OnReuse();
                            instance.gameObject.SetActive(true);

                            return instance;
                        }

                        count--;
                    }

                    instance = CreateInstance();
                    instance.gameObject.SetActive(true);

                    return instance;
                }

                instance.OnReuse();
                instance.gameObject.SetActive(true);

                return instance;
            }
            else
            {
                var instance = CreateInstance();
                instance.gameObject.SetActive(true);

                return instance;
            }
        }

        private Poolable CreateInstance()
        {
            var instance = Object.Instantiate(_prototype);
            var instanceGameObject = instance.gameObject;
            
            _instanceLookup.Add(instanceGameObject, this);
            _allInstances.Add(instanceGameObject);

            return instance;
        }
    }
}
