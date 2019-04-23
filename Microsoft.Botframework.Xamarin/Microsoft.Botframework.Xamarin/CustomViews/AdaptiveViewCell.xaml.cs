using AdaptiveCards;
using AdaptiveCards.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Microsoft.Botframework.Xamarin.CustomViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdaptiveViewCell : ViewCell
	{
        private AdaptiveCardRenderer _adaptiveCardsRenderer;
        public AdaptiveViewCell ()
		{
			InitializeComponent ();
            _adaptiveCardsRenderer = new AdaptiveCardRenderer();
        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is Bot.Connector.DirectLine.Activity)
            {
                var activity = BindingContext as Bot.Connector.DirectLine.Activity;

                if (activity?.Attachments != null &&
                    activity.Attachments.Any(m => m.ContentType == "application/vnd.microsoft.card.adaptive"))
                {
                    var cardAttachments = activity.Attachments.Where(m => m.ContentType == "application/vnd.microsoft.card.adaptive");

                    foreach (var attachment in cardAttachments)
                    {
                        var jObject = (JObject)attachment.Content;
                        AdaptiveCards.AdaptiveCard card;
                        try
                        {
                            card = jObject.ToObject<AdaptiveCard>();
                        }
                        catch (Exception ex)
                        {
                            return;
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (!content.Children.Any())
                            {
                                var xaml = _adaptiveCardsRenderer.RenderCard(card);

                                if (xaml.View != null)
                                {
                                    xaml.OnAction += (s, args) => {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            MessagingCenter.Send(this, "AdaptiveCardAction", args);
                                        });
                                    };

                                    content.Children.Add(xaml.View);

                                    MessagingCenter.Send(this, "ReloadUITableViewData");

                                }
                                else
                                {
                                    content.Children.Add(new Label() { Text = activity.Summary });
                                }
                            }
                        });
                    }
                }
            }

            base.OnBindingContextChanged();
        }
    }
}