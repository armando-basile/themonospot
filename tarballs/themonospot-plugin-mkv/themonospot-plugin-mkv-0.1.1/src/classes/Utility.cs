
using System;

namespace ThemonospotPluginMkv
{
	
	
	public class Utility
	{
		
		
		#region Properties
		
		
		private bool _debugMode = false;
		
		public bool DebugMode
		{
			get {return _debugMode;}
			set {_debugMode = value;}
		}
		
		
		
		
		
		
		#endregion Properties
		
		
		
		public Utility()
		{
		}
		
		
		
		
		
		
		/// <summary>
		/// Get Hexadecimal values of byte array
		/// </summary>
		/// <param name="inputBytes">
		/// A <see cref="System.Byte"/> Byte array to convert
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> Hexadecimal values
		/// </returns>
		public string GetHexFromBytes(byte[] inputBytes)
		{
			return GetHexFromBytes(inputBytes, 0, inputBytes.Length);		
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get Hexadecimal values of byte array
		/// </summary>
		/// <param name="inputBytes">
		/// A <see cref="System.Byte"/> Byte array to convert
		/// </param>
		/// <param name="offSet">
		/// A <see cref="System.Int32"/> Start position
		/// </param>
		/// <param name="len">
		/// A <see cref="System.Int32"/> Positions to convert
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> Hexadecimal values
		/// </returns>
		public string GetHexFromBytes(byte[] inputBytes, int offSet, int len)
		{
			int j;
			string tmpHexOut = "";
			
			if (inputBytes.Length < (offSet + len) )
			{
				return "";
			}
			
			
			for (j=offSet; j<(offSet + len); j++)
			{
				tmpHexOut = tmpHexOut + inputBytes[j].ToString("X2");
			}
			
			return tmpHexOut;		
		}
		
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get Hexadecimal values from ascii string
		/// </summary>
		/// <param name="inputAscii">
		/// A <see cref="System.String"/> Ascii string to convert
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> Hexadecimal values
		/// </returns>
		public string GetHexFromAscii(string inputAscii)
		{
			return GetHexFromBytes( System.Text.Encoding.ASCII.GetBytes(inputAscii) );
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get Ascii value from hexadecimal values
		/// </summary>
		/// <param name="inputHex">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string GetAsciiFromHex(string inputHex)
		{
			int n;
			string tmpAsciiOut = "";
			
			if (inputHex.Length == 0 )
			{
				return "";
			}
			
			for (int j=0; j<inputHex.Length; j+=2)
			{
				n = Convert.ToInt32(inputHex.Substring(j,2), 16);
				tmpAsciiOut = tmpAsciiOut + (char)n;
			}
			return tmpAsciiOut;
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get Ascii values from byte array
		/// </summary>
		/// <param name="inputBytes">
		/// A <see cref="System.Byte"/> Byte array to convert
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> Ascii values
		/// </returns>
		public string GetAsciiFromBytes(byte[] inputBytes)
		{
			string tmpAsciiOut = "";
			
			if (inputBytes.Length == 0 )
			{
				return "";
			}
			
			for (int j=0; j<inputBytes.Length; j++)
			{
				if (inputBytes[j] != 0)
				{
					tmpAsciiOut = tmpAsciiOut + (char)inputBytes[j];
				}
			}
				
			return tmpAsciiOut;			
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Swap passed bytes
		/// </summary>
		/// <param name="input">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		public byte[] swapByte(byte[] input)
		{
		    byte[] output = new byte[input.Length];
		    
		    for(int k=0; k<input.Length; k++)
		        output[input.Length - (k+1)] = input[k];
		    
		    return output;
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Debug message on console
		/// </summary>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public void DebugMsg(string msg)
		{
			//Output only with debug mode active
			if (_debugMode)
			{
				Console.WriteLine(msg);
			}
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Debug message on console with formatted string
		/// </summary>
		/// <param name="elementID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="length">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="offSet">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void DebugWrite(string elementID, long length, long offSet)
		{
			//Output only with debug mode active
			if (_debugMode)
			{
				string outMessage = elementID.PadRight(10) + " " +
					offSet.ToString("#,##0").PadLeft(16) + " " +
						length.ToString("#,##0").PadLeft(16);
				
				Console.WriteLine(outMessage);
			}
		}
		
		
		
		
	}
}
