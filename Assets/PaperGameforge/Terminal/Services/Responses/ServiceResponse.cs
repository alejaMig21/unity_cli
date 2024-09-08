namespace Assets.PaperGameforge.Terminal.Services.Responses
{
    public class ServiceResponse
    {
        private string text = string.Empty;
        private bool backgroundProcess = false; // Means the text is not going to be showned on screen

        public string Text { get => text; set => text = value; }
        public bool BackgroundProcess { get => backgroundProcess; set => backgroundProcess = value; }

        public ServiceResponse(string text, bool backgroundProcess)
        {
            this.text = text;
            this.backgroundProcess = backgroundProcess;
        }
    }
}
