namespace Assets.PaperGameforge.Terminal.TEST
{
    public class ServiceError : ServiceResponse
    {
        private int priority = 0;

        public int Priority { get => priority; set => priority = value; }

        public ServiceError(string text, bool backgroundProcess, int priority) : base(text, backgroundProcess)
        {
            this.priority = priority;
        }
    }
}
