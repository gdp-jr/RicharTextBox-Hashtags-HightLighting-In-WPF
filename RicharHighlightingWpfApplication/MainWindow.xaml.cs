using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System;

using Image_Slide_Show_Library;
using RichTextBox_HighLighting_for_Hash_tags;
using SoundBed;

namespace Richar_highlighting_WpfApplication {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow( ) {
			InitializeComponent( );
			var IntervalTimer=Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"]);
			var strImagePath=ConfigurationManager.AppSettings["ImagePath"];
			DirSearch(AppDomain.CurrentDomain.BaseDirectory+"..\\..\\Images");
			_HighLighter=new HighLight_Hash_Links_in_RicharTextBox_WPF(richTextBox) { };
			_DispatcherTimer =new DispatcherTimer( ) { Interval=new TimeSpan(0, 0, 5-4, 0, 10) };
			_DispatcherTimer.Tick+=new EventHandler(DelayedUpdate);
			_DispatcherTimer.Start( );
			var TransitionType="Fade";
			var StboardFadeOut = (Resources[string.Format("{0}Out", TransitionType.ToString( ))] as Storyboard).Clone( );
			var StboardFadeIn = Resources[string.Format("{0}In", TransitionType.ToString( ))] as Storyboard;
			_ImageSlideShow=new ImageSlideShow(AppDomain.CurrentDomain.BaseDirectory+"..\\..\\Images", IntervalTimer, myImageSlide1, myImageSlide2, StboardFadeIn, StboardFadeOut);
			_SoundLooper=new SoundLooper(AppDomain.CurrentDomain.BaseDirectory+"..\\..\\Music");
			}

		private HighLight_Hash_Links_in_RicharTextBox_WPF _HighLighter = null;
		private ImageSlideShow _ImageSlideShow = null;
		private SoundLooper _SoundLooper = null;

		protected DispatcherTimer _DispatcherTimer = null;

		public List<Uri> _ImageListFullFileNames=new List<Uri>( );
		private int _ImageListIndex = -1;

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
				if(++_ImageListIndex>=_ImageListFullFileNames.Count) _ImageListIndex=0;
				image.Source=(new BitmapImage(_ImageListFullFileNames[_ImageListIndex]));
				this.InvalidateVisual( );
				}
			catch(System.Exception ex) {
				}
			}

		private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) {
			Environment.Exit(0);
			}

		private void MenuItem_Play_Click(object sender, RoutedEventArgs e) {
			_SoundLooper.Player.Play( );
			}
		private void MenuItem_Pause_Click(object sender, RoutedEventArgs e) {
			_SoundLooper.Player.Pause( );
			}
		private void MenuItem_Stop_Click(object sender, RoutedEventArgs e) {
			_SoundLooper.Player.Stop( );
			}

		private void MenuItem_Style_Click(object sender, RoutedEventArgs e) {
			var mi = sender as MenuItem;
			var stylefile =System.IO.Path.Combine(App.Directory, "..\\..\\Themes\\", mi.Name+".xaml");
			App.Instance.LoadStyleDictionaryFromFile(stylefile);
			}

		private void button1_Click(object sender, RoutedEventArgs e) {
			richTextBox.Document.Blocks.Add(new Paragraph(new Run("Cool #Picture at @Google")));
			}
		private void button2_Click(object sender, RoutedEventArgs e) {
			richTextBox.Document.Blocks.Add(new Paragraph(new Run(@"@you #hello there #welcome to @ our
#Þárţƴ
a b
@ #
@a #b
#a @b
@a ##b
@#a #@b
#a#b
a@b
a#b
@a@b
@#a#@b
#@a#@b
@#@#a #@#@b
@#@#a#@#@b")));
			}
		};
	};