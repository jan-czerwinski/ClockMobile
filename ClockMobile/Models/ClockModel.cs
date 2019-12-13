using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClockMobile.Models
{
    public class ClockModel : BindableBase
    {
        private bool on;
        public bool On { get => on; set => SetProperty(ref on, value); }
        public BrightnessSettings Brightness { get; set; }
        public ColorSettings Color { get; set; }
    }

    public class BrightnessSettings : BindableBase
    {
        private double day;
        private bool nightMode;
        private double night;
        private TimeSpan startTime;
        private TimeSpan endTime;

        public double Day { get => day; set => SetProperty(ref day, value); }
        public bool NightMode { get => nightMode; set => SetProperty(ref nightMode, value); }
        public double Night { get => night; set => SetProperty(ref night, value); }
        public TimeSpan StartTime { 
            get => startTime; 
            set => SetProperty(ref startTime, value); } 
        public TimeSpan EndTime { get => endTime; set => SetProperty(ref endTime, value); }
        public string ToSend => Compress();
        private string Compress()
        {
            string output = "";
            output += Convert.ToInt32(Day).ToString().PadLeft(3, '0');
            output += NightMode ? "1" : "0";
            output += Convert.ToInt32(Night).ToString().PadLeft(3, '0');
            output += StartTime.ToString().Substring(0, 2);
            output += StartTime.ToString().Substring(3, 2);
            output += EndTime.ToString().Substring(0, 2);
            output += EndTime.ToString().Substring(3, 2);
            return output;
        }
    }

    public class ColorSettings : BindableBase //Convert to hsv
    {
        private double r;
        private double g;
        private double b;
        private Color hexColor;
        public double R
        {
            get { return r; }
            set
            {
                SetProperty(ref r, value);
                HexColor = ToHex;
            }
        }

        public double G
        {
            get { return g; }
            set
            {
                SetProperty(ref g, value);
                HexColor = ToHex;
            }
        }
        public double B
        {
            get { return b; }
            set
            {
                SetProperty(ref b, value);
                HexColor = ToHex;
            }
        }

        private Color ToHex => Color.FromArgb((int)R, (int)G, (int)B);
        public Color HexColor { get => hexColor; set => SetProperty(ref hexColor, value); }

        public int ToSend => (int)R * 1000000 + (int)G * 1000 + (int)B;
    }
}