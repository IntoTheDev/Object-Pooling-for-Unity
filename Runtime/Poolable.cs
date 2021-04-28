using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	internal sealed class Poolable : MonoBehaviour
	{
		private IPoolable[] _poolables = new IPoolable[0];

		private void Awake() =>
			_poolables = GetComponentsInChildren<IPoolable>(true);

		public void OnGet()
		{
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnGet();
		}

		public void OnRelease()
		{
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].OnRelease();
		}
	}
}
