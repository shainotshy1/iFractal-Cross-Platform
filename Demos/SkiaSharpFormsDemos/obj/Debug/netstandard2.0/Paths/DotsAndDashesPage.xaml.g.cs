//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::Xamarin.Forms.Xaml.XamlResourceIdAttribute("SkiaSharpFormsDemos.Paths.DotsAndDashesPage.xaml", "Paths/DotsAndDashesPage.xaml", typeof(global::SkiaSharpFormsDemos.Paths.DotsAndDashesPage))]

namespace SkiaSharpFormsDemos.Paths {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("Paths\\DotsAndDashesPage.xaml")]
    public partial class DotsAndDashesPage : global::Xamarin.Forms.ContentPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.Picker strokeCapPicker;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.Picker dashArrayPicker;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::SkiaSharp.Views.Forms.SKCanvasView canvasView;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(DotsAndDashesPage));
            strokeCapPicker = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Picker>(this, "strokeCapPicker");
            dashArrayPicker = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Picker>(this, "dashArrayPicker");
            canvasView = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::SkiaSharp.Views.Forms.SKCanvasView>(this, "canvasView");
        }
    }
}
