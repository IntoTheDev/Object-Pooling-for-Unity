using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	public sealed class Pool
	{
		private readonly Poolable _prefab = null;
		private readonly Stack<Poolable> _instances = null;
		private readonly Quaternion _rotation = default;
		private readonly Vector3 _scale = default;

		private static readonly Dictionary<int, Pool> _prefabLookup = new Dictionary<int, Pool>();
		private static readonly Dictionary<int, Pool> _instanceLookup = new Dictionary<int, Pool>();

		public Pool(GameObject prefab)
		{
			_prefab = prefab.GetComponent<Poolable>();

			if (_prefab == null)
			{
				_prefab = Object.Instantiate(prefab).AddComponent<Poolable>();
				Object.DontDestroyOnLoad(_prefab);
				_prefab.gameObject.SetActive(false);
			}

			_instances = new Stack<Poolable>();
			_prefabLookup.Add(prefab.GetHashCode(), this);

			var transform = prefab.transform;
			_rotation = transform.rotation;
			_scale = transform.localScale;
		}

		public static Pool GetPrefabPool(GameObject prefab)
		{
			bool hasPool = _prefabLookup.TryGetValue(prefab.GetHashCode(), out var pool);

			if (!hasPool)
				pool = new Pool(prefab);

			return pool;
		}

		public static bool TryGetInstancePool(GameObject instance, out Pool pool) =>
			_instanceLookup.TryGetValue(instance.GetHashCode(), out pool);

		public void Populate(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var instance = CreateInstance();
				_instances.Push(instance);
			}
		}

		public GameObject Get() =>
			GetInstance().gameObject;

		public GameObject Get(Transform parent)
		{
			var instance = GetInstance();

			instance.transform.SetParent(parent);

			return instance.gameObject;
		}

		public GameObject Get(Transform parent, bool worldPositionStays)
		{
			var instance = GetInstance();

			instance.transform.SetParent(parent, worldPositionStays);

			return instance.gameObject;
		}

		public GameObject Get(Vector3 position, Quaternion rotation)
		{
			var instance = GetInstance();

			instance.transform.SetPositionAndRotation(position, rotation);

			return instance.gameObject;
		}

		public GameObject Get(Vector3 position, Quaternion rotation, Transform parent)
		{
			var instance = GetInstance();
			var instanceTransform = instance.transform;

			instanceTransform.SetPositionAndRotation(position, rotation);
			instanceTransform.SetParent(parent);

			return instance.gameObject;
		}

		public void Release(GameObject instance)
		{
			instance.SetActive(false);

			var instanceTransform = instance.transform;
			instanceTransform.SetParent(null);
			instanceTransform.rotation = _rotation;
			instanceTransform.localScale = _scale;

			var poolable = instance.GetComponent<Poolable>();
			poolable.OnRelease();
			_instances.Push(poolable);
		}

		private Poolable GetInstance()
		{
			Poolable instance;

			if (_instances.Count == 0)
			{
				instance = CreateInstance();
			}
			else
			{
				instance = _instances.Pop();

				if (instance == null)
					instance = CreateInstance();
				else
					instance.OnGet();
			}

			instance.gameObject.SetActive(true);
			return instance;
		}

		private Poolable CreateInstance()
		{
			var instance = Object.Instantiate(_prefab);
			_instanceLookup.Add(instance.gameObject.GetHashCode(), this);

			return instance;
		}
	}
}
