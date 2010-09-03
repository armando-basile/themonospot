
using System;

namespace ThemonospotPluginMkv
{
	
	
	public class MkvAudioEntity
	{
		
		#region Properties
		
		private float _frequency = 0;
		
        public float Frequency
		{
			get{return _frequency;}
			set{_frequency = value;}
		}
        
		
		private int _channels = 0;
		
		public int Channels
		{
			get{return _channels;}
			set{_channels = value;}
		}		
		
		#endregion Properties
		

		
	}
}
