﻿using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using iFractal.iOS;

[assembly: Dependency(typeof(SQLiteDb))]

namespace iFractal.iOS
{
	public class SQLiteDb : ISQLiteDb
	{
		public SQLiteAsyncConnection GetConnection()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documentsPath, "iFractalSettings.db3");

			return new SQLiteAsyncConnection(path);
		}
	}
}