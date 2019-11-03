using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	[System.Serializable]
	public class Pool
	{
		[SerializeField] private Poolable prefab;
		[SerializeField] private int startCount;
		[SerializeField] private bool isResizable;
		[SerializeField] private Transform parent;

		private Queue<Poolable> entities;
		private int currentCount;
		private bool isFilled;

		public void Fill()
		{
			if (isFilled)
				return;

			entities = new Queue<Poolable>(startCount);
			currentCount = startCount;

			for (int i = 0; i < startCount; i++)
			{
				Poolable entity = Object.Instantiate(prefab, parent.localPosition, Quaternion.identity, parent);
				entity.SetPool(this);
				entities.Enqueue(entity);
				entity.gameObject.SetActive(false);
			}

			isFilled = true;
		}

		public Poolable GetEntity() => TakeEntity();

		public Poolable GetEntity(Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();

			entity.transform.SetParent(parent, spawnInWorldSpace);

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation)
		{
			Poolable entity = TakeEntity();

			entity.transform.SetPositionAndRotation(position, rotation);

			return entity;
		}

		public Poolable GetEntity(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace)
		{
			Poolable entity = TakeEntity();

			Transform entityTransform = entity.transform;
			entityTransform.SetPositionAndRotation(position, rotation);
			entityTransform.SetParent(parent, spawnInWorldSpace);

			return entity;
		}

		public void ReturnEntity(Poolable entity)
		{
			if (entity.Pool != this)
				return;

			entities.Enqueue(entity);

			entity.transform.SetParent(parent, false);
			entity.gameObject.SetActive(false);

			currentCount++;
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
					entity = Object.Instantiate(prefab, parent.localPosition, Quaternion.identity, parent);
					entity.SetPool(this);
					entity.ReturnFromPool();
					return entity;
				}
				else
				{
					return null;
				}
			}

			entity = entities.Dequeue();
			entity.gameObject.SetActive(true);
			entity.ReturnFromPool();

			currentCount--;

			return entity;
		}
	}
}
