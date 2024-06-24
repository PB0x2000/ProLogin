using ProLogin.Core;
using System;
using System.Collections.Generic;
using System.Device.Location;
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
using System.Windows.Shapes;

namespace ProLogin
{
    /// <summary>
    /// Interaktionslogik für ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private string UID;
        private Profile Profile;
        public ProfileWindow(KeyValuePair<string, Profile> profileUIDPair)
        {
            InitializeComponent();

            UID = profileUIDPair.Key;
            Profile = profileUIDPair.Value;

            LoadProfile();
        }

        #region Events
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyAndSaveProfile())
            {
                Close();
            }
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyAndSaveProfile();
        }
        #endregion

        private void LoadProfile()
        {
            #region Setting
            // Name
            profileNameTextBox.Text = Profile.Name;

            // Description
            profileDescriptionTextBox.Text = Profile.Description;

            // ChromeVersion
            foreach (string version in DriverManager.ChromeDrivers.Keys)
                profileVersionComboBox.Items.Add(version);

            profileVersionComboBox.SelectedIndex = DriverManager.ChromeDrivers.Keys.ToList().IndexOf(Profile.ChromeVersion);

            // UserAgent
            profileUserAgentTextBox.Text = Profile.UserAgent;

            profileOverrideUserAgentCheckBox.IsChecked = Profile.OverrideUserAgent;

            // Proxy
            if (Profile.Proxy != null)
            {
                profileProxyTextBox.Text = Profile.Proxy.ToString();
            }

            // Resolution
            profileResolutionTextBox.Text = $"{Profile.Resolution.X},{Profile.Resolution.Y}";

            profileResolutionComboBox.Items.Add("1280,720");
            profileResolutionComboBox.Items.Add("1920,1080");
            profileResolutionComboBox.Items.Add("2560,1440");
            profileResolutionComboBox.Items.Add("3840,2160");

            // Monitor Resolution
            profileMonitorResolutionTextBox.Text = $"{Profile.MonitorResolution.X},{Profile.MonitorResolution.Y}";

            profileMonitorResolutionComboBox.Items.Add("1280,720");
            profileMonitorResolutionComboBox.Items.Add("1920,1080");
            profileMonitorResolutionComboBox.Items.Add("2560,1440");
            profileMonitorResolutionComboBox.Items.Add("3840,2160");

            // Languages
            if (Profile.Languages != null)
            {
                profileLanguagesTextBox.Text = string.Join(",", Profile.Languages);
            }

            // GeoLocation
            if (Profile.GeoLocation != null)
            {
                profileGeoLocationTextBox.Text = $"{Profile.GeoLocation.Latitude}:{Profile.GeoLocation.Longitude}";
            }

            profileUseProxyLocationCheckBox.IsChecked = Profile.UseProxyLocation;
            #endregion
        }
        private bool ApplyAndSaveProfile()
        {
            if (profileNameTextBox.Text == "")
            {
                MessageBox.Show("Please give your profile a name first!");
                return false;
            }

            #region Settings
            // Name
            Profile.Name = profileNameTextBox.Text;

            // Description
            Profile.Description = profileDescriptionTextBox.Text;

            // ChromeVersion
            if (profileVersionComboBox.SelectedIndex == -1)
            {
                Profile.ChromeVersion = "Latest";
            }
            else
            {
                Profile.ChromeVersion = DriverManager.ChromeDrivers.Keys.ToArray()[profileVersionComboBox.SelectedIndex];
            }

            // UserAgent
            Profile.UserAgent = profileUserAgentTextBox.Text;

            // Proxy
            if (profileProxyTextBox.Text != "" && profileProxyTextBox.Text != null)
            {
                Profile.Proxy = new Proxy(profileProxyTextBox.Text);
            }
            else
            {
                Profile.Proxy = null;
            }

            // Resolution
            if (profileResolutionTextBox.Text != "" && profileResolutionTextBox.Text != null)
            {
                int[] pointXAndY = profileResolutionTextBox.Text.Split(',').Select(x => int.Parse(x)).ToArray();

                Profile.Resolution = new System.Drawing.Point(pointXAndY[0], pointXAndY[1]);
            }

            // Monitor Resolution
            if (profileMonitorResolutionTextBox.Text != "" && profileMonitorResolutionTextBox.Text != null)
            {
                int[] pointXAndY = profileMonitorResolutionTextBox.Text.Split(',').Select(x => int.Parse(x)).ToArray();

                Profile.MonitorResolution = new System.Drawing.Point(pointXAndY[0], pointXAndY[1]);
            }

            // Languages
            Profile.Languages = profileLanguagesTextBox.Text.Split(',').ToList();

            // GeoLocation
            if (profileGeoLocationTextBox.Text != "")
            {
                string[] geoLocationLatitudeAndLongitude = profileGeoLocationTextBox.Text.Split(':');
                Profile.GeoLocation = new GeoCoordinate(Convert.ToDouble(geoLocationLatitudeAndLongitude[0]), Convert.ToDouble(geoLocationLatitudeAndLongitude[1]));
            }
            else
            {
                Profile.GeoLocation = null;
            }

            Profile.UseProxyLocation = profileUseProxyLocationCheckBox.IsChecked.Value;
            #endregion

            ProfileManager.Profiles[UID] = Profile;
            ProfileManager.Save();

            App.MainWindowInstance.LoadProfileList();

            return true;
        }

        private void profileResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            profileResolutionTextBox.Text = (string)profileResolutionComboBox.SelectedValue;
        }

        private void profileMonitorResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            profileMonitorResolutionTextBox.Text = (string)profileMonitorResolutionComboBox.SelectedValue;
        }

        private void userAgentBuildButton_Click(object sender, RoutedEventArgs e)
        {
            new UserAgentBuilderWindow(Profile.UserAgent).ShowDialog();
        }
    }
}
