using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VlogManager_Client.Rules
{
    public class ImageSourceRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string imagePath = (string)value;
            if (string.IsNullOrEmpty(imagePath)) return new ValidationResult(false, "Incorrect format of path");
            if(!File.Exists(imagePath)) return new ValidationResult(false, "Image with that url does not exist");
            return new ValidationResult(true, null);
        }
    }
}
