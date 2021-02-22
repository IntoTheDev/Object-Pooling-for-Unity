using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	public sealed class Pool
	{
		private Poolable _prefab = null;
		private int _currentCount = 0;
		private Queue<Poolable> _entities = null;

		private static Dictionary<int, Pool> _pools = new Dictionary<int, Pool>();

		public Pool(Poolable prefab, int count)
		{
			_prefab = prefab;
			_currentCount = count;
			_entities = new Queue<Poolable>(count);
			_pools.Add(prefab.gameObject.GetHashCode(), this);

			Populate(count);
		}

		public Pool(GameObject prefab, int count)
		{
			_prefab = prefab.GetComponent<Poolable>();

			if (_prefab == null)
			{
				_prefab = Object.Instantiate(prefab).AddComponent<Poolable>();
				UnityEngine.Object.DontDestroyOnLoad(_prefab);
				_prefab.gameObject.SetActive(false);
			}

			_currentCount = count;
			_entities = new Queue<Poolable>(count);
			_pools.Add(prefab.GetHashCode(), this);

			Populate(count);
		}

		public static Pool Get(GameObject prefab)
		{
			var hasPool = _pools.TryGetValue(prefab.GetHashCode(), out var pool);

			if (!hasPool)
				pool = new Pool(prefab, 0);

			return pool;
		}

		public void Populate(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Poolable entity = Object.Instantiate(_prefab);
				entity.SetPool(this);
				_entities.Enqueue(entity);
				entity.gameObject.SetActive(false);
			}

			_currentCount += count;
		}

		public Poolable GetEntity()
		{
			Poolable entity = GetEntityFromPool();
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = GetEntityFromPool();

			entity.transform.SetParent(parent, spawnInWorldSpace);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation)
		{
			Poolable entity = GetEntityFromPool();

			entity.transform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = GetEntityFromPool();
			Transform entityTransform = entity.transform;

			entityTransform.SetParent(parent, spawnInWorldSpace);
			entityTransform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

		public T GetEntity<T>() where T : Component =>
			GetEntity().GetComponent<T>();

		public T GetEntity<T>(Transform parent, bool spawnInWorldSpace) where T : Component =>
			GetEntity(parent, spawnInWorldSpace).GetComponent<T>();

		public T GetEntity<T>(Vector3 position, Quaternion rotation) where T : Component =>
			GetEntity(position, rotation).GetComponent<T>();

		public T GetEntity<T>(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) where T : Component =>
			GetEntity(position, rotation, parent, spawnInWorldSpace).GetComponent<T>();

		public void ReturnEntity(Poolable entity)
		{
			if (entity.Pool != this)
				return;

			_entities.Enqueue(entity);
			_currentCount++;

			entity.transform.SetParent(null, false);
			entity.gameObject.SetActive(false);
		}

		private Poolable GetEntityFromPool()
		{
			Poolable entity;

			if (_currentCount == 0)
			{
				entity = Object.Instantiate(_prefab);
				entity.SetPool(this);
				entity.gameObject.SetActive(true);

				return entity;
			}

			entity = _entities.Dequeue();

			if (entity == null)
			{
				entity = Object.Instantiate(_prefab);
				entity.SetPool(this);
				_currentCount++;
			}

			entity.gameObject.SetActive(true);
			_currentCount--;

			return entity;
		}
	}
}
