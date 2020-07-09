using Sirenix.OdinInspector;
using System;
using ToolBox.Reactors;
using UnityEngine;

namespace ToolBox.Pools
{
    [CreateAssetMenu(menuName = "ToolBox/Global Pool"), Required, AssetSelector]
    public sealed class GlobalPool : ScriptableObject, ISetupable
    {
        [SerializeField, HideLabel] private Pool _pool = null;
        [ShowInInspector, ReadOnly, NonSerialized] private bool _isFilled = false;

        public Pool Pool => _pool;

        public void Setup()
        {
            if (!_isFilled)
            {
                _pool.Fill();
                _isFilled = true;
            }
        }
    }
}
