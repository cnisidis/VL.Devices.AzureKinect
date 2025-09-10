using CommunityToolkit.HighPerformance;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Runtime.InteropServices;
using ImageFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat;
using KinectImage = Microsoft.Azure.Kinect.Sensor.Image;

namespace VL.Devices.AzureKinect
{
    static class ImageDecompression
    {
        
        public static KinectImage Decompress(this KinectImage image)
        {
            if (image.Format != ImageFormat.ColorMJPG)
                throw new ArgumentException("Expected MJPG format", nameof(image));

            JpegDecoderOptions decoderOptions = new JpegDecoderOptions()
            {
               GeneralOptions = new DecoderOptions()
               { 
                   TargetSize = new Size(image.WidthPixels, image.HeightPixels),    
               
               }

            };
            

            using (var decodedImage = JpegDecoder.Instance.Decode<Bgra32>(decoderOptions, image.AsStream()))
            {
                var kinectImage = new KinectImage(ImageFormat.ColorBGRA32, decodedImage.Width, decodedImage.Height)
                {
                    DeviceTimestamp = image.DeviceTimestamp,
                    Exposure = image.Exposure,
                    ISOSpeed = image.ISOSpeed,
                    SystemTimestampNsec = image.SystemTimestampNsec,
                    WhiteBalance = image.WhiteBalance
                };
                decodedImage.CopyPixelDataTo(kinectImage.Memory.Span);
                return kinectImage;
            }
        }
    }
}
