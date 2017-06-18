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
using Java.Lang;
using Square.Picasso;
using Android.Webkit;
using System.Threading.Tasks;

namespace SpotzDroid
{

    public class ViewHolder : Java.Lang.Object
    {
        public ImageView ImageViewSpotz { get; set; }
        public WebView WebImageView { get; set; }
        public TextView TextTitle { get; set; }
        public TextView TextDescription { get; set; }
        public TextView UserName { get; set; }
    }



    class ListSpotzAdapter : BaseAdapter<SpotzViewModel>
    {

        public ListSpotzAdapter(Activity activity, List<SpotzViewModel> spotzes, string baseUrl)
        {
            this._activity = activity;
            this._spotzes = spotzes;
            this._baseUrl = baseUrl;
        }


        private readonly Activity _activity;
        private readonly List<SpotzViewModel> _spotzes;
        private readonly string _baseUrl;
        //private SpotzService _spotzService = new SpotzService();


        public override int Count
        {
            get
            {
                return _spotzes.Count;
            }
        }

        public override SpotzViewModel this[int position]
        {
            get
            {
                return _spotzes[position];
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return Convert.ToInt32(_spotzes[position].SpotzId);
        }

        public override long GetItemId(int position)
        {
            return Convert.ToInt64(position);
        }



        public override View GetView(int position, View convertView, ViewGroup parent)
        {


            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.list_view_data_template, parent, false);

            var textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            var textDescription = view.FindViewById<TextView>(Resource.Id.textDescription);
            var textUserName = view.FindViewById<TextView>(Resource.Id.textUserName);
            var imageView = view.FindViewById<ImageView>(Resource.Id.imageViewSpotz);

            textTitle.Text = _spotzes[position].Title;
            textDescription.Text = _spotzes[position].ShortDescription;
            textUserName.Text = _spotzes[position].UserName;

            Picasso.With(parent.Context)
                .Load(_baseUrl + _spotzes[position].ImageUrl)
                .Resize(400, 400)
                .CenterCrop()
                .Into(imageView);

            return view;
        }
    }
}