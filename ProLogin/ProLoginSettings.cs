using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProLogin.Core;

namespace ProLogin
{
    public class ProLoginSettings
    {
        public Point MainWindowDimensions { get; set; }

        #region Internal
        internal static ProLoginSettings Instance;
        #endregion

        #region Private
        static string settingsPath;
        #endregion

        public static void Load(string path = null)
        {
            if (path == null)
            {
                path = "ProLoginSettings.json";
            }

            settingsPath = path;

            if (!File.Exists(path))
            {
                Init();
                Save();
            }

            Instance = JsonConvert.DeserializeObject<ProLoginSettings>(File.ReadAllText(path), JsonHelper.DefaultSettings);
        }
        public static bool Save(string path = null)
        {
            if (path == null)
            {
                path = settingsPath;
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(Instance));

            return true;
        }

        internal static void Init()
        {
            Instance = new ProLoginSettings();

            Instance.MainWindowDimensions = new Point(800, 450);
        }
    }
}
