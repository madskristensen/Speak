namespace Speak
{
    /// <summary>
    /// Specifies the types of listeners available in the Speak application.
    /// </summary>
    public enum ListenerTypes
    {
        /// <summary>
        /// Listener that uses Windows Runtime (WinRT) APIs.
        /// </summary>
        WinRT,

        /// <summary>
        /// Listener that uses the System.Speech API.
        /// </summary>
        SystemSpeech,
    }
}
