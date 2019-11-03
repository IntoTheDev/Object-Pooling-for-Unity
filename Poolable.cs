using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		public Pool pool { private get; set; } = default;

		private IPoolable[] poolables = null;
		private int count = 0;

		private void Awake()
		{
			poolables = GetComponentsInChildren<IPoolable>();
			count = poolables.Length;
		}

		public void Return()
		{
			for (int i = 0; i < count; i++)
				poolables[i].Reset();

			pool.ReturnEntity(this);
		}
	}
}
