using Adept.UnityXaml;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EditorSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void CubeButton_Click(object sender, RoutedEventArgs e)
        {
            await Unity.LoadSceneAsync("CubeScene");
            await new MessageDialog("Loaded").ShowAsync();
        }

        private async void Sphere_Click(object sender, RoutedEventArgs e)
        {
            await Unity.LoadSceneAsync("SphereScene");
            await new MessageDialog("Loaded").ShowAsync();
        }
    }
}
