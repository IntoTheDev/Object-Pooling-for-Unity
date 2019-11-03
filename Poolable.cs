using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		public Pool Pool { get; private set; } = null;

		private IPoolResetter[] poolResetters = null;
		private IPoolInitializer[] poolInitializers = null;

		private int poolResettersCount = 0;
		private int poolInitializersCount = 0;

		private bool isPooled = false;
		private bool isEnabled = true;

		private void Awake()
		{
			poolResetters = GetComponentsInChildren<IPoolResetter>();
			poolInitializers = GetComponentsInChildren<IPoolInitializer>();
			poolResettersCount = poolResetters.Length;
			poolInitializersCount = poolInitializers.Length;
		}

		public void ReturnToPool()
		{
			if (!isEnabled)
				return;

			for (int i = 0; i < poolResettersCount; i++)
				poolResetters[i].Reset();

			Pool.ReturnEntity(this);
			isEnabled = false;
		}

		public void ReturnFromPool()
		{
			for (int i = 0; i < poolInitializersCount; i++)
				poolInitializers[i].Initialize();

			isEnabled = true;
		}

		public void SetPool(Pool pool)
		{
			if (!isPooled)
			{
				Pool = pool;
				isPooled = true;
			}
		}
	}
}
