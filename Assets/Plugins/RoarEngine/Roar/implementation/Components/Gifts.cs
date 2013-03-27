using System.Collections.Generic;
using Roar.Components;
using UnityEngine;
using Roar.DomainObjects;

namespace Roar.implementation.Components
{
	public class Gifts : IGifts
	{
		protected IDataStore dataStore;
		protected ILogger logger;
		IWebAPI.IMailActions giftsActions;

		public Gifts (IWebAPI.IMailActions mailActions, IDataStore dataStore, ILogger logger)
		{
			giftsActions = mailActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback< IDictionary<string,DomainObjects.MailPackage> > callback)
		{
			dataStore.giftsAcceptable.Fetch (callback);
		}
		
		public void FetchSendable (Roar.Callback< IDictionary<string,DomainObjects.Mailable> > callback)
		{
			dataStore.giftsSendable.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.giftsAcceptable.HasDataFromServer; } }

		public IList<DomainObjects.MailPackage> List ()
		{
			return dataStore.giftsAcceptable.List();
		}


		// Returns the gift Hashtable associated with attribute `id`
		public DomainObjects.MailPackage get (string id)
		{
			return dataStore.giftsAcceptable.Get (id);
		}
		
		protected class AcceptMailCallback : CBBase<WebObjects.Mail.AcceptResponse>
		{
			public AcceptMailCallback (Roar.Callback<WebObjects.Mail.AcceptResponse> in_cb) : base (in_cb) {}
		}
		
		public void AcceptGift (string gift_id, Roar.Callback<WebObjects.Mail.AcceptResponse> callback)
		{
			WebObjects.Mail.AcceptArguments args = new WebObjects.Mail.AcceptArguments();
			args.mail_id = gift_id;
			giftsActions.accept(args, new AcceptMailCallback(callback));
		}
		
		protected class SendMailCallback : CBBase<WebObjects.Mail.SendResponse>
		{
			public SendMailCallback (Roar.Callback<WebObjects.Mail.SendResponse> in_cb) : base (in_cb) {}
			
			public void OnError (Roar.RequestResult nn)
			{
			}
		}
		
		public void SendGift (string recipient_id, string mailable_id, string message, Roar.Callback<WebObjects.Mail.SendResponse> callback)
		{
			WebObjects.Mail.SendArguments args = new WebObjects.Mail.SendArguments();
			args.mailable_id = mailable_id;
			args.recipient_id = recipient_id;
			args.message = message;
			giftsActions.send(args, new SendMailCallback(callback));
		}
		
		protected class WhatCanIAcceptCallback : CBBase<WebObjects.Mail.WhatCanIAcceptResponse>
		{
			public WhatCanIAcceptCallback (Roar.Callback<WebObjects.Mail.WhatCanIAcceptResponse> in_cb) : base (in_cb) {}
		}
		
		public void ListAcceptableGifts (Roar.Callback<WebObjects.Mail.WhatCanIAcceptResponse> callback)
		{
			WebObjects.Mail.WhatCanIAcceptArguments args = new Roar.WebObjects.Mail.WhatCanIAcceptArguments();
			giftsActions.what_can_i_accept(args, new WhatCanIAcceptCallback(callback));
		}
		
		protected class WhatCanISendCallback : CBBase<WebObjects.Mail.WhatCanISendResponse>
		{
			public WhatCanISendCallback (Roar.Callback<WebObjects.Mail.WhatCanISendResponse> in_cb) : base (in_cb) {}
		}
		
		public void ListSendableGifts (Roar.Callback<WebObjects.Mail.WhatCanISendResponse> callback)
		{
			WebObjects.Mail.WhatCanISendArguments args = new WebObjects.Mail.WhatCanISendArguments();
			giftsActions.what_can_i_send(args, new WhatCanISendCallback(callback));
		}
	}
}
