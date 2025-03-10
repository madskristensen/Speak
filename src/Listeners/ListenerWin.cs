using Windows.Media.SpeechRecognition;
using WindowsInput;

namespace Speak
{
    /// <summary>
    /// Provides functionality to listen for voice commands and simulate keyboard input.
    /// This class uses the SpeechRecognitionEngine to recognize spoken words and the InputSimulator to simulate keyboard input.
    /// </summary>
    internal class ListenerWin : IListener
    {
        private readonly SpeechRecognizer _recognizer;
        private readonly InputSimulator _simulator;
        private bool _isCompiled;
        private bool _isDisposed;

        /// <summary>
        /// Initializes static members of the <see cref="ListenerWin"/> class.
        /// Sets up the speech recognition engine and input simulator.
        /// </summary>
        public ListenerWin()
        {
            _simulator = new InputSimulator();

            var dictation = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");

            _recognizer = new SpeechRecognizer(new Windows.Globalization.Language("en-us"));
            _recognizer.Constraints.Add(dictation);
            _recognizer.ContinuousRecognitionSession.ResultGenerated += OnVoiceRecognized;
            _recognizer.HypothesisGenerated += OnHypothesisGenerated;
        }

        public ListenerTypes Type => ListenerTypes.WinRT;

        /// <summary>
        /// Starts the speech recognition engine to listen for voice commands.
        /// This method begins asynchronous recognition of speech input.
        /// </summary>
        public void Start()
        {
            if (_recognizer.State != SpeechRecognizerState.Idle)
            {
                return;
            }

            _ = ThreadHelper.JoinableTaskFactory.StartOnIdle(async delegate
            {
                try
                {
                    await EnsureCompiledAsync();
                    await _recognizer.ContinuousRecognitionSession.StartAsync();

                    if (General.Instance.PlayBeep)
                    {
                        Console.Beep(100, 10);
                    }

                    await VS.StatusBar.ShowMessageAsync($"🎤 Listening");
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
            });
        }

        /// <summary>
        /// Ensures that the speech recognizer's constraints are compiled.
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
        /// Stops the speech recognition engine.
        /// This method cancels asynchronous recognition of speech input.
        /// </summary>
        public void Stop()
        {
            if (_recognizer.State != SpeechRecognizerState.Idle)
            {
                _recognizer.ContinuousRecognitionSession.StopAsync().AsTask().FireAndForget();
                VS.StatusBar.ClearAsync().FireAndForget();
            }
        }

        /// <summary>
        /// Handles the SpeechRecognized event of the recognizer.
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
        /// Handles the HypothesisGenerated event of the recognizer.
        /// This method is called when the speech recognizer has a hypothesis about the spoken words.
        /// It updates the status bar with the current hypothesis text.
        /// </summary>
        private void OnHypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            var text = args.Hypothesis.Text;

            if (text.Length > 75)
            {
                text = $"🎤 ...{text.Substring(text.Length - 75)}";
            }

            VS.StatusBar.ShowMessageAsync($"🎤 {text}").FireAndForget();
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="ListenerWin"/> class.
        /// This must only be called when the application is shutting down to release resources properly.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _recognizer.ContinuousRecognitionSession.ResultGenerated -= OnVoiceRecognized;
                _recognizer.HypothesisGenerated -= OnHypothesisGenerated;
                _recognizer?.Dispose();
                _isDisposed = true;
            }
        }
    }
}
