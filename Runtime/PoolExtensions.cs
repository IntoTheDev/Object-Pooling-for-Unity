using UnityEngine;

namespace ToolBox.Pools
{
	public static class PoolExtensions
	{
		public static void Populate(this GameObject prefab, int count) =>
			Pool.GetPrefabPool(prefab).Populate(count);

		public static GameObject Get(this GameObject prefab) =>
			Pool.GetPrefabPool(prefab).Get();

		public static GameObject Get(this GameObject prefab, Transform parent) =>
			Pool.GetPrefabPool(prefab).Get(parent);

		public static GameObject Get(this GameObject prefab, Transform parent, bool worldPositionStays) =>
			Pool.GetPrefabPool(prefab).Get(parent, worldPositionStays);

		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation) =>
			Pool.GetPrefabPool(prefab).Get(position, rotation);

		public static GameObject Get(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) =>
			Pool.GetPrefabPool(prefab).Get(position, rotation, parent);

		public static T Get<T>(this GameObject prefab) where T : Component =>
			prefab.Get().GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Transform parent) where T : Component =>
			prefab.Get(parent).GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Transform parent, bool spawnInWorldSpace) where T : Component =>
			prefab.Get(parent, spawnInWorldSpace).GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Vector3 position, Quaternion rotation) where T : Component =>
			prefab.Get(position, rotation).GetComponent<T>();

		public static T Get<T>(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component =>
			prefab.Get(position, rotation, parent).GetComponent<T>();

		public static void Release(this GameObject instance)
		{
			bool isPooled = Pool.TryGetInstancePool(instance, out var pool);

			if (isPooled)
				pool.Release(instance);
			else
				Object.Destroy(instance);
		}
	}
}
