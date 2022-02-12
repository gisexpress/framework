using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System.Workspace.Drawing
{
    public class DirectBitmap : IDisposable
    {
        static DirectBitmap()
        {
            MaxSize = SystemInformation.PrimaryMonitorSize;
            Format = PixelFormat.Format32bppArgb;
            BitsPerPixel = ((int)Format & 0xff00) >> 8;
            BytesPerPixel = (BitsPerPixel + 7) / 8;

            AlphaAttributes = new ImageAttributes();
            AlphaAttributes.SetColorMatrix(AlphaMatrix);
        }

        static readonly ImageAttributes AlphaAttributes;
        static readonly ColorMatrix AlphaMatrix = new ColorMatrix { Matrix00 = 1F, Matrix11 = 1F, Matrix22 = 1F, Matrix33 = 1F, Matrix44 = 1F };

        public static readonly Size MaxSize;
        public static readonly int BitsPerPixel;
        public static readonly int BytesPerPixel;
        public static readonly PixelFormat Format;

        public Size Size;
        public Bitmap Image;
        public Graphics Surface;
        public Graphics SurfaceOriginal;
        public bool IsTransparent;
        public int Stride;
        public int[] PixelData;

        GCHandle Handle;
        DirectBitmap Copy;

        public DirectBitmap()
        {
        }

        public DirectBitmap(int size)
        {
            Init(new Size(size, size));
        }

        public DirectBitmap(int width, int height)
        {
            Init(new Size(width, height));
        }

        public DirectBitmap(Size size)
        {
            Init(size);
        }

        public void InitMax()
        {
            Init(MaxSize);
        }

        public void Clear(Color color)
        {
            if (Surface == null)
            {
                return;
            }

            Surface.Clear(color);
        }

        public void CopyTo(DirectBitmap target)
        {
            target.Init(Size);

            if (Copy.HasValue())
            {
                target.BeginTransparency();
            }
        }

        public bool Grow(int newSize)
        {
            return Init(Math.Max(newSize, Size.Width), Math.Max(newSize, Size.Height));
        }

        public bool Grow(int width, int height)
        {
            return Init(Math.Max(width, Size.Width), Math.Max(height, Size.Height));
        }

        public bool Init(int newSize)
        {
            return Init(newSize, newSize);
        }

        public bool Init(Size newSize)
        {
            return Init(newSize.Width, newSize.Height);
        }

        public bool Init(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return false;
            }

            if (Size.Width == width && Size.Height == height)
            {
                return false;
            }

            Dispose();

            PixelData = new int[width * height];
            Handle = GCHandle.Alloc(PixelData, GCHandleType.Pinned);
            Image = new Bitmap(width, height, Stride = 4 * ((width * BytesPerPixel + 3) / 4), Format, Handle.AddrOfPinnedObject());

            Size.Width = width;
            Size.Height = height;

            return Refresh();
        }

        public bool Refresh()
        {
            Surface.DisposeSafely();

            if (Image == null)
            {
                return false;
            }

            Surface = Graphics.FromImage(Image);
            Surface.SetHighQuality();

            return true;
        }

        public void SetBits(int[] bits)
        {
            if (bits == null || bits.Length == 0)
            {
                return;
            }

            if (bits.Length == PixelData.Length)
            {
                bits.CopyTo(PixelData, 0);
            }
            else
            {
                Init((int)Math.Sqrt(bits.Length));
                bits.CopyTo(PixelData, 0);
            }
        }

        public void BeginTransparency()
        {
            if (IsTransparent)
            {
                return;
            }

            try
            {
                SurfaceOriginal = Surface;

                if (Copy == null)
                {
                    Copy = new DirectBitmap(Size);
                }
                else
                {
                    Copy.Surface = Graphics.FromImage(Copy.Image);
                    Copy.Surface.Clear(Color.Empty);
                }

                Surface = Copy.Surface;
                Surface.SetHighQuality();
            }
            finally
            {
                IsTransparent = true;
            }
        }

        public void EndTransparency(int percent)
        {
            EndTransparency(Convert.ToByte(Math.Ceiling((255F * (100 - percent)) / 100F)));
        }

        public void EndTransparency(byte alpha)
        {
            try
            {
                if (IsTransparent == false || Copy == null)
                {
                    return;
                }

                Surface.DisposeSafely();
                Surface = SurfaceOriginal;
                Surface.SetHighQuality();
                DrawImage(Surface, Copy.Image, 0, 0, alpha);

                Copy.Dispose();
                Copy = default(DirectBitmap);
            }
            finally
            {
                IsTransparent = false;
            }
        }

        public void SetPixel(int x, int y, Color c)
        {
            if (Size.IsEmpty || x < 0 || y < 0 || x > Size.Width || y > Size.Height)
            {
                return;
            }

            int i = x + y * Size.Width;

            if (i < 0 || i >= PixelData.Length)
            {
                return;
            }

            PixelData[i] = c.ToArgb();
        }

        public Color GetPixel(int x, int y)
        {
            return Color.FromArgb(PixelData[x + y * Size.Width]);
        }

        public static void DrawImage(Graphics g, Image img, float x, float y, int alpha)
        {
            lock (AlphaAttributes)
            {
                var srcRect = new RectangleF(0, 0, img.Width, img.Height);
                var destPoints = new PointF[] { new PointF(x, y), new PointF(x + img.Width, y), new PointF(x, y + img.Height) };

                AlphaMatrix.Matrix33 = alpha / 255F;
                AlphaAttributes.SetColorMatrix(AlphaMatrix);

                g.DrawImage(img, destPoints, srcRect, GraphicsUnit.Pixel, AlphaAttributes);
            }
        }

        public void Parse(string value)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Join("-", PixelData);
        }

        public DirectBitmap Clone()
        {
            return new DirectBitmap();
        }

        public void Dispose()
        {
            PixelData = null;

            if (Handle.IsAllocated)
            {
                Handle.Free();
                Image.Dispose();
                Surface.Dispose();
                SurfaceOriginal.DisposeSafely();
            }

            Size = default(Size);
            Image = default(Bitmap);
            Surface = default(Graphics);
            SurfaceOriginal = default(Graphics);

            GC.SuppressFinalize(this);
        }
    }
}
