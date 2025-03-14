using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Speak
{
    /// <summary>
    /// Listens for the creation of <see cref="IWpfTextView"/> instances and attaches a <see cref="KeypressHandler"/> to them.
    /// This class is intended to monitor text view creation events and ensure that key press events are handled appropriately.
    /// The primary purpose is to enable speech control functionality by listening to specific key press events.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(ContentTypes.Any)]
    internal class TypingCommandHandler : IWpfTextViewCreationListener
    {
        private KeypressHandler _keypressHandler;

        /// <summary>
        /// Called when an <see cref="IWpfTextView"/> is created. Attaches a <see cref="KeypressHandler"/> to the text view.
        /// This method ensures that every new text view has a <see cref="KeypressHandler"/> attached to handle key press events.
        /// This is crucial for enabling the speech control functionality across all text views.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> instance that was created.</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            _keypressHandler = textView.Properties.GetOrCreateSingletonProperty(() => new KeypressHandler(textView.VisualElement));
            textView.Closed += OnClosed;
        }

        /// <summary>
        /// Called when the text view is closed. Disposes the <see cref="KeypressHandler"/>.
        /// This method ensures that resources are properly cleaned up when the text view is closed.
        /// Proper disposal of the <see cref="KeypressHandler"/> is necessary to prevent memory leaks and ensure that event handlers are detached.
        /// </summary>
        private void OnClosed(object sender, EventArgs e)
        {
            if (sender is IWpfTextView textView)
            {
                textView.Closed -= OnClosed;
                _keypressHandler?.Dispose();
            }
        }
    }
}
