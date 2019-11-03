using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	[System.Serializable]
	public struct Pool
	{
		[SerializeField] private Poolable prefab;
		[SerializeField] private int startCount;
		[SerializeField] private bool isResizable;
		[SerializeField] private Transform parent;

		private Queue<Poolable> entities;
		private int currentCount;

		public void Fill()
		{
			entities = new Queue<Poolable>(startCount);
			currentCount = startCount;

			for (int i = 0; i < startCount; i++)
			{
				Poolable entity = Object.Instantiate(prefab, parent.localPosition, Quaternion.identity, parent);
				entity.pool = this;
				entities.Enqueue(entity);
				entity.gameObject.SetActive(false);
			}
		}

		public Poolable GetEntity()
		{
			Poolable entity = TakeEntity();

			return entity;
		}

		public void ReturnEntity(Poolable entity)
		{
			entities.Enqueue(entity);

			entity.transform.SetParent(parent, false);
			entity.gameObject.SetActive(false);

			currentCount++;
		}

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

		private Poolable TakeEntity()
		{
			Poolable entity = null;

			if (currentCount <= 0)
			{
				if (isResizable)
				{
					entity = Object.Instantiate(prefab, parent.localPosition, Quaternion.identity, parent);
					entity.pool = this;
					return entity;
				}
				else
				{
					return null;
				}
			}

			entity = entities.Dequeue();
			entity.gameObject.SetActive(true);

			currentCount--;

			return entity;
		}
	}
}
