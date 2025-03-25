using System.Drawing.Imaging;

namespace ExternalScreenshotTool;

public partial class ExternalScreenshotTool_Form : Form {
   private bool      is_drawing;
   private Point     start_point;
   private Rectangle selection_rectangle;
   private FileInfo? saved_file; // Now storing the screenshot info as FileInfo

   // We store the base path so we can combine it with the generated filename
   private readonly DirectoryInfo base_path; // Now using DirectoryInfo

   // Indicates whether a screenshot was actually saved
   public bool screenshot_saved { get; private set; }

   public ExternalScreenshotTool_Form() {
      DoubleBuffered  = true;
      WindowState     = FormWindowState.Maximized;
      FormBorderStyle = FormBorderStyle.None;
      TopMost         = true;
      Opacity         = 0.4;
      Cursor          = Cursors.Cross;

      // Default to Environment.CurrentDirectory if no path is provided
      base_path = new DirectoryInfo(Environment.CurrentDirectory);
      if (!base_path.Exists) {
         base_path.Create();
      }
   }

   // New constructor allowing the caller to pass a path
   public ExternalScreenshotTool_Form(DirectoryInfo base_path) : this() {
      this.base_path = base_path;
      if (!this.base_path.Exists)
         this.base_path.Create();
   }

   protected override void OnMouseDown(MouseEventArgs e) {
      base.OnMouseDown(e);
      if (e.Button == MouseButtons.Left) {
         is_drawing          = true;
         start_point         = e.Location;
         selection_rectangle = new Rectangle(start_point, new Size(0, 0));
      }
   }

   protected override void OnMouseMove(MouseEventArgs e) {
      base.OnMouseMove(e);
      if (is_drawing) {
         Point current_point = e.Location;
         int   x             = Math.Min(start_point.X, current_point.X);
         int   y             = Math.Min(start_point.Y, current_point.Y);
         int   w             = Math.Abs(start_point.X - current_point.X);
         int   h             = Math.Abs(start_point.Y - current_point.Y);

         selection_rectangle = new Rectangle(x, y, w, h);
         Invalidate(); // Repaint to show the new rectangle
      }
   }

   protected override void OnMouseUp(MouseEventArgs e) {
      base.OnMouseUp(e);
      if (e.Button != MouseButtons.Left) {
         // If right-click or something else, just close with no screenshot
         saved_file = null;
         Close();
      }
      else {
         is_drawing = false;
         Hide(); // Hide overlay so it's not captured

         // Convert client coords to screen coords
         var       upper_left = PointToScreen(selection_rectangle.Location);
         var       screenshot = new Bitmap(selection_rectangle.Width, selection_rectangle.Height);
         using var g          = Graphics.FromImage(screenshot);
         g.CopyFromScreen(upper_left, new Point(0, 0), selection_rectangle.Size);

         var random_hex_5 = new Random().Next(0x100000).ToString("X5");
         var filename     = $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}_{random_hex_5}.png";

         // Save screenshot using FileInfo
         saved_file = new FileInfo(Path.Combine(base_path.FullName, filename));
         screenshot.Save(saved_file.FullName, ImageFormat.Png);

         // Signal we're done
         Close();
      }
   }

   protected override void OnPaint(PaintEventArgs e) {
      base.OnPaint(e);
      using var pen = new Pen(Color.Red, 2);
      e.Graphics.DrawRectangle(pen, selection_rectangle);
   }

   protected override bool ProcessCmdKey(ref Message msg, Keys key_data) {
      if (key_data == Keys.Escape) {
         // Esc => user cancels
         saved_file = null;
         Close();
         return true;
      }

      return base.ProcessCmdKey(ref msg, key_data);
   }

   protected override void OnFormClosing(FormClosingEventArgs e) {
      base.OnFormClosing(e);
      // Write the final path to Console so the calling process can read it
      if (saved_file is not null) {
         Console.WriteLine(saved_file.FullName);
         screenshot_saved = true; // Mark that we actually produced a screenshot
      }
   }
}