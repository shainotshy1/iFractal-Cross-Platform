﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace iFractal
{
    public class ColorMethod
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
