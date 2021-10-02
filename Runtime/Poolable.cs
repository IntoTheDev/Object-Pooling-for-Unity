using UnityEngine;
using System;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	internal sealed class Poolable : MonoBehaviour
	{
		private IPoolable[] _poolables = Array.Empty<IPoolable>();
		private bool _isInitialized = false;
		
		private void Awake()
		{
			_poolables = GetComponentsInChildren<IPoolable>(true);
			_isInitialized = true;
		}

		public void OnGet()
		{
			if (_isInitialized)
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
