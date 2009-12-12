
using System;
using System.Collections.Generic;
using System.IO;

namespace ThemonospotPluginAvi
{
	
	/// <summary>
	/// Utility functions to manage avi file
	/// </summary>
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
		/// Read a DWORD from file stream
		/// </summary>
		/// <param name="fs">
		/// A <see cref="FileStream"/> File stream to read
		/// </param>
		/// <param name="sPos">
		/// A <see cref="System.Int32"/> Stream position to increment after reading
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> DWORD value
		/// </returns>
		public uint ReadDWord(ref FileStream fs, ref long sPos)
		{
			uint retValue;			
			byte[] tmpBuffer = new byte[4];
			
			fs.Read(tmpBuffer,0,4);			
			retValue = 
					(uint)(tmpBuffer[0])+
					(uint)((tmpBuffer[1]<<8))+
					(uint)((tmpBuffer[2]<<16))+
					(uint)((tmpBuffer[3]<<24));
			sPos += 4;
			
			return retValue;
		}
		
		
		
		/// <summary>
		/// Move pointer on file stream
		/// </summary>
		/// <param name="fs">
		/// A <see cref="FileStream"/>
		/// </param>
		/// <param name="sPos">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="seekLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void StreamSeek(ref FileStream fs, ref long sPos, uint seekLen)
		{
			fs.Seek(seekLen, SeekOrigin.Current);
			sPos+=seekLen;
		}
		
		
		
		/// <summary>
		/// Read bytes from a specific stream
		/// </summary>
		/// <param name="fs">
		/// A <see cref="FileStream"/>
		/// </param>
		/// <param name="sPos">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="readLen">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		public byte[] StreamRead(ref FileStream fs, ref long sPos, uint readLen)
		{
			byte[] tmpBytes = new byte[(int)readLen];
			fs.Read(tmpBytes, 0, (int)readLen);
			sPos += readLen;
			return tmpBytes;
		}
		
		
		
		
		
		/// <summary>
		/// Swap bytes in array
		/// </summary>
		/// <param name="inputArray">
		/// A <see cref="System.Byte"/> Array to swap
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/> Array swapped
		/// </returns>
		public byte[] SwapByte(byte[] inputArray)
		{
		    byte[] outputArray = new byte[inputArray.Length];
		    
		    for(int k=0; k<inputArray.Length; k++)
			{
		        outputArray[inputArray.Length - (k+1)] = inputArray[k];
			}
			
		    return outputArray;
		}
		
		
		
		
		/// <summary>
		/// Swap hexadecimal values in string
		/// </summary>
		/// <param name="inputHex">
		/// A <see cref="System.String"/> Hexadecimal to swap
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> Hexadecimal swapped
		/// </returns>
		public string SwapHex(string inputHex)
		{
			int j;
			string tmpHexOut = "";
			
			if (inputHex.Length == 0 )
			{
				return "";
			}
			
			for (j=(inputHex.Length-2); j>=0; j-=2)
			{
				tmpHexOut = tmpHexOut + inputHex.Substring(j,2) ;
			}
			
			
			return tmpHexOut;
		
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
		/// Get integer value from byte array (lsB = [0]; msB = [<max>];)
		/// </summary>
		/// <param name="inputBytes">
		/// A <see cref="System.Byte"/> Byte array to compute
		/// </param>
		/// <param name="offSet">
		/// A <see cref="System.Int32"/> Offset
		/// </param>
		/// <param name="len">
		/// A <see cref="System.Int32"/> Length
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> Integer value obtained
		/// </returns>
		public uint GetIntFromBytes(byte[] inputBytes, int offSet, int len)
		{
			uint n = 0;
			
			if (inputBytes.Length == 0 )
				return 0;
			
			for (int j=offSet; j<(len + offSet); j++)
			{
				n = n + (uint)(inputBytes[j]<<( 8*(j-offSet) ) );
			}
			
			return n;
		}
		
				
		
		
		
		#region FourCC Functions		
		
		
				
		/// <summary>
		/// Return chars of fourcc
		/// </summary>
		/// <param name="dWordFourcc">
		/// A <see cref="System.Int32"/> FourCC int value
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> FourCC chars
		/// </returns>
		public string GetFourccFromInt(uint Fourcc)
        {
            char[] chars = new char[4];
            chars[0] = (char)(Fourcc & 0xFF);
            chars[1] = (char)((Fourcc >> 8) & 0xFF);
            chars[2] = (char)((Fourcc >> 16) & 0xFF);
            chars[3] = (char)((Fourcc >> 24) & 0xFF);

            return new string(chars);
        }
		
		
		
		
		/// <summary>
		/// Get DWord from FourCC digits
		/// </summary>
		/// <param name="charsFourCC">
		/// A <see cref="System.String"/> FourCC digits
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> FourCC DWord
		/// </returns>
        public uint GetIntFromFourcc(string charsFourCC)
        {
            if (charsFourCC.Length != 4)
            {
                throw new Exception("FourCC strings must be 4 characters long " + charsFourCC);
            }

            uint result = ((uint)charsFourCC[3]) << 24
                        | ((uint)charsFourCC[2]) << 16
                        | ((uint)charsFourCC[1]) << 8
                        | ((uint)charsFourCC[0]);

            return result;
        }
		
				
		
		
		/// <summary>
		/// Get integer value from byte array
		/// </summary>
		/// <param name="FourCC">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
        public uint GetIntFromFourcc(byte[] FourCC)
        {
            return GetIntFromFourcc(System.Text.Encoding.ASCII.GetString(FourCC));
        }
		
		
		

		/// <summary>
		/// Get byte array from ascii FourCC value
		/// </summary>
		/// <param name="charsFourCC">
		/// A <see cref="System.String"/> Ascii FourCC value
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/> Byte array
		/// </returns>
		public byte[] GetBytesFromFourcc(string charsFourCC)
		{
            if (charsFourCC.Length != 4)
            {
                throw new Exception("FourCC strings must be 4 characters long " + 
				                    charsFourCC);
            }
			
			return System.Text.Encoding.ASCII.GetBytes(charsFourCC);
		}

		
		
		
		
		
		
		
		
		/// <summary>
		/// Obtain byte array from DWord
		/// </summary>
		/// <param name="FourCC">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		public byte[] GetBytesFromFourcc(uint FourCC)
		{			
			byte[] dword = new byte[4];
			FourCC = FourCC;
			dword[0] = (byte) (FourCC & 0x00FF);
			dword[1] = (byte) ((FourCC >> 8) & 0x000000FF);
			dword[2] = (byte) ((FourCC >> 16) & 0x000000FF);
			dword[3] = (byte) ((FourCC >> 24) & 0x000000FF);
			return dword;
		}


		
		
		
		#endregion FourCC Functions
		
		
		
		
		/// <summary>
		/// Add long value to long array
		/// </summary>
		/// <param name="inputArray">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="outputArray">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="newValue">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void AddValueToLongArray(long[] inputArray, out long[] outputArray, long newValue)
        {
        	long[] tmpArray = new long[inputArray.Length + 1];        	
        	Array.Copy(inputArray, tmpArray, inputArray.Length);        	
        	tmpArray[tmpArray.Length-1] = newValue;
        	outputArray = tmpArray;
        }
		
		
		
		
		/// <summary>
		/// Verify if smallArray is contained in bigArray, if yes, return offset
		/// </summary>
		/// <param name="bigArray">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <param name="smallArray">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <param name="offSet">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int CompareByteArray(byte[] bigArray, byte[] smallArray, int offSet)
		{
			int len1 = bigArray.Length;
			int len2 = smallArray.Length;

			if (len2 > len1)
			{
				return -2;
			}
			
			if ((offSet + len2 + 1) > len1)
			{
				return -3;
			}
			
			
			bool okCompare=false;
			
			for (int k=offSet; k<(len1-len2); k++)
			{
				okCompare=true;
				
				for (int j=0; j<len2; j++)
				{
					if (bigArray[k+j] != smallArray[j])
					{
						okCompare=false;
						break;					
					}
					
				}
				
				if (okCompare==true)	
				{
					return k;
				}
			
			}
			
			return -1;
		}
		
		
		
		
		/// <summary>
		/// Perform a good debug for avi sections
		/// </summary>
		/// <param name="hfourcc">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="sfourcc">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="position">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="length">
		/// A <see cref="System.UInt32"/>
		/// </param>
		public void DebugWrite(string hfourcc, string sfourcc, long position, uint length)
		{
			//Output only with debug mode active
			if (_debugMode)
			{
				string outMessage = sfourcc.PadRight(20, Convert.ToChar(".")) + " " +
					position.ToString("#,###").PadLeft(13) + " " +
						length.ToString("#,###").PadLeft(13) + "  " +
						"0x" + hfourcc;
				
				Console.WriteLine(outMessage);
			}
		}
		
		
	}
}
