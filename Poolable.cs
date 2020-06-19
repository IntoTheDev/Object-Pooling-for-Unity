using ToolBox.Reactors;
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour, IReactor
	{
		[SerializeField] private Component _component = null;
		[SerializeField] private Reactor _onBackToPool = null;
		[SerializeField] private Reactor _onBackFromPool = null;

		public Pool Pool { get; private set; } = null;
		public Component Component => _component;

		private bool _isPooled = false;
		private bool _isEnabled = true;

		private void Awake()
		{
			_onBackToPool.Setup();
			_onBackFromPool.Setup();
		}

		public void ReturnToPool()
		{
			if (!_isEnabled)
				return;

			_onBackToPool.SendReaction();

			Pool.ReturnEntity(this);
			_isEnabled = false;
		}

		public void ReturnFromPool()
		{
			_onBackFromPool.SendReaction();
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

		public void HandleReaction() =>
			ReturnToPool();
	}
}
