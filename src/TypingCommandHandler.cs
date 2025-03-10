using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Speak
{
    /// <summary>
    /// Listens for the creation of IWpfTextView instances and attaches a KeypressHandler to them.
    /// This class is intended to monitor text view creation events and ensure that key press events are handled appropriately.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(ContentTypes.Any)]
    internal class TypingCommandHandler : IWpfTextViewCreationListener
    {
        private KeypressHandler _keypressHandler;

        /// <summary>
        /// Called when an IWpfTextView is created. Attaches a KeypressHandler to the text view.
        /// This method ensures that every new text view has a KeypressHandler attached to handle key press events.
        /// </summary>
        public void TextViewCreated(IWpfTextView textView)
        {
            _keypressHandler = new KeypressHandler(textView.VisualElement);
            textView.Closed += OnClosed;
        }

        /// <summary>
        /// Called when the text view is closed. Disposes the KeypressHandler.
        /// This method ensures that resources are properly cleaned up when the text view is closed.
        /// </summary>
        private void OnClosed(object sender, EventArgs e)
        {
            ((IWpfTextView)sender).Closed -= OnClosed;
            _keypressHandler?.Dispose();
        }
    }
}
