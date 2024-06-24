using ProLogin.Core;
using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für ProfileDangerZoneWindow.xaml
    /// </summary>
    public partial class ProfileDangerZoneWindow : Window
    {
        private string UID;
        private Profile Profile;
        public ProfileDangerZoneWindow(KeyValuePair<string, Profile> profileUIDPair)
        {
            InitializeComponent();

            UID = profileUIDPair.Key;
            Profile = profileUIDPair.Value;

            Title = $"Profile: {Profile.Name}";
        }

        private void profileArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?\nThis can be undone but its annoying.", "Attention!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ProfileManager.ArchiveProfile(UID);

                App.MainWindowInstance.LoadProfileList();

                Close();
            }
        }

        private void profileDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?\nThis cannot be undone!!!", "Attention!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ProfileManager.DeleteProfile(UID);

                App.MainWindowInstance.LoadProfileList();

                Close();
            }
        }
    }
}
