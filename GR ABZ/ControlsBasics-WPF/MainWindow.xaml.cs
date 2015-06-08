
namespace Microsoft.Samples.Kinect.ControlsBasics
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Data.SqlClient;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;
    using Microsoft.Speech.Recognition;
    using Microsoft.Speech.AudioFormat;
    using System.IO;
    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        
        public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register(
            "PageLeftEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty PageRightEnabledProperty = DependencyProperty.Register(
            "PageRightEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        private const double ScrollErrorMargin = 0.001;
        private KinectSensor sensor;
        private SpeechRecognitionEngine speechEngine;
        private const int PixelScrollByAmount = 20;

        private readonly KinectSensorChooser sensorChooser;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class. 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            

            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            //this.wrapPanel.Children.Add(selectionDisplay);            
            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
            
            // Clear out placeholder content
            this.wrapPanel.Children.Clear();

            

            String[] names = {"Snacks","Fruits","Vegetables","Cosmetics","Misc","Beverage"};
            for (var index = 0; index < 6; ++index)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("H:\\GRABS\\GR ABZ\\GR ABZ\\ControlsBasics-WPF\\Images\\logo" + index + ".jpg", UriKind.Relative);
                bi.EndInit();        
                    
                var button = new KinectTileButton
                {
                    Background = new ImageBrush(bi),
                    //Label = lst[i],
                    Height = 200,
                    Width = 250,
                    Label = names[index]
                };
                this.wrapPanel.Children.Add(button);
            }

            var buton = new KinectTileButton
            {
                
                //Label = lst[i],
                Height = 200,
                Width = 250,
                Label = "Check Out"
            };
            this.wrapPanel.Children.Add(buton);
            // Bind listner to scrollviwer scroll position change, and check scroll viewer position
            this.UpdatePagingButtonState();
            scrollViewer.ScrollChanged += (o, e) => this.UpdatePagingButtonState();
        }

        /// <summary>
        /// CLR Property Wrappers for PageLeftEnabledProperty
        /// </summary>
        public bool PageLeftEnabled
        {
            get
            {
                return (bool)GetValue(PageLeftEnabledProperty);
            }

            set
            {
                this.SetValue(PageLeftEnabledProperty, value);
            }
        }

        /// <summary>
        /// CLR Property Wrappers for PageRightEnabledProperty
        /// </summary>
        public bool PageRightEnabled
        {
            get
            {
                return (bool)GetValue(PageRightEnabledProperty);
            }

            set
            {
                this.SetValue(PageRightEnabledProperty, value);
            }
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {
                    // Start the sensor!
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
                

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                 var directions = new Choices();
                 directions.Add(new SemanticResultValue("snacks", "SNACKS"));
                 directions.Add(new SemanticResultValue("vegetables", "VEGETABLES"));
                 directions.Add(new SemanticResultValue("fruits", "FRUITS"));
                 directions.Add(new SemanticResultValue("beverage", "BEVERAGE"));
                 directions.Add(new SemanticResultValue("cosmetics", "COSMETICS"));
                 directions.Add(new SemanticResultValue("miscellaneous", "MISC"));
                 directions.Add(new SemanticResultValue("checkout", "CHECKOUT"));
                 directions.Add(new SemanticResultValue("check out", "CHECKOUT"));
                
                 var gb = new GrammarBuilder { Culture = ri.Culture };
                 gb.Append(directions);
                
                 var g = new Grammar(gb);
                 
                

                speechEngine.LoadGrammar(g);
                

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {

            }
      

        }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        /// <summary>
        /// Handle a button click from the wrap panel.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (KinectTileButton)e.OriginalSource;
            
            this.sensorChooser.Stop();
            if (button.Label as string == "Check Out")
            {
                String name = "The order has been placed successfully! \nThe products will be delivered at your home within a working day :)";
                if (Window1.sum == 0)
                    name = "Add items to cart before checking out!!!";
                //MessageBox.Show("The order has been placed successfully! The products will be delivered at your home in a within 2 working days :)", "Purchase Complete", MessageBoxButton.OK);
                var selectionDisplay = new SelectionDisplay(name);
                this.kinectRegionGrid.Children.Add(selectionDisplay);
                // e.Handled = true;
                return;
            }
            Window1 w = new Window1(button.Label as string);
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.SourceInitialized += (s, a) => w.WindowState = WindowState.Maximized;
            w.Show();
            
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            this.sensorChooser.Start();
            cartAmt.Text = "Total Amount : " + Window1.sum.ToString() ;
            List<Product> carty = Window1.cart;
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = carty;
            
        }
        /// <summary>
        /// Handle paging right (next button).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PageRightButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + PixelScrollByAmount);
        }

        /// <summary>
        /// Handle paging left (previous button).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PageLeftButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - PixelScrollByAmount);
        }

        /// <summary>
        /// Change button state depending on scroll viewer position
        /// </summary>
        private void UpdatePagingButtonState()
        {
            this.PageLeftEnabled = scrollViewer.HorizontalOffset > ScrollErrorMargin;
            this.PageRightEnabled = scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth - ScrollErrorMargin;
        }
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.2;
            Window1 w = null;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "SNACKS":
                        w = new Window1("Snacks");
                        break;

                    case "FRUITS":
                        w = new Window1("Fruits");
                        break;

                    case "VEGETABLES":
                        w = new Window1("Vegetables");
                        break;

                    case "COSMETICS":
                        w = new Window1("Cosmetics");
                        break;

                    case "MISC":
                        w = new Window1("Misc");
                        break;

                    case "BEVERAGE":
                        w = new Window1("Beverage");
                        break;

                    case "CHECKOUT":
                        var selectionDisplay = new SelectionDisplay("The order has been placed successfully! \n The products will be delivered at your home in a within a working day :)");
                        this.kinectRegionGrid.Children.Add(selectionDisplay);
                        return;


                }
                this.sensorChooser.Stop();
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.SourceInitialized += (s, a) => w.WindowState = WindowState.Maximized;
                w.Show();

            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            
        }
    }
}
