using Android.App;
using Android.Views;
using Android.Widget;
using CCBBAndroid;

namespace br.com.idbgames.chickachickaboomboomone
{
    public class MySpinnerAdapter : BaseAdapter<int>
    {
        int[] items; 
        Activity context;

        public MySpinnerAdapter(Activity context, int[] items)
           : base()
       {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override int this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;

            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.row, null);

            ImageView icon = (ImageView)view.FindViewById(Resource.Id.image);
            icon.SetImageResource(item);

            return view;
        }       
    }
}