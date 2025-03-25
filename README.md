# ExternalScreenshotTool

Capture partial screenshots with a single command — no fuss, just snap and go! Snap that snippet faster than lightning ⚡.

## Features
- **Partial Screen Capture**: Draw a rectangle on the screen to just grab what you need.  
- **Easy CLI**: Launch with a single command and supply an output path.  
- **Transparent Overlay**: Quickly see the region you're capturing.  
- **Escape to Cancel**: Press ESC if you change your mind.  
- **Saved as PNG**: Automatically time-stamped and saved to your chosen folder.  

## Requirements
- .NET 8.0 or higher
- Windows (WinForms, oh yeah!)

## Installation
1. Clone this repo or download the latest release.  
2. Build or grab the compiled binary.  

## Usage
Open a Terminal or PowerShell in the folder containing the tool, then run:

```shell
> ExternalScreenshotTool.exe <output_directory>
```

For example:

```shell
> ExternalScreenshotTool.exe C:\Screenshots
```

• Click, drag, and release to capture.  
• ESC to cancel.  
• Output is automatically named using the pattern:  
  ```
  yyyy-MM-dd HH-mm-ss_<random_hex_5>.png
  ```
• When you’re done, you’ll find your new screenshot in the specified folder!  

• The tool returns 0 on success, and a non-zero code otherwise (for cancellation or invalid input).  
• If a directory error or another issue occurs, you’ll see a helpful error message on stderr.  

## License
This project is licensed under the [MIT License](LICENSE).
Feel free to use, modify, and distribute it as you see fit, as long as you ship along the license. Enjoy! 🤗
