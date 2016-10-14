using System.Collections.Generic;
using System.Windows.Media;
using System.IO;
using System;

namespace SoundBed {
	public class SoundLooper {
		public SoundLooper(string DirectoryName) {
			DirSearch(DirectoryName);
			if(_SoundUri.Count>0) {
				_SoundFileName=_SoundFullFileNames[1+_LoopIndex];
				_MediaPlayer.Open(_SoundUri[++_LoopIndex]); _MediaPlayer.Play( );
				_MediaPlayer.MediaEnded+=((o, e) => {
					_MediaPlayer.Open(_SoundUri[++_LoopIndex%_SoundUri.Count]);
					_SoundFileName=_SoundFullFileNames[_LoopIndex];
					_MediaPlayer.Play( );
				  });
				};
			}

		private List<Uri> _SoundUri = new List<Uri>( );
		private List<string> _SoundFullFileNames = new List<string>( );
		private string _SoundFileName=null;
		private int _LoopIndex = -1;
		private MediaPlayer _MediaPlayer = new MediaPlayer( );
		public MediaPlayer Player { get { return _MediaPlayer; } set { _MediaPlayer=value; } }

		public void DirSearch(string dir) {
			try {
				foreach(var f in Directory.GetFiles(dir)) {
					_SoundFullFileNames.Add(f);
					_SoundUri.Add(new Uri(f));
					}
				foreach(var d in Directory.GetDirectories(dir))
					DirSearch(d);
					}
			catch(System.Exception ex) {
				//Console.WriteLine(ex.Message);
				}
			}
		};
	};