using System;
using System.Collections.Generic;
using System.Text;

namespace ClockMobile
{
    public static class Constants
    {
        public static readonly Guid DeviceGuid = new Guid("00000000-0000-0000-0000-b827ebf4860a");
        public static readonly Guid ServiceGuid = new Guid("3cefb000-82c1-4929-8021-6d793424227c");
        public static readonly Guid SwitchGuid = new Guid("3cefb001-82c1-4929-8021-6d793424227c");
        public static readonly Guid BrightnessGuid = new Guid("3cefb002-82c1-4929-8021-6d793424227c");
        public static readonly Guid ColorGuid = new Guid("3cefb003-82c1-4929-8021-6d793424227c");
        public static readonly Guid TimeGuid = new Guid("3cefb004-82c1-4929-8021-6d793424227c");
    }
}