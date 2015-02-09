using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;
using GraphicsService;

using cvfn;
using System.Text.RegularExpressions;
using VAS.Properties;
using System.Threading;



namespace VAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Timer readStateTimer;

        class ConfigInfo
        {
            public cvfn.ProcessorTechnology  ProcessorTech;
            public cvfn.Rect                 AreaTracking;
            public uint                 MinPoints;
            public bool                 ActivateSBD;
            public double               ThresholdSBD;

        }

        IGraphicsService mGraphicsService;
        IComputerVisionManager mComputerVisionManager; // = new ComputerVisionManager();

        public MainWindow()
        {
            InitializeComponent();
            
            mComputerVisionManager = new ComputerVisionManager();

            TxtTCPAddress.Text = Settings.Default.TCPAddress;
            TxtUDPAddress.Text = Settings.Default.UDPAddress;
            TxtTCPPort.Text = Settings.Default.TCPPort.ToString();
            TxtUDPPort.Text = Settings.Default.UDPPort.ToString();
            TxtAreaTracking.Text = Settings.Default.AnchorSize;
            ChkActivateSBD.IsChecked = Settings.Default.SBDActive;
            TxtSBDThreshold.Text = Settings.Default.SBDThreshold.ToString("N2");
            if (Settings.Default.FromDevice)
                RadFromDevice.IsChecked = true;
            else
                RadFromFile.IsChecked = true;

            TxtFileName.Text = Settings.Default.FileName;
            TxtDeviceNumber.Text = Settings.Default.DeviceNumber.ToString();
            TxtScene.Text = Settings.Default.VIZSceneName;
        }


        public void connectVIZ( string tcpadd, string udpadd, int tcpport, int udpport ) {
        
            ParamsCreateScreenVIZ paramsVIZ = new ParamsCreateScreenVIZ(
                IPAddress.Parse( tcpadd ),
                tcpport,
                IPAddress.Parse( udpadd ),
                udpport );

            string sceneName = TxtScene.Text;

            mGraphicsService = GraphicsServiceVAS0Factory.Create(paramsVIZ, sceneName, mComputerVisionManager);

            mGraphicsService.RenderEngine.CommandValueSentEvent += RenderEngine_CommandValueSentEvent;
            mGraphicsService.RenderEngine.CommandAnimationSentEvent += RenderEngine_CommandAnimationSentEvent;

            mGraphicsService.Start();
        }

        void RenderEngine_CommandAnimationSentEvent(object sender, RenderEngine.Events.NewAnimationArgs e)
        {
            this.Dispatcher.Invoke(delegate()
            {
                int MAXITEMS = 100;

                this.LstCommands.Items.Insert(0, e.Name);

                while (this.LstCommands.Items.Count >= MAXITEMS)
                    this.LstCommands.Items.RemoveAt(MAXITEMS - 1);
            });
        }

        void RenderEngine_CommandValueSentEvent(object sender, RenderEngine.Events.NewValueArgs e)
        {
            this.Dispatcher.Invoke(delegate()
            {
                TxtAnchorPt.Text = e.Name + " - " + e.Value;
            });
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool getAndValidateConfig(ConfigInfo cf)
        {
            string areaInput = TxtAreaTracking.Text;
            string errMessage = "";
            if (!Regex.IsMatch( areaInput, @"^[0-9]+,([0-9]+){1}$" )) 
            {
                errMessage += "Area Tracking debe ser Width,Height \n";
                return false;
            }

            int [] size = Array.ConvertAll(areaInput.Split(','), int.Parse);

            
            int minPoints;
            Int32.TryParse( TxtMinPoints.Text, out minPoints );

            if (minPoints < 3) {
                errMessage += "MinPoints debe ser mayor de 2 \n";
            }

            ProcessorTechnology ptech;
            string tech = CmbProcTech.Text;
            if (tech == "GPU")
                ptech = ProcessorTechnology.GPU;
            else
                ptech = ProcessorTechnology.CPU;



            double sbdThreshold = 0.0;
            bool sbd = false;
            if (ChkActivateSBD.IsChecked != null)
            {
                sbd = ChkActivateSBD.IsChecked.Value;
            }
            if (sbd)
            {
                Double.TryParse(TxtSBDThreshold.Text, out sbdThreshold);

                if (sbdThreshold < 0.01)
                    errMessage += "SBD Threshold es demasiado pequeño: no saltará nunca \n";
                if (sbdThreshold > 0.99)
                    errMessage += "SBD Threshold es demasiado grande: saltará muy a menudo \n";

            }

            if (errMessage != "")
            {
                MessageBox.Show(errMessage);
            }


            cf.ProcessorTech = ptech;
            cf.MinPoints = (uint)minPoints;
            cf.AreaTracking = new cvfn.Rect(400, 400, size[0], size[1]);
            cf.ActivateSBD = sbd;
            cf.ThresholdSBD = sbdThreshold;


            return true;
        }




        private void Anchor_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo1", "Show()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Anchor_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo1", "Hide()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Anchor2_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo2", "Show()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Anchor3_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo3", "Show()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Anchor2_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo2", "Hide()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Anchor3_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand("AnchorInfo3", "Hide()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void StartTracker_Checked(object sender, RoutedEventArgs e)
        {
            ConfigInfo cfgInfo = new ConfigInfo();

            bool fromDevice = RadFromDevice.IsChecked.Value;

            string fileName = TxtFileName.Text;
            int device;
            Int32.TryParse(TxtDeviceNumber.Text, out device);


            if (getAndValidateConfig(cfgInfo))
            {
                mComputerVisionManager.setSingleFeatureTrackCtrl(cfgInfo.ProcessorTech,
                    cfgInfo.AreaTracking,
                    cfgInfo.MinPoints,
                    cfgInfo.ActivateSBD,
                    cfgInfo.ThresholdSBD);

                if (fromDevice)
                    mComputerVisionManager.startVideoProcessorFromDevice(device);
                else
                    mComputerVisionManager.startVideoProcessorFromFile(fileName);
            }

            
            Settings.Default.AnchorSize = TxtAreaTracking.Text;
            Settings.Default.SBDActive = ChkActivateSBD.IsChecked.Value;
            Settings.Default.SBDThreshold = cfgInfo.ThresholdSBD;
            Settings.Default.FromDevice = fromDevice;
            Settings.Default.DeviceNumber = device;
            Settings.Default.FileName = fileName;
            Settings.Default.Save();


            StartTracker.Content = "Stop Tracker";

            readStateTimer = new Timer(readStateCallback, null, 0, 250);

            ChkAnchor.IsEnabled = true;
            ChkAnchor2.IsEnabled = true;
            ChkAnchor3.IsEnabled = true;
            

        }

        private void readStateCallback(object obj)
        {
            this.Dispatcher.Invoke(delegate()
            {
                long pfr = mComputerVisionManager.getPotentialFrameRate();
                double avg = mComputerVisionManager.getAverageFrameTime();

                TxtPotentialFR.Content = pfr.ToString();
                TxtAverageFrameTime.Content = avg.ToString("N2");
            });
        }

        private void StartTracker_Unchecked(object sender, RoutedEventArgs e)
        {
            mComputerVisionManager.stopVideoProcessor();
            StartTracker.Content = "Start Tracker";

            ChkAnchor.IsEnabled = false;
            ChkAnchor2.IsEnabled = false;
            ChkAnchor3.IsEnabled = false;
            
            
        }

        private void ChkConnectVIZ_Click(object sender, RoutedEventArgs e)
        {
            int tcpPort, udpPort;
            Int32.TryParse(TxtTCPPort.Text, out tcpPort);
            Int32.TryParse(TxtUDPPort.Text, out udpPort);

            connectVIZ(TxtTCPAddress.Text, TxtUDPAddress.Text, tcpPort, udpPort);

            Settings.Default.TCPAddress = TxtTCPAddress.Text;
            Settings.Default.UDPAddress = TxtUDPAddress.Text;
            Settings.Default.TCPPort = tcpPort;
            Settings.Default.UDPPort = udpPort;
            Settings.Default.VIZSceneName = TxtScene.Text;

            Settings.Default.Save();

            
            enableStartTrack( true );
        }


        private void enableStartTrack(bool enabled)
        {
            StartTracker.IsEnabled = enabled;
        }

        private void ChkActivateSBD_Checked(object sender, RoutedEventArgs e)
        {
            TxtSBDThreshold.IsEnabled = true;
        }

        private void ChkActivateSBD_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtSBDThreshold.IsEnabled = false;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help wnd = new Help();

        }




    }
}
