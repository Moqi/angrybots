/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using UnityEngine;
using System.Collections;
using System;

using Roar.Adapters;


namespace Roar.implementation.Adapters
{
	/**
	 * The interface to UrbanAirship notifications.
	 * 
	 * The call sequence is a little messy here.
	 * 
	 *     RoarIO.Start -> UrbanAirship.OnStart -> NotificationServices.RegisterForRemoteNotificationTypes (Apple)
	 * 
	 * Then we wait and check for registration to complete:
	 * 
	 *     RoarIO.Update -> UrbanAirship.OnUpdate -> remoteRegistrationSucceeded Event -> registerWithUrbanAirship (Roar Server) -> handleUASIOSRegister Callback.
	 */

	public class UrbanAirship : IUrbanAirship
	{
		public UrbanAirship( IWebAPI webAPI )
		{
			WebAPI_ = webAPI;
		}

		// Flag if the Device token has been sent to UA *this* session
		protected bool hasTokenBeenSentToUA = false;
		protected bool firedNotificationEvent = false;

		// How many remote notifications have we received this session
		protected int lastRemoteNotificationCount = 0;
		
		protected IWebAPI WebAPI_;

		// --------
		// Fired when remote notifications are successfully registered for
		public static event Action remoteRegistrationSucceeded;

		// Fired when remote notification registration fails
		// @note This is currently never triggered!
		public static event Action<string> remoteRegistrationFailed;

		// Fired when UrbanAirship registration succeeds
		public static event Action urbanAirshipRegistrationSucceeded;

		// Fired when UrbanAirship registration fails
		public static event Action<string> urbanAirshipRegistrationFailed;

		// Fired when a remote notification is received or game was launched from a remote notification
		// public static event Action<Hashtable> remoteNotificationReceived;
		// --------



		public void OnStart()
		{
			// Need to call register on EVERY start of the application
            //		
			// At some time in the future this call will cause NotificationServices.deviceToken to be set to 
		    // a usefull value. Since we dont know when this will happen we have to watch for it inside the OnUpdate
		    // function
			NotificationServices.RegisterForRemoteNotificationTypes( RemoteNotificationType.Alert |
					RemoteNotificationType.Badge | 
					RemoteNotificationType.Sound );

			remoteRegistrationSucceeded += registerWithUrbanAirship;
		}

		// Called onve we have a valid NotificationServices.deviceToken . We pass that value
		// on to the roar server in the correct form.
		void registerWithUrbanAirship()
		{
			byte[] token = NotificationServices.deviceToken;

			// Assuming we have a device token, proceed...
			if (token == null) return;
		
			// Swaps out XX-YY-ZZ to the required format XXYYZZ
			string formatToken = System.BitConverter.ToString( token ).Replace( "-", "" );

			Hashtable post = new Hashtable();
			post["device_token"] = formatToken;

			// Send registration token to UrbanAirship
			WebAPI_.urbanairship.ios_register( post, handleUASIOSRegister, null );
		}

		// Called with the response from the call to roar to register the deviceToken.
		void handleUASIOSRegister( IXMLNode d, int code, string msg, string callid, Hashtable opt )
		{
			Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;;

			if (code != IWebAPI.OK)
			{
				if (cb!=null) cb(new Roar.CallbackInfo(null,code,msg));
				if( urbanAirshipRegistrationFailed!=null) urbanAirshipRegistrationFailed(msg);
				return;
			}

			hasTokenBeenSentToUA = true;
			if( urbanAirshipRegistrationSucceeded!=null) urbanAirshipRegistrationSucceeded();
			if (cb!=null) cb( new Roar.CallbackInfo(null, IWebAPI.OK, null) );
		}


		public void OnUpdate()
		{
			//Check whether registration has returned from Apple.
			if( NotificationServices.deviceToken == null ) return;

			if( ! firedNotificationEvent )
			{
				firedNotificationEvent = true;
				if( remoteRegistrationSucceeded!=null) remoteRegistrationSucceeded();
			}

			pollForNewRemoteNotifications();
			return;
		}



		// Expose this to developers to enable them to wire push notices on ANYthing...
		// @todo This should accept a callback so that if roar goes bad they can catch it.
		public void sendPushNotification( string message, string targetUserID )
		{
			// Only allow send if this device has been registered
			if (!hasTokenBeenSentToUA) return;

			Hashtable post = new Hashtable();
			post["message"] = message;
			post["roar_id"] = targetUserID;
			
			WebAPI_.urbanairship.push( post, null, null );
		}


		protected void pollForNewRemoteNotifications()
		{
			if (NotificationServices.remoteNotificationCount > lastRemoteNotificationCount)
			{
				lastRemoteNotificationCount = NotificationServices.remoteNotificationCount;

				// @TODO: Fire event on new remote Notification/s
				// string tokenText = ( NotificationServices.remoteNotifications[0] as RemoteNotification).alertBody;
			}
		}
	}
}
