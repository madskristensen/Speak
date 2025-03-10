namespace Speak
{
    internal static class SpeechController
    {
        private static IListener _listener;

        static SpeechController()
        {
            SetListener();

            General.Saved += OnSettingsSaved;
        }

        private static void SetListener()
        {
            _listener = General.Instance.ListenerType switch
            {
                ListenerTypes.WinRT => new ListenerWin(),
                ListenerTypes.SystemSpeech => new ListenerSystem(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static void OnSettingsSaved(General settings)
        {
            if (settings.ListenerType != _listener.Type)
            {
                _listener.Dispose();
                SetListener();
            }
        }

        public static void Start()
        {
            _listener?.Start();
        }

        public static void Stop()
        {
            _listener?.Stop();
        }

        public static void Dispose()
        {
            _listener?.Dispose();
            General.Saved -= OnSettingsSaved;
        }

        public static IListener GetListener()
        {
            _listener = General.Instance.ListenerType == ListenerTypes.SystemSpeech ? new ListenerSystem() : new ListenerWin();

            return _listener;
        }
    }
}
