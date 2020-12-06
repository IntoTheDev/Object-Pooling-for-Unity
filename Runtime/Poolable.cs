#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		public Pool Pool { get; private set; } = null;
		public bool IsPooled { get; private set; } = false;

		private IPoolable[] _poolables = new IPoolable[0];
		private bool _isEnabled = false;

		private void Awake() =>
			_poolables = GetComponentsInChildren<IPoolable>(true);

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
			if (!IsPooled)
			{
				Pool = pool;
				IsPooled = true;
			}
		}
	}
}
