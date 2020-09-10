#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		[SerializeField] private Pool _pool = null;

		public Pool Pool { get; private set; } = null;

		private IPoolable[] _poolables = null;
		private bool _isPooled = false;
		private bool _isEnabled = true;

		private void Awake()
		{
			if (_pool != null)
				SetPool(_pool);

			_poolables = GetComponentsInChildren<IPoolable>(true);
		}

#if ODIN_INSPECTOR
		[Button]
#endif
		public void ReturnToPool()
		{
			if (!_isEnabled)
				return;

			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnDespawn();

			Pool.ReturnEntity(this);
			_isEnabled = false;
		}

		public void ReturnFromPool()
		{
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnSpawn();

			_isEnabled = true;
		}

		public void SetPool(Pool pool)
		{
			if (!_isPooled)
			{
				Pool = pool;
				_isPooled = true;
			}
		}
	}
}
