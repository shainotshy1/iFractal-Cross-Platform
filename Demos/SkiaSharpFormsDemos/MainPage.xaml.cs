using SkiaSharp;
using SkiaSharp.Views.Forms;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SkiaSharpFormsDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CarouselPage
    {
        public static SQLiteAsyncConnection _connection;
        public static ObservableCollection<FractalRecipe> SavedFractals;

        public MainPage()
        {
            InitializeComponent();

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            Settings._connectionColor = DependencyService.Get<ISQLiteDb>().GetConnection();
            Settings._connectionQuality = DependencyService.Get<ISQLiteDb>().GetConnection();

            //Page refresh every 1/20 of second
            Device.StartTimer(TimeSpan.FromSeconds(1f / 20), () =>
               {
                   canvasImage.InvalidateSurface();
                   canvasImageFast.InvalidateSurface();
                   return true;
               });

            BindingContext = this;
        }

        //Painting of the fractal on the canvas
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            args.Surface.Canvas.DrawBitmap(bitmap, args.Info.Rect, BitmapStretch.Uniform);
        }
        private void canvasImageFast_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.DrawBitmap(bitmapFast, e.Info.Rect, BitmapStretch.Uniform);
        }

        //Color lists for macro and micro
        public static ObservableCollection<string> listColors = new ObservableCollection<string>
        {
            "Red","Green","Blue"


        };
        public static ObservableCollection<string> listColorsMicro = new ObservableCollection<string>
        {
            "Red","Green","Blue"
        };

        //Variables

        public static string selectedFractal;
        public static string selectedColor;
        public static string selecterColorMicro;

        static readonly int h = 400;
        static readonly int w = 400;
        static int iterations;
        public static string outputName = "FractalImage";

        public static int widthExport;
        public static int heightExport;

        public static int intInput1;
        public static int intInput2;
        public static int intInput3;

        public static List<string> fractalList = new List<string>{"Full Mandelbrot","Front Half","Crack","Swirl","Spiral","Girl With Braids","Spiky Mini Mandelbrot","Spiky Tail","Tunnel","Hole","Unconnected Julia","Inner Hole","Spiky Mini Mandelbrot 2"};

        //Bitmap set to low automatically
        public static SKBitmap bitmap = new SKBitmap(400,400,false);
        public static SKBitmap bitmapFast = new SKBitmap(300,300,false);

        //Creates fractal if pressed create on Advanced page
        protected override async void OnAppearing()
        {
            isCreating = false;

            await _connection.CreateTableAsync<FractalRecipe>();

            var recipes = await _connection.Table<FractalRecipe>().ToListAsync();
            SavedFractals = new ObservableCollection<FractalRecipe>(recipes);

            await Settings._connectionColor.CreateTableAsync<ColorMethod>();

            var recipes2 = await Settings._connectionColor.Table<ColorMethod>().ToListAsync();
            Settings.ColorOptions = new ObservableCollection<ColorMethod>(recipes2);

            await Settings._connectionQuality.CreateTableAsync<SavedSettings>();

            var recipes3 = await Settings._connectionQuality.Table<SavedSettings>().ToListAsync();
            Settings.QualityOptions = new ObservableCollection<SavedSettings>(recipes3);

            if (Settings.ColorOptions.Count > 0)
            {
                switch (Settings.ColorOptions[0].Name)
                {
                    case "Color Blend":
                        colorBlendFactor = 1;
                        colorGreyScale = false;
                        break;
                    case "Black & White":
                        colorBlendFactor = 0;
                        colorGreyScale = true;
                        break;
                    case "None":
                        colorBlendFactor = 0;
                        colorGreyScale = false;
                        break;
                    default:
                        throw new Exception("Invalid");

                }
            }

            Refresh();

            if (Settings.QualityOptions.Count > 0)
            {
                heightExport = (int)(Settings.QualityOptions[0].Quality);
                widthExport = (int)(Settings.QualityOptions[0].Quality);
                iterations = (int)(Settings.QualityOptions[0].Iterations);
            }
            else
            {
                heightExport = h;
                widthExport = w;
                iterations = 500;
            }

            

            canvasImage.IsVisible = true;
            canvasImageFast.IsVisible = true;

            if (AdvancedModes.createAdvanced)
            {
                canvasImage.IsVisible = true;
                canvasImageFast.IsVisible = true;

                GenerateMandelbrotZoom(AdvancedModes.realInput, AdvancedModes.complexInput, AdvancedModes.lengthInput, AdvancedModes.macroInput, AdvancedModes.microInput);

            }

            if (blueMacro.IsChecked == false && greenMacro.IsChecked == false && redMacro.IsChecked == false)
            {
                redMacro.IsChecked = true;
            }
            if (blueMicro.IsChecked == false && greenMicro.IsChecked == false && redMicro.IsChecked == false)
            {
                redMicro.IsChecked = true;
            }

            loadingText.ItemsSource = loading;

            /*** ADVANCED PAGE ONAPPEARING CODE BELOW ***/

            isCreating = false;
            AdvancedModes.isSetting = true;

            colorListAdvancedMicro.ItemsSource = listColorsAdvancedMicro;
            colorListAdvancedMacro.ItemsSource = listColorsAdvancedMacro;

            if (AdvancedModes.microInput == null)
            {
                AdvancedModes.microInput = "Red";
                AdvancedModes.macroInput = "Red";

                colorListAdvancedMicro.SelectedItem = listColorsAdvancedMicro[0];
                colorListAdvancedMacro.SelectedItem = listColorsAdvancedMacro[0];
            }
            else
            {
                colorListAdvancedMacro.SelectedItem = AdvancedModes.macroInput;
                colorListAdvancedMicro.SelectedItem = AdvancedModes.microInput;
            }

            riEntry.Text = Convert.ToString(AdvancedModes.realInput);
            ciEntry.Text = Convert.ToString(AdvancedModes.complexInput);
            lEntry.Text = Convert.ToString(AdvancedModes.lengthInput);

            riSlider.Value = AdvancedModes.realInput;
            ciSlider.Value = AdvancedModes.complexInput;
            lSlider.Value = AdvancedModes.lengthInput;

            AdvancedModes.isSetting = false;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            fractalList = new List<string> { "Full Mandelbrot", "Front Half", "Crack", "Swirl", "Spiral", "Girl With Braids", "Spiky Mini Mandelbrot", "Spiky Tail", "Tunnel", "Hole", "Unconnected Julia", "Inner Hole", "Spiky Mini Mandelbrot 2" };

            base.OnDisappearing();
        }

        //List of preset values for different fractals already discovered
        public static double[] varList1 = new double[] { 0.0, 0.2, 0.4, 0.45, 0.45057, 0.4513, 0.305, 0.10725, 0.10725556, 0.107255653, 0.2, 0.10725566, 0.74934 };
        public static double[] varList2 = new double[] { 0.0, 0.35, 0.4, 0.45, 0.4505, 0.44961, 0.4, 0.4970, 0.49740059, 0.497400682, 0.0, 0.4974006898, 0.2943 };
        public static double[] varList3 = new double[] { 1.0, 0.3, 0.1, 0.005, 0.0005, 0.002, 0.02, 0.0011, 0.0000002, 0.000000018, 1.0, 0.000000003, 0.000022 };

        
        public static int item;

        //Create function using x,y, and l values
        private async void Create(object sender, EventArgs e)
        {
            await createButton.ScaleTo(1.1, 200, Easing.SpringOut);
            await createButton.ScaleTo(1, 300, Easing.SpringIn);

            if (AdvancedModes.createAdvanced)
            {
                GenerateMandelbrotZoom(AdvancedModes.realInput, AdvancedModes.complexInput, AdvancedModes.lengthInput, AdvancedModes.macroInput, AdvancedModes.microInput);
            }
            else
            {
                if (item <= 12)
                {
                    GenerateMandelbrotZoom(varList1[item], varList2[item], varList3[item], selectedColor, selecterColorMicro);
                }

                else
                {
                    GenerateMandelbrotZoom(SavedFractals[item - 13].SavedReal, SavedFractals[item - 13].SavedComplex, SavedFractals[item - 13].SavedLength, selectedColor, selecterColorMicro);
                }

            }
  
        }

        //loading list changes with every frame to mimic progress animation
        public static ObservableCollection<string> loading = new ObservableCollection<string> { };

        //Mandelbrot generation using real value, complex value, zoom length 0 - 1, and colors

        public static bool isRunning;
        public async void GenerateMandelbrotZoom(double ri, double ci, double l, string macroColor, string microColor)
        {
            if (loading.Count == 0)
            {
                isRunning = true;
                cancel = false;

                canvasImage.IsVisible = false;

                loading.Add("Loading . . . ");

                bitmapFast = new SKBitmap(widthExport, heightExport, true);

                canvasImageFast.IsVisible = true;

                cancel = false;
                //Making 2x2 array with size height export and width export
                Complex complix;
                Pixel[,] picture = new Pixel[widthExport, heightExport];
                int a;
                int b;

                //Setting format to Jpeg

                SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg;
                int quality = 100;

                //For loop with calculations, every column calulated between loading changes
                //i goes up by the length value to make proportional

                for (double i = ri * widthExport; i < (ri + l) * widthExport; i += l)
                {
                    if (cancel)
                    {
                        break;
                    }
                    for (double j = ci * heightExport; j < (ci + l) * heightExport; j += l)
                    {
                        if (cancel)
                        {
                            break;
                        }
                        //setting shift of and position
                        a = (int)((i - ri * widthExport) / l);
                        b = (int)((j - ci * heightExport) / l);

                        complix = new Complex((i - (14 * heightExport / 20.0)) / (widthExport / 3.0), (j - (widthExport / 2.0)) / (heightExport / 3.0 * widthExport / heightExport));

                        int onoff = (int)(Iterate(complix) * 255);

                        updatePixelInputs(macroColor, microColor, onoff);

                        picture[a, b] = new Pixel(intInput1, intInput2, intInput3);

                        //Sets individual pixels
                        if (picture[a, b] == null)
                        {
                            picture[a, b] = new Pixel(0, 0, 0);

                            SKColor colorPixelNull = new SKColor((byte)picture[a, b].GetRed(), (byte)picture[a, b].GetGreen(), (byte)picture[a, b].GetBlue());

                            bitmapFast.SetPixel(a, b, colorPixelNull);

                        }
                        else
                        {
                            SKColor colorPixel = new SKColor((byte)picture[a, b].GetRed(), (byte)picture[a, b].GetGreen(), (byte)picture[a, b].GetBlue());

                            bitmapFast.SetPixel(a, b, colorPixel);
                        }
                    }
                    //Saves image every column
                    using (MemoryStream memStream = new MemoryStream())
                    using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                    {
                        bitmapFast.Encode(wstream, imageFormat, quality);

                        byte[] data = memStream.ToArray();

                        if (data != null && data.Length != 0)
                        {
                            string fileName = null;
                            await DependencyService.Get<IPhotoLibrary>().
                            SavePhotoAsync(data, "SaveFileFormats", fileName + ".Jpeg");
                        }
                    }
                    Loading();
                }
                loading.RemoveAt(0);
                isRunning = false;
            }
        }
        //Make iterations of calculations: the more iterations, the more accurate the curve
        public static double Iterate(Complex num)
        {
            Complex numi = num;

            for (int convergence = 0; convergence < iterations; convergence++)
            {
                num = num.Times(num).Plus(numi);
                if (num.Abs() > 4)
                {
                    return 1 - convergence / (double)iterations;
                }
            }
            return 0;
        }

        //Assigns color values
        public void updatePixelInputs(string macroColor, string microColor, int onoff)
        {
            if (colorGreyScale)
            {
                intInput1 = onoff * onoff;
                intInput2 = onoff * onoff;
                intInput3 = onoff * onoff;
            }
            else
            {
                if (macroColor == "Red")
                {
                    if (microColor == "Red")
                    {
                        intInput1 = onoff * onoff;
                        intInput2 = colorBlendFactor * onoff * onoff / 100;
                        intInput3 = colorBlendFactor * onoff * onoff / 100;
                    }
                    else if (microColor == "Blue")
                    {
                        intInput1 = onoff * onoff / 50;
                        intInput2 = colorBlendFactor * onoff * onoff / 50;
                        intInput3 = onoff * onoff;
                    }
                    else if (microColor == "Gree")
                    {
                        intInput1 = onoff * onoff;
                        intInput2 = onoff * onoff / 50;
                        intInput3 = colorBlendFactor * onoff * onoff / 100;
                    }
                }
                else if (macroColor == "Green")
                {
                    if (microColor == "Red")
                    {
                        intInput1 = onoff * onoff / 40;
                        intInput2 = onoff * onoff / 50;
                        intInput3 = colorBlendFactor * onoff * onoff / 100;
                    }
                    else if (microColor == "Blue")
                    {
                        intInput1 = colorBlendFactor * onoff * onoff / 100;
                        intInput2 = onoff * onoff / 50;
                        intInput3 = onoff * onoff / 40;
                    }
                    else if (microColor == "Green")
                    {
                        intInput1 = colorBlendFactor * onoff * onoff / 100;
                        intInput2 = onoff * onoff;
                        intInput3 = colorBlendFactor * onoff * onoff / 100;
                    }
                }
                else if (macroColor == "Blue")
                {
                    if (microColor == "Red")
                    {
                        intInput1 = onoff * onoff;
                        intInput2 = colorBlendFactor * onoff * onoff / 50;
                        intInput3 = onoff * onoff / 40;
                    }
                    else if (microColor == "Blue")
                    {
                        intInput1 = colorBlendFactor * onoff * onoff / 100;
                        intInput2 = colorBlendFactor * onoff * onoff / 100;
                        intInput3 = onoff * onoff;
                    }
                    else if (microColor == "Green")
                    {
                        intInput1 = colorBlendFactor * onoff * onoff / 100;
                        intInput2 = onoff * onoff / 40;
                        intInput3 = onoff * onoff / 50;
                    }
                }
            }
        }

        public static int colorBlendFactor;
        public static bool colorGreyScale;
        //If on, all three colors rgb are present
        /*private void switchColor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (switchColor.IsToggled)
            {
                colorBlendFactor = 1;
            }
            else
            {
                colorBlendFactor = 0;
            }
        }
        //Makes the bitmap B&W
        private void switchColorGreyScale_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (switchColorGreyScale.IsToggled)
            {
                colorGreyScale = true;
            }
            else
            {
                colorGreyScale = false;
            }
        }*/
        //Starts off with both colors set as red
        private void colorList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedColor = listColors[e.SelectedItemIndex];
        }

        public static string option;

        //Changes dimensions of bidmap
        public async void Option_Check()
        {
            if (option == "Max High")
            {
                widthExport = 10 * w;
                heightExport = 10 * h;
            }
            else if (option == "Ultra High")
            {
                widthExport = 5 * w;
                heightExport = 5 * h;
            }
            else if (option == "High")
            {
                widthExport = 3 * w;
                heightExport = 3 * h;
            }
            else if (option == "Medium")
            {
                widthExport = 2 * w;
                heightExport = 2 * h;
            }
            else if (option == "Low")
            {

                widthExport = w;
                heightExport = h;
            }
            else if (option == "Very Low")
            {
                widthExport = w / 3;
                heightExport = h / 3;
            }

            //Custom sized square bitmap

            if (option == "Custom")
            {
                int dimension = Convert.ToInt32(await DisplayPromptAsync("Custom", "What pixel height would you like?", "OK", "Cancel", "1000"));
                widthExport = dimension;
                heightExport = dimension;
            }
            //Makes it so the quality is not set to the word "Cancel"
            if (option != "Cancel")
            {
                bitmap = new SKBitmap(widthExport, heightExport, true);
            }
        }

        //Loading animation
        void Loading()
        {
            if (! cancel)
            {
                if (loading[0] == "Loading . . . ")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading  . . .");
                }
                else if (loading[0] == "Loading  . . .")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading   . . ");
                }
                else if (loading[0] == "Loading   . . ")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading    . .");
                }
                else if (loading[0] == "Loading    . .")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading     . ");
                }
                else if (loading[0] == "Loading     . ")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading      .");
                }
                else if (loading[0] == "Loading      .")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading       ");
                }
                else if (loading[0] == "Loading       ")
                {
                    loading.RemoveAt(0);

                    loading.Add("Loading . . . ");
                }
            } 
        }
        //Gives choices of preset
        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            string[] fractalOptions = fractalList.ToArray();

            string option = await DisplayActionSheet("Fractal Type", "Cancel", null, fractalOptions);

            bool clicked = false;

            if (option != "Cancel" && option != "OK" && option != "")
            {
                int setItem = fractalOptions.IndexOf(option);
                clicked = true;

                if (setItem <= 12)
                {
                    string action = await DisplayActionSheet("Fractal Actions", "Cancel", null, "Set Fractal", "Info", "Advanced");

                    if (action == "Set Fractal")
                    {
                        item = setItem;

                        await DisplayAlert("Fractal Set", "The current set fractal is: " + fractalOptions[item], "OK");
                    }
                    else if (action == "Info")
                    {
                        await DisplayAlert("Fractal Info", "X Value (Real value on complex plane): " + varList1[setItem] + "\nY Value (Complex value on complex plane): " + varList2[setItem] + "\nSize (Length of pixel intervals): " + varList3[setItem], "OK");
                    }
                    else if (action == "Advanced")
                    {
                        carouselPage.CurrentPage = carouselPage.Children[1];
                        riSlider.Value = varList1[setItem];
                        ciSlider.Value = varList2[setItem];
                        lSlider.Value = varList3[setItem];
                    }
                }
                else
                {
                    string action = await DisplayActionSheet("Fractal Actions", "Cancel", null, "Set Fractal", "Delete", "Info", "Rename", "Advanced");

                    if (action == "Set Fractal")
                    {
                        item = setItem;

                        await DisplayAlert("Fractal Set", "The current set fractal is: " + fractalOptions[item], "OK");
                    }
                    else if (action == "Delete")
                    {
                        bool deleteCheck = await DisplayAlert("Delete", "Are you sure you would like to delete?", "OK", "Cancel");

                        if (deleteCheck)
                        {
                            var recipeDelete = SavedFractals[setItem - 13];
                            await _connection.DeleteAsync(recipeDelete);
                            SavedFractals.Remove(recipeDelete);

                            if (item == setItem)
                            {
                                item = 0;
                            }
                        }

                        Refresh();

                    }
                    else if (action == "Info")
                    {
                        await DisplayAlert("Fractal Info", "X Value (Real value on complex plane): " + SavedFractals[setItem - 13].SavedReal + "\nY Value (Complex value on complex plane): " + SavedFractals[setItem - 13].SavedComplex + "\nSize (Length of pixel intervals): " + SavedFractals[setItem - 13].SavedLength, "OK");
                    }
                    else if (action == "Rename")
                    {
                        string newFractal = await DisplayPromptAsync("Rename Fractal", "What would you like to rename " + SavedFractals[setItem - 13].FractalName + "?", "OK", "Cancel", SavedFractals[setItem - 13].FractalName);

                        if (newFractal != null && newFractal != "" && newFractal != SavedFractals[setItem - 13].FractalName)
                        {
                            double realCache = SavedFractals[setItem - 13].SavedReal;
                            double complexCache = SavedFractals[setItem - 13].SavedComplex;
                            double lengthCache = SavedFractals[setItem - 13].SavedLength;

                            var recipe = SavedFractals[setItem - 13];
                            await _connection.DeleteAsync(recipe);
                            SavedFractals.Remove(recipe);

                            recipe = new FractalRecipe { FractalName = newFractal, SavedComplex = complexCache, SavedLength = lengthCache, SavedReal = realCache };

                            await _connection.InsertAsync(recipe);
                            SavedFractals.Add(recipe);

                            Refresh();
                        }
                    }
                    else if (action == "Advanced")
                    {
                        carouselPage.CurrentPage = carouselPage.Children[1];
                        riSlider.Value = SavedFractals[setItem - 13].SavedReal;
                        ciSlider.Value = SavedFractals[setItem - 13].SavedComplex;
                        lSlider.Value = SavedFractals[setItem - 13].SavedLength;
                    }
                }
            }

            if (clicked)
            {
                AdvancedModes.createAdvanced = false;
            }
        }

        private void Refresh()
        {
            fractalList = new List<string> { "Full Mandelbrot", "Front Half", "Crack", "Swirl", "Spiral", "Girl With Braids", "Spiky Mini Mandelbrot", "Spiky Tail", "Tunnel", "Hole", "Unconnected Julia", "Inner Hole", "Spiky Mini Mandelbrot 2" };

            if (SavedFractals.Count > 0)
            {
                for (int i = 0; i < SavedFractals.Count; i++)
                {
                    fractalList.Add(SavedFractals[i].FractalName);
                }
            }
        }

        private async void ImageButton_Clicked_2(object sender, EventArgs e)
        {
            if (loading.Count == 0)
            {
                isRunning = true;
                string fileName = await DisplayPromptAsync("Save", "What would you like the file name to be?", "OK", "Cancel", "File Name");


                //Gives option to save file with a custom name on photo gallery

                if (fileName != null && fileName != "" && fileName != "Cancel" && fileName != "OK")
                {

                    option = await DisplayActionSheet("Quality", null, null, "Custom", "Max High", "Ultra High", "High", "Medium", "Low", "Very Low");

                    Option_Check();

                    cancel = false;

                    canvasImage.IsVisible = true;
                    canvasImageFast.IsVisible = false;

                    loading.Add("Loading . . . ");

                    double ri;
                    double ci;
                    double l;

                    string downloadMacro;
                    string downloadMicro;

                    if (AdvancedModes.createAdvanced)
                    {
                        downloadMacro = AdvancedModes.macroInput;
                        downloadMicro = AdvancedModes.microInput;

                        ri = AdvancedModes.realInput;
                        ci = AdvancedModes.complexInput;
                        l = AdvancedModes.lengthInput;
                    }
                    else
                    {
                        downloadMacro = selectedColor;
                        downloadMicro = selecterColorMicro;

                        if (item <= 12)
                        {
                            ri = varList1[item];
                            ci = varList2[item];
                            l = varList3[item];
                        }
                        else
                        {
                            ri = SavedFractals[item-13].SavedReal;
                            ci = SavedFractals[item - 13].SavedComplex;
                            l = SavedFractals[item - 13].SavedLength;
                        }
                    }


                    //Making 2x2 array with size height export and width export
                    Complex complix;
                    Pixel[,] picture = new Pixel[heightExport, widthExport];
                    int a;
                    int b;

                    //Setting format to Jpeg

                    SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Jpeg;
                    int quality = 100;

                    //For loop with calculations, every column calulated between loading changes
                    //i goes up by the length value to make proportional
                    for (double i = ri * heightExport; i < (ri + l) * heightExport; i += l)
                    {
                        if (cancel)
                        {
                            break;
                        }
                        Loading();

                        for (double j = ci * widthExport; j < (ci + l) * widthExport; j += l)
                        {
                            if (cancel)
                            {
                                break;
                            }
                            //setting shift of and position
                            a = (int)((i - ri * heightExport) / l);
                            b = (int)((j - ci * widthExport) / l);

                            complix = new Complex((i - (14 * heightExport / 20.0)) / (widthExport / 3.0), (j - (widthExport / 2.0)) / (heightExport / 3.0 * widthExport / heightExport));

                            int onoff = (int)(Iterate(complix) * 255);

                            updatePixelInputs(downloadMacro, downloadMicro, onoff);

                            picture[a, b] = new Pixel(intInput1, intInput2, intInput3);

                            //Sets individual pixels
                            if (picture[a, b] == null)
                            {
                                picture[a, b] = new Pixel(0, 0, 0);

                                SKColor colorPixelNull = new SKColor((byte)picture[a, b].GetRed(), (byte)picture[a, b].GetGreen(), (byte)picture[a, b].GetBlue());

                                bitmap.SetPixel(a, b, colorPixelNull);

                            }
                            else
                            {
                                SKColor colorPixel = new SKColor((byte)picture[a, b].GetRed(), (byte)picture[a, b].GetGreen(), (byte)picture[a, b].GetBlue());

                                bitmap.SetPixel(a, b, colorPixel);
                            }
                        }

                        //Saves image every column
                        using (MemoryStream memStream = new MemoryStream())
                        using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                        {
                            bitmap.Encode(wstream, imageFormat, quality);

                            byte[] data = memStream.ToArray();

                            if (data != null && data.Length != 0)
                            {
                                await DependencyService.Get<IPhotoLibrary>().
                                SavePhotoAsync(data, "SaveFileFormats", fileName + ".Jpeg");
                            }
                        }
                    }

                    cancel = false;

                }
                heightExport = (int)(Settings.QualityOptions[0].Quality);
                widthExport = (int)(Settings.QualityOptions[0].Quality);
                if (loading.Count != 0)
                {
                    loading.RemoveAt(0);
                }
                isRunning = false;
            }
            
        }

        public static object boxObjectMacro;
        public static object boxObjectMicro;

        private void checkBoxMacro_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender == boxObjectMacro)
            {
                boxObjectMacro = null;
            }
            else
            {
                if (boxObjectMacro == null)
                {
                    boxObjectMacro = sender;
                }
                else
                {

                    if (boxObjectMacro == redMacro)
                    {
                        redMacro.IsChecked = false;
                    }
                    else if (boxObjectMacro == greenMacro)
                    {
                        greenMacro.IsChecked = false;
                    }
                    else if (boxObjectMacro == blueMacro)
                    {
                        blueMacro.IsChecked = false;
                    }

                    boxObjectMacro = sender;
                }
            }
            if (boxObjectMacro == redMacro)
            {
                selectedColor = listColors[0];
            }
            else if (boxObjectMacro == greenMacro)
            {
                selectedColor = listColors[1];
            }
            else if (boxObjectMacro == blueMacro)
            {
                selectedColor = listColors[2];
            }
        }

        private void checkBoxMicro_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender == boxObjectMicro)
            {
                boxObjectMicro = null;
            }
            else
            {
                if (boxObjectMicro == null)
                {
                    boxObjectMicro = sender;
                }
                else
                {

                    if (boxObjectMicro == redMicro)
                    {
                        redMicro.IsChecked = false;
                    }
                    else if (boxObjectMicro == greenMicro)
                    {
                        greenMicro.IsChecked = false;
                    }
                    else if (boxObjectMicro == blueMicro)
                    {
                        blueMicro.IsChecked = false;
                    }

                    boxObjectMicro = sender;
                }
            }
            if (boxObjectMicro == redMicro)
            {
                selecterColorMicro = listColorsMicro[0];
            }
            else if (boxObjectMicro == greenMicro)
            {
                selecterColorMicro = listColorsMicro[1];
            }
            else if (boxObjectMicro == blueMicro)
            {
                selecterColorMicro = listColorsMicro[2];
            }
        }

        public static bool cancel;

        private async void ImageButton_Clicked_3(object sender, EventArgs e)
        {
            await stopButton.ScaleTo(.55, 200, Easing.SpringOut);
            await stopButton.ScaleTo(.39, 200, Easing.SpringIn);
            cancel = true;
            bitmap = new SKBitmap(widthExport, heightExport,true);
            bitmapFast = new SKBitmap(widthExport, heightExport,true);
            canvasImage.IsVisible = false;
            canvasImageFast.IsVisible = false;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        /*** ADVANCED PAGE CODE ***/
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        
        public static ObservableCollection<string> listColorsAdvancedMacro = new ObservableCollection<string>
        {
            "Red","Green","Blue"

        };
        public static ObservableCollection<string> listColorsAdvancedMicro = new ObservableCollection<string>
        {
            "Red","Green","Blue"

        };

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            string newName = await DisplayPromptAsync("Save Settings", "Input the name for your new fractal", "OK", "Cancel", "Fractal Name");

            if (newName != null && newName != "")
            {
                var recipe = new FractalRecipe { FractalName = newName, SavedComplex = ciSlider.Value, SavedLength = lSlider.Value, SavedReal = riSlider.Value };

                await _connection.InsertAsync(recipe);
                SavedFractals.Add(recipe);

                Refresh();
            }
        }

        public static bool isCreating;
        private async void Button_Clicked(object sender, EventArgs e)
        {
            double x;
            if (Double.TryParse(riEntry.Text, out x) && Double.TryParse(ciEntry.Text, out x) && Double.TryParse(lEntry.Text, out x))
            {
                AdvancedModes.createAdvanced = true;

                AdvancedModes.realInput = Convert.ToDouble(riEntry.Text);
                AdvancedModes.complexInput = Convert.ToDouble(ciEntry.Text);
                AdvancedModes.lengthInput = Convert.ToDouble(lEntry.Text);

                isCreating = true;

                carouselPage.CurrentPage = carouselPage.Children[0];


                canvasImage.IsVisible = true;
                canvasImageFast.IsVisible = true;

                GenerateMandelbrotZoom(AdvancedModes.realInput, AdvancedModes.complexInput, AdvancedModes.lengthInput, AdvancedModes.macroInput, AdvancedModes.microInput);

                AdvancedModes.createAdvanced = false;
            }
            else
            {
                await DisplayAlert("Error", "You can only input double type values in the entry boxes.", "OK");
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            riSlider.Value = 0.000;
            ciSlider.Value = 0.000;
            lSlider.Value = 0.000;

            riEntry.Text = Convert.ToString(riSlider.Value);
            ciEntry.Text = Convert.ToString(ciSlider.Value);
            lEntry.Text = Convert.ToString(lSlider.Value);

            colorListAdvancedMicro.SelectedItem = listColorsAdvancedMicro[0];
            colorListAdvancedMacro.SelectedItem = listColorsAdvancedMacro[0];
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!AdvancedModes.isSetting)
            {
                riEntry.Text = Convert.ToString(riSlider.Value);
                ciEntry.Text = Convert.ToString(ciSlider.Value);
                lEntry.Text = Convert.ToString(lSlider.Value);

                AdvancedModes.realInput = riSlider.Value;
                AdvancedModes.complexInput = ciSlider.Value;
                AdvancedModes.lengthInput = lSlider.Value;
            }

        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AdvancedModes.isSetting)
            {
                double i = 0.00000;
                if (riEntry.Text != "")
                {
                    if (Double.TryParse(riEntry.Text, out i))
                    {
                        if (Double.Parse(riEntry.Text) != riSlider.Value)
                        {
                            riSlider.Value = Double.Parse(riEntry.Text);
                        }
                    }
                }
                if (ciEntry.Text != "")
                {
                    if (Double.TryParse(ciEntry.Text, out i))
                    {
                        if (Double.Parse(ciEntry.Text) != ciSlider.Value)
                        {
                            ciSlider.Value = Double.Parse(ciEntry.Text);

                        }
                    }
                }
                if (lEntry.Text != "")
                {
                    if (Double.TryParse(lEntry.Text, out i))
                    {
                        if (Double.Parse(lEntry.Text) != lSlider.Value)
                        {
                            lSlider.Value = Double.Parse(lEntry.Text);

                        }
                    }
                }

                if (riEntry.Text == "")
                {
                    riEntry.Text = "0.0";

                }
                if (ciEntry.Text == "")
                {
                    ciEntry.Text = "0.0";

                }
                if (lEntry.Text == "")
                {
                    lEntry.Text = "1.0";

                }
                AdvancedModes.realInput = riSlider.Value;
                AdvancedModes.complexInput = ciSlider.Value;
                AdvancedModes.lengthInput = lSlider.Value;
            }
        }

        private void colorListAdvancedMacro_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (sender == colorListAdvancedMacro)
            {
                AdvancedModes.macroInput = (string)(colorListAdvancedMacro.SelectedItem);
            }
            else
            {
                AdvancedModes.microInput = (string)(colorListAdvancedMicro.SelectedItem);
            }
        }
    }
}