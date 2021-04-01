#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		private IPoolable[] _poolables = new IPoolable[0];

		private void Awake() =>
			_poolables = GetComponentsInChildren<IPoolable>(true);

#if ODIN_INSPECTOR
		[Button]
#endif
		public void ReturnToPool()
		{
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnDespawn();
		}

		public void ReturnFromPool()
		{
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnSpawn();
		}
	}
}
