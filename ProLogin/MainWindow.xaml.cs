using ProLogin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Discord.Webhook;
using Discord;

namespace ProLogin
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HashSet<ProfileListItem> ProfileList = new HashSet<ProfileListItem>();
        public string SelectedProfileListListMethod = "";
        public bool Descending = false;
        public List<SessionHandle> ActiveSessions = new List<SessionHandle>();

        public MainWindow()
        {
            InitializeComponent();

            Width = ProLoginSettings.Instance.MainWindowDimensions.X;
            Height = ProLoginSettings.Instance.MainWindowDimensions.Y;

            #region Load
            AddListViewColumns();

            LoadProfileList();
            #endregion

            Console.WriteLine($"Chrome: {ChromeManager.FullVersion}");
        }

        #region Load
        private void AddListViewColumns()
        {
            // Get the GridView from the ListView
            GridView gridView = (GridView)profileListView.View;

            List<GridViewColumn> columns = new List<GridViewColumn>()
            {
                /*
                new GridViewColumn
                {
                    Header = "Open",
                    DisplayMemberBinding = new Binding("IsActive"),
                    Width = 50
                },
                */
                new GridViewColumn
                {
                    Header = "Name",
                    DisplayMemberBinding = new Binding("Name"),
                    Width = 150
                },
                new GridViewColumn
                {
                    Header = "Description",
                    DisplayMemberBinding = new Binding("Description"),
                    Width = 150
                },
                new GridViewColumn
                {
                    Header = "User Agent",
                    DisplayMemberBinding = new Binding("UserAgent"),
                    Width = 100
                },
                new GridViewColumn
                {
                    Header = "Chrome Ver.",
                    DisplayMemberBinding = new Binding("ChromeVersion"),
                    Width = 80
                },
                new GridViewColumn
                {
                    Header = "Resolution",
                    DisplayMemberBinding = new Binding("Resolution"),
                    Width = 100
                }
            };

            foreach (var column in columns)
                gridView.Columns.Add(column);
        }
        public void LoadProfileList()
        {
            profileListView.ItemsSource = null;

            ProfileList.Clear();

            var newList = ProfileManager.Profiles;
            if (SelectedProfileListListMethod != "")
            {
                object condition(KeyValuePair<string, Profile> kvp)
                {
                    object result = kvp.Value.GetType().GetProperty(SelectedProfileListListMethod)?.GetValue(kvp.Value, null);

                    if (result is string)
                    {
                        return result;
                    }
                    else
                    {
                        return result.ToString();
                    }
                };

                newList = newList.OrderBy(condition).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            if (!Descending)
            {
                newList = newList.Reverse().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            foreach (var item in newList)
            {
                ProfileList.Add(new ProfileListItem(item.Key, ActiveSessions.Select(c => c.UID).Contains(item.Key), item.Value));
            }

            profileListView.ItemsSource = ProfileList;
        }
        #endregion

        #region Events

        #region MenuBar

        private void proLoginQuit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void proLoginHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Theres no help idiot!!!", "Bruh", MessageBoxButton.OK, MessageBoxImage.Error);
            // help window will open here
        }
        #endregion

        private void profileListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (profileListView.SelectedIndex == -1)
                profileListView.SelectedIndex = 0;

            LoadSession(ProfileList.ElementAt(profileListView.SelectedIndex).UID);
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditProfile(profileListView.SelectedIndex);
        }

        private void DangerZoneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ProfileDangerZoneWindow(ProfileManager.GetUIDProfilePair(ProfileList.ElementAt(profileListView.SelectedIndex).UID)).ShowDialog();
        }

        private void AddProfileButton_Checked(object sender, RoutedEventArgs e)
        {
            var newProfile = new Profile()
            {
                ChromeVersion = "Latest",
                Resolution = new System.Drawing.Point(1280, 720)
            };

            string newProfileUid = ProfileManager.AddProfile(newProfile);

            new ProfileWindow(ProfileManager.GetUIDProfilePair(newProfileUid)).ShowDialog();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfile(profileListView.SelectedIndex);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

                using (var webhookClient = new DiscordWebhookClient("https://discord.com/api/webhooks/1182072440600395856/HCyJpsP1WUzTgl-jSttrXhk63wfhdHLOx5VO1d7gs7BavBzC1k1P5loED6NB6-mhkdrj"))
                {
                    webhookClient.SendMessageAsync("Rickroll happened!!!").Wait();
                }
            }
            catch
            {

            }
        }
        #endregion

        public void TriggerProfileListViewRefresh(string uid)
        {
            Console.WriteLine("Session Stopped!");

            Dispatcher.Invoke(() => CloseSession(uid));
        }

        public void LoadSession(string uid)
        {
            ActiveSessions.Add(ProfileExecutor.RunProfile(uid, ProfileManager.GetProfileByUID(uid), TriggerProfileListViewRefresh));

            LoadProfileList();
        }
        public void CloseSession(string uid)
        {
            ActiveSessions.RemoveAt(ActiveSessions.FindIndex(c => c.UID == uid));

            LoadProfileList();
        }

        #region Edit
        public void EditProfile(int id)
        {
            if (id == -1)
            {
                MessageBox.Show("Please select a Profile first!");
                return;
            }

            new ProfileWindow(ProfileManager.GetUIDProfilePair(ProfileList.ElementAt(id).UID)).ShowDialog();
        }
        #endregion

        private void GridViewColumnHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GridViewColumnHeader header = sender as GridViewColumnHeader;
            if (header != null && header.Column != null)
            {
                //Console.WriteLine(header.Column.Header.ToString());

                if (SelectedProfileListListMethod == (header.Column.DisplayMemberBinding as Binding).Path.Path)
                {
                    Descending = !Descending;
                }

                SelectedProfileListListMethod = (header.Column.DisplayMemberBinding as Binding).Path.Path;
            }
            else
            {
                //Console.WriteLine("Column header is null");

                if (SelectedProfileListListMethod == "")
                {
                    Descending = !Descending;
                }

                SelectedProfileListListMethod = "";
            }

            LoadProfileList();
        }
    }
}
