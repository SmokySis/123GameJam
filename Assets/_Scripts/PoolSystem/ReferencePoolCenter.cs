using UnityEngine;
using Utility;
using System;
namespace PoolSystem
{
    public sealed class ReferencePoolCenter : Singleton<ReferencePoolCenter>
    {
        protected override bool _isDonDestroyOnLoad => true;
        private ReferencePool[] _referencePools = new ReferencePool[ReferenceTypes.REFERENCE_TYPE_COUNT];
        /// <summary>
        /// 获取池化对象
        /// </summary>
        /// <typeparam name="TReference">
        /// 池化对象类型
        /// </typeparam>
        /// <returns></returns>
        public TReference GetReference<TReference>() where TReference : IReference<TReference>, new()
        {
            int index = ReferenceTypes.GetReferenceTypeIndex<TReference>();
            if (index == -1)
            {
                Debug.LogError($"ReferencePool GetReference Error:Cant Find {typeof(TReference).Name} In ReferencePool");
                return default;
            }
            if (_referencePools[index] == null)
            {
                ReferencePool referencePool = new ReferencePool();
                referencePool.Init<TReference>();
                _referencePools[index] = referencePool;
            }
            return _referencePools[index].GetReference<TReference>();
        }
        /// <summary>
        /// 归还池化对象
        /// </summary>
        /// <typeparam name="TReference">
        /// 池化对象类型
        /// </typeparam>
        /// <param name="reference"></param>
        public void ReleaseReference<TReference>(TReference reference) where TReference : IReference<TReference>, new()
        {
            int index = ReferenceTypes.GetReferenceTypeIndex<TReference>();
            if (index == -1)
            {
                Debug.LogError($"ReferencePool ReleaseReference Error:Cant Find {typeof(TReference).Name} In ReferencePool");
                return;
            }
            if (_referencePools[index] == null)
            {
                Debug.LogError($"ReferencePool ReleaseReference Error:Cant Find ReferencePool");
                return;
            }
            _referencePools[index].ReleaseReference(reference);
        }
        public void OnDestroy()
        {
            foreach (var temp in _referencePools)
                temp?.OnDestroy();
            Array.Clear(_referencePools, 0, _referencePools.Length);
            _referencePools = null;
        }
    }
}
