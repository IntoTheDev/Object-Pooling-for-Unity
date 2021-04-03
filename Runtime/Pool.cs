using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	public sealed class Pool
	{
		private Poolable _prefab = null;
		private Stack<Poolable> _entities = null;

		private static Dictionary<int, Pool> _prefabLookup = new Dictionary<int, Pool>();
		private static Dictionary<int, Pool> _instanceLookup = new Dictionary<int, Pool>();

		public Pool(GameObject prefab)
		{
			_prefab = prefab.GetComponent<Poolable>();

			if (_prefab == null)
			{
				_prefab = Object.Instantiate(prefab).AddComponent<Poolable>();
				Object.DontDestroyOnLoad(_prefab);
				_prefab.gameObject.SetActive(false);
			}

			_entities = new Stack<Poolable>();
			_prefabLookup.Add(prefab.GetHashCode(), this);
		}

		public static Pool GetPrefabPool(GameObject prefab)
		{
			bool hasPool = _prefabLookup.TryGetValue(prefab.GetHashCode(), out var pool);

			if (!hasPool)
				pool = new Pool(prefab);

			return pool;
		}

		public static Pool GetInstancePool(GameObject instance)
		{
			_instanceLookup.TryGetValue(instance.GetHashCode(), out var pool);
			return pool;
		}

		public void Populate(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var entity = Object.Instantiate(_prefab);
				_entities.Push(entity);
				entity.gameObject.SetActive(false);
			}
		}

		public GameObject Get()
		{
			var entity = GetEntityFromPool();

			return entity.gameObject;
		}

		public GameObject Get(Transform parent, bool spawnInWorldSpace)
		{
			var entity = GetEntityFromPool();

			entity.transform.SetParent(parent, spawnInWorldSpace);

			return entity.gameObject;
		}

		public GameObject Get(Vector3 position, Quaternion rotation)
		{
			var entity = GetEntityFromPool();

			entity.transform.SetPositionAndRotation(position, rotation);

			return entity.gameObject;
		}

		public GameObject Get(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			var entity = GetEntityFromPool();
			var entityTransform = entity.transform;

			entityTransform.SetParent(parent, spawnInWorldSpace);
			entityTransform.SetPositionAndRotation(position, rotation);

			return entity.gameObject;
		}

		public T Get<T>() where T : Component =>
			Get().GetComponent<T>();

		public T Get<T>(Transform parent, bool spawnInWorldSpace) where T : Component =>
			Get(parent, spawnInWorldSpace).GetComponent<T>();

		public T Get<T>(Vector3 position, Quaternion rotation) where T : Component =>
			Get(position, rotation).GetComponent<T>();

		public T Get<T>(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) where T : Component =>
			Get(position, rotation, parent, spawnInWorldSpace).GetComponent<T>();

		public void Release(Poolable entity)
		{
			_entities.Push(entity);

			entity.transform.SetParent(null, false);
			entity.gameObject.SetActive(false);
			entity.OnRelease();
		}

		private Poolable GetEntityFromPool()
		{
			if (_entities.Count == 0)
			{
				var entity = Object.Instantiate(_prefab);
				_instanceLookup.Add(entity.gameObject.GetHashCode(), this);
				entity.gameObject.SetActive(true);

				return entity;
			}
			else
			{
				var entity = _entities.Pop();

				if (entity == null)
				{
					entity = Object.Instantiate(_prefab);
					_instanceLookup.Add(entity.gameObject.GetHashCode(), this);
				}
				else
				{
					entity.OnGet();
				}

				entity.gameObject.SetActive(true);

				return entity;
			}
		}
	}
}
