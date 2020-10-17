using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpFormsDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Settings : ContentPage
    {
        public static SQLiteAsyncConnection _connectionColor;
        public static  ObservableCollection<ColorMethod> ColorOptions;
        public static SQLiteAsyncConnection _connectionQuality;
        public static ObservableCollection<SavedSettings> QualityOptions;

        private IList<ColorMethod> _colorMethods;
        public Settings()
        {
            InitializeComponent();

            _colorMethods = GetColorMethods();

            foreach (var method in _colorMethods)
                colorMethods.Items.Add(method.Name);

            _connectionColor = DependencyService.Get<ISQLiteDb>().GetConnection();
            _connectionQuality = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        public static bool creatAdvancedCheck;

        protected override async void OnAppearing()
        {
            await _connectionColor.CreateTableAsync<ColorMethod>();

            var recipes1 = await _connectionColor.Table<ColorMethod>().ToListAsync();
            ColorOptions = new ObservableCollection<ColorMethod>(recipes1);

            await _connectionQuality.CreateTableAsync<ColorMethod>();

            var recipes2 = await _connectionQuality.Table<SavedSettings>().ToListAsync();
            QualityOptions = new ObservableCollection<SavedSettings>(recipes2);

            if (ColorOptions.Count == 0)
            {
                var recipe1 = new ColorMethod { Name = "None"};
                await _connectionColor.InsertAsync(recipe1);
                ColorOptions.Add(recipe1);
            }

            if (QualityOptions.Count == 0)
            {
                var recipe2 = new SavedSettings { Iterations = 500, Quality = 300 };
                await _connectionQuality.InsertAsync(recipe2);
                QualityOptions.Add(recipe2);
            }

            int length = QualityOptions.Count;

            for (int i = 0; i < length - 1; i++)
            {
                var recipeDelete = QualityOptions[0];
                await _connectionQuality.DeleteAsync(recipeDelete);
                QualityOptions.Remove(recipeDelete);
            }

            int length2 = ColorOptions.Count;

            for (int i = 0; i < length2 - 1; i++)
            {
                var recipeDelete = ColorOptions[0];
                await _connectionColor.DeleteAsync(recipeDelete);
                ColorOptions.Remove(recipeDelete);
            }

            colorMethods.SelectedIndex = _options.IndexOf(ColorOptions[0].Name);

            stepper1.Value = QualityOptions[0].Iterations;
            stepper2.Value = QualityOptions[0].Quality;

            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            AdvancedModes.createAdvanced = false;

            int length = QualityOptions.Count;

            for (int i = 0; i < length - 1; i++)
            {
                var recipeDelete = QualityOptions[0];
                await _connectionQuality.DeleteAsync(recipeDelete);
                QualityOptions.Remove(recipeDelete);
            }

            base.OnDisappearing();
        }

        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = colorMethods.Items[colorMethods.SelectedIndex];
            var colorMethod = _colorMethods.Single(cm => cm.Name == name);

            var recipe = new ColorMethod{ Name = name };
            await _connectionColor.InsertAsync(recipe);
            ColorOptions.Add(recipe);

            int length = ColorOptions.Count;

            for (int i = 0; i < length - 1; i++)
            {
                var recipeDelete = ColorOptions[0];
                await _connectionColor.DeleteAsync(recipeDelete);
                ColorOptions.Remove(recipeDelete);
            }
        }

        private List<string> _options = new List<string> { "None", "Color Blend", "Black & White" };


        private IList<ColorMethod> GetColorMethods()
        {
            return new List<ColorMethod>
            {
                new ColorMethod{Id = 1,Name="None"},
                new ColorMethod{Id = 2,Name="Color Blend"},
                new ColorMethod{Id = 3,Name="Black & White"},
            };
        }

        private async void stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var recipe = new SavedSettings { Iterations = stepper1.Value, Quality = stepper2.Value };
            await _connectionQuality.InsertAsync(recipe);
            QualityOptions.Add(recipe);

            int length = QualityOptions.Count;

            for(int i = 0; i < length-1; i++)
            {
                var recipeDelete = QualityOptions[0];
                await _connectionQuality.DeleteAsync(recipeDelete);
                QualityOptions.Remove(recipeDelete);
            }
        }
    }
}