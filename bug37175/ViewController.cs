using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace bug37175
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			NSTimer.CreateScheduledTimer (1, (v) => Tick ());
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		async void Tick ()
		{

			var downloadmanager = new MyDownloadManager();

			var downloadqueue = new NSOperationQueue {
				MaxConcurrentOperationCount = 1
			};

			string sessionidentifier = "myapp.download.images";
			NSUrlSession nsurlsession = null;
			using (var configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration (sessionidentifier)) {
				nsurlsession = NSUrlSession.FromConfiguration (configuration, downloadmanager, downloadqueue);
			}

			for (int i = 0; i < 2000; i++) {
				var image = string.Format ("https://xamarin.com/content/images/pages/index/platform-screenshot@2x.png?id={0}", i);

				var ii = i;
				NSTimer.CreateScheduledTimer (TimeSpan.FromMilliseconds (10 + i / 10), (v) => {
					using (var url = NSUrl.FromString (image)) {
						using (var request = NSUrlRequest.FromUrl (url)) {
								
							var task = nsurlsession.CreateDownloadTask (request);
							task.Resume ();
							Console.WriteLine ("Created task #{0}: {1}", ii, task);
						}
					}
				});
			}
		}
	}

	class MyDownloadManager : NSUrlSessionDownloadDelegate {
		#region implemented abstract members of NSUrlSessionDownloadDelegate
		public override void DidFinishDownloading (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
		{
			Console.WriteLine ("DidFinishDownloading: {0} {1} {2}", session, downloadTask, location);
		}
		#endregion
	}
}

