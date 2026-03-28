namespace AudioSystem
{
    public struct AudioHandle
    {
        internal string id;
        internal int token;
        internal PooledAudioSource pooled;
        public bool IsValid => pooled != null && pooled.Token == token;
    }
}