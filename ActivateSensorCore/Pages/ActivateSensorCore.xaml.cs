using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ActivateSensorCore.Resources;
using Lumia.Sense;

namespace ActivateSensorCore.Pages
{
    public partial class ActivateSensorCore : PhoneApplicationPage
    {
        public ActivateSensorCoreResult ActivationResult;

        public static bool OptedOut()
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("SensorCoreDontBugMe.txt"))
                    return true;
            }
            return false;
        }

        public ActivateSensorCore()
        {
            InitializeComponent();
            Loaded += ActivateSensorCore_Loaded;
        }

        void ActivateSensorCore_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDialog();

        //    AppName.Text = GetApplicationName();
        }

        void ShowMotionDataActivationBox()
        {
            MotionDataActivationTitle.Text = AppResources.MotionDataActivationTitle;
            MotionDataActivationExplanation.Text = AppResources.MotionDataActivationExplanation;
            MotionDataActivationButton.Content = AppResources.MotionDataActivationButton;
            MotionDataActivationLaterButton.Content = AppResources.LaterButton;
            MotionDataActivationNeverButton.Content = AppResources.NeverButton;
            MotionDataActivationBox.Visibility = System.Windows.Visibility.Visible;
        }

        void ShowLocationActivationBox()
        {
            LocationActivationTitle.Text = AppResources.LocationActivationTitle;
            LocationActivationExplanation.Text = AppResources.LocationActivationExplanation;
            LocationActivationButton.Content = AppResources.LocationActivationButton;
            LocationActivationLaterButton.Content = AppResources.LaterButton;
            LocationActivationNeverButton.Content = AppResources.NeverButton;
            LocationActivationBox.Visibility = System.Windows.Visibility.Visible;
        }


        async void UpdateDialog()
        {
            MotionDataActivationBox.Visibility = System.Windows.Visibility.Collapsed;
            ActivationResult.ActivationRequestResult = ActivationRequestResults.AskMeLater;

            Exception failure = null;
            try
            {
                await Lumia.Sense.PlaceMonitor.GetDefaultAsync();
            }
            catch (Exception exception)
            {
                switch (SenseHelper.GetSenseError(exception.HResult))
                {
                    case SenseError.LocationDisabled:
                        ShowLocationActivationBox();
                        break;
                    case SenseError.SenseDisabled:
                        ShowMotionDataActivationBox();
                        break;
                    default:
                        // do something clever here
                        break;
                }

                failure = exception;
            }
            if (failure == null)
            {
                // All is now good, dismiss the dialog.

                ActivationResult.ActivationRequestResult = ActivationRequestResults.AllEnabled;
                NavigationService.GoBack();
            }

        }
        /// <summary>
        /// Get application name.
        /// </summary>
        /// <returns>Name of the application.</returns>
        private string GetApplicationName()
        {
            string appName = AppResources.AppName;

            // If application name has not been defined by the application,
            // extract it from the Application class.
            if (appName == null || appName.Length <= 0)
            {
                appName = Application.Current.ToString();
                if (appName.EndsWith(".App"))
                {
                    appName = appName.Remove(appName.LastIndexOf(".App"));
                }
            }

            return appName;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator && e.NavigationMode == NavigationMode.Back)
            {
                // Back from the setting view.
                UpdateDialog();
            }
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            ActivationResult.ActivationRequestResult = ActivationRequestResults.AskMeLater;
            NavigationService.GoBack();
        }

        private void NeverButton_Click(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.CreateFile("SensorCoreDontBugMe.txt");
            }

            ActivationResult.ActivationRequestResult = ActivationRequestResults.NoAndDontAskAgain;
            NavigationService.GoBack();
        }


        private async void MotionDataActivationButton_Click(object sender, RoutedEventArgs e)
        {
            await SenseHelper.LaunchSenseSettingsAsync(); 
            // Although asynchoneous, this completes before the user has actually done anything.
            // The application will loose control, the system settings will be displayed.
            // We will get the control back to our application via an OnNavigatedTo event.
        }

        private async void LocationActivationButton_Click(object sender, RoutedEventArgs e)
        {
            await SenseHelper.LaunchLocationSettingsAsync();
            // Although asynchoneous, this completes before the user has actually done anything.
            // The application will loose control, the system settings will be displayed.
            // We will get the control back to our application via an OnNavigatedTo event.
        }

    }
}