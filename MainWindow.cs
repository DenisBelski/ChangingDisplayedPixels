using System.Diagnostics;

namespace ChangingDisplayedPixels
{
    public partial class MainWindow : Form
    {
        private List<Bitmap> bitmaps = new List<Bitmap>();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var stopWatch = Stopwatch.StartNew();

                menuStrip.Enabled = false;
                trackBar.Enabled = false;
                pictureBox.Image = null;
                bitmaps.Clear();

                var bitmap = new Bitmap(openFileDialog.FileName);
                await Task.Run(() => { RunProcessing(bitmap); });

                menuStrip.Enabled = true;
                trackBar.Enabled = true;
                stopWatch.Stop();

                TimeSpan timeSpan = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

                Text = elapsedTime;
            } 
        }

        private void RunProcessing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);

            for (int i = 1; i < trackBar.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = random.Next(pixels.Count);
                    currentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }

                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach (var pixel in currentPixelsSet)
                {
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }

                bitmaps.Add(currentBitmap);
                this.Invoke(new Action(() =>
                {
                    Text = $"{i} %";
                }));

            }

            bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y },
                    });
                }
            }

            return pixels;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (bitmaps == null || bitmaps.Count == 0)
            {
                return;
            }

            pictureBox.Image = bitmaps[trackBar.Value - 1];
        }
    }
}