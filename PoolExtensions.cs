using UnityEngine;

namespace ToolBox.Pools
{
	public static class PoolExtensions
	{
		public static void Populate(this GameObject prefab, int count)
		{
			var pool = Pool.Get(prefab);
			pool.Populate(count);
		}

		public static Poolable Spawn(this GameObject prefab)
		{
			var pool = Pool.Get(prefab);
			var entity = pool.GetEntity();

			return entity;
		}

		public static Poolable Spawn(this GameObject prefab, Transform parent, bool spawnInWorldSpace)
		{
			var pool = Pool.Get(prefab);
			var entity = pool.GetEntity(parent, spawnInWorldSpace);

			return entity;
		}

		public static Poolable Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			var pool = Pool.Get(prefab);
			var entity = pool.GetEntity(position, rotation);

			return entity;
		}

		public static Poolable Spawn(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			var pool = Pool.Get(prefab);
			var entity = pool.GetEntity(position, rotation, parent, spawnInWorldSpace);

			return entity;
		}

		public static T Spawn<T>(this GameObject prefab) where T : Component =>
			prefab.Spawn().GetComponent<T>();

		public static T Spawn<T>(this GameObject prefab, Transform parent, bool spawnInWorldSpace) where T : Component =>
			prefab.Spawn(parent, spawnInWorldSpace).GetComponent<T>();

		public static T Spawn<T>(this GameObject prefab, Vector3 position, Quaternion rotation) where T : Component =>
			prefab.Spawn(position, rotation).GetComponent<T>();

		public static T Spawn<T>(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) where T : Component =>
			prefab.Spawn(position, rotation, parent, spawnInWorldSpace).GetComponent<T>();

		/// <summary>
		/// Use this method only with Instances of Prefab
		/// </summary>
		public static void Despawn(this Poolable instance) =>
			instance.ReturnToPool();

		/// <summary>
		/// Use this method only with Instances of Prefab
		/// </summary>
		public static void Despawn(this GameObject instance) =>
			instance.GetComponent<Poolable>().ReturnToPool();

		/// <summary>
		/// Use this method only with Instances of Prefab
		/// </summary>
		public static void Despawn(this Transform instance) =>
			instance.GetComponent<Poolable>().ReturnToPool();
	}
}
