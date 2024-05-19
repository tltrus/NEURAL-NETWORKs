using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyMINST.Classes;

namespace MyMINST
{
    public partial class MainWindow : Window
    {
        private string pixelFileTrain = @"train-images.idx3-ubyte";
        private string labelFileTrain = @"train-labels.idx1-ubyte";
        private int trainImgNum = 60000;

        private string pixelFileTest = @"t10k-images.idx3-ubyte";
        private string labelFileTest = @"t10k-labels.idx1-ubyte";
        private int testImgNum = 500;

        private DigitImage[] images28, images14;
        int imgIndex, imgLabel;
        Random rnd = new Random();
        WriteableBitmap bitMap14, bitMap28;
        NNclass nn;
        double[] input14 = new double[196]; // 14x14
        double[][,] inputs;
        List<double> epoh_error;
        int epohs = 3000;
        int epoh_cycles = 100;

        public MainWindow()
        {
            InitializeComponent();

            nn = new NNclass(
                196,               // inputs
                new int[] { 40 },  // hiddens
                10,                // outputs
                rnd
            );

            nn.SetLearningRate(0.1);
            nn.SetActivationFunction(NNclass.ActivateFunctions.Sigmoid);
        }

        private void btnLoadTrainData_Click(object sender, RoutedEventArgs e)
        {
            bool result = LoadData(pixelFileTrain, labelFileTrain, trainImgNum);
            if (result)
            {
                AddToConsole("Train files " + pixelFileTrain + ", " + labelFileTrain + " loaded.");
                AddToConsole("Loaded " + trainImgNum.ToString() + " train images.");
            }
        }
        private void btnConvertImage_Click(object sender, RoutedEventArgs e)
        {
            var result = Convert28to14Image();
            ShowImage();

            if (result) AddToConsole("Converting img 28x28 to 14x14 done");
        }
        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SafeToFile("idx" + imgIndex + "_image28_lab" + imgIndex + ".bmp", bitMap28);
            SafeToFile("idx" + imgIndex + "_image16_lab" + imgIndex + ".bmp", bitMap14);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) { imgIndex++; ShowImage(); lbResult.Content = "..."; }
        private void btnPrev_Click(object sender, RoutedEventArgs e) { if (imgIndex <= 0) return; imgIndex--; ShowImage(); lbResult.Content = "..."; }
        private async void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            AddToConsole("Training in progress. Epoh = " + epohs + " / cycles per epoh = " + epoh_cycles);

            pbTraining.Minimum = 0;
            pbTraining.Maximum = epohs;

            var progressIndicator = new Progress<int>((value) => { pbTraining.Value = value;  });
            await StartTrainingAsync(progressIndicator);

            AddToConsole("Training done");

            btnIdentify.IsEnabled = true;
            btnLoadTest.IsEnabled = true;
        }

        private void btnLoadTestData_Click(object sender, RoutedEventArgs e)
        {
            bool result = LoadData(pixelFileTest, labelFileTest, testImgNum);
            if (result)
            {
                AddToConsole("Test files " + pixelFileTest + ", " + labelFileTest + " loaded.");
                AddToConsole("Loaded " + testImgNum.ToString() + " test images.");
            }

            result = Convert28to14Image();
            if (result) AddToConsole("Test images converted to 14x14");
        }
        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            // Input
            DigitImage imageInput = images14[imgIndex];
            DigitImageToArray(imageInput, input14);

            var output = nn.Calculate(input14);
            var sum = output.Sum();
            var max = output.Max();
            var index = output.IndexOf(max);
            lbResult.Content = index.ToString() + " probability: " + (max * 100).ToString("F2") + " %";
        }
        private void btnConsoleClear_Click(object sender, RoutedEventArgs e) => rtbConsole.Document.Blocks.Clear();

        private void AddToConsole(string str)
        {
            if (str == null) return;
            rtbConsole.AppendText("\r" + str);
        }
        private async Task StartTrainingAsync(IProgress<int> progress)
        {
            int len = images14.Length;
            epoh_error = new List<double>();

            await Task.Run(() => 
            {
                for (int epoh = 0; epoh < epohs; epoh++)
                {
                    for (var i = 0; i < epoh_cycles; i++)
                    {
                        var index = i;
                        // Input
                        DigitImage imageInput = images14[index];
                        input14 = Array2DTo1D(inputs[i]);

                        // Target
                        double[] target = new double[10];
                        target[imageInput.label] = 1;

                        // Training
                        nn.Train(input14, target);

                        // For debugging - errors calculation
                        var output = nn.output;
                        var logs = MyMatrix.Log(output).To2DArray();
                        double sum = 0;
                        for (int k = 0; k < target.Length; k++)
                        {
                            sum += target[k] * logs[k, 0];
                        }
                        var sum2 = nn.output_errors.To2DArray().Sum();

                        if (epoh % 100 == 0)
                            epoh_error.Add(sum2);
                    }
                    progress.Report(epoh);
                }
            });
        }


        // -------- Logic --------------------------------------------------------------
        public bool LoadData(string pixelFile, string labelFile, int numImages)
        {
            // Load MNIST training set of 60,000 images into memory
            // remove static to access listBox1
            images28 = new DigitImage[numImages];

            byte[][] pixels = new byte[28][];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new byte[28];

            FileStream ifsPixels = new FileStream(pixelFile, FileMode.Open);
            FileStream ifsLabels = new FileStream(labelFile, FileMode.Open);

            BinaryReader brImages = new BinaryReader(ifsPixels);
            BinaryReader brLabels = new BinaryReader(ifsLabels);

            int magic1 = brImages.ReadInt32(); // stored as Big Endian
            magic1 = ReverseBytes(magic1); // convert to Intel format

            int imageCount = brImages.ReadInt32();
            imageCount = ReverseBytes(imageCount);

            int numRows = brImages.ReadInt32();
            numRows = ReverseBytes(numRows);
            int numCols = brImages.ReadInt32();
            numCols = ReverseBytes(numCols);

            int magic2 = brLabels.ReadInt32();
            magic2 = ReverseBytes(magic2);

            int numLabels = brLabels.ReadInt32();
            numLabels = ReverseBytes(numLabels);

            // each image
            for (int di = 0; di < numImages; ++di)
            {
                for (int i = 0; i < 28; ++i) // get 28x28 pixel values
                {
                    for (int j = 0; j < 28; ++j)
                    {
                        byte b = brImages.ReadByte();
                        pixels[i][j] = b;
                    }
                }

                byte lbl = brLabels.ReadByte(); // get the label
                DigitImage dImage = new DigitImage(28, 28, pixels, lbl);
                images28[di] = dImage;
            } // each image

            ifsPixels.Close(); brImages.Close();
            ifsLabels.Close(); brLabels.Close();

            if (images28.Length > 0) return true;
            return false;
        } // LoadData
        public int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }
        public WriteableBitmap MakeBitmap(DigitImage dImage, int mag)
        {
            // create a C# Bitmap suitable for display in a PictureBox control
            int width = dImage.width * mag;
            int height = dImage.height * mag;
            WriteableBitmap wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            for (int i = 0; i < dImage.height; ++i)
            {
                for (int j = 0; j < dImage.width; ++j)
                {
                    byte pixelColor = (byte)(255 - dImage.pixels[i][j]); // white background, black digits

                    Int32Rect rect = new Int32Rect(j * mag, i * mag, 1, 1);
                    byte[] colorData = { pixelColor, pixelColor, pixelColor, (byte)255 };
                    wb.WritePixels(rect, colorData, 4, 0);
                }
            }
            return wb;
        }
        public class DigitImage
        {
            // an MNIST image of a '0' thru '9' digit
            public int width; // 28
            public int height; // 28
            public byte[][] pixels; // 0(white) - 255(black)
            public byte label; // '0' - '9'

            public DigitImage(int width, int height, byte[][] pixels, byte label)
            {
                this.width = width; this.height = height;
                this.pixels = new byte[height][];
                for (int i = 0; i < this.pixels.Length; ++i)
                    this.pixels[i] = new byte[width];

                for (int i = 0; i < height; ++i)
                    for (int j = 0; j < width; ++j)
                        this.pixels[i][j] = pixels[i][j];

                this.label = label;
            }
        }
        private void ShowImage()
        {
            DigitImage img28 = images28[imgIndex];
            bitMap28 = MakeBitmap(img28, 1);
            image28.Source = bitMap28;

            DigitImage img14 = images14[imgIndex];
            bitMap14 = MakeBitmap(img14, 1);
            image14.Source = bitMap14;

            imgLabel = img28.label;
            lbIndex.Content = imgIndex.ToString();
            lbLabel.Content = imgLabel;
        }
        private bool Convert28to14Image()
        {
            var len = images28.Length; // 60 000
            images14 = new DigitImage[len];
            inputs = new double[len][,];

            for (int i = 0; i < len; i++)
            {
                // Заполняем массив trainImages14
                int width = 14;
                int height = 14;
                byte label = images28[i].label;
                byte[][] pixels = new byte[height][];

                inputs[i] = new double[14, 14];

                for (int y = 0; y < height; y++)
                {
                    pixels[y] = new byte[width];
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y][x] = images28[i].pixels[y * 2][x * 2];

                        // Заполняем массив inputs
                        inputs[i][y, x] = pixels[y][x];
                    }
                }
                images14[i] = new DigitImage(width, height, pixels, label);
            }

            return true;
        }

        private void DigitImageToArray(DigitImage imageInput, double[] inp)
        {
            for (int i = 0; i < imageInput.height; i++)
                for (int j = 0; j < imageInput.width; j++)
                {
                    var idx = imageInput.width * i + j;
                    var val = (double)imageInput.pixels[i][j];
                    //val = Numerics.Map(val, 0, 255, 0, 10);
                    //if (val > 0) val = 1;
                    inp[idx] = val / 255;
                }
        }
        private double[] Array2DTo1D(double[,] array)
        {
            var rows = array.GetLength(0);
            var cols = array.GetLength(1);
            var len = rows * cols;

            double[] result = new double[len];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    var idx = cols * i + j;
                    var val = array[i,j];
                    result[idx] = val / 255;
                }
            return result;
        }
        private void SafeToFile(string filename, BitmapSource image)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image));
                    encoder5.Save(stream);
                }
            }
        }
    }
}
