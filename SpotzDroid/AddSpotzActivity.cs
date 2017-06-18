using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using Android;


namespace SpotzDroid
{
    [Activity(Label = "Add Spotz")]
    public class AddSpotzActivity : Activity
    {
        private Button _btnAddSpotz;
        private Button _btnTakePic;
        private ImageView _imageView;

        private static Java.IO.File _dir;
        private static Java.IO.File _file;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSpotzLayout);


            if (IsThereAnAppToTakePictures())
            {
                CheckPermissions();
                CheckIfDirectoryExists();
            }

            _imageView = FindViewById<ImageView>(Resource.Id.imageViewNewPic);



            _btnTakePic = FindViewById<Button>(Resource.Id.btnTakePic);
            _btnTakePic.Click += TakeApic;

            _btnAddSpotz = FindViewById<Button>(Resource.Id.btnAddSpotz);
            _btnAddSpotz.Click += _btnAddSpotz_Click;

        }

        private void TakeApic(object sender, EventArgs e)
        {
            var photoIntent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, $"Spotz_{Guid.NewGuid()}.jpg");
            photoIntent.PutExtra(
                MediaStore.ExtraOutput,
                Android.Net.Uri.FromFile(_file));

            StartActivityForResult(photoIntent, RequestCodes.TakePhotoRequest);
        }

        private void _btnAddSpotz_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }



        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok)
                return;

            switch (requestCode)
            {
                case RequestCodes.TakePhotoRequest:
                    MakeAvailableInGallery(_file);
                    ShowImage(_file.AbsolutePath);
                    break;
                default:
                    break;
            }
        }

        private void ShowImage(string path)
        {
            var bitmap = BitmapHelpers.LoadAndResizeBitmap(
                path,
                Resources.DisplayMetrics.HeightPixels,
                _imageView.Height);

            if (bitmap != null)
            {
                _imageView.SetImageBitmap(bitmap);
                //uploadButton.Enabled = true;
                bitmap.Dispose();
            }
        }

        private void MakeAvailableInGallery(Java.IO.File file)
        {
            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(file));

            SendBroadcast(mediaScanIntent);
        }




        private bool IsThereAnAppToTakePictures()
        {
            var takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            var availableActivities = PackageManager.QueryIntentActivities(
                takePictureIntent,
                PackageInfoFlags.MatchDefaultOnly);

            return availableActivities?.Count > 0;
        }

        private void CheckPermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var permission = Manifest.Permission.WriteExternalStorage;
                if (CheckSelfPermission(permission) == Permission.Denied)
                {
                    RequestPermissions(new[] { permission }, RequestCodes.UploadPhotoPermissionRequest);
                };
            }
        }

        private void CheckIfDirectoryExists()
        {
            _dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures),
                "Spotz");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }



    }

    public class RequestCodes
    {

        public const int TakePhotoRequest = 30101;
        public const int UploadPhotoRequest = 30102;
        public const int UploadPhotoPermissionRequest = 30101;
    }



    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }



}