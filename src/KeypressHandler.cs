using System.Windows;
using System.Windows.Input;

namespace Speak
{
    internal class KeypressHandler : IDisposable
    {
        private bool _isCtrlHeld = false;
        private readonly FrameworkElement _element;

        public KeypressHandler(FrameworkElement element)
        {
            _element = element;
            element.PreviewKeyDown += OnKeyDown;
            element.PreviewKeyUp += OnKeyUp;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Check if only Ctrl is being pressed
            if ((e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) && !_isCtrlHeld)
            {
                _isCtrlHeld = true;
                Listener.Start();
                VS.StatusBar.ShowMessageAsync("Listening...").FireAndForget();
            }
            else if (!e.IsRepeat)
            {
                // If any other key is pressed, cancel out
                Cancel();
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.LeftCtrl or Key.RightCtrl)
            {
                Cancel();
            }
        }

        private void Cancel()
        {
            if (_isCtrlHeld)
            {
                VS.StatusBar.ClearAsync().FireAndForget();
                Listener.Stop();
            }
            _isCtrlHeld = false;
        }

        public void Dispose()
        {
            _element.PreviewKeyDown -= OnKeyDown;
            _element.PreviewKeyUp -= OnKeyUp;
        }
    }
}
