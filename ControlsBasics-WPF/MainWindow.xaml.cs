
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

        private const int PixelScrollByAmount = 20;

        private readonly KinectSensorChooser sensorChooser;
        private SpeechRecognitionEngine speechEngine;
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

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
            
            // Clear out placeholder content
            this.wrapPanel.Children.Clear();
            
            
             

                  RecognizerInfo ri = GetKinectRecognizer();
                  if (null != ri)
                  {
                      this.speechEngine = new SpeechRecognitionEngine(ri.Id);
                  }
      
            var directions = new Choices();
  directions.Add(new SemanticResultValue("forward", "FORWARD"));
  directions.Add(new SemanticResultValue("forwards", "FORWARD"));
  directions.Add(new SemanticResultValue("straight", "FORWARD"));
  directions.Add(new SemanticResultValue("backward", "BACKWARD"));
  directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
 directions.Add(new SemanticResultValue("back", "BACKWARD"));
 directions.Add(new SemanticResultValue("turn left", "LEFT"));
 directions.Add(new SemanticResultValue("turn right", "RIGHT"));
 var gb = new GrammarBuilder { Culture = ri.Culture };
 gb.Append(directions);
 
 var g = new Grammar(gb);
 speechEngine.LoadGrammar(g);
 //KinectSensor sensor = KinectSensor.KinectSensors[0];
 //sensor.Start();
 speechEngine.SpeechRecognized += SpeechRecognized;
 

 //speechEngine.SetInputToAudioStream(
   //  sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

 //speechEngine.RecognizeAsync(RecognizeMode.Multiple);
      

            String[] names = {"Snacks","Fruits","Vegetables","Cosmetics","Misc","Beverage"};
            for (var index = 0; index < 6; ++index)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("C:\\Users\\SUJATHA\\Documents\\Visual Studio 2013\\Projects\\GR ABZ\\ControlsBasics-WPF\\Images\\logo"+ index + ".jpg", UriKind.Relative);
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
   private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
  {
    // Speech utterance confidence below which we treat speech as if it hadn't been heard
    const double ConfidenceThreshold = 0.3;
    MessageBox.Show(e.Result.Text); 
    if (e.Result.Confidence >= ConfidenceThreshold)
    {

    }
  }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();
        }

        /// <summary>
        /// Handle a button click from the wrap panel.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (KinectTileButton)e.OriginalSource;
            /*var selectionDisplay = new SelectionDisplay(button.Label as string);
            this.kinectRegionGrid.Children.Add(selectionDisplay);
            e.Handled = true;*/
            this.sensorChooser.Stop();
            if (button.Label as string == "Check Out")
                {
                    if (Window1.sum > 0)
                    {
                        MessageBox.Show("The order has been placed successfully! The products will be delivered at your home in a within a working day :)", "Purchase Complete", MessageBoxButton.OK);
                        this.Close();
                        
                    }
                    
                
            else
                MessageBox.Show("Your Cart is empty :)", "No Purchase Made", MessageBoxButton.OK);
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
            cartAmt.Text = "Total Amount : " + Window1.sum.ToString() ;
            List<Product> carty = Window1.cart;
            lvUsers.ItemsSource = null;
            lvUsers.ItemsSource = carty;
            this.sensorChooser.Start();
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
    }
}
