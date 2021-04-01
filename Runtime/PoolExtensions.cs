using UnityEngine;

namespace ToolBox.Pools
{
	public static class PoolExtensions
	{
		public static void Populate(this GameObject prefab, int count)
		{
			var pool = Pool.GetPrefabPool(prefab);
			pool.Populate(count);
		}

		public static GameObject Get(this GameObject prefab)
		{
			var pool = Pool.GetPrefabPool(prefab);
			var entity = pool.Get();

			return entity;
		}

		public static GameObject Get(this GameObject prefab, Transform parent, bool spawnInWorldSpace)
		{
			var pool = Pool.GetPrefabPool(prefab);
			var entity = pool.Get(parent, spawnInWorldSpace);

			return entity;
		}

		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			var pool = Pool.GetPrefabPool(prefab);
			var entity = pool.Get(position, rotation);

			return entity;
		}

		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			var pool = Pool.GetPrefabPool(prefab);
			var entity = pool.Get(position, rotation, parent, spawnInWorldSpace);

			return entity;
		}

		public static T Get<T>(this GameObject prefab) where T : Component =>
			prefab.Get().GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Transform parent, bool spawnInWorldSpace) where T : Component =>
			prefab.Get(parent, spawnInWorldSpace).GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Vector3 position, Quaternion rotation) where T : Component =>
			prefab.Get(position, rotation).GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) where T : Component =>
			prefab.Get(position, rotation, parent, spawnInWorldSpace).GetComponent<T>();

		public static void Release(this GameObject instance)
		{
			var pool = Pool.GetInstancePool(instance);

			if (pool == null)
				Object.Destroy(instance);
			else
				pool.Release(instance.GetComponent<Poolable>());
		}
	}
}
