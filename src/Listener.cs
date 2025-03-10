using System.Globalization;
using System.Speech.Recognition;
using WindowsInput;

namespace Speak
{
    /// <summary>
    /// Provides functionality to listen for voice commands and simulate keyboard input.
    /// This class uses the SpeechRecognitionEngine to recognize spoken words and the InputSimulator to simulate keyboard input.
    /// </summary>
    internal static class Listener
    {
        private static readonly SpeechRecognitionEngine _recognizer;
        private static readonly InputSimulator _simulator;
        private static bool _isDisposed;

        /// <summary>
        /// Initializes static members of the <see cref="Listener"/> class.
        /// Sets up the speech recognition engine and input simulator.
        /// </summary>
        static Listener()
        {
            _simulator = new InputSimulator();

            _recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += OnVoiceRecognized;
        }

        /// <summary>
        /// Starts the speech recognition engine to listen for voice commands.
        /// This method begins asynchronous recognition of speech input.
        /// </summary>
        public static void Start()
        {
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Handles the SpeechRecognized event of the recognizer.
        /// Simulates keyboard input based on the recognized text.
        /// </summary>
        private static void OnVoiceRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            _ = _simulator.Keyboard.TextEntry(e.Result.Text);
        }

        /// <summary>
        /// Stops the speech recognition engine.
        /// This method cancels asynchronous recognition of speech input.
        /// </summary>
        public static void Stop()
        {
            _recognizer.RecognizeAsyncCancel();
        }

        /// <summary>
        /// Disposes the resources used by the <see cref="Listener"/> class.
        /// This must only be called when the application is shutting down to release resources properly.
        /// </summary>
        public static void Dispose()
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
