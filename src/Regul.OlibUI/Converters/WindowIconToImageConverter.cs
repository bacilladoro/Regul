﻿using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Icon = System.Drawing.Icon;

namespace Regul.OlibUI.Converters
{
    public class WindowIconToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                WindowIcon wIcon = value as WindowIcon;
                MemoryStream stream = new();
                wIcon.Save(stream);
                stream.Position = 0;
                try
                {
                    return new Bitmap(stream);
                }
                catch
                {
                    try
                    {
                        Icon icon = new(stream);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        bmp.Save(stream, ImageFormat.Png);
                        return new Bitmap(stream);
                    }
                    catch
                    {
                        Icon icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        Stream stream3 = new MemoryStream();
                        bmp.Save(stream3, ImageFormat.Png);
                        return new Bitmap(stream3);
                    }
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
