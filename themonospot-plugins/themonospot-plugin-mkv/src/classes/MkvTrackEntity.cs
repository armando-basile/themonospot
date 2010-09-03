
using System;
using System.Collections.Generic;

namespace ThemonospotPluginMkv
{
	
	
	public class MkvTrackEntity
	{

		
		#region Properties
		
		
        private int _number = 0;
        
		public int Number 
		{
			get {return _number;}
			set {_number = value;}
		}
		
		
		
		
		private string _uID = "";
        
		public string UID 
		{
			get {return _uID;}
			set {_uID = value;}
		}
		
		
		
		private int _type = 0;
        
		public int Type 
		{
			get {return _type;}
			set {_type = value;	}
		}
		
		
		
		private string _lang = "";
        
		public string Lang 
		{
			get {return _lang;}
			set {_lang = value;}
		}
		
		
		
		private string _fourCC = "";

		public string FourCC 
		{
			get {return _fourCC;}
			set {_fourCC = value;}
		}		

		
		private string _codec = "";

		public string Codec 
		{
			get {return _codec;}
			set {_codec = value;}
		}		
		
		
		private float _frameRate = 0;

		public float FrameRate 
		{
			get {return _frameRate;}
			set {_frameRate = value;}
		}		
		
		
		
		
		private List<MkvVideoEntity> _video = new List<MkvVideoEntity>();
		
		public List<MkvVideoEntity> Video 
		{
			get {return _video;}
			set {_video = value;}
		}
		
		
		
		
		private List<MkvAudioEntity> _audio = new List<MkvAudioEntity>();
		
		public List<MkvAudioEntity> Audio 
		{
			get {return _audio;}
			set {_audio = value;}
		}
		
		
		#endregion Properties








		
		
		
		
		
	}
}
