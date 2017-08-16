//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Drawing;
    using Microsoft.Kinect;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;
    using System.IO;
using System.Configuration;

namespace Microsoft.Samples.Kinect.DepthBasics
{
    
    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MapDepthToByte = 8000 / 256;
        
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader = null;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription = null;
            
        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthBitmap = null;

        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] depthPixels = null;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        // Object Count
        private int blobCount = 0;

        //Object Data
        private List<MCvBox2D> objs;

        //Object Height
        private List<float> objH;

        //Approximate Poly's
        private List<String> poly;

        //Message Array
        private List<Message> msg;

        //Send Rate 
        private int sendRate = 0;

        //Bool Send
        private bool sendMsg;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();

            // open the reader for the depth frames
            this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();

            // wire handler for frame arrival
            this.depthFrameReader.FrameArrived += this.Reader_FrameArrived;

            // get FrameDescription from DepthFrameSource
            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // allocate space to put the pixels being received and converted
            this.depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];

            // create the bitmap to display
            this.depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            //Initialize Object List
            objs = new List<MCvBox2D>();
            objH = new List<float>();

            sendMsg = true;

            //Get Last Settings
            try{
                sliderMin.Value = Properties.Settings.Default.minDist;
                sliderMax.Value = Properties.Settings.Default.maxDist;
                sliderMinSize.Value = Properties.Settings.Default.minObjSize;
                sliderMaxSize.Value = Properties.Settings.Default.maxObjSize;
                renderScale.Value = Properties.Settings.Default.renderScale;
                offsetX.Value = Properties.Settings.Default.offsetX;
                offsetY.Value = Properties.Settings.Default.offsetY;
                roiXMin.Value = Properties.Settings.Default.roiXMin;
                roiXMax.Value = Properties.Settings.Default.roiXMax;
                roiYMin.Value = Properties.Settings.Default.roiYMin;
                roiYMax.Value = Properties.Settings.Default.roiYMax;
            }
            catch (SettingsPropertyNotFoundException ex) {
                    
            }    
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.depthBitmap;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.depthFrameReader != null)
            {
                // DepthFrameReader is IDisposable
                this.depthFrameReader.Dispose();
                this.depthFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        /// <summary>
        /// Handles the user clicking on the screenshot button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.depthBitmap != null)
            {
                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new PngBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(this.depthBitmap));

                string time = System.DateTime.UtcNow.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

                string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                string path = Path.Combine(myPhotos, "KinectScreenshot-Depth-" + time + ".png");

                // write the new file to disk
                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }

                    this.StatusText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SavedScreenshotStatusTextFormat, path);
                }
                catch (IOException)
                {
                    this.StatusText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedScreenshotStatusTextFormat, path);
                }
            }
        }

        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            bool depthFrameProcessed = false;

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    // the fastest way to process the body index data is to directly access 
                    // the underlying buffer
                    using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        // verify data and write the color data to the display bitmap
                        if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / this.depthFrameDescription.BytesPerPixel)) &&
                            (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                        {
                            // Note: In order to see the full range of depth (including the less reliable far field depth)
                            // we are setting maxDepth to the extreme potential depth threshold
                            ushort maxDepth = ushort.MaxValue;

                            // If you wish to filter by reliable depth distance, uncomment the following line:
                            //// maxDepth = depthFrame.DepthMaxReliableDistance
                            
                            this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, depthFrame.DepthMinReliableDistance, maxDepth);
                            processBlobs(depthFrame);     
                            depthFrameProcessed = true;
                        }
                    }
                }
            }

            if (depthFrameProcessed)
            {
                //this.RenderDepthPixels();
            }
        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort depth = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
            }
        }

        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.depthPixels,
                this.depthBitmap.PixelWidth,
                0);
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        private void processBlobs(DepthFrame depthFrame)
        {
            BitmapSource depthBmp = null;
            blobCount = 0;

            objs = new List<MCvBox2D>();
            poly = new List<String>();
            objH = new List<float>();

            depthBmp = depthFrame.SliceDepthImage((int)roiXMin.Value, (int)roiXMax.Value, (int)roiYMin.Value, (int)roiYMax.Value, (int)sliderMin.Value, (int)sliderMax.Value);

            Image<Bgr, Byte> openCVImg = new Image<Bgr, byte>(depthBmp.ToBitmap());
            Image<Gray, byte> gray_image = openCVImg.Convert<Gray, byte>();

            using (MemStorage stor = new MemStorage())
            {
                //Find contours with no holes try CV_RETR_EXTERNAL to find holes
                Contour<System.Drawing.Point> contours = gray_image.FindContours(
                 Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                 Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,
                 stor);

                ushort[] rawDepthData = new ushort[depthFrame.FrameDescription.Width * depthFrame.FrameDescription.Height];
                depthFrame.CopyFrameDataToArray(rawDepthData);
                //int width = depthFrame.FrameDescription.Width;

                for (int i = 0; contours != null; contours = contours.HNext)
                {
                    i++;

                    if ((contours.Area > Math.Pow(sliderMinSize.Value, 2)) && (contours.Area < Math.Pow(sliderMaxSize.Value, 2)))
                    {
                        /*
                        Contour<System.Drawing.Point> pts = contours.ApproxPoly(contours.Perimeter * 0.009, contours.Storage);
                        String temp = String.Format("- {0:00}: ",pts.ToArray().Length);
                        Message t_msg = new Message(poly.Count,pts.ToArray().Length);
                        foreach(System.Drawing.Point p in pts){
                            temp += String.Format("({0:000},{1:000})",p.X,p.Y);
                            t_msg.addVertex(p.X, (int)rawDepthData[(p.Y*width)+p.X], p.Y);
                        }
                        poly.Add(temp+"\n");
                        */

                        MCvBox2D box = contours.GetMinAreaRect();
                        
                                                
                        objs.Add(box);
                        objH.Add(calculateAvgHeight(depthFrame, box));
                        blobCount++;
                        sendMsg = true;
                            
                        
                        openCVImg.Draw(box, new Bgr(System.Drawing.Color.Red), 2);
                    }
                }
            }

            this.outImg.Source = ImageHelpers.ToBitmapSource(openCVImg);
            //txtBlobCount.Text = blobCount.ToString();
            //String infoText = "";
            String msg = "";

            if (dataTrans.IsChecked.Value)
            {
                Transmission.sendMessage(String.Format("Scale={0:00} BlobCount:{1:00} OffsetX:{2:-000;+000} OffsetY:{3:-000;+000} X({4:000},{5:000}) Y({6:000},{7:000})\n",
                    renderScale.Value, objs.Count, offsetX.Value, offsetY.Value, (int)roiXMin.Value, (int)roiXMax.Value, (int)roiYMin.Value, (int)roiYMax.Value));


                for (int c = 0; c < objs.Count; c++) {
                    msg = String.Format("${0:00}: Center:({1:000},{2:000}) Angle:{3:-000;+000} Size:({4:000},{5:000}) Heigth:{6:000}\n",
                        c, (int)objs[c].center.X, (int)objs[c].center.Y, objs[c].angle, (int)objs[c].size.Width, (int)objs[c].size.Height, (int)objH[c]);
                    //infoText += infoText;
                    Transmission.sendMessage(msg);                    
                }
            }
            /*
            for (int i = 0; i < poly.Count; i++) {
                Ola Menezes do futuro tens que resolver a cena dos valores negativos 
                if ((int)objH[i] >= 0){
                    infoText += String.Format("${0:00} H:{1:0000} ", i, (int)objH[i]) + poly[i];
                }
            }*/

            //txtInfo.Text = infoText;
            /*
            if (dataTrans.IsChecked.Value)
            {
                Transmission.sendMessage(infoText);
            }
            */
            //txtInfo.Text = "POLY COUNT : " + poly.Count + "   HEIGHT COUNT: " + objH.Count; 

            //sendRate = (sendRate + 1) % 4;

            //long memory = GC.GetTotalMemory(true) / 1048576;
            //txtBlobCount.Text += "      MEM: " + memory+"  MB";

            sendMsg = false;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Transmission.end();
            //Save User Settings
            Properties.Settings.Default.minDist = (int)sliderMin.Value;
            Properties.Settings.Default.maxDist = (int)sliderMax.Value;
            Properties.Settings.Default.minObjSize = (int)sliderMinSize.Value;
            Properties.Settings.Default.maxObjSize = (int)sliderMaxSize.Value;
            Properties.Settings.Default.renderScale = renderScale.Value;
            Properties.Settings.Default.offsetX = (int)offsetX.Value;
            Properties.Settings.Default.offsetY = (int)offsetY.Value;
            Properties.Settings.Default.roiXMin = (int)roiXMin.Value;
            Properties.Settings.Default.roiXMax = (int)roiXMax.Value;
            Properties.Settings.Default.roiYMin = (int)roiYMin.Value;
            Properties.Settings.Default.roiYMax = (int)roiYMax.Value;
            Properties.Settings.Default.Save();
        }

        private float calculateAvgHeight(DepthFrame depthFrame, MCvBox2D box) { 
            ushort[] rawDepthData = new ushort[depthFrame.FrameDescription.Width * depthFrame.FrameDescription.Height];
            depthFrame.CopyFrameDataToArray(rawDepthData);
            int W = depthFrame.FrameDescription.Width,x,y;
            ushort min = 9999,temp;

            x = (int)box.center.X;
            y = (int)box.center.Y;
            temp = rawDepthData[(W * y) + x];
            if (temp < min && temp !=0) { min = temp; }

            for (int i = 4; i <= 5; i++)
            {
                x = (int)(box.center.X + ((box.size.Width / i) * Math.Cos(box.angle)));
                y = (int)box.center.Y;
                if (x >= 0 && x < depthFrame.FrameDescription.Width && y >= 0 && y < depthFrame.FrameDescription.Height)
                {
                    temp = rawDepthData[(W * y) + x];
                    if (temp < min && temp != 0) { min = temp; }
                }

                x = (int)box.center.X;
                y = (int)(box.center.Y - ((box.size.Height / i) * Math.Sin(box.angle)));
                if (x >= 0 && x < depthFrame.FrameDescription.Width && y >= 0 && y < depthFrame.FrameDescription.Height)
                {
                    temp = rawDepthData[(W * y) + x];
                    if (temp < min && temp != 0) { min = temp; }
                }

                x = (int)(box.center.X - ((box.size.Width / i) * Math.Cos(box.angle)));
                y = (int)box.center.Y;
                if (x >= 0 && x < depthFrame.FrameDescription.Width && y >= 0 && y < depthFrame.FrameDescription.Height)
                {
                    temp = rawDepthData[(W * y) + x];
                    if (temp < min && temp != 0) { min = temp; }
                }

                x = (int)box.center.X;
                y = (int)(box.center.Y + ((box.size.Height / i) * Math.Sin(box.angle)));
                if (x >= 0 && x < depthFrame.FrameDescription.Width && y >= 0 && y < depthFrame.FrameDescription.Height)
                {
                    temp = rawDepthData[(W * y) + x];
                    if (temp < min && temp != 0) { min = temp; }
                }
            }

            return (float) (sliderMax.Value - min);
        }

    }
}
