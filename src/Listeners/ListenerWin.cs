using Windows.Media.SpeechRecognition;
using WindowsInput;

namespace Speak
{
    /// <summary>
    /// ListenerWin is an implementation of the <see cref="IListener"/> interface that uses Windows Runtime (WinRT) APIs
    /// to recognize speech and simulate keyboard input based on recognized speech commands.
    /// </summary>
    internal class ListenerWin : IListener
    {
        private readonly SpeechRecognizer _recognizer;
        private readonly InputSimulator _simulator;
        private bool _isCompiled;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerWin"/> class.
        /// Sets up the speech recognizer with dictation constraints and hooks up event handlers for speech recognition results.
        /// </summary>
        public ListenerWin()
        {
            _simulator = new InputSimulator();

            var dictation = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");

            _recognizer = new SpeechRecognizer(new Windows.Globalization.Language("en-us"));
            _recognizer.Constraints.Add(dictation);
            _recognizer.ContinuousRecognitionSession.ResultGenerated += OnVoiceRecognized;
        }

        /// <summary>
        /// Gets the type of the listener, indicating that this listener uses WinRT APIs.
        /// </summary>
        public ListenerTypes Type => ListenerTypes.WinRT;

        /// <summary>
        /// Starts the speech recognition session. If the recognizer is not idle, it does nothing.
        /// Ensures the recognizer is compiled and starts continuous recognition.
        /// </summary>
        public async Task StartAsync()
        {
            if (_recognizer.State != SpeechRecognizerState.Idle)
            {
                return;
            }

            try
            {
                await EnsureCompiledAsync();
                _recognizer.ContinuousRecognitionSession.StartAsync().AsTask().FireAndForget();

                if (General.Instance.PlayBeep)
                {
                    Console.Beep(200, 100);
                }

                await VS.StatusBar.ShowMessageAsync($"🎤 Listening...");
            }
            catch (Exception ex) when (ex.Message.Contains("privacy"))
            {
                await ex.LogAsync();

                var confirmed = await VS.MessageBox.ShowConfirmAsync("Enable Windows speech recognition", "Enable Windows speech recognition to use this feature. Do you want to open the settings?");

                if (confirmed)
                {
                    // Open the settings page for speech recognition
                    _ = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-speech"));
                }
            }
        }

        /// <summary>
        /// Ensures that the speech recognizer's constraints are compiled before starting recognition.
        /// This is necessary to prepare the recognizer for accurate speech recognition.
        /// </summary>
        private async Task EnsureCompiledAsync()
        {
            if (!_isCompiled)
            {
                await VS.StatusBar.ShowMessageAsync($"🎤 Initializing...");
                await _recognizer.CompileConstraintsAsync();
                _isCompiled = true;
            }
        }

        /// <summary>
        /// Stops the speech recognition session if it is currently running.
        /// </summary>
        public async Task StopAsync()
        {
            if (_recognizer.State != SpeechRecognizerState.Idle)
            {
                await VS.StatusBar.ClearAsync();
                _recognizer.ContinuousRecognitionSession.StopAsync().AsTask().FireAndForget();
            }
        }

        /// <summary>
        /// Event handler for when speech is successfully recognized.
        /// Simulates keyboard input based on the recognized text.
        /// </summary>
        private void OnVoiceRecognized(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Status == SpeechRecognitionResultStatus.Success)
            {
                _ = _simulator.Keyboard.TextEntry(args.Result.Text);
            }
        }

        /// <summary>
        /// Disposes the resources used by the listener.
        /// Unhooks event handlers and disposes the speech recognizer.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _recognizer.ContinuousRecognitionSession.ResultGenerated -= OnVoiceRecognized;
                _recognizer?.Dispose();
                _isDisposed = true;
            }
        }
    }
}
