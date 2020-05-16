using Sirenix.OdinInspector;
using Sirenix.Serialization;
using ToolBox.Modules;
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : SerializedMonoBehaviour, IModule
	{
		[SerializeField] private Component component = null;
		[OdinSerialize] private ModulesContainer onBackToPool = default;
		[OdinSerialize] private ModulesContainer onBackFromPool = default;

		public Pool Pool { get; private set; } = null;
		public Component Component => component;

		private bool isPooled = false;
		private bool isEnabled = true;

		public void ReturnToPool()
		{
			if (!isEnabled)
				return;

			onBackToPool.Process();

			Pool.ReturnEntity(this);
			isEnabled = false;
		}

		public void ReturnFromPool()
		{
			onBackFromPool.Process();
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

		public void Process() =>
			ReturnToPool();
	}
}
