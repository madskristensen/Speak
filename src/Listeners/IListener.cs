namespace Speak
{
    /// <summary>
    /// Defines the interface for a listener that can start and stop listening for voice commands.
    /// </summary>
    internal interface IListener
    {
        /// <summary>
        /// Gets the type of the listener.
        /// </summary>
        ListenerTypes Type { get; }

        /// <summary>
        /// Disposes the resources used by the listener.
        /// This must only be called when the application is shutting down to release resources properly.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Starts the listener to begin listening for voice commands.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the listener from listening for voice commands.
        /// </summary>
        void Stop();
    }
}