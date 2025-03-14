[marketplace]: https://marketplace.visualstudio.com/items?itemName=MadsKristensen.Speak
[vsixgallery]: http://vsixgallery.com/extension/Speak.899ea22a-c6e2-43f6-b3eb-50a3713c4df7/
[repo]:https://github.com/madskristensen/Speak

# Voice Typing for Visual Studio

[![Build](https://github.com/madskristensen/Speak/actions/workflows/build.yaml/badge.svg)](https://github.com/madskristensen/Speak/actions/workflows/build.yaml)
![GitHub Sponsors](https://img.shields.io/github/sponsors/madskristensen)

Download this extension from the [Visual Studio Marketplace][marketplace]
or get the [CI build][vsixgallery]

----------------------------------------

Press and hold the `CTRL + Shift` keys to activate dictation mode in Visual Studio. Your voice will be converted into text in the editor or any other input field that currently has focus.

**Tip!** Hit the `CTRL` key first, and then the `Shift` key immediately after for the best results.

This feature makes it incredibly easy to write prompts for Copilot and add code comments using your voice instead of typing.

- Choose between WinRT and System.Speech engines
  - WinRT is the default and recommended for most users
- A low beep is played when listening is activated
- Control the delay before it begins to listen

When you press and hold the `CTRL + Shift` keys, the status bar will show an indicator that it's listening.

![Statusbar](art/statusbar.png)

## WinRT Speech Recognition Engine
To use the WinRT speech recognition engine, you must enable the Windows privacy setting for "Speech". The first time you use this extension, you will be prompted to enable it.

![Enable speech privacy setting](art/enable-speech.png)

## Settings
You can configure the extension in the Visual Studio options dialog under **Tools > Options > Voice Typing**.

![Options dialog](art/options.png)

## How can I help?
If you enjoy using the extension, please give it a ★★★★★ rating on the [Visual Studio Marketplace][marketplace].

Should you encounter bugs or have feature requests, head over to the [GitHub repo][repo] to open an issue if one doesn't already exist.

Pull requests are also very welcome, as I can't always get around to fixing all bugs myself. This is a personal passion project, so my time is limited.

Another way to help out is to [sponsor me on GitHub](https://github.com/sponsors/madskristensen).
