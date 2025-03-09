using System.Speech.Recognition;
using WindowsInput;

namespace Speak
{
    internal static class Listener
    {
        private static readonly SpeechRecognitionEngine _recognizer;
        private static readonly InputSimulator _simulator;

        static Listener()
        {
            _simulator = new InputSimulator();

            _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += VoiceRecognizer;
        }

        public static void Start()
        {
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static void VoiceRecognizer(object sender, SpeechRecognizedEventArgs e)
        {
            _ = _simulator.Keyboard.TextEntry(e.Result.Text);
        }

        public static void Stop()
        {
            _recognizer.RecognizeAsyncCancel();
        }
    }
}
