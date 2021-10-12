using UnityEngine;

namespace ToolBox.Pools
{
	public static class PoolHelper
	{
		public static void Populate(this GameObject prefab, int count)
		{
			Pool.GetPrefabPool(prefab).Populate(count);
		}

		public static GameObject Reuse(this GameObject prefab)
		{
			return Pool.GetPrefabPool(prefab).Reuse();
		}

		public static GameObject Reuse(this GameObject prefab, Transform parent)
		{
			return Pool.GetPrefabPool(prefab).Reuse(parent);
		}

		public static GameObject Reuse(this GameObject prefab, Transform parent, bool worldPositionStays)
		{
			return Pool.GetPrefabPool(prefab).Reuse(parent, worldPositionStays);
		}

		public static GameObject Reuse(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return Pool.GetPrefabPool(prefab).Reuse(position, rotation);
		}

		public static GameObject Reuse(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
		{
			return Pool.GetPrefabPool(prefab).Reuse(position, rotation, parent);
		}

		public static T Reuse<T>(this GameObject prefab) where T : Component
		{
			return prefab.Reuse().GetComponent<T>();
		}

		public static T Reuse<T>(this GameObject prefab, Transform parent) where T : Component
		{
			return prefab.Reuse(parent).GetComponent<T>();
		}

		public static T Reuse<T>(this GameObject prefab, Transform parent, bool worldPositionStays) where T : Component
		{
			return prefab.Reuse(parent, worldPositionStays).GetComponent<T>();
		}

		public static T Reuse<T>(this GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			return prefab.Reuse(position, rotation).GetComponent<T>();
		}

		public static T Reuse<T>(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
		{
			return prefab.Reuse(position, rotation, parent).GetComponent<T>();
		}

		public static void Release(this GameObject instance)
		{
			bool isPooled = Pool.TryGetInstancePool(instance, out var pool);

			if (isPooled)
			{
				pool.Release(instance);
			}
			else
			{
				Object.Destroy(instance);
			}
		}
	}
}
