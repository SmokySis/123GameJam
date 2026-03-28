using System;

namespace PoolSystem
{
    public interface IReference : IDisposable
    {
        public uint ReferenceType { get; }
        internal int IndexInReferencePool { get; set; }
        public IReference GetNewInstance();
        public void OnRecycle();
    }
    public interface IReference<TReference>:IReference where TReference : IReference, new() { }
}