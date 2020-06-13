using ToolBox.Reactors;
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour, IReactor
	{
		[SerializeField] private Component component = null;
		[SerializeField] private Reactor onBackToPool = default;
		[SerializeField] private Reactor onBackFromPool = default;

		public Pool Pool { get; private set; } = null;
		public Component Component => component;

		private bool isPooled = false;
		private bool isEnabled = true;

		public void ReturnToPool()
		{
			if (!isEnabled)
				return;

			onBackToPool.SendReaction();

			Pool.ReturnEntity(this);
			isEnabled = false;
		}

		public void ReturnFromPool()
		{
			onBackFromPool.SendReaction();
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

		public void HandleReaction() =>
			ReturnToPool();
	}
}
