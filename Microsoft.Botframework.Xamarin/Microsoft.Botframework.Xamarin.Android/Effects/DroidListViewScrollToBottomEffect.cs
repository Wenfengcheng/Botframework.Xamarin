﻿using Android.Widget;
using Microsoft.Botframework.Xamarin.CustomViews;
using Microsoft.Botframework.Xamarin.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("conversation.Effects")]
[assembly: ExportEffect(typeof(DroidListViewScrollToBottomEffect), "ListViewScrollToBottomEffect")]

namespace Microsoft.Botframework.Xamarin.Droid.Effects
{
    public class DroidListViewScrollToBottomEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = Control as Android.Widget.ListView;
            if (listView != null)
            {
                listView.TranscriptMode = TranscriptMode.AlwaysScroll;

                MessagingCenter.Subscribe<AdaptiveViewCell>(this, "ScrollToBottom", (sender) => {
                    listView.SmoothScrollToPosition(listView.Adapter.Count);
                });
            }
        }

        protected override void OnDetached()
        {
            MessagingCenter.Unsubscribe<AdaptiveViewCell>(this, "ScrollToBottom");
        }
    }
}