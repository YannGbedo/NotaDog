using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace NotaDog.Windows
{
    public class BaseWindow : Window
    {
        public BaseWindow()
        {
            Title = "NotaDog";
            Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/NotaDog.ico"));
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Height = 400;
            Width = 500;

            // Set default minimum size
            MinHeight = 350;
            MinWidth = 500;

            //sizing window
            Loaded += BaseWindow_Loaded;
            Closing += BaseWindow_Closing;

            // Catching Unhandled exceptions
            System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle exception
            System.Windows.MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        protected void Log(string message)
        {
            // Implement logging logic
            Console.WriteLine($"{DateTime.Now}: {message}");
        }

        protected void ShowError(string errorMessage)
        {
            System.Windows.MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void SwitchToWindow(Window newWindow)
        {
            // Copier les dimensions de la fenêtre actuelle vers la nouvelle fenêtre
            newWindow.Height = Height;
            newWindow.Width = Width;
            newWindow.Left = Left;
            newWindow.Top = Top;
            newWindow.WindowState = WindowState;

            // Afficher la nouvelle fenêtre
            newWindow.Show();

            // Fermer la fenêtre actuelle
            Close();
        }

        //Event Handling
        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Restore window size and position
            if (Properties.Settings.Default.WindowHeight > 0)
            {
                Height = Properties.Settings.Default.WindowHeight;
            }
            if (Properties.Settings.Default.WindowWidth > 0)
            {
                Width = Properties.Settings.Default.WindowWidth;
            }
            if (Properties.Settings.Default.WindowTop >= 0)
            {
                Top = Properties.Settings.Default.WindowTop;
            }
            if (Properties.Settings.Default.WindowLeft >= 0)
            {
                Left = Properties.Settings.Default.WindowLeft;
            }

            // fade in animation
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            BeginAnimation(OpacityProperty, fadeIn);
        }

        private void BaseWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save window size and position
            Properties.Settings.Default.WindowHeight = Height;
            Properties.Settings.Default.WindowWidth = Width;
            Properties.Settings.Default.WindowTop = Top;
            Properties.Settings.Default.WindowLeft = Left;
            Properties.Settings.Default.Save();
        }
    }
}
