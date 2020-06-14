using Sirenix.OdinInspector;
using System.Collections.Generic;
using ToolBox.Reactors;
using UnityEngine;

namespace ToolBox.Pools
{
	[System.Serializable]
	public class Pool
	{
		[SerializeField, AssetsOnly, ValueDropdown(nameof(GetPoolables))] private Poolable _prefab = null;
		[SerializeField] private int _startCount = 0;
		[SerializeField, SceneObjectsOnly] private Transform _holder = null;
		[SerializeField] private GameObjectReactor _objectInitializator = null; 

		private int _currentCount = 0;
		private Queue<Poolable> _entities = null;

		public Pool(Poolable prefab, int startCount, Transform holder, GameObjectReactor objectInitializator)
		{
			_prefab = prefab;
			_startCount = startCount;
			_holder = holder;
			_objectInitializator = objectInitializator;
		}

		private IEnumerable<Poolable> GetPoolables() =>
			Resources.FindObjectsOfTypeAll<Poolable>();

		public void Fill()
		{
			_entities = new Queue<Poolable>(_startCount);
			_currentCount = _startCount;

			Poolable original = Object.Instantiate(_prefab, _holder);

			if (_objectInitializator != null)
				_objectInitializator.SendReaction(original.gameObject);

			AddToPool(original);

			for (int i = 0; i < _startCount - 1; i++)
			{
				Poolable entity = Object.Instantiate(original, _holder);
				AddToPool(entity);
			}

			void AddToPool(Poolable newEntity)
			{
				newEntity.SetPool(this);
				_entities.Enqueue(newEntity);
				newEntity.gameObject.SetActive(false);
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

			_entities.Enqueue(entity);
			_currentCount++;

			entity.transform.SetParent(_holder, false);
			entity.gameObject.SetActive(false);
		}

		private Poolable TakeEntity()
		{
			Poolable entity;

			if (_currentCount == 0)
			{
				entity = Object.Instantiate(_prefab, _holder);
				entity.SetPool(this);

				return entity;
			}

			entity = _entities.Dequeue();

			if (entity == null)
			{
				entity = Object.Instantiate(_prefab, _holder);
				entity.SetPool(this);
				_currentCount++;
			}

			entity.gameObject.SetActive(true);
			_currentCount--;

			return entity;
		}
	}
}
