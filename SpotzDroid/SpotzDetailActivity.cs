using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Square.Picasso;
using Android.Graphics;
using Android.Util;

namespace SpotzDroid
{
    [Activity(Label = "Spotz Detailz")]
    public class SpotzDetailActivity : Activity
    {

        private readonly SpotzService _spotzService = new SpotzService();

        private readonly string _baseUrl = Application.Context.GetString(Resource.String.BaseUrl);


        private ImageView _imageMain;
        private ImageView _imageAvatar;
        private TextView _textTitle;
        private TextView _textDescription;
        private TextView _textAllTags;
        private TextView _textUsername;
        //private LinearLayout wrapperTags;
        private LinearLayout _wrapperComments;
        private Button _btnOpenMap;

        private string _spotzLatitude;
        private string _spotzLongitude;
        private string _spotzId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SpotzDetailLayout);

            _spotzId = Intent.GetStringExtra("spotz.spotzId");
            _spotzLatitude = Intent.GetStringExtra("spotz.latitude");
            _spotzLongitude = Intent.GetStringExtra("spotz.longitude");


            if (savedInstanceState != null)
            {
                _spotzId = savedInstanceState.GetString("spotz.spotzId");
                _spotzLatitude = savedInstanceState.GetString("spotz.latitude");
                _spotzLongitude = savedInstanceState.GetString("spotz.longitude");


                PopulateView(_spotzId);

            }

            _btnOpenMap = FindViewById<Button>(Resource.Id.btnOpenMap);

            _btnOpenMap.Click += _btnOpenMap_Click;

            PopulateView(_spotzId);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {

            outState.PutString("spotz.spotzId", _spotzId);
            outState.PutString("spotz.latitude", _spotzLatitude);
            outState.PutString("spotz.longitude", _spotzLongitude);

            base.OnSaveInstanceState(outState);
        }



        private void _btnOpenMap_Click(object sender, EventArgs e)
        {
            var geoUri = Android.Net.Uri.Parse(String.Format($"geo:{_spotzLatitude},{_spotzLongitude}?z=16"));
            var mapIntent = new Intent(Intent.ActionView, geoUri);

            // Sjekker om det finnes en map-app:
            var packageManager = PackageManager;
            var activities = packageManager.QueryIntentActivities(mapIntent, 0);

            // Hvis ingen map-app, åpnes stedet i browser:
            if (activities.Count == 0)
            {

                var uri = Android.Net.Uri.Parse(string.Format($"http://maps.google.com/maps?z=12&t=m&q=loc:{_spotzLatitude}+{_spotzLongitude}"));
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
            else
            {
                StartActivity(mapIntent);
            }
        }

        private void PopulateView(string spotzId)
        {
            _textTitle = FindViewById<TextView>(Resource.Id.textTitleSpotzDetail);
            _textDescription = FindViewById<TextView>(Resource.Id.textDescriptionSpotzDetail);
            _textUsername = FindViewById<TextView>(Resource.Id.textUsernameSpotzDetail);
            _textAllTags = FindViewById<TextView>(Resource.Id.textAllTags);
            _imageMain = FindViewById<ImageView>(Resource.Id.imageMainSpotzDetail);
            _imageAvatar = FindViewById<ImageView>(Resource.Id.imageAvatarSpotzDetail);
            //wrapperTags = FindViewById<LinearLayout>(Resource.Id.wrapperTagsSpotzDetail);
            _wrapperComments = FindViewById<LinearLayout>(Resource.Id.wrapperComments);

            RunOnUiThread(async () =>
            {

                var spotz = await _spotzService.GetOneSpotzFromJson(spotzId, _baseUrl, this);

                _textTitle.Text = spotz.Title;
                _textDescription.Text = spotz.Description;
                _textUsername.Text = spotz.UserName;
                _textAllTags.Text = spotz.TagsJoined;

                Picasso.With(this)
                .Load(_baseUrl + spotz.ImageUrl)
                //.Resize(400, 400)
                //.CenterCrop()
                .Into(_imageMain);

                Picasso.With(this)
                .Load(_baseUrl + spotz.GravatarUrl)
                .Resize(200, 200)
                .CenterCrop()
                .Into(_imageAvatar);


                //foreach (var tag in spotz.Tags)
                //{
                //    var text = new TextView(this);
                //    text.SetBackgroundColor(Color.DarkOrange);
                //    text.TextSize = 10;
                //    text.SetTextColor(Color.Black);

                //    text.Text = tag.TagName + ", ";

                //    wrapperTags.AddView(text);

                //}

                foreach (var comment in spotz.Comments)
                {
                    //var layout = new LinearLayout(this);
                    //layout.Orientation = Orientation.Horizontal;
                    //layout.SetBackgroundColor(Color.AliceBlue);
                    //layout.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
                    //layout.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;

                    var image = new ImageView(this);

                    //Picasso.With(this)
                    //    .Load(baseUrl + comment.CommentUserImgUrl)
                    //    .Resize(100, 100)
                    //    .CenterCrop()
                    //    .Into(image);

                    var textComment = new TextView(this);
                    textComment.SetBackgroundColor(Color.White);
                    textComment.SetTextColor(Color.Black);
                    textComment.Text = comment.UserName + " says:" + comment.CommentText;
                    textComment.Gravity = GravityFlags.Right;

                    //var textUsername = new TextView(this);
                    //textUsername.SetBackgroundColor(Color.PaleVioletRed);
                    //textUsername.Text = comment.UserName;
                    //textUsername.Gravity = GravityFlags.Left;

                    //layout.AddView(image);
                    //layout.AddView(text);

                    _wrapperComments.AddView(textComment);
                    //wrapperComments.AddView(textUsername);
                    //wrapperComments.AddView(image);

                }

            });
        }
    }
}