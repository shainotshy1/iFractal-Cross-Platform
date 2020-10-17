using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
