﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace iFractal
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
