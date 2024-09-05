namespace Assets.PaperGameforge.Terminal.TEST
{
    public class ErrorKey
    {
        private string cmd = "";

        public string Cmd { get => cmd; set => cmd = value; }

        public ErrorKey(string cmd)
        {
            this.cmd = cmd;
        }
    }
}
