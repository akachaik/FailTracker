namespace FailTracker.Web.Infrastructure.Tasks
{
    public interface IRunAtInit
    {
        void Execute();
    }

    public interface IRunAtStartup
    {
        void Execute();
    }

    public interface IRunOnEachRequest
    {
        void Execute();
    }
    public interface IRunOnError
    {
        void Execute();
    }


    public interface IRunAfterEachRequest
    {
        void Execute();
    }


}
