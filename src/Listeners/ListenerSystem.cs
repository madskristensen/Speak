using System.Globalization;
using System.Speech.Recognition;
using WindowsInput;

namespace Speak
{
    /// <summary>
    /// The <see cref="ListenerSystem"/> class implements the <see cref="IListener"/> interface
    /// to provide voice command recognition using the System.Speech.Recognition namespace.
    /// This class is responsible for initializing the speech recognition engine, handling recognized speech,
    /// and simulating keyboard input based on the recognized text.
    /// </summary>
    internal class ListenerSystem : IListener
    {
        private readonly SpeechRecognitionEngine _recognizer;
        private readonly InputSimulator _simulator;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerSystem"/> class.
        /// Sets up the speech recognition engine with the default audio device and loads a dictation grammar.
        /// </summary>
        public ListenerSystem()
        {
            _simulator = new InputSimulator();

            _recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += OnVoiceRecognized;
        }

        /// <summary>
        /// Gets the type of the listener, indicating the underlying technology used for voice command recognition.
        /// </summary>
        public ListenerTypes Type => ListenerTypes.SystemSpeech;

        /// <summary>
        /// Starts the speech recognition engine to begin listening for voice commands.
        /// If the General.Instance.PlayBeep is true, a beep sound is played to indicate the start of listening.
        /// </summary>
        public async Task StartAsync()
        {
            try
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);

                if (General.Instance.PlayBeep)
                {
                    Console.Beep(100, 10);
                }

                await VS.StatusBar.ShowMessageAsync($"🎤 Listening...");
            }
            catch (Exception ex)
            {
                await VS.StatusBar.ShowMessageAsync($"🎤 An error occurred. See output window for details");
                await ex.LogAsync();
            }
        }

        /// <summary>
        /// Event handler for the <see cref="SpeechRecognitionEngine.SpeechRecognized"/> event.
        /// Simulates keyboard input based on the recognized text.
        /// </summary>
        private void OnVoiceRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            _ = _simulator.Keyboard.TextEntry(e.Result.Text);
        }

        /// <summary>
        /// Stops the speech recognition engine from listening for voice commands.
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                _recognizer.RecognizeAsyncCancel();
                await VS.StatusBar.ClearAsync();
            }
            catch (Exception ex)
            {
                ex.Log();
                await VS.StatusBar.ShowMessageAsync($"🎤 An error occurred. See output window for details");
            }
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="ListenerSystem"/> class.
        /// This method should be called when the application is shutting down to release resources properly.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _recognizer.SpeechRecognized -= OnVoiceRecognized;
                _recognizer?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
