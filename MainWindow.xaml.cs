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
using System.ComponentModel;
using RenderEngineVIZ;
using RenderEngineVIZ.CommandSenderVIZ;
using RenderEngineVIZ.CommandSender;



namespace VAS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        Timer readStateTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        // A partir de este dato, se ha empezado a utilizar los bindings
        // TODO: Ir pasando todos los datos
        bool _activateSCIM;
        public bool ActivateSCIM
        {
            get { return _activateSCIM; }
            set { _activateSCIM = value; OnPropertyChanged("ActivateSCIM"); }
        }

        string _defaultSceneToReload;
        public string DefaultSceneToReload
        {
            get { return _defaultSceneToReload; }
            set { _defaultSceneToReload = value; OnPropertyChanged("DefaultSceneToReload"); }
        }


        private double _sendDelay;
        public double SendDelay
        {
            get { return _sendDelay; }
            set
            {
                if (_sendDelay == value)
                    return;

                _sendDelay = value;
                OnPropertyChanged("SendDelay");
            }
        }

        class ConfigInfo
        {
            public cvfn.ProcessorTechnology ProcessorTech;
            public cvfn.Rect AreaTracking;
            public uint MinPoints;
            public bool ActivateSBD;
            public double ThresholdSBD;
            public bool ResizeFrame;
            public cvfn.Size2D ResizeFrameSize;
            public cvfn.Size2D CaptureFrameSize;
            public double CaptureFrameRate;
            public uint MaxDistAnchorInterframe;
        }

        bool mSliderUpdateByFilm = false;

        IGraphicsService mGraphicsService;
        IComputerVisionManager mComputerVisionManager; // = new ComputerVisionManager();


        //UniversalNETCom.UniversalNETCom mUnc;


        public MainWindow()
        {
            InitializeComponent();

            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.Save();
            }


            /*ServiceProvider.ServiceProviderUDPMulticast v = new ServiceProvider.ServiceProviderUDPMulticast(IPAddress.Parse("225.0.140.1"), 11972);
            telemetryNET.Start();
            mUnc = new UniversalNETCom.UniversalNETCom( new CLProtocol.Codecs.CodecTXTSerial_02(),
                                                        telemetryNET);*/



            this.Title = "Video Analyzer System - Prototype v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            mComputerVisionManager = new ComputerVisionManager();

            mComputerVisionManager.OnKeyboardEvent += mComputerVisionManager_OnKeyboardEvent;

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
            TxtResizeAt.Text = Settings.Default.FrameWorkingSize;
            ChkResize.IsChecked = Settings.Default.ActivateResize;
            TxtCaptureResolution.Text = Settings.Default.CaptureResolution;
            TxtCaptureFrameRate.Text = Settings.Default.CaptureFrameRate.ToString("N2");
            TxtInterFrameAnchorDisp.Text = Settings.Default.MaxAnchorDisplacement.ToString();


            //TxtSendDataDelay.Text = Settings.Default.SendDataDelay.ToString();
            SendDelay = Settings.Default.SendDataDelay;

            DefaultSceneToReload = Settings.Default.DefaultSceneToReload;

            ActivateSCIM = Settings.Default.ActivateSCIM;

        }

        void mComputerVisionManager_OnKeyboardEvent(object sender, KeyboardEvtArgs e)
        {
            this.Dispatcher.Invoke(delegate()
            {
                const int KeyQ = 113;
                const int KeyA = 97;
                const int KeyW = 119;
                const int KeyE = 101;
                const int KeyS = 115;
                const int KeyD = 100;


                const int KeyR = 114;
                const int KeyT = 116;
                const int KeyY = 121;
                const int KeyU = 117;
                const int KeyI = 105;
                const int KeyO = 111;
                const int KeyP = 112;

                const int KeyF = 102;
                const int KeyG = 103;
                const int KeyH = 104;
                const int KeyJ = 106;
                const int KeyK = 107;
                const int KeyL = 108;
                const int KeyÑ = 241;


                switch (e.KbCode)
                {
                    case KeyQ:
                        onKey( Key.Q ); break;
                    case KeyA:
                        onKey( Key.A ); break;
                    case KeyW:
                        onKey( Key.W ); break;
                    case KeyE:
                        onKey( Key.E ); break;
                    case KeyS:
                        onKey( Key.S ); break;
                    case KeyD:
                        onKey( Key.D ); break;
                    case KeyR:
                        onKey(Key.R); break;
                    case KeyT:
                        onKey(Key.T); break;
                    case KeyY:
                        onKey(Key.Y); break;
                    case KeyU:
                        onKey(Key.U); break;
                    case KeyI:
                        onKey(Key.I); break;
                    case KeyO:
                        onKey(Key.O); break;
                    case KeyP:
                        onKey(Key.P); break;
                    case KeyF:
                        onKey(Key.F); break;
                    case KeyG:
                        onKey(Key.G); break;
                    case KeyH:
                        onKey(Key.H); break;
                    case KeyJ:
                        onKey(Key.J); break;
                    case KeyK:
                        onKey(Key.K); break;
                    case KeyL:
                        onKey(Key.L); break;
                    case KeyÑ:
                        onKey(Key.Oem3 ); break;

                    

                }
            });
        }



        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }





        public void connectVIZ(string tcpadd, string udpadd, int tcpport, int udpport)
        {

            ParamsCreateScreenVIZ paramsVIZ = new ParamsCreateScreenVIZ(
                IPAddress.Parse(tcpadd),
                tcpport,
                IPAddress.Parse(udpadd),
                udpport);

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
                int MAXITEMS = 25;

                this.LstCommands.Items.Add(e.Name);

                while (this.LstCommands.Items.Count >= MAXITEMS)
                    this.LstCommands.Items.RemoveAt(MAXITEMS - 1);
            });
        }

        void RenderEngine_CommandValueSentEvent(object sender, RenderEngine.Events.NewValueArgs e)
        {
            this.Dispatcher.Invoke(delegate()
            {
                TxtAnchorPt.Text = e.Name + ": " + e.Value;
            });
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool getAndValidateConfig(ConfigInfo cf)
        {
            string areaInput = TxtAreaTracking.Text;
            string errMessage = "";
            int[] sizeAreaTracking = new int[2];
            if (!Regex.IsMatch(areaInput, @"^[0-9]+,([0-9]+){1}$"))
            {
                errMessage += "Area Tracking debe ser Width,Height \n";
            }
            else
            {
                sizeAreaTracking = Array.ConvertAll(areaInput.Split(','), int.Parse);
            }


            int minPoints;
            Int32.TryParse(TxtMinPoints.Text, out minPoints);

            if (minPoints < 3)
            {
                errMessage += "MinPoints debe ser mayor de 2 \n";
            }

            ProcessorTechnology ptech;
            string tech = CmbProcTech.Text;
            if (tech == "GPU")
                ptech = ProcessorTechnology.GPU;
            else
                ptech = ProcessorTechnology.CPU;


            uint maxDistAnchorInterframe;
            UInt32.TryParse(TxtInterFrameAnchorDisp.Text, out maxDistAnchorInterframe);



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

            bool resizeFrame = false;
            if (ChkResize.IsChecked != null)
            {
                resizeFrame = ChkResize.IsChecked.Value;
            }

            int[] sizeFrame = new int[2];
            if (resizeFrame)
            {
                string resizeSize = TxtResizeAt.Text;
                if (!Regex.IsMatch(resizeSize, @"^[0-9]+,([0-9]+){1}$"))
                {
                    errMessage += "Area Tracking debe ser Width,Height \n";
                }

                sizeFrame = Array.ConvertAll(resizeSize.Split(','), int.Parse);
            }


            string captureSize = TxtCaptureResolution.Text;

            int[] captureFrameSize = new int[2];
            if (!Regex.IsMatch(captureSize, @"^[0-9]+,([0-9]+){1}$"))
            {
                if (RadFromDevice.IsChecked.Value) // sólo informar si ha sido seleccionado
                {
                    errMessage += "Capture Resolution debe ser Width,Height \n";
                }
            }
            else
            {
                captureFrameSize = Array.ConvertAll(captureSize.Split(','), int.Parse);
            }
            cf.CaptureFrameSize = new cvfn.Size2D(captureFrameSize[0], captureFrameSize[1]);

            double captureRate;
            Double.TryParse(TxtCaptureFrameRate.Text, out captureRate);
            cf.CaptureFrameRate = captureRate;



            if (errMessage != "")
            {
                MessageBox.Show(errMessage);
                return false;
            }


            cf.ProcessorTech = ptech;
            cf.MinPoints = (uint)minPoints;
            cf.AreaTracking = new cvfn.Rect(400, 400, sizeAreaTracking[0], sizeAreaTracking[1]);
            cf.ResizeFrame = resizeFrame;
            cf.ResizeFrameSize = new cvfn.Size2D(sizeFrame[0], sizeFrame[1]);
            cf.ActivateSBD = sbd;
            cf.ThresholdSBD = sbdThreshold;
            cf.MaxDistAnchorInterframe = maxDistAnchorInterframe;


            return true;
        }



        private void ShowOut(string name)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand(name, "Show()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HideOut(string name)
        {
            try
            {
                mGraphicsService.UserCommandParser.NewCommand(name, "Hide()", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void Anchor_Checked(object sender, RoutedEventArgs e)
        {
            ShowOut("AnchorInfo1");
        }

        private void Anchor_Unchecked(object sender, RoutedEventArgs e)
        {
            HideOut("AnchorInfo1");
        }

        private void Anchor2_Checked(object sender, RoutedEventArgs e)
        {
            ShowOut("AnchorInfo2");
        }

        private void Anchor3_Checked(object sender, RoutedEventArgs e)
        {
            ShowOut("AnchorInfo3");
        }

        private void Anchor2_Unchecked(object sender, RoutedEventArgs e)
        {
            HideOut("AnchorInfo2");
        }

        private void Anchor3_Unchecked(object sender, RoutedEventArgs e)
        {
            HideOut("AnchorInfo3");
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
                if (!ActivateSCIM)
                {

                    mComputerVisionManager.setCCFeatureTracker(
                        cfgInfo.AreaTracking,
                        cfgInfo.MinPoints,
                        cfgInfo.ActivateSBD);


                }
                else
                {
                    mComputerVisionManager.setSCIMFeatureTracker(
                        cfgInfo.AreaTracking,
                        cfgInfo.MinPoints,
                        cfgInfo.ActivateSBD,
                        cfgInfo.ThresholdSBD,
                        cfgInfo.MaxDistAnchorInterframe);
                }

                if (fromDevice)
                {
                    if (!ChkDecklink.IsChecked.Value)
                    {
                        mComputerVisionManager.startVideoProcessorFromDevice(device, cfgInfo.CaptureFrameSize, cfgInfo.CaptureFrameRate, cfgInfo.ResizeFrame, cfgInfo.ResizeFrameSize);
                    }
                    else
                    {
                        mComputerVisionManager.startVideoProcessorFromDecklinkDevice(cfgInfo.CaptureFrameSize, cfgInfo.ResizeFrame, cfgInfo.ResizeFrameSize);
                    }
                }
                else
                    mComputerVisionManager.startVideoProcessorFromFile(fileName, cfgInfo.ResizeFrame, cfgInfo.ResizeFrameSize);


                StartTracker.Content = "Stop Tracker";

                GraphismVAS0.GraphismVAS2015_0.SendDataDelay = (uint)SendDelay;
                readStateTimer = new Timer(readStateCallback, null, 0, 250);

                ChkAnchor.IsEnabled = true;
                ChkAnchor2.IsEnabled = true;
                ChkAnchor3.IsEnabled = true;
            }
            else
            {
                StartTracker.IsChecked = false;
            }


            Settings.Default.AnchorSize = TxtAreaTracking.Text;
            Settings.Default.SBDActive = ChkActivateSBD.IsChecked.Value;
            Settings.Default.SBDThreshold = cfgInfo.ThresholdSBD;
            Settings.Default.FromDevice = fromDevice;
            Settings.Default.DeviceNumber = device;
            Settings.Default.FileName = fileName;
            Settings.Default.ActivateResize = cfgInfo.ResizeFrame;
            Settings.Default.FrameWorkingSize = TxtResizeAt.Text;
            Settings.Default.CaptureResolution = TxtCaptureResolution.Text;
            Settings.Default.CaptureFrameRate = cfgInfo.CaptureFrameRate;
            Settings.Default.SendDataDelay = (uint)SendDelay;
            Settings.Default.ActivateSCIM = ActivateSCIM;
            Settings.Default.Save();




        }

        private void readStateCallback(object obj)
        {
            this.Dispatcher.Invoke(delegate()
            {
                long pfr = mComputerVisionManager.getPotentialFrameRate();
                double avg = mComputerVisionManager.getAverageFrameTime();
                double pct = mComputerVisionManager.getRelativeVideoProgression();

                double readTime = mComputerVisionManager.getAverageGetFrameTime();
                double processTime = mComputerVisionManager.getAverageProcessTime();
                double waitingTime = mComputerVisionManager.getAverageWaitingTime();


                Dictionary<string, string> debugInfo = mComputerVisionManager.getDebugInfo();


                TxtPotentialFR.Content = pfr.ToString();
                TxtAverageFrameTime.Content = avg.ToString("N2");

                TxtDebugInfo.Content = readTime.ToString("N1") + " / " + processTime.ToString("N1") + " / " + waitingTime.ToString("N1");

                //dynamic d = mUnc.getData();

                mSliderUpdateByFilm = true;
                SlVideoProgression.Value = pct;
                mSliderUpdateByFilm = false;

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

            Settings.Default.TCPAddress = TxtTCPAddress.Text;
            Settings.Default.UDPAddress = TxtUDPAddress.Text;
            Settings.Default.TCPPort = tcpPort;
            Settings.Default.UDPPort = udpPort;
            Settings.Default.VIZSceneName = TxtScene.Text;
            Settings.Default.DefaultSceneToReload = TxtSceneReload.Text;

            Settings.Default.Save();

            connectVIZ(TxtTCPAddress.Text, TxtUDPAddress.Text, tcpPort, udpPort);

            enableStartTrack(true);
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

            wnd.Show();

        }

        private void SlVideoProgression_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!mSliderUpdateByFilm)
                mComputerVisionManager.setRelativeVideoProgression(e.NewValue);
        }

        private void TxtInterFrameAnchorDisp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.Filter = "Clip (*.avi;*.mov;*.mp4)|*.avi;*.mov;*.mp4";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                TxtFileName.Text = filename;
            }
        }


        private void BtnPausePlay_Checked(object sender, RoutedEventArgs e)
        {
            BtnPausePlay.Content = "Play";

            mComputerVisionManager.pause(true);
        }


        private void BtnPausePlay_Unchecked(object sender, RoutedEventArgs e)
        {
            BtnPausePlay.Content = "Pause";
            mComputerVisionManager.pause(false);
        }

        private void ChkSCIM_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChkSCIM_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSendDataDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            double dataDelay;

            double.TryParse(TxtSendDataDelay.Text, out dataDelay);

            GraphismVAS0.GraphismVAS2015_0.SendDataDelay = (uint)dataDelay;
        }


        private void MainWindow1_Closing(object sender, CancelEventArgs e)
        {
            if (readStateTimer != null)
            {
                readStateTimer.Dispose();
                readStateTimer = null;
            }

            // Grabamos los settings que se pueden haber modificado DURANTE el tracking
            Settings.Default.SendDataDelay = (uint)SendDelay;
            Settings.Default.Save();

            mComputerVisionManager.stopVideoProcessor();

        }

        private void MainWindow1_Closed(object sender, EventArgs e)
        {
            if (readStateTimer != null)
            {
                readStateTimer.Dispose();
                readStateTimer = null;
            }

        }

        private void MainWindow1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (readStateTimer != null)
            {
                readStateTimer.Dispose();
                readStateTimer = null;
            }

        }

        private void MainWindow1_KeyDown(object sender, KeyEventArgs e)
        {
            onKey(e.Key);
        }


        void onKey(Key thekey)
        {
            this.Dispatcher.Invoke(delegate()
            {
                switch (thekey)
                {
                    case Key.Q:
                        ShowOut("AnchorInfo1"); break;
                    case Key.A:
                        HideOut("AnchorInfo1"); break;
                    case Key.W:
                        ShowOut("AnchorInfo2"); break;
                    case Key.S:
                        HideOut("AnchorInfo2"); break;
                    case Key.E:
                        ShowOut("AnchorInfo3"); break;
                    case Key.D:
                        HideOut("AnchorInfo3"); break;

                    case Key.R:
                        ShowOut("AnchorInfo4"); break;
                    case Key.T:
                        ShowOut("AnchorInfo5"); break;
                    case Key.Y:
                        ShowOut("AnchorInfo6"); break;
                    case Key.U:
                        ShowOut("AnchorInfo7"); break;
                    case Key.I:
                        ShowOut("AnchorInfo8"); break;
                    case Key.O:
                        ShowOut("AnchorInfo9"); break;
                    case Key.P:
                        ShowOut("AnchorInfo10"); break;
                    case Key.F:
                        HideOut("AnchorInfo4"); break;
                    case Key.G:
                        HideOut("AnchorInfo5"); break;
                    case Key.H:
                        HideOut("AnchorInfo6"); break;
                    case Key.J:
                        HideOut("AnchorInfo7"); break;
                    case Key.K:
                        HideOut("AnchorInfo8"); break;
                    case Key.L:
                        HideOut("AnchorInfo9"); break;
                    case Key.Oem3:
                        HideOut("AnchorInfo10"); break;

                }
            });

        }

        private void BtnReloadScene_Click(object sender, RoutedEventArgs e)
        {
            enableStartTrack(false);

            Settings.Default.DefaultSceneToReload = TxtSceneReload.Text;
            Settings.Default.Save();


            if (mGraphicsService != null)
            {
                IRenderEngineVIZ reVIZ = mGraphicsService.RenderEngine as IRenderEngineVIZ;
                ICommandSenderVIZ cmsVIZ = reVIZ.CommandSender as ICommandSenderVIZ;

                ISceneParams theParam = new SceneParamsVIZ() { Layer = "", Name = TxtSceneReload.Text };
                cmsVIZ.LoadScene(theParam, "");
            }
        }
    }
}
