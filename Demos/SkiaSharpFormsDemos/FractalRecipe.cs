using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public class FractalRecipe
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FractalName { get; set; }
        public double SavedReal { get; set; }
        public double SavedComplex { get; set; }
        public double SavedLength { get; set; }
    }
}
