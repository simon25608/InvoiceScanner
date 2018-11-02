using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesseract;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace InvoiceScanner
{
    public partial class MainPage : ContentPage
    {
        private readonly ITesseractApi _tesseractApi;
        private readonly IDevice _device;
        private Image _takenImage;
        private string _texto;
        private string myStringProperty;
        public string MyStringProperty
        {
            get { return myStringProperty; }
            set
            {
                myStringProperty = value;
                OnPropertyChanged(nameof(MyStringProperty)); // Notify that there was a change on this property
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _tesseractApi = Resolver.Resolve<ITesseractApi>();
            _device = Resolver.Resolve<IDevice>();
            initialize();
        }

        private async void initialize()
        {
            if (!_tesseractApi.Initialized)
            {
                await _tesseractApi.Init("spa");
                _tesseractApi.SetPageSegmentationMode(PageSegmentationMode.SparseText);
                string textPermited = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,.;€:";
                //abcdefghijklmnopqrstuvwxyz
                _tesseractApi.SetWhitelist(textPermited);
            }
        }

        async void TakePictureButton_Clicked(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                throw new Exception("No camera available.");

            }


            var opts = new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                //PhotoSize = PhotoSize.Full,
                //SaveToAlbum = true,
                //CompressionQuality = 90,
                //SaveMetaData = false,
                AllowCropping = true,                                
            };

            var file = await CrossMedia.Current.TakePhotoAsync(opts);




            var photo = file?.GetStream();
            if (photo != null)
            {



                var tessResult = await _tesseractApi.SetImage(photo);
                if (tessResult)
                {
                    var textReconized = _tesseractApi.Text;
                    var resultado = textReconized.Contains("Importe venta");
                    var test = _tesseractApi.Results(PageIteratorLevel.Textline);
                    foreach (var item in test)
                    {
                        if (item.Text.Contains("IMPORTE VENTA")) {
                            MyStringProperty = item.Text;
                        }

                    }

                }
            }

        }

    }
}
