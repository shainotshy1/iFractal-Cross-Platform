using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public class SavedSettings
    {
        [PrimaryKey,AutoIncrement]
        public int Id {get; set;}
        public double Iterations { get; set; }
        public double Quality { get; set; }
    }
}
