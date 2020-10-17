using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public class ColorMethod
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
