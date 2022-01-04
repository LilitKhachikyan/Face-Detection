using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;

namespace GUI2
{
    public partial class Form1 : Form
    {
        bool startPaiting = false;
        float x=0, y=0;
        Pen p = new Pen(Brushes.Black);
        Graphics gPanel;

        public Form1()
        {
            InitializeComponent();
            btnStart.Enabled = false;
            panel1.Hide();
            btnClear.Hide();
            pic2.Hide();
            gPanel = panel1.CreateGraphics(); 
        }

        FilterInfoCollection filter;
        VideoCaptureDevice device;

        private void Form1_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
                cboDevice.Items.Add(device.Name);
            cboDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("demqi_train.xml");

        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
              
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
                Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
                foreach (Rectangle rectangle in rectangles)
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        using (Pen pen = new Pen(Color.Blue, 1))
                        {
                            graphics.DrawRectangle(pen, rectangle);
                            btnStart.Enabled = true;
                        using (Bitmap bmp = new Bitmap(pic.Image))
                        {
                            var newImg = bmp.Clone(rectangle, bmp.PixelFormat);
                            pic2.Image = newImg;
                            pic2.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                    }
                    }
                }
                pic.Image = bitmap;
          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
            label1.Hide();
            cboDevice.Hide();
            btnDetect.Hide();
            pic.Hide();
            pic2.Show();
            btnStart.Hide();
            panel1.Show();
            btnClear.Show();

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            startPaiting = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(startPaiting)
            {
                if(x>0 && y>0)
                {
                    gPanel.DrawLine(p, x, y, e.X, e.Y);
                }
                x = e.X;
                y = e.Y;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            startPaiting = true;
            x = e.X;
            y = e.Y;
        }

    }
}
