using System.Windows;
using System.Windows.Input;

namespace Speak
{
    /// <summary>
    /// Handles key press events for a specified <see cref="FrameworkElement"/>.
    /// This class listens for Ctrl and Shift key presses and triggers specific actions when these keys are pressed or released.
    /// The primary purpose is to start and stop the speech controller based on key press events.
    /// </summary>
    internal class KeypressHandler : IDisposable
    {
        private bool _isListening;
        private readonly FrameworkElement _element;
        private readonly RatingPrompt _ratingPrompt = new("MadsKristensen.Speak", Vsix.Name, General.Instance);
        private DateTime _ctrlFirstPress;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypressHandler"/> class.
        /// Attaches key press event handlers to the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> to attach key press events to.</param>
        public KeypressHandler(FrameworkElement element)
        {
            _element = element;
            element.PreviewKeyDown += OnKeyDown;
            element.PreviewKeyUp += OnKeyUp;
        }

        /// <summary>
        /// Handles the <see cref="UIElement.PreviewKeyDown"/> event.
        /// Starts listening if the Ctrl and Shift keys are pressed.
        /// Cancels the operation if any other key is pressed.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl and Shift are being pressed
            if (AreValidKeysPressed())
            {
                if (e.IsRepeat && !_isListening && DateTime.Now - _ctrlFirstPress > TimeSpan.FromMilliseconds(General.Instance.Delay))
                {
                    SpeechController.StartAsync().FireAndForget();
                    _isListening = true;
                    _ratingPrompt.RegisterSuccessfulUsage();
                }
                else if (!e.IsRepeat)
                {
                    _ctrlFirstPress = DateTime.Now;
                }
            }
            // If any other key is pressed, cancel out
            else
            {
                Cancel();
            }
        }

        /// <summary>
        /// This method is used to determine if the speech controller should start listening.
        /// </summary>
        /// <returns><c>true</c> if both Ctrl and Shift keys are pressed; otherwise, <c>false</c>.</returns>
        private bool AreValidKeysPressed()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        /// <summary>
        /// Handles the <see cref="UIElement.PreviewKeyUp"/> event.
        /// Cancels the operation when the Ctrl key is released.
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Cancel();
        }

        /// <summary>
        /// Cancels the current operation and resets the state.
        /// Stops listening and clears the status bar message.
        /// </summary>
        private void Cancel()
        {
            if (_isListening)
            {
                _ = ThreadHelper.JoinableTaskFactory.StartOnIdle(async () =>
                {
                    await Task.Delay(200); // Listen a little bit after the CTRL key is released to get the last word spoken
                    await SpeechController.StopAsync();
                });
            }

            _isListening = false;
        }

        /// <summary>
        /// Disposes the <see cref="KeypressHandler"/> and detaches event handlers.
        /// This is necessary to prevent memory leaks and ensure that the event handlers are properly cleaned up.
        /// </summary>
        public void Dispose()
        {
            _element.PreviewKeyDown -= OnKeyDown;
            _element.PreviewKeyUp -= OnKeyUp;
        }
    }
}
