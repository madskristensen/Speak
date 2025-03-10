using System.Windows;
using System.Windows.Input;

namespace Speak
{
    /// <summary>
    /// Handles key press events for a specified FrameworkElement.
    /// This class listens for Ctrl key presses and triggers specific actions when the Ctrl key is pressed or released.
    /// </summary>
    internal class KeypressHandler : IDisposable
    {
        private bool _isCtrlHeld = false;
        private readonly FrameworkElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypressHandler"/> class.
        /// Attaches key press event handlers to the specified FrameworkElement.
        /// </summary>
        /// <param name="element">The FrameworkElement to attach key press events to.</param>
        public KeypressHandler(FrameworkElement element)
        {
            _element = element;
            element.PreviewKeyDown += OnKeyDown;
            element.PreviewKeyUp += OnKeyUp;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event.
        /// Starts listening if the Ctrl key is pressed.
        /// Cancels the operation if any other key is pressed.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Check if only Ctrl is being pressed
            if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) && !_isCtrlHeld)
            {
                _isCtrlHeld = true;
                Listener.Start();
                VS.StatusBar.ShowMessageAsync("🎤 Listening...").FireAndForget();
            }
            // If any other key is pressed, cancel out
            else if (!e.IsRepeat)
            {
                Cancel();
            }
        }

        /// <summary>
        /// Handles the PreviewKeyUp event.
        /// Cancels the operation when the Ctrl key is released.
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            {
                Cancel();
            }
        }

        /// <summary>
        /// Cancels the current operation and resets the state.
        /// Stops listening and clears the status bar message.
        /// </summary>
        private void Cancel()
        {
            if (_isCtrlHeld)
            {
                VS.StatusBar.ClearAsync().FireAndForget();
                Listener.Stop();
            }
            _isCtrlHeld = false;
        }

        /// <summary>
        /// Disposes the KeypressHandler and detaches event handlers.
        /// </summary>
        public void Dispose()
        {
            _element.PreviewKeyDown -= OnKeyDown;
            _element.PreviewKeyUp -= OnKeyUp;
        }
    }
}
