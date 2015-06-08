using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;



namespace Microsoft.Samples.Kinect.ControlsBasics
{
    
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window 
    {
            
            public static List<Product> cart = new List<Product>();
            string[] prod = new string[100];
            string server = "localhost";
            string database = "Grabs";
            string uid = "root";
            string password = "";
            string connectionString;
            public static int sum;
            

        public string[] mySqlGet(String cat)
        {
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);
            ///Database Credentials
            int i = 0;

            string query = "SELECT * FROM `product` WHERE `Cat` = '" + cat + "'";
            MySqlCommand cmd = new MySqlCommand(query, connection);

            connection.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    prod[i] = reader["Name"].ToString();
                    i++;
                }

                connection.Close();
            }        
            return prod;
        }

     public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register(
            "PageLeftEnabled", typeof(bool), typeof(Window1), new PropertyMetadata(false));

        public static readonly DependencyProperty PageRightEnabledProperty = DependencyProperty.Register(
            "PageRightEnabled", typeof(bool), typeof(Window1), new PropertyMetadata(false));

        private const double ScrollErrorMargin = 0.001;

        private const int PixelScrollByAmount = 20;

        private readonly KinectSensorChooser sensorChooser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window1"/> class. 
        /// </summary>
        public Window1(String choice)
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
            

            string[] names = new string[] { };
            string type = "";
            
            // Add in display content
            switch (choice)
            {
                case "Snacks":
                    names = mySqlGet("Snacks");
                    type = "Snacks";
                    break;
                case "Fruits":
                    names =  mySqlGet("Fruits");
                    type = "Fruits";
                    break;
                case "Vegetables":
                    names =  mySqlGet("Vegetables");
                    type = "Vegetables";
                    break;
                case "Cosmetics":
                    names = mySqlGet("Cosmetics");
                    type = "Cosmetics";
                    break;
                case "Misc":
                    names = mySqlGet("Misc");
                    type = "Misc";
                    break;
                case "Beverage":
                    names = mySqlGet("Beverage");
                    type = "Beverage";
                    break;
                default:
                    break;
            }
            BitmapImage bim = new BitmapImage();
            bim.BeginInit();
            bim.UriSource = new Uri("C:\\Users\\SUJATHA\\Documents\\Visual Studio 2013\\Projects\\GR ABZ\\ControlsBasics-WPF\\Images\\images.png", UriKind.Relative);
            bim.EndInit();
                

            var but = new KinectTileButton
            {
                
                //Label = lst[i],
                Height = 200,
                Width = 250,
                Label = "Go Back",
                

            };
            this.wrapPanel.Children.Add(but);

            
            

            for (var index = 0; index < 10; ++index)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("C:\\Users\\SUJATHA\\Documents\\Visual Studio 2013\\Projects\\GR ABZ\\ControlsBasics-WPF\\Images\\"+type+ index + ".jpg", UriKind.Relative);
                bi.EndInit();
                if (index < names.Length)
                {
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
            }
            
            this.wrapPanel.Children.Add(this.Back);
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
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();
        }

        /// <summary>
        /// Handle a button click from the wrap panel.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        ///
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            
            
            
            var button = (KinectTileButton)e.OriginalSource;
            if(button.Label as String == "Go Back")
            {
                this.Close();
                return;
            }
            lvUsers.ItemsSource = null;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT `Price` FROM `product` WHERE `Name` = '" + button.Label as string + "'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (Product p in cart)
            {
                if (p.Name == button.Label as string)
                {      
                    p.Quantity += 1;
                    p.Amount = Int32.Parse(p.Cost) * p.Quantity;
                    sum += Int32.Parse(p.Cost);
                    lvUsers.ItemsSource = cart;
                    cartAmt.Text = "Tota Amount : " + sum.ToString();
                    return;
                }
            }
            connection.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
            
                    cart.Add(new Product() { Name = button.Label as string, Cost = reader["Price"].ToString(), Quantity = 1, Amount = Int32.Parse(reader["Price"].ToString())});        
                    sum += Int32.Parse(reader["Price"].ToString());
                }
                connection.Close();
            }
            
            lvUsers.ItemsSource = cart;
            cartAmt.Text = "Tota Amount : " + sum.ToString();
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

        private void KinectCircleButton_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
    public class Product
    {
        public string Name { get; set; }

        public string Cost { get; set; }

        public int Quantity { get; set; }

        public int Amount { get; set; }
    }
}
