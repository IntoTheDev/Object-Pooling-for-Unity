using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Pools
{
	[CreateAssetMenu(menuName = "ToolBox/Pool"), Required, AssetSelector]
    public sealed class Pool : ScriptableObject
    {
		[SerializeField, AssetList, AssetsOnly, Required] private Poolable _prefab = null;
		[SerializeField] private int _startCount = 0;

		private int _currentCount = 0;
		private Queue<Poolable> _entities = null;

		[Button]
		public void Fill()
		{
			_entities = new Queue<Poolable>(_startCount);
			_currentCount = _startCount;

			for (int i = 0; i < _startCount; i++)
			{
				Poolable entity = Instantiate(_prefab);
				entity.SetPool(this);
				_entities.Enqueue(entity);
				entity.gameObject.SetActive(false);
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

		public T GetEntity<T>() =>
			GetEntity().GetComponent<T>();

		public T GetEntity<T>(Transform parent, bool spawnInWorldSpace) =>
			GetEntity(parent, spawnInWorldSpace).GetComponent<T>();

		public T GetEntity<T>(Vector3 position, Quaternion rotation) =>
			GetEntity(position, rotation).GetComponent<T>();

		public T GetEntity<T>(Vector3 position, Quaternion rotation, Transform parent, bool spawnInWorldSpace) =>
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

		private Poolable TakeEntity()
		{
			Poolable entity;

			if (_currentCount == 0)
			{
				entity = Instantiate(_prefab);
				entity.SetPool(this);

				return entity;
			}

			entity = _entities.Dequeue();

			if (entity == null)
			{
				entity = Instantiate(_prefab);
				entity.SetPool(this);
				_currentCount++;
			}

			entity.gameObject.SetActive(true);
			_currentCount--;

			return entity;
		}
	}
}
