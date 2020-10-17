using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using iFractal.Droid;

[assembly: Dependency(typeof(SQLiteDb))]

namespace iFractal.Droid
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