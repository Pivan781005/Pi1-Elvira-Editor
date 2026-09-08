namespace Pi1ElviraEditor;

// Borderless startup splash. Presentation only: it never touches installation
// detection, project loading, variants, game files, or Runtime UI state.
internal sealed class StartupSplashForm : Form
{
    private readonly PictureBox _picture;
    private Image? _splashImage;

    internal Size SplashImageSizeForTest => _splashImage?.Size ?? Size.Empty;

    internal Rectangle ComputedBoundsForTest { get; }

    internal StartupSplashForm()
        : this(BrandingAssets.LoadSplashImage(), GetStartupWorkingArea())
    {
    }

    internal StartupSplashForm(Image image, Rectangle workingArea)
    {
        ArgumentNullException.ThrowIfNull(image);
        if (image.Width <= 0 || image.Height <= 0)
            throw new InvalidDataException("Splash image has invalid dimensions.");
        if (workingArea.Width <= 0 || workingArea.Height <= 0)
            throw new InvalidDataException("Splash working area is invalid.");

        _splashImage = image;

        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.None;
        ControlBox = false;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ShowIcon = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        Text = string.Empty;
        BackColor = Color.Black;
        Padding = Padding.Empty;
        Margin = Padding.Empty;

        Rectangle bounds = ComputeSplashBounds(image.Size, workingArea);
        ComputedBoundsForTest = bounds;
        Bounds = bounds;

        _picture = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = _splashImage,
            BackColor = Color.Black,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };
        Controls.Add(_picture);
    }

    // Pure sizing policy, headless-safe and deterministically smoke-tested
    // without displaying a window. Preserves aspect ratio, never upscales
    // beyond the original artwork, and fits inside the working area.
    // Target is ~70% of working-area width (within the 60-75% band) subject
    // to an 80% height cap so the image never exceeds the screen.
    internal static Size ComputeSplashSize(Size imageSize, Size workingAreaSize, double maxWidthFraction = 0.70, double maxHeightFraction = 0.80)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0)
            throw new InvalidDataException("Splash image size must be positive.");
        if (workingAreaSize.Width <= 0 || workingAreaSize.Height <= 0)
            throw new InvalidDataException("Working area size must be positive.");
        if (maxWidthFraction <= 0 || maxWidthFraction > 1 || maxHeightFraction <= 0 || maxHeightFraction > 1)
            throw new InvalidDataException("Splash fractions must be in (0, 1].");

        double maxWidth = workingAreaSize.Width * maxWidthFraction;
        double maxHeight = workingAreaSize.Height * maxHeightFraction;
        double scale = Math.Min(1.0, Math.Min(maxWidth / imageSize.Width, maxHeight / imageSize.Height));
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale));
        return new Size(width, height);
    }

    internal static Rectangle ComputeSplashBounds(Size imageSize, Rectangle workingArea)
    {
        Size size = ComputeSplashSize(imageSize, workingArea.Size);
        int x = workingArea.X + Math.Max(0, (workingArea.Width - size.Width) / 2);
        int y = workingArea.Y + Math.Max(0, (workingArea.Height - size.Height) / 2);
        return new Rectangle(x, y, size.Width, size.Height);
    }

    internal static Rectangle GetStartupWorkingArea()
    {
        try
        {
            return Screen.FromPoint(Cursor.Position).WorkingArea;
        }
        catch
        {
            try
            {
                if (Screen.PrimaryScreen is not null)
                    return Screen.PrimaryScreen.WorkingArea;
            }
            catch
            {
            }
            return new Rectangle(0, 0, 1024, 768);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _picture.Dispose();
            _splashImage?.Dispose();
            _splashImage = null;
        }
        base.Dispose(disposing);
    }
}
