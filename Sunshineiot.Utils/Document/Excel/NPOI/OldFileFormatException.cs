﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sunshineiot.Utils.NPOI
{
    [Serializable]
    public class OldFileFormatException : Exception
    {
        public OldFileFormatException(String s)
            : base(s)
        { }

    }
}
