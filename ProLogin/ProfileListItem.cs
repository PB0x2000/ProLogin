using ProLogin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProLogin
{
    public class ProfileListItem : Profile
    {
        public Brush TextColor
        {
            get
            {
                return IsActive ? new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)) : new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
        }

        public string UID { get; set; }
        public bool IsActive { get; set; }

        public string TooltipText
        {
            get
            {
                if (Description == "")
                {
                    return null;
                }
                return Description;
            }
        }

        public ProfileListItem(string uid, bool isActive, Profile profile)
        {
            foreach (PropertyInfo property in typeof(Profile).GetProperties())
            {
                PropertyInfo thisProperty = typeof(ProfileListItem).GetProperty(property.Name);
                if (thisProperty != null && thisProperty.CanWrite)
                {
                    thisProperty.SetValue(this, property.GetValue(profile));
                }
            }

            UID = uid;
            IsActive = isActive;
        }
    }
}
