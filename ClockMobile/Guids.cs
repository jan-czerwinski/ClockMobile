using System;
using System.Collections.Generic;
using System.Text;

namespace ClockMobile
{
    public static class Guids
    {
        public static readonly Guid DeviceOnAndroidGuid = new Guid("00000000-0000-0000-0000-b827ebf4860a");
        public static readonly Guid DeviceOnIosGuid = new Guid("5f00a184-1875-439b-6282-d5094fcd7736");

        public static readonly Guid ServiceGuid = new Guid("3cefb000-82c1-4929-8021-6d793424227c");
        public static readonly Guid SwitchGuid = new Guid("3cefb001-82c1-4929-8021-6d793424227c");
        public static readonly Guid BrightnessGuid = new Guid("3cefb002-82c1-4929-8021-6d793424227c");
        public static readonly Guid ColorGuid = new Guid("3cefb003-82c1-4929-8021-6d793424227c");
        public static readonly Guid TimeGuid = new Guid("3cefb004-82c1-4929-8021-6d793424227c");
        public static readonly Guid SnakeGuid = new Guid("3cefb005-82c1-4929-8021-6d793424227c");

    }
    public static class Snake
    {
        public static readonly byte[] Start = { 20 };
        public static readonly byte[] Stop = { 40 };
        public static readonly byte[] Right = { 1 };
        public static readonly byte[] Left = {0};
    }
}