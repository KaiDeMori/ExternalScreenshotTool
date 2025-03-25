using System.Runtime.InteropServices;

namespace ExternalScreenshotTool;

internal static class ExternalScreenshotTool_Main {
   // Forcing the process DPI context to PerMonitorV2 with a known flag.
   // -4 => DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2
   [DllImport("user32.dll", SetLastError = true)]
   private static extern bool SetProcessDpiAwarenessContext(int dpiFlag);

   [STAThread]
   public static void Main(string[] args) {
      SetProcessDpiAwarenessContext(-4); // Force PerMonitorV2

      // Handle command-line usage
      if (args.Length != 1 || is_help_arg(args[0])) {
         print_usage();
         Environment.Exit(1);
      }

      // Validate or create the directory
      DirectoryInfo base_path;
      try {
         base_path = new DirectoryInfo(args[0]);
         if (!base_path.Exists) {
            base_path.Create();
         }
      }
      catch (Exception ex) {
         Console.Error.WriteLine($"Error creating or using directory: {ex.Message}");
         Environment.Exit(1);
         return; // Just for clarity
      }

      // Run the WinForms application
      ApplicationConfiguration.Initialize();
      var screenshot_form = new ExternalScreenshotTool_Form(base_path);
      Application.Run(screenshot_form);

      // If the form indicated a screenshot was saved, exit 0, else 1
      Environment.Exit(screenshot_form.screenshot_saved ? 0 : 1);
   }

   private static bool is_help_arg(string arg) {
      var lowered = arg.ToLowerInvariant();
      return lowered == "-h" || lowered == "--help" || lowered == "/?" || lowered == "help";
   }

   private static void print_usage() {
      Console.WriteLine("Usage: ExternalScreenshotTool <output_directory>");
      Console.WriteLine("Captures a region-based screenshot and saves it to the specified directory.");
      Console.WriteLine("If canceled or invalid input is provided, the program returns a nonzero exit code.\n");
   }
}
