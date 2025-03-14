using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Speak
{
    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>, IRatingConfig
    {
        [Browsable(false)]
        public int RatingRequests { get; set; }

        [Category("General")]
        [DisplayName("Play beep")]
        [Description("Play a beep sound when listening mode begins.")]
        [DefaultValue(true)]
        public bool PlayBeep { get; set; } = true;

        [Category("General")]
        [DisplayName("Listening delay")]
        [Description("The delay in milliseconds from hitting the CTRL key to the extension starts listening for your speech. Default: 400")]
        [DefaultValue(400)]
        public int Delay { get; set; } = 400;

        [Category("General")]
        [DisplayName("Speech engine")]
        [Description("Determines which speech recognition engine to use. Requires restart")]
        [DefaultValue(ListenerTypes.WinRT)]
        [TypeConverter(typeof(EnumConverter))]
        public ListenerTypes ListenerType { get; set; } = ListenerTypes.WinRT;

    }
}
