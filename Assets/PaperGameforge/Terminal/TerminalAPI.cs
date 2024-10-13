namespace Assets.PaperGameforge.Terminal
{
    public static class TerminalAPI
    {
        public static Terminal WorkingTerminal { get; set; } = null;

        public static void RunCommandRemotely(string userInput)
        {
            if (WorkingTerminal == null)
            {
                return;
            }

            WorkingTerminal.MainBehavior(userInput);
        }
    }
}