namespace Speak
{
    /// <summary>
    /// The <see cref="SpeechController"/> class is responsible for managing the speech recognition listener.
    /// It initializes the appropriate listener based on the settings, and handles starting, stopping, and disposing of the listener.
    /// </summary>
    internal static class SpeechController
    {
        private static IListener _listener;

        /// <summary>
        /// Static constructor to initialize the <see cref="SpeechController"/> class.
        /// It sets the listener and subscribes to the settings saved event.
        /// </summary>
        static SpeechController()
        {
            SetListener();
            General.Saved += OnSettingsSaved;
        }

        /// <summary>
        /// Sets the listener based on the current settings.
        /// It creates an instance of the appropriate listener type.
        /// </summary>
        private static void SetListener()
        {
            _listener?.Dispose();
            _listener = General.Instance.ListenerType switch
            {
                ListenerTypes.WinRT => new ListenerWin(),
                ListenerTypes.SystemSpeech => new ListenerSystem(),
                _ => throw new ArgumentOutOfRangeException(nameof(General.Instance.ListenerType), "Unsupported listener type")
            };
        }

        /// <summary>
        /// Event handler for when the settings are saved.
        /// It updates the listener if the listener type has changed.
        /// </summary>
        /// <param name="settings">The updated settings.</param>
        private static void OnSettingsSaved(General settings)
        {
            if (settings.ListenerType != _listener.Type)
            {
                SetListener();
            }
        }

        /// <summary>
        /// Starts the speech recognition listener.
        /// </summary>
        public static async Task StartAsync()
        {
            await _listener?.StartAsync();
        }

        /// <summary>
        /// Stops the speech recognition listener.
        /// </summary>
        public static async Task StopAsync()
        {
            await _listener?.StopAsync();
        }

        /// <summary>
        /// Disposes the speech recognition listener and unsubscribes from the settings saved event.
        /// </summary>
        public static void Dispose()
        {
            _listener?.Dispose();
            General.Saved -= OnSettingsSaved;
        }
    }
}
