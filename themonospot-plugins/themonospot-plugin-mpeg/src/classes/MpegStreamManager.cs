
using System;
using System.IO;
using System.Text;
using ThemonospotComponents;
using log4net;

namespace ThemonospotPluginMpeg
{
	
	
	public class MpegStreamManager
	{
		// CONSTANTS
		const int BUFFER_SIZE = 8192; // 8K buffer
		
		// ATTRIBUTES		
		private BinaryReader br;
		
		private byte[] forwardBuffer;
	    private long forwardBufferStart = 0;
	    private long forwardBufferEnd = 0;		
		private long fileSize = 0;
		
		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(MpegStreamManager));
		
		
		// PROPERTIES
		
		public long StreamSize
		{	
			get {	return fileSize;	}
		}
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MpegStreamManager(string mpegFilePath)
		{
			br = new BinaryReader(new FileStream(mpegFilePath, 
			                                     FileMode.Open, 
			                                     FileAccess.Read, 
			                                     FileShare.Read));
			fileSize = br.BaseStream.Length;
			forwardBuffer = new byte[BUFFER_SIZE];
		}
		
		
		
		public void Close()
		{
			br.Close();
			br = null;			
		}
		
	
	 
		/// <summary>
		/// Read single byte from file buffer managing it
		/// </summary>
	    public byte GetByte(long offset)	
	    {

	        long pos = -1;
	
	        if ((offset >= forwardBufferEnd) || 
			    (offset < forwardBufferStart) ) 	
	        {
				// need new buffer
	            pos = offset + BUFFER_SIZE;
	
	            if (pos > fileSize)
				{ 
					pos = fileSize; 
				}
	
	            forwardBufferStart = offset;
				forwardBufferEnd = pos;

				br.BaseStream.Seek(offset, SeekOrigin.Begin);	
	            br.Read(forwardBuffer, 0, BUFFER_SIZE);
	
	        }
	        return forwardBuffer[offset - forwardBufferStart];
	
	    }
			
		
		
		
	    public int GetSize(long offset)	
	    {
	        return GetByte(offset) * 256 + 
				   GetByte(offset + 1);
	    }
		
		
		
		
		
		/// <summary>
		/// Search position of marker
		/// </summary>
		public bool SearchMarker(ref long offset, byte[] marker)
		{
			bool isFounded;
			
	        for (long i = offset; i < fileSize - marker.Length; i++)	
	        {
				isFounded = true;
				
				for (int j=0; j<marker.Length; j++)
				{
					if (GetByte(i+j) != marker[j])
					{
						isFounded = false;
						break;
					}
				}
				
				if (isFounded)
				{	
	                offset = i;	
	                return true;

	            }
	
	        }	
			
	        return false;
		}
		
		
		
		
		
		
		
		
		
		
		

	    public bool FindNextMarker(ref long offset, ref byte marker)
	    {	
			bool isFounded = SearchMarker(ref offset, new byte[3]{0x00, 0x00, 0x01});
			
			if (!isFounded)
			{
				return false;
			}
			
			marker = GetByte(offset + 3);
			
			return true;
	    }
	
	 
	
	 

	
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	}
}
