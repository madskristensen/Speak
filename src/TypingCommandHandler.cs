using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Speak
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(ContentTypes.Any)]
    internal class TypingCommandHandler : IWpfTextViewCreationListener
    {
        private KeypressHandler _keypressHandler;

        public void TextViewCreated(IWpfTextView textView)
        {
            _keypressHandler = new KeypressHandler(textView.VisualElement);
            textView.Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            ((IWpfTextView)sender).Closed -= OnClosed;
            _keypressHandler?.Dispose();
        }
    }
}
