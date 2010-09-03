
using System;
using ThemonospotComponents;

namespace ThemonospotPluginAvi
{
	
	
	/// <summary>
	/// AVIEXTHEADER - Structure of AVIEXTHEADER ('dmlh')
	/// </summary>
	public class AVIEXTHEADER
    {   
		
		#region Private Variables

        private int 	_dwGrandFrames;
        private int[] 	_dwFuture;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int DwGrandFrames 
		{	get {	return _dwGrandFrames;	}	}
	
		public int[] DwFuture
		{	get {	return _dwFuture;	}	}
	

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN, int dwFutureLen)
		{	
			BytesManipulation myEnc = new BytesManipulation();

			
			// Fill data in the structure			
			_dwGrandFrames = (int)myEnc.GetIntFromBytes(dataIN,0,4) ;
			if (dwFutureLen > 0)
			{
				_dwFuture = new int[dwFutureLen];
				for (int k=0; k<dwFutureLen; k++)
					_dwFuture[k] = dataIN[k+4];
			}
			
			return;
		}
	
	}

	
}
