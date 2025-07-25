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
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            _element.PreviewKeyDown += OnKeyDown;
            _element.PreviewKeyUp += OnKeyUp;
        }

        private void DetachEventHandlers()
        {
            _element.PreviewKeyDown -= OnKeyDown;
            _element.PreviewKeyUp -= OnKeyUp;
        }

        /// <summary>
        /// Handles the <see cref="UIElement.PreviewKeyDown"/> event.
        /// Starts listening if the Ctrl and Shift keys are pressed.
        /// Cancels the operation if any other key is pressed.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (AreValidKeysPressed(e))
            {
                HandleValidKeyPress(e);
            }
            else
            {
                Cancel();
            }
        }

        /// <summary>
        /// Handles valid key press events.
        /// If the key press is a repeat and the handler is not already listening,
        /// and the time since the first Ctrl press exceeds the configured delay,
        /// it starts the speech controller.
        /// Otherwise, it records the time of the first Ctrl press if it's not a repeat.
        /// </summary>
        private void HandleValidKeyPress(KeyEventArgs e)
        {
            if (e.IsRepeat && !_isListening && DateTime.Now - _ctrlFirstPress > TimeSpan.FromMilliseconds(General.Instance.Delay))
            {
                StartListening();
            }
            else if (!e.IsRepeat)
            {
                _ctrlFirstPress = DateTime.Now;
            }
        }

        private void StartListening()
        {
            SpeechController.StartAsync().FireAndForget();
            _isListening = true;
            _ratingPrompt.RegisterSuccessfulUsage();
        }

        /// <summary>
        /// This method is used to determine if the speech controller should start listening.
        /// </summary>
        private bool AreValidKeysPressed(KeyEventArgs e)
        {
            // Check if the pressed key is Ctrl or Shift and if only Ctrl and Shift are pressed
            return (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftShift || e.Key == Key.RightShift) &&
                   Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift);
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
                StopListening();
            }

            _isListening = false;
        }

        private void StopListening()
        {
            _ = ThreadHelper.JoinableTaskFactory.StartOnIdle(async () =>
            {
                await Task.Delay(200); // Listen a little bit after the CTRL key is released to get the last word spoken
                await SpeechController.StopAsync();
            });
        }

        /// <summary>
        /// Disposes the <see cref="KeypressHandler"/> and detaches event handlers.
        /// This is necessary to prevent memory leaks and ensure that the event handlers are properly cleaned up.
        /// </summary>
        public void Dispose()
        {
            DetachEventHandlers();
        }
    }
}
