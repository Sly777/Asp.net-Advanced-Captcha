using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;

/// <summary>
/// Captcha for ASP.NET
/// v.1.0.2
/// Created by Ilker Guller
/// </summary>
public class Captcha
{
    private static int _charLength = 4;
    private static string _captchaSessionName = "CaptchaNET";
    private static bool _caseSensitive = true;
    private static bool _onlyNumber = false;
    private static bool _onlyChar = false;

    private readonly string[] fontList = new string[]
                                             {
                                                 "Times New Roman", "Verdana", "Comic Sans MS", "Arial", "Courier New",
                                                 "Georgia", "Impact", "Palatino Linotype", "Lucida Console", "Marlett"
                                             };

    private string familyName;
    private int height;
    private Bitmap image;
    private Random rand = new Random();
    private Random random = new Random();
    private string text;
    private int width;

    public Captcha(int width, int height)
    {
        text = generateRandomCode();
        HttpContext.Current.Session.Add(_captchaSessionName, text);

        SetDimensions(width, height);
        SetFamilyName(fontList[random.Next(0, fontList.Length - 1)]);
        GenerateImage();
    }

    /// <summary>
    /// Get Text on the Captcha Image 
    /// </summary>
    public string Text
    {
        get { return text; }
    }

    /// <summary>
    /// Get Captcha Image
    /// </summary>
    public Bitmap Image
    {
        get { return image; }
    }

    /// <summary>
    /// Get Width of Captcha Image
    /// </summary>
    public int Width
    {
        get { return width; }
    }

    /// <summary>
    /// Get Height of Captcha Image
    /// </summary>
    public int Height
    {
        get { return height; }
    }

    /// <summary>
    /// Get / Set Captcha Session Name
    /// </summary>
    public string captchaSessionName
    {
        get { return _captchaSessionName; }
        set { _captchaSessionName = value; }
    }

    /// <summary>
    /// Get / Set Text Length on the Captcha Image 
    /// </summary>
    public static int charLength
    {
        get { return _charLength; }
        set { _charLength = value; }
    }

    /// <summary>
    /// Get / Set Case Sensitive
    /// </summary>
    public static bool caseSensitive
    {
        get { return _caseSensitive; }
        set { _caseSensitive = value; }
    }

    /// <summary>
    /// Set True if you want to see only Number
    /// </summary>
    public static bool onlyNumber
    {
        get { return _onlyNumber; }
        set { _onlyNumber = value; }
    }

    /// <summary>
    /// Set True if you want to see only latin characters
    /// </summary>
    public static bool onlyChar
    {
        get { return _onlyChar; }
        set { _onlyChar = value; }
    }

    ~Captcha()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            image.Dispose();
        }
    }

    private void SetDimensions(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException("width", width, "Argument out of range, must be greater than zero.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException("height", height, "Argument out of range, must be greater than zero.");
        this.width = width;
        this.height = height;
    }

    private void SetFamilyName(string familyName)
    {
        try
        {
            Font font = new Font(this.familyName, 12F);
            this.familyName = familyName;
            font.Dispose();
        }
        catch
        {
            this.familyName = FontFamily.GenericSerif.Name;
        }
    }

    private void GenerateImage()
    {
        Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        Graphics g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new Rectangle(0, 0, width, height);

        Color backColor = Color.FromArgb(random.Next(128, 255), random.Next(128, 255), random.Next(128, 255));
        Color foreColor = ColorInvert(backColor);

        HatchBrush hatchBrush = new HatchBrush(RandomEnum<HatchStyle>(), backColor, Color.White);
        g.FillRectangle(hatchBrush, rect);

        SizeF size;
        float fontSize = rect.Height + 11;
        Font font;
        do
        {
            fontSize--;
            font = new Font(familyName, fontSize, FontStyle.Bold);
            size = g.MeasureString(text, font);
        } while (size.Width > rect.Width);
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        GraphicsPath path = new GraphicsPath();
        path.AddString(text, font.FontFamily, (int) font.Style, font.Size, rect, format);
        float v = 4F;
        PointF[] points =
            {
                new PointF(random.Next(rect.Width)/v, random.Next(rect.Height)/v),
                new PointF(rect.Width - random.Next(rect.Width)/v, random.Next(rect.Height)/v),
                new PointF(random.Next(rect.Width)/v, rect.Height - random.Next(rect.Height)/v),
                new PointF(rect.Width - random.Next(rect.Width)/v, rect.Height - random.Next(rect.Height)/v)
            };
        Matrix matrix = new Matrix();
        matrix.Translate(0F, 0F);
        path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

        hatchBrush = new HatchBrush(RandomEnum<HatchStyle>(), Color.DimGray, foreColor);
        g.FillPath(hatchBrush, path);

        int m = Math.Max(rect.Width, rect.Height);
        for (int i = 0; i < (int) (rect.Width*rect.Height/30F); i++)
        {
            int x = random.Next(rect.Width);
            int y = random.Next(rect.Height);
            int w = random.Next(m/50);
            int h = random.Next(m/50);
            g.FillEllipse(hatchBrush, x, y, w, h);
        }

        font.Dispose();
        hatchBrush.Dispose();
        g.Dispose();
        image = bitmap;
    }

    public static string generateRandomCode()
    {
        ArrayList Kod = new ArrayList();
        Random rnd = new Random();

        if (!_onlyNumber)
        {
            for (char i = 'A'; i <= 'Z'; i++)
                Kod.Add(i);

            for (char i = 'a'; i <= 'z'; i++){
                if (i == 'l') i++;
                Kod.Add(i);
            }
        }

        if (!_onlyChar)
        {
            for (char i = '0'; i <= '9'; i++)
                Kod.Add(i);
        }

        string val = "";

        for (int i = 0; i < _charLength; i++)
            val += Kod[rnd.Next(Kod.Count)].ToString();

        if (!_caseSensitive)
        {
            val = val.ToLower();
        }

        return val;
    }

    private T RandomEnum<T>()
    {
        T[] values = (T[]) Enum.GetValues(typeof (T));
        return values[rand.Next(0, values.Length)];
    }

    private Color ColorInvert(Color colorIn)
    {
        return Color.FromArgb(colorIn.A, Color.White.R - colorIn.R,
                              Color.White.G - colorIn.G, Color.White.B - colorIn.B);
    }
}