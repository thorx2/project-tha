namespace ProjTha
{
    /// <summary>
    /// Simple PubSub object dispatcher which is triggered when a IMonoUpdateHook objects
    /// is created
    /// </summary>
    public class MonoTransport
    {
        public IMonoUpdateHook MonoSpawned;
        public bool IsSpawned;
    }
}