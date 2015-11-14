using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsStoreTemplateUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private DataTransferManager dataTransferManager;


        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ReviewButton_Click(object sender, RoutedEventArgs e)
        {

            var uri = new Uri("ms-windows-store://review/?ProductId=9wzdncrdkwgx");  // replace "9wzdncrdkwgx" with your product ID
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // register this page as share source
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // unregister as share source
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }


        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            Uri dataPackageUri = new Uri("https://www.microsoft.com/store/apps/9nblgggz54hx");
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = "Funny App";
            requestData.SetWebLink(dataPackageUri);
            requestData.Properties.Description = "You can add some description there. Usually recommendation to install app";
        }


        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }



        public void ToggleAppBarButton(bool showPinButton)
        {
            if (showPinButton)
            {
                btnSecTile.Label = "Pin";
                btnSecTile.Icon = new SymbolIcon(Symbol.Pin);
                ShowToast("Tile is unpinned");
            }
            else
            {
                btnSecTile.Label = "UnPin";
                btnSecTile.Icon = new SymbolIcon(Symbol.UnPin);
                ShowToast("Tile is pinned");
            }
            this.btnSecTile.UpdateLayout();
        }


        private async void btnSecTile_Click(object sender, RoutedEventArgs e)
        {

            Windows.Foundation.Rect rect = GetElementRect((FrameworkElement)sender);

            if (SecondaryTile.Exists("MyUnicTileID"))
            {
                SecondaryTile secondaryTile = new SecondaryTile("MyUnicTileID");

                bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(rect, Windows.UI.Popups.Placement.Above);
                ToggleAppBarButton(isUnpinned);
            }
            else
            {
                // Pin
                Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
                string tileActivationArguments = "Secondary tile was pinned at = " + DateTime.Now.ToLocalTime().ToString();
                string displayName = "App Template";

                TileSize newTileDesiredSize = TileSize.Square150x150;
                SecondaryTile secondaryTile = new SecondaryTile("MyUnicTileID",
                                                                displayName,
                                                                tileActivationArguments,
                                                                square150x150Logo,
                                                                newTileDesiredSize);

                secondaryTile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;

                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(rect, Windows.UI.Popups.Placement.Above);
                ToggleAppBarButton(!isPinned);
 
            }

        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }



        void ShowToast(string whattext)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(whattext));
            ToastNotification toast = new ToastNotification(toastXml);

            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs args) { }
        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs args) { }
        private void ToastActivated(ToastNotification sender, object args) { }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowToast("You can show toast with simple void");
        }

    }
}
