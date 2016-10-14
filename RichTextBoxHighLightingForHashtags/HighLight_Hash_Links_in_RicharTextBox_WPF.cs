using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;

namespace RichTextBox_HighLighting_for_Hash_tags {
	public class HighLight_Hash_Links_in_RicharTextBox_WPF : IDisposable {
		public HighLight_Hash_Links_in_RicharTextBox_WPF(RichTextBox initRichTextBox) {
			DataObject.AddSettingDataHandler(initRichTextBox, MySettingDataCommand);
			DataObject.AddPastingHandler(initRichTextBox, MyPasteCommand);
			DataObject.AddCopyingHandler(initRichTextBox, MyCopyCommand);
			_DispatcherTimer=new DispatcherTimer( ) { Interval=new TimeSpan(0, 0, 0, 0, 10) };
			_DispatcherTimer.Tick+=new EventHandler(DelayedUpdate);
			_RichTextBox=initRichTextBox;
			_RichTextBox.TextChanged+=new System.Windows.Controls.TextChangedEventHandler(TextChanged);
			_RichTextBox.KeyDown+=new System.Windows.Input.KeyEventHandler(KeyDown);
			_RichTextBox.KeyUp+=new System.Windows.Input.KeyEventHandler(KeyUp);
			_RichTextBox.MouseDown+=new System.Windows.Input.MouseButtonEventHandler(MouseDown);
			_RichTextBox.MouseUp+=new System.Windows.Input.MouseButtonEventHandler(MouseUp);
			StartMagic( );
			}
		private void DelayedUpdate(object s, EventArgs e) {
			_DispatcherTimer.Stop( );
			StartMagic( );
			}
		private void MySettingDataCommand(object sender, DataObjectEventArgs e) {
			//do stuff
			}
		private void MyPasteCommand(object sender, DataObjectEventArgs e) {
			//do stuff
			}
		private void MyCopyCommand(object sender, DataObjectEventArgs e) {
			//do stuff
			}
		protected DispatcherTimer _DispatcherTimer = null;
		private FlowDocument _DocumentNew = null;
		private String _TextOld = "";
		private bool _Changed = false;
		private RichTextBox _RichTextBox;
		private int _Offset = -1;
		private Regex _FindRegExp = new Regex(@"([^@#]+|([@#]\w[\w\d]*(\s|$)+)|[@#]|\s+)", RegexOptions.Multiline|RegexOptions.Compiled);
		public void KeyDown(object sender, KeyEventArgs e) {
			_Offset=CharOffset( );
			}
		public List<Inline> MagicList = null;
		protected RichTextBox RTB { get { return _RichTextBox; } }
		public string Text {
			get {
				var tp = new TextRange(_RichTextBox.Document.ContentStart, _RichTextBox.Document.ContentEnd).Text;
				return _RichTextBox.Document.Tag is int ? (int)_RichTextBox.Document.Tag==-2 ? tp.Substring(0, tp.Length-2) : tp : tp;
				}
			set { new TextRange(_RichTextBox.Document.ContentStart, _RichTextBox.Document.ContentEnd).Text=value; }
			}
		public void Changed( ) {
			_Changed=Text!=_TextOld;
			}
		public void Update( ) {
			_TextOld=Text;
			}
		public void TextChanged(object sender, TextChangedEventArgs e) {
			_DispatcherTimer.Start( );
			}
		public bool DoMagic = true;
		public void KeyUp(object sender, KeyEventArgs e) {
			StartMagic( );
			}
		public void MouseDown(object sender, MouseButtonEventArgs e) {
			_Offset=CharOffset( );
			}
		public void MouseUp(object sender, MouseButtonEventArgs e) {
			_Offset=CharOffset( );
			}
		public Stack<String> StartMagic( ) {
			Changed( );
			_RichTextBox.InvalidateVisual( );
			if(DoMagic=_Changed||_Offset==-1) {
				MagicList=MagicHell( );
				var CO = CharOffset( );
				_DocumentNew.Tag=-2;
				_RichTextBox.Document=_DocumentNew;
				_RichTextBox.Document.Tag=-2;
				_RichTextBox.CaretPosition=((_Offset=CO)<MagicList.Count) ? MagicList[_Offset=CO].ContentEnd : _DocumentNew.ContentEnd;
				_RichTextBox.InvalidateVisual( );
				}
			_TextOld=Text;
			return Mtch;
			}
		public Stack<String> Mtch = new Stack<String>( );
		public int CharOffset(FlowDocument document = null) {
			int Backs = -1, Returns = 0;
			var CaretStart = _RichTextBox.CaretPosition;
			var SegmentContentStart = _RichTextBox.Document.ContentStart;
			while(CaretStart!=null&&CaretStart.CompareTo(SegmentContentStart)>0) {
				CaretStart=CaretStart.GetPositionAtOffset(-1, LogicalDirection.Backward);
				while(CaretStart!=null&&CaretStart.GetPointerContext(LogicalDirection.Backward)!=TextPointerContext.Text)
					CaretStart=CaretStart.GetPositionAtOffset(-1, LogicalDirection.Backward);
				if(CaretStart!=null&&((System.Windows.Documents.Run)CaretStart.Parent).Text=="\r\n") Returns++;
				Backs++;
				}
			return Backs-(Returns/2);
			}
		public List<Inline> MagicHell(FlowDocument document = null, Regex FindRegex = null) {
			var Return = new List<Inline>( );
			var textRun = document==null ? Text : new TextRange(document.ContentStart, document.ContentEnd).Text;
			if(textRun==null||textRun=="") return Return;
			var matches = FindRegex==null ? _FindRegExp.Matches(textRun) : FindRegex.Matches(textRun);
			var mcFlowDoc = new FlowDocument( );            // Create a FlowDocument
			var para = new Paragraph( );                   // Create a paragraph with text
			var Offset = 0;
			Inline TextElement = null;
			Mtch.Clear( );
			foreach(Match match in matches) {
				int startIndex = match.Index;
				int length = match.Length;
				Mtch.Push(match.Value);
				for(int c = 0, l = length;c<l;c++) {
					if(match.Value[c]==' ')
						para.Inlines.Add(TextElement=new Run(" ") { Tag=Offset, Foreground=Brushes.White });
					else if(match.Value[c]=='\r')
						continue;
					else if(match.Value[c]=='\n')
						para.Inlines.Add(TextElement=new Run("\r\n") { Tag=Offset, Foreground=Brushes.White });
					else if(match.Value[c]=='\t')
						para.Inlines.Add(TextElement=new Run("\t") { Tag=Offset, Foreground=Brushes.White });
					else if(match.Value.Length==1)
						para.Inlines.Add(TextElement=new Run(match.Value[c].ToString( )) { Tag=Offset, Foreground=Brushes.Black });
					else if(match.Value.StartsWith("@"))
						para.Inlines.Add(TextElement=new Run(match.Value[c].ToString( )) { Tag=Offset, Foreground=Brushes.Red });
					else if(match.Value.StartsWith("#"))
						para.Inlines.Add(TextElement=new Run(match.Value[c].ToString( )) { Tag=Offset, Foreground=Brushes.Blue });
					else
						para.Inlines.Add(TextElement=new Run(match.Value[c].ToString( )) { Tag=Offset, Foreground=Brushes.Black });
					Return.Add(TextElement); //Offset++
					}
				}
			Mtch.Push(textRun);
			mcFlowDoc.Blocks.Add(para);            // Add the paragraph to blocks of paragraph
			_DocumentNew=mcFlowDoc;
			var CaretTarget = mcFlowDoc.ContentStart;
			return Return;
			}
		private void LoadTextDocument(string fileName) {
			TextRange range;
			System.IO.FileStream fStream;
			if(System.IO.File.Exists(fileName)) {
				range=new TextRange(_RichTextBox.Document.ContentStart, _RichTextBox.Document.ContentEnd);
				fStream=new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate);
				range.Load(fStream, System.Windows.DataFormats.Text);
				fStream.Close( );
				}
			}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					this._DispatcherTimer.Stop( );
					this._DispatcherTimer=null;
					_RichTextBox.TextChanged-=new System.Windows.Controls.TextChangedEventHandler(TextChanged);
					_RichTextBox.KeyDown-=new System.Windows.Input.KeyEventHandler(KeyDown);
					_RichTextBox.KeyUp-=new System.Windows.Input.KeyEventHandler(KeyUp);
					_RichTextBox.MouseDown-=new System.Windows.Input.MouseButtonEventHandler(MouseDown);
					_RichTextBox.MouseUp+=new System.Windows.Input.MouseButtonEventHandler(MouseUp);
					this._RichTextBox=null;
					this._DocumentNew=null;
					this._FindRegExp=null;
					this.MagicList=null;
					this._TextOld=null;
					this.Mtch=null;
					}
				// TO: free unmanaged resources (unmanaged objects) and override a finalizer below. : set large fields to null.
				disposedValue=true;
				}
			}

		// Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~HighLight_Links_in_RicharTextBox_WPF() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }
		void IDisposable.Dispose( ) {
			Dispose(true);
			GC.SuppressFinalize(this);
			}
		#endregion
		};
	};