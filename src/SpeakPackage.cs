global using System;
global using System.Threading;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;

namespace Speak
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.SpeakString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)] // Loads on startup after VS is initialized
    public sealed class SpeakPackage : ToolkitPackage
    {
        private KeypressHandler _keypressHandler;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();

            // Register the keypress handler for the main window of VS. This includes
            // all tool windows, but not the editor and dialog/modal windows.
            // The keypress handler for the editor is registered in TypingCommandHandler.cs.
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            _keypressHandler = new KeypressHandler(Application.Current.MainWindow);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _keypressHandler?.Dispose();
                Listener.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}