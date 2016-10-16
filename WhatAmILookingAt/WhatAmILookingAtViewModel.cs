using System;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Threading.Tasks;

using System.ComponentModel;

namespace WhatAmILookingAt
{
    public class WhatAmILookingAtViewModel : INotifyPropertyChanged
    {
        // Get your key at: https://www.microsoft.com/cognitive-services/en-us/computer-vision-api
        private const string COMPUTER_VISION_API_KEY = "YOUR_API_KEY";

        public WhatAmILookingAtViewModel ()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Command _takePictureCommand;
        public Command TakePictureCommand
        {
            get
            {
                return _takePictureCommand ??
                    (_takePictureCommand = new Command (async () => await ExecuteTakePictureCommandAsync ()));
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            private set
            {
                _description = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged (this,
                        new PropertyChangedEventArgs ("Description"));
                }
            }
        }

        async Task ExecuteTakePictureCommandAsync ()
        {
            try
            {
                // Take a picture.
                await CrossMedia.Current.Initialize ();

                MediaFile photo;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync (new StoreCameraMediaOptions {
                        Directory = "WhatAmILookingAt",
                        Name = "what.jpg"
                    });
                }
                else
                {
                    photo = await CrossMedia.Current.PickPhotoAsync ();
                }

                // Let Computer Vision help on what you're looking at
                Description = "Let me think...";

                AnalysisResult result;
                var client = new VisionServiceClient (COMPUTER_VISION_API_KEY);
                using (var photoStream = photo.GetStream ())
                {
                    result = await client.DescribeAsync (photoStream);
                }

                // Parse the result
                var caption = result.Description.Captions [0].Text;
                var confidence = Math.Floor( result.Description.Captions [0].Confidence * 100);

                Description = $"I think you're looking at {caption}. I'm {confidence}% sure.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine ($"ERROR: {ex.Message}");
                Description = "Sorry, I don't know what you're looking at.";
            }
        }
    }
}
