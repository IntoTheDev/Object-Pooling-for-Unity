using Sirenix.OdinInspector;
using Sirenix.Serialization;
using ToolBox.Observer;
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : SerializedMonoBehaviour, IReactor
	{
		[SerializeField] private Component component = null;
		[OdinSerialize] private IReactor[] onBackToPool = null;
		[OdinSerialize] private IReactor[] onBackFromPool = null;

		public Pool Pool { get; private set; } = null;
		public Component Component => component;

		private bool isPooled = false;
		private bool isEnabled = true;

		public void ReturnToPool()
		{
			if (!isEnabled)
				return;

			onBackToPool.Dispatch();

			Pool.ReturnEntity(this);
			isEnabled = false;
		}

		public void ReturnFromPool()
		{
			onBackFromPool.Dispatch();
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
