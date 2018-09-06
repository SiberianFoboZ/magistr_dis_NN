using System;
using System.Linq;
using System.Windows.Forms;
using System.Numerics;
using NAudio.Wave;


namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public WaveIn wi;
        public BufferedWaveProvider bwp;
        private int bitRate = 44100;
        private int bufferSize = (int)Math.Pow(2, 11);

        public Form1()
        {
            InitializeComponent();
            int devcount = WaveIn.DeviceCount;
            Console.Out.WriteLine("Device Count {0}", devcount);
            WaveIn wi = new WaveIn
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(bitRate, 1)
            };
            wi.DataAvailable += new EventHandler<WaveInEventArgs>(Wi_DataAvailable);
            bwp = new BufferedWaveProvider(wi.WaveFormat)
            {
                BufferLength = bufferSize * 2,
                DiscardOnBufferOverflow = true
            };
            wi.StartRecording();
        }
        void Wi_DataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
        public void UpdateAudioGrafh()
        {
            int frameSize = bufferSize;
            var frames = new byte[frameSize];
            bwp.Read(frames, 0, frameSize);
            if (frames.Length == 0) return;
            if (frames[frameSize - 2] == 0) return;
            timer1.Enabled = false;
            int SAMLPLE_RESOLUTOIN = 16;
            int BYTE_PER_POINT = SAMLPLE_RESOLUTOIN / 8;
            Int32[] vals = new Int32[frames.Length / BYTE_PER_POINT];
            double[] Ys = new double[frames.Length / BYTE_PER_POINT];
            double[] Xs = new double[frames.Length / BYTE_PER_POINT];
            double[] Ys2 = new double[frames.Length / BYTE_PER_POINT];
            double[] Xs2 = new double[frames.Length / BYTE_PER_POINT];
            for (int i = 0; i < vals.Length; i++)
            {
                byte hByte = frames[i * 2 + 1];
                byte iByte = frames[i * 2+0];
                vals[i] = (int)(short)((hByte << 8) | iByte);
                Xs[i] = i;
                Ys[i] = vals[i];
                Xs2[i] = (double)i / Ys.Length * bitRate / 1000.0;
                              
            }
           
            scottPlotUC1.PlotXY(Xs, Ys,System.Drawing.Color.Red);
            Ys2 = FFT(Ys);
            scottPlotUC2.PlotXY(Xs2.Take(Xs2.Length / 2).ToArray(), Ys2.Take(Ys2.Length / 2).ToArray(),null);
            scottPlotUC1.Clear();
            scottPlotUC2.Clear();
            Application.DoEvents();
            scottPlotUC1.Update();
            scottPlotUC2.Update();
            timer1.Enabled = true;

        }
        public double[] FFT(double[] data)
        {
            double[] fft = new double[data.Length];
            Complex[] fftComplex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);
            }
            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            for (int i = 0; i < data.Length; i++)
            {
                fft[i] = fftComplex[i].Magnitude;
            }
            return fft;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            UpdateAudioGrafh();
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateAudioGrafh();
        }

        private void scottPlotUC1_Load(object sender, EventArgs e)
        {

        }
    }
}
