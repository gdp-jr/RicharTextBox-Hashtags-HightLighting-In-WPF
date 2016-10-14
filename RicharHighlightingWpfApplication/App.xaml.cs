using System.Windows.Markup;
using System.Windows;
using System.IO;
using System;

namespace Richar_highlighting_WpfApplication {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public static App Instance;
		public static String Directory;
		private String _DefaultStyle = "ShinyBlue.xaml";

		public App( ) {
			Directory=System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly( ).Location);
			string stringsFile = Path.Combine(Directory, "..\\..\\Themes\\", _DefaultStyle);
			LoadStyleDictionaryFromFile(stringsFile);
			Instance=this;
			}

		/// <summary>
		/// This funtion loads a ResourceDictionary from a file at runtime
		/// </summary>
		public void LoadStyleDictionaryFromFile(string inFileName) {
			if(File.Exists(inFileName)) {
				try {
					using(var fs = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
						var dic = (ResourceDictionary)XamlReader.Load(fs);	// Read in ResourceDictionary File
						Resources.MergedDictionaries.Clear( );							// Clear any previous dictionaries loaded
						Resources.MergedDictionaries.Add(dic);							// Add in newly loaded Resource Dictionary
						}
					}
				catch {
					}
				}
			}
		};
	};