namespace Speak
{
    /// <summary>
    /// Defines the interface for a listener that can start and stop listening for voice commands.
    /// This interface is intended to be implemented by different types of listeners that handle voice commands
    /// using various underlying technologies (e.g., WinRT, SystemSpeech).
    /// </summary>
    internal interface IListener
    {
        /// <summary>
        /// Gets the type of the listener, indicating the underlying technology used for voice command recognition.
        /// </summary>
        ListenerTypes Type { get; }

        /// <summary>
        /// Disposes the resources used by the listener.
        /// This must only be called when the application is shutting down to release resources properly.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Starts the listener to begin listening for voice commands.
        /// This method should be called when the application is ready to receive voice commands.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the listener from listening for voice commands.
        /// This method should be called when the application no longer needs to receive voice commands,
        /// or when it is temporarily pausing voice command recognition.
        /// </summary>
        Task StopAsync();
    }
}