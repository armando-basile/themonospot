
using System;
using log4net;


namespace ThemonospotComponents
{


	public class BytesManipulation
	{

		public BytesManipulation ()
		{
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
			dword[0] = (byte) (FourCC & 0x00FF);
			dword[1] = (byte) ((FourCC >> 8) & 0x000000FF);
			dword[2] = (byte) ((FourCC >> 16) & 0x000000FF);
			dword[3] = (byte) ((FourCC >> 24) & 0x000000FF);
			return dword;
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
		/// Get Ascii values from byte array, passed offset and bytes length
		/// </summary>		
		public string GetAsciiFromBytes(byte[] inputBytes, int offset, int length)
		{
			string tmpAsciiOut = "";
			
			if (inputBytes.Length == 0 )
			{
				return "";
			}
			
			for (int j=offset; j<(offset+length); j++)
			{
				if (inputBytes[j] != 0)
				{
					tmpAsciiOut = tmpAsciiOut + (char)inputBytes[j];
				}
			}
				
			return tmpAsciiOut;			
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
		
		
		
	}
}
