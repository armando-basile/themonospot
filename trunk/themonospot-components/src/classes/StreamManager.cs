
using System;
using System.IO;
using System.Collections.Generic;

namespace ThemonospotComponents
{


	public class StreamManager
	{

		public StreamManager ()
		{
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
		
		
		
		
		

		
		
		
	}
}
