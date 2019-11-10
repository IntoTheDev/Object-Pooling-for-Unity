using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	[System.Serializable]
	public class Pool
	{
		[SerializeField] private Poolable prefab = null;
		[SerializeField] private int startCount = 0;
		[SerializeField] private bool isResizable = false;
		[SerializeField] private Transform holder = null;

		private Queue<Poolable> entities = null;
		private bool isFilled = false;
		private int currentCount = 0;

		public Pool(Poolable prefab, int startCount, bool isResizable, Transform holder)
		{
			this.prefab = prefab;
			this.startCount = startCount;
			this.isResizable = isResizable;
			this.holder = holder;
		}

		public void Fill()
		{
			if (isFilled)
				return;

			entities = new Queue<Poolable>(startCount);
			currentCount = startCount;

			for (int i = 0; i < startCount; i++)
			{
				Poolable entity = Object.Instantiate(prefab, holder);
				entity.SetPool(this);
				entities.Enqueue(entity);
				entity.gameObject.SetActive(false);
			}

			isFilled = true;
		}

		public Poolable GetEntity()
		{
			Poolable entity = TakeEntity();

			if (IsEmpty(entity))
				return null;

			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();

			if (IsEmpty(entity))
				return null;

			entity.transform.SetParent(parent, spawnInWorldSpace);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation)
		{
			Poolable entity = TakeEntity();

			if (IsEmpty(entity))
				return null;

			entity.transform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();

			if (IsEmpty(entity))
				return null;

			Transform entityTransform = entity.transform;

			entityTransform.SetParent(parent, spawnInWorldSpace);
			entityTransform.SetPositionAndRotation(position, rotation);
			entity.ReturnFromPool();

			return entity;
		}

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
			if (!isFilled)
				Fill();

			Poolable entity;

			if (currentCount == 0)
			{
				if (isResizable)
				{
					entity = Object.Instantiate(prefab, holder);
					entity.SetPool(this);
					return entity;
				}
				else
				{
					return null;
				}
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

		private bool IsEmpty(Poolable entity) => !isResizable && entity == null;
	}
}
