using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using System;

namespace Image_Slide_Show_Library {
	public class ImageSlideShow: FrameworkElement {
		public ImageSlideShow(string strImagePath,int init_IntervalTimer,Image myImage1,Image myImage2, Storyboard FadeIn, Storyboard FadeOut) {
			DirSearch(strImagePath);
			_ImageControls=new[] { myImage1, myImage2 }; _FadeIn=FadeIn; _FadeOut=FadeOut;
			_DispatcherTimer=new DispatcherTimer( ) { Interval=new TimeSpan(0, 0, _IntervalTimer=init_IntervalTimer) };
			_DispatcherTimer.Tick+=new EventHandler(DelayedUpdate);
			_DispatcherTimer.Start( );
			}

		protected DispatcherTimer _DispatcherTimer = null;

		public List<Uri> _ImageListFullFileNames = new List<Uri>( );
		private int _ImageListIndex = -1;
		private Storyboard _FadeIn, _FadeOut;
		public void DirSearch(string dir) {
			try {
				foreach(var f in Directory.GetFiles(dir))
					_ImageListFullFileNames.Add(new Uri(f)); //Console.WriteLine(f);
				foreach(var d in Directory.GetDirectories(dir))
					DirSearch(d);
				}
			catch(System.Exception ex) {
				//Console.WriteLine(ex.Message);
				}
			}
		private void DelayedUpdate(object s, EventArgs e) {
			try {
				PlaySlideShow( );
				}
			catch(System.Exception ex) {
				}
			}

		private int _CurrentSourceIndex=0, _CurrentCtrlIndex=0, _EffectIndex = 0, _IntervalTimer = 1;
		private List<ImageSource> _Images = new List<ImageSource>( );
		private static string[] _ValidImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
		private static string[] _TransitionEffects = new[] { "Fade" };
		private Image[] _ImageControls;

		private void PlaySlideShow( ) {
			try {
				if(_ImageListFullFileNames.Count==0)
					return;
				var oldCtrlIndex = _CurrentCtrlIndex;
				_CurrentCtrlIndex=(_CurrentCtrlIndex+1)%2;
				_CurrentSourceIndex=(_CurrentSourceIndex+1)%_ImageListFullFileNames.Count;
				var imgFadeOut = _ImageControls[oldCtrlIndex];
				var imgFadeIn = _ImageControls[_CurrentCtrlIndex];
				var newSource = (new BitmapImage(_ImageListFullFileNames[_ImageListIndex=_CurrentSourceIndex]));
				imgFadeIn.Source=newSource;
				_FadeOut.Begin(imgFadeOut);
				_FadeIn.Begin(imgFadeIn);
				}
			catch(Exception ex) { }
			}
		};
	};