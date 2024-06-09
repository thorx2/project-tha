namespace ProjTha
{
    public interface IMonoUpdateHook
    {
        void CustomUpdate();
        void CustomFixedUpdate();
        bool CanUpdate();
        bool CanFixedUpdate();
    }
}