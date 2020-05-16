using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	[System.Serializable]
	public class Pool
	{
		[SerializeField, AssetsOnly, ValueDropdown(nameof(GetPoolables))] private Poolable prefab = null;
		[SerializeField] private int startCount = 0;
		[SerializeField, SceneObjectsOnly] private Transform holder = null;
		[SerializeField, ReadOnly] private int currentCount = 0;

		private Queue<Poolable> entities = null;

		public Pool(Poolable prefab, int startCount, Transform holder)
		{
			this.prefab = prefab;
			this.startCount = startCount;
			this.holder = holder;
		}

		private IEnumerable<Poolable> GetPoolables() =>
			Resources.FindObjectsOfTypeAll<Poolable>();

		public void Fill()
		{
			entities = new Queue<Poolable>(startCount);
			currentCount = startCount;

			Poolable original = CreateObject(prefab);

			for (int i = 0; i < startCount - 1; i++)
				CreateObject(original);

			Poolable CreateObject(Poolable prototype)
			{
				Poolable entity = Object.Instantiate(prototype, holder);
				entity.SetPool(this);
				entities.Enqueue(entity);
				entity.gameObject.SetActive(false);

				return entity;
			}
		}

		public Poolable GetEntity()
		{
			Poolable entity = TakeEntity();
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();

			entity.transform.SetParent(parent, spawnInWorldSpace);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation)
		{
			Poolable entity = TakeEntity();

			entity.transform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();
			Transform entityTransform = entity.transform;

			entityTransform.SetParent(parent, spawnInWorldSpace);
			entityTransform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

		public T GetEntity<T>() where T : Component =>
			GetEntity().Component as T;

		public T GetEntity<T>(Transform parent, bool spawnInWorldSpace) where T : Component =>
			GetEntity(parent, spawnInWorldSpace).Component as T;

		public T GetEntity<T>(Vector3 position, Quaternion rotation) where T : Component =>
			GetEntity(position, rotation).Component as T;

		public T GetEntity<T>(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) where T : Component =>
			GetEntity(position, rotation, parent, spawnInWorldSpace).Component as T;

		public void ReturnEntity(Poolable entity)
		{
			if (entity.Pool != this)
				return;

			entities.Enqueue(entity);
			currentCount++;

			entity.transform.SetParent(holder, false);
			entity.gameObject.SetActive(false);
		}

		private Poolable TakeEntity()
		{
			Poolable entity;

			if (currentCount == 0)
			{
				entity = Object.Instantiate(prefab, holder);
				entity.SetPool(this);
				return entity;
			}

			entity = entities.Dequeue();

			if (entity == null)
			{
				entity = Object.Instantiate(prefab, holder);
				entity.SetPool(this);
				currentCount++;
			}

			entity.gameObject.SetActive(true);
			currentCount--;

			return entity;
		}
	}
}
