#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ToolBox.Pools
{
	[DefaultExecutionOrder(-9999)]
	public class PoolFiller : MonoBehaviour
	{
#if ODIN_INSPECTOR
		[AssetList(AutoPopulate = true)]
#endif
		[SerializeField] private Pool[] _pools = null;

		private void Awake()
		{
			for (int i = 0; i < _pools.Length; i++)
				_pools[i].Fill();
		}
	}
}
