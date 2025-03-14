namespace Speak
{
    /// <summary>
    /// The <see cref="SpeechController"/> class is responsible for managing the speech recognition listener.
    /// It initializes the appropriate listener based on the settings, and handles starting, stopping, and disposing of the listener.
    /// This class ensures that the correct listener is used according to the user's configuration, and it manages the lifecycle of the listener.
    /// </summary>
    internal static class SpeechController
    {
        private static IListener _listener;

        /// <summary>
        /// Static constructor to initialize the <see cref="SpeechController"/> class.
        /// It sets the listener and subscribes to the settings saved event.
        /// This constructor ensures that the listener is correctly initialized when the class is first accessed, and it listens for any changes in settings to update the listener accordingly.
        /// </summary>
        static SpeechController()
        {
            SetListener();
            General.Saved += OnSettingsSaved;
        }

        /// <summary>
        /// Sets the listener based on the current settings.
        /// It creates an instance of the appropriate listener type.
        /// This method ensures that the correct listener is instantiated based on the user's configuration, and it disposes of any existing listener to free up resources.
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
        /// This method ensures that the listener is updated to match the new settings, maintaining the correct behavior of the application.
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
        /// This method initiates the listener to begin recognizing speech commands, enabling the application's voice command functionality.
        /// </summary>
        public static async Task StartAsync()
        {
            await _listener?.StartAsync();
        }

        /// <summary>
        /// Stops the speech recognition listener.
        /// This method stops the listener from recognizing speech commands, which can be useful when the application no longer needs to listen for voice commands.
        /// </summary>
        public static async Task StopAsync()
        {
            await _listener?.StopAsync();
        }

        /// <summary>
        /// Disposes the speech recognition listener and unsubscribes from the settings saved event.
        /// This method ensures that resources are properly released and that the application stops listening for settings changes when it is no longer needed.
        /// It is important to call this method when the application is shutting down or the speech recognition functionality is no longer required to prevent memory leaks and other resource issues.
        /// </summary>
        public static void Dispose()
        {
            _listener?.Dispose();
            General.Saved -= OnSettingsSaved;
        }
    }
}
