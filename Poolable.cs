using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using ToolBox.Reactors;
using UnityEngine;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour, IReactor
	{
		[SerializeField, TabGroup("Callbacks")] private Reactor _onBackToPool = null;
		[SerializeField, TabGroup("Callbacks")] private Reactor _onBackFromPool = null;

		[SerializeField, TabGroup("Poolables"), OnValueChanged(nameof(OnPoolablesChange)), ValueDropdown(nameof(GetPoolables)), HideInPlayMode] 
		private MonoBehaviour[] _possiblePoolables = null;

		[ShowInInspector, HideInEditorMode, TabGroup("Poolables"), ReadOnly]
		private IPoolable[] _poolables = null;

		public Pool Pool { get; private set; } = null;

		private bool _isPooled = false;
		private bool _isEnabled = true;

		private void Awake()
		{
			_onBackToPool.Setup();
			_onBackFromPool.Setup();

			int count = _possiblePoolables.Length;
			_poolables = new IPoolable[count];

			for (int i = 0; i < count; i++)
				_poolables[i] = _possiblePoolables[i] as IPoolable;

			_possiblePoolables = null;
		}

		[Button]
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
			for (int i = 0; i < _poolables.Length; i++)
				_poolables[i].Reset();

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

		private void OnPoolablesChange()
		{
			IEnumerable<MonoBehaviour> poolables = _possiblePoolables.Where(x => x is IPoolable);
			poolables = poolables.GroupBy(x => x.GetHashCode()).Select(y => y.First());
			_possiblePoolables = poolables.ToArray();
		}

		private IEnumerable<IPoolable> GetPoolables() =>
			GetComponentsInChildren<IPoolable>(true);
	}
}
