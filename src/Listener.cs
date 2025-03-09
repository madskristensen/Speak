using System.Globalization;
using System.Speech.Recognition;
using WindowsInput;

namespace Speak
{
    internal static class Listener
    {
        private static readonly SpeechRecognitionEngine _recognizer;
        private static readonly InputSimulator _simulator;
        private static bool _isDisposed;

        static Listener()
        {
            _simulator = new InputSimulator();

            _recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += OnVoiceRecognized;
        }

        public static void Start()
        {
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void OnVoiceRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            _ = _simulator.Keyboard.TextEntry(e.Result.Text);
        }

        public static void Stop()
        {
            _recognizer.RecognizeAsyncCancel();
        }

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
