using Xamarin.Forms;

namespace WhatAmILookingAt
{
    public partial class WhatAmILookingAtPage : ContentPage
    {
        public WhatAmILookingAtPage ()
        {
            InitializeComponent ();
            BindingContext = new WhatAmILookingAtViewModel ();
        }
    }
}
