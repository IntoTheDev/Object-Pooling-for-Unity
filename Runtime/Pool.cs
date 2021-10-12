using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
    internal sealed class Pool
    {
        private readonly Poolable _prefab = null;
        private readonly Stack<Poolable> _instances = null;
        private readonly Quaternion _rotation = default;
        private readonly Vector3 _scale = default;

        private static readonly Dictionary<GameObject, Pool> _prefabLookup = new Dictionary<GameObject, Pool>(64);
        private static readonly Dictionary<GameObject, Pool> _instanceLookup = new Dictionary<GameObject, Pool>(512);

        private const int INITIAL_SIZE = 128;

        public Pool(GameObject prefab)
        {
            _prefab = prefab.GetComponent<Poolable>();

            if (_prefab == null)
            {
                _prefab = Object.Instantiate(prefab).AddComponent<Poolable>();
                Object.DontDestroyOnLoad(_prefab);
                _prefab.gameObject.SetActive(false);
            }
            
            _instances = new Stack<Poolable>(INITIAL_SIZE);
            _prefabLookup.Add(prefab, this);

            var transform = prefab.transform;
            _rotation = transform.rotation;
            _scale = transform.localScale;
        }

        public static Pool GetPrefabPool(GameObject prefab)
        {
            bool hasPool = _prefabLookup.TryGetValue(prefab, out var pool);

            if (!hasPool)
                pool = new Pool(prefab);

            return pool;
        }

        public static bool TryGetInstancePool(GameObject instance, out Pool pool)
        {
            return _instanceLookup.TryGetValue(instance, out pool);
        }

        public void Populate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var instance = CreateInstance();
                instance.gameObject.SetActive(false);
                _instances.Push(instance);
            }
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
            int count = _instances.Count;

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
            var instance = Object.Instantiate(_prefab);
            var instanceGameObject = instance.gameObject;
            _instanceLookup.Add(instanceGameObject, this);

            return instance;
        }
    }
}
