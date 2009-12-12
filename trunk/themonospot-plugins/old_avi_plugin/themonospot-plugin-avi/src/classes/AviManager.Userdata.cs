
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ThemonospotPluginAvi
{
	
	
	public partial class AviManager
	{
		
		public event StatusChanged StatusChangedEvent;
		
		private int copyBufferSize = 4096;
		private int updateEachFrames = 10;
		private long moviItems = 0;
		private long newMoviSize = 0;
		private long[] moviFrameOffset;
		private long[] moviFrameLength;
		
		
		/// <summary>
		/// Change UserData if there is DivX... info
		/// </summary>
		public void ModifyUserData(string outputFilePath)
		{
			// Refill all structures
			StatusChangedEvent(this, new StatusChangedEventArgs("Scan source avi file", 50));
			GetInfo();
			
			moviItems = (_aviContainer.IdxSize / 16) + 3000;
			
			userDataToChange = "";
			
			// Verify if is necessary update UserData
			for (int h=0; h<_aviContainer.MoviUserData.Count; h++)
			{
				if (_aviContainer.MoviUserData[h].IndexOf("DivX") == 0 &&
				    _aviContainer.MoviUserData[h] != "DivX999b000p")
				{
					userDataToChange = _aviContainer.MoviUserData[h];
					break;
				}
			}
			
			
			if (userDataToChange == "")
			{
				// Changing of userdata is not necessary
				StatusChangedEvent(this, new StatusChangedEventArgs("Changing is not necessary", 100));
				return;
			}
			
			moviFrameOffset = new long[moviItems];
			moviFrameLength = new long[moviItems];
			
			WriteNewAviFile(outputFilePath);			
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Write new avi file with UserData value changed
		/// </summary>
		/// <param name="newFilePath">
		/// A <see cref="System.String"/>
		/// </param>
		private void WriteNewAviFile(string newFilePath)
		{
			// Open source file and reset stream position
			ifs = new FileStream(_aviFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			streamPosition=0;
			
			// Create New destination File
			ofs = new FileStream(newFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
			
			// Copy all bytes before MOVI chunk
			if (!WriteBeforeMovi())
			{
				return;
			}
			
			
			// Copy MOVI area modified
			if (!WriteMovi())
			{
				return;
			}
			

			
			// Copy area before IDX area
			if (!WriteBeforeIDX())
			{
				return;
			}
			
			
			
			
			if(!WriteIDX())
			{
				return;
			}
			
			
			if(!WriteAfterIDX())
			{
				return;
			}
			
			ofs.Close();
			ofs.Dispose();
			ofs = null;
			ifs.Close();
			ifs.Dispose();
			ifs = null;
			StatusChangedEvent(this, new StatusChangedEventArgs("", 100));			
		}

		
		
		
		
		
		/// <summary>
		/// Copy all bytes before MOVI chunk (SIZE 
		/// </summary>
		private bool WriteBeforeMovi()
		{
			StatusChangedEvent(this, new StatusChangedEventArgs("Write area before MOVI chunk", 0));
			
			int bufferSize = copyBufferSize;
			int percent = 0;
			
			byte[] copyBuffer;
			
			while (streamPosition < _aviContainer.MoviOffset)
			{
				if (streamPosition + bufferSize > _aviContainer.MoviOffset)
				{
					bufferSize = (int)(_aviContainer.MoviOffset - streamPosition);
				}
				
				// Update destination from source file
				copyBuffer = new byte[bufferSize];
				ifs.Read(copyBuffer, 0, bufferSize);
				ofs.Write(copyBuffer, 0, bufferSize);
				streamPosition += bufferSize;
				
				percent = Convert.ToInt32((streamPosition / _aviContainer.MoviOffset) * 100);
				if (percent == 100)
				{
					percent = 99;
				}
				
				StatusChangedEvent(this, new StatusChangedEventArgs("Write area before MOVI chunk", percent));
				
				// Cancelled operation by user
				if (_cancelOperation)
				{
					ofs.Close();
					ofs.Dispose();
					ofs = null;
					ifs.Close();
					ifs.Dispose();
					ifs = null;
					_cancelOperation = false;
					StatusChangedEvent(this, new StatusChangedEventArgs("", 100));
					return false;
				}
				
				
				
			}
						
			return true;
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// write new MOVI area
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool WriteMovi()
		{
			byte[] tmpBuffer;
			uint FourCClen;
			uint FourCClenOriginal;
			string sFourCC = "";
			uint FourCC;
			string hexFourCC = "";
			int stepFrame = 0;
			long frameCount = 0;

			
			// parse all MOVI subsections
			while ((streamPosition - _aviContainer.MoviOffset)< _aviContainer.MoviSize)
			{
				FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
				hexFourCC = FourCC.ToString("X8");
				sFourCC = utils.GetFourccFromInt(FourCC);
				FourCClen = utils.ReadDWord(ref ifs, ref streamPosition);
				FourCClenOriginal = FourCClen;
				
				streamPosition += 8;
				
				// Adjust bytes to read (no odd)
				if ((FourCClen % 2) != 0)
				{
					FourCClen++;
				}
				
				
				
				// Manage subsection
				tmpBuffer = new byte[(int)FourCClen];
				ifs.Read(tmpBuffer, 0, (int)FourCClen);
				stepFrame ++;
				
				
				
				
				if (sFourCC.Substring(2,2) == "dc" || 
				    sFourCC.Substring(2,2) == "db")
	    		{
	    			// Modify frame and add
	    			tmpBuffer = processFrame(tmpBuffer, ref FourCClenOriginal);
	    			
	    			moviFrameOffset[frameCount] = ofs.Position;
					moviFrameLength[frameCount] = (long)tmpBuffer.Length;

	    			ofs.Write(utils.GetBytesFromFourcc(FourCC),0, 4);
	    			ofs.Write(utils.GetBytesFromFourcc(FourCClenOriginal),0, 4);
	    			ofs.Write(tmpBuffer, 0, tmpBuffer.Length);
	    			
	    			streamPosition += FourCClen;
	    			newMoviSize += tmpBuffer.Length;
					
				}
	    		else
	    		{
					// Other subsections
	    			moviFrameOffset[frameCount] = ofs.Position;
					moviFrameLength[frameCount] = (long)tmpBuffer.Length;
					
	    			ofs.Write(utils.GetBytesFromFourcc(FourCC),0, 4);
	    			ofs.Write(utils.GetBytesFromFourcc(FourCClenOriginal),0, 4);
	    			ofs.Write(tmpBuffer, 0, tmpBuffer.Length);

					streamPosition += FourCClen;
					newMoviSize += tmpBuffer.Length;
	    		}

	    		frameCount ++;

				
	    		if (stepFrame >= updateEachFrames)
	    		{
					int percent = Convert.ToInt32(((streamPosition - _aviContainer.MoviOffset) / _aviContainer.MoviSize) * 100);
					if (percent == 100)
					{
						percent = 99;
					}
					
		    		// Update status
					StatusChangedEvent(this, new StatusChangedEventArgs("Write MOVI chunk", percent));
					
					stepFrame = 0;
				}
				
				
				// Cancelled operation by user
				if (_cancelOperation)
				{
					ofs.Close();
					ofs.Dispose();
					ofs = null;
					ifs.Close();
					ifs.Dispose();
					ifs = null;
					_cancelOperation = false;
					StatusChangedEvent(this, new StatusChangedEventArgs("", 100));
					return false;
				}
				
			}
			
			return true;
		}
		
		
		
		
		
		
		
		/// <summary>
		/// Modify frame with new userdata value
		/// </summary>
		/// <param name="inByteArray">
		/// A <see cref="System.Byte"/>
		/// </param>
		/// <param name="frameLength">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte"/>
		/// </returns>
		private byte[] processFrame(byte[] inByteArray, ref uint frameLength)
		{
			byte[] tmpByteArray = null;
			byte[] userdataOld = null;
			byte[] outByteArray = null;
			ASCIIEncoding TextEncoding = new ASCIIEncoding();
			
			//tmpByteArray = inByteArray;
			userdataOld = TextEncoding.GetBytes(userDataToChange);

			int startPos = utils.CompareByteArray(inByteArray, userdataOld, 0);
			
			// int totalFrameBytes = 0;
			int totalFrameBytes, newFrameLength;
			
			
			if (startPos >= 0)
			{
				newFrameLength = (int)(frameLength - userDataToChange.Length) + 12;
				
				// Padded to an even number of bytes but make sure the padding isn't included
                // in the size written to the chunk header or index
				totalFrameBytes = newFrameLength;
				
				if ((totalFrameBytes % 2) != 0)
					totalFrameBytes ++;
				
				
				outByteArray = new byte[totalFrameBytes];
				Array.Copy(inByteArray, outByteArray, startPos);
				Array.Copy(TextEncoding.GetBytes("DivX999b000p"), 0, outByteArray, startPos, 12);
				Array.Copy(inByteArray, 
				           startPos + userDataToChange.Length , 
				           outByteArray, 
				           startPos + 12, 
				           frameLength - (userDataToChange.Length + startPos));
				
				frameLength = (uint)newFrameLength;
			}
			else
				outByteArray = inByteArray;
			
			
			
			return outByteArray;			
		}
		
		
		
		
		
		
		
		/// <summary>
		/// IDX section update
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool WriteBeforeIDX()
		{
			StatusChangedEvent(this, new StatusChangedEventArgs("Write area before IDX chunk", 0));
			
			int bufferSize = copyBufferSize;
			int percent = 0;
			long startSection = streamPosition;
			
			byte[] copyBuffer;
			
			while (streamPosition < _aviContainer.IdxOffset)
			{
				if (streamPosition + bufferSize > _aviContainer.IdxOffset)
				{
					bufferSize = (int)(_aviContainer.IdxOffset - streamPosition);
				}
				
				// Update destination from source file
				copyBuffer = new byte[bufferSize];
				ifs.Read(copyBuffer, 0, bufferSize);
				ofs.Write(copyBuffer, 0, bufferSize);
				streamPosition += bufferSize;
				
				percent = Convert.ToInt32(((streamPosition - startSection) / (_aviContainer.IdxOffset - startSection)) * 100);
				if (percent == 100)
				{
					percent = 99;
				}
				
				StatusChangedEvent(this, new StatusChangedEventArgs("Write area before IDX chunk", percent));
				
				// Cancelled operation by user
				if (_cancelOperation)
				{
					ofs.Close();
					ofs.Dispose();
					ofs = null;
					ifs.Close();
					ifs.Dispose();
					ifs = null;
					_cancelOperation = false;
					StatusChangedEvent(this, new StatusChangedEventArgs("", 100));
					return false;
				}

			}
			
			return true;
			
		}
		
		
		
		
		
		
		
		
		private bool WriteIDX()
		{
			byte[] tmpBuffer;
			byte[] tmpDWordOffset;
			byte[] tmpDWordLength;
			int stepFrame = 0;
			long frameCount = 0;

			
			// parse all IDX subsections
			while ((streamPosition - _aviContainer.IdxOffset) < _aviContainer.IdxSize)
			{
				tmpBuffer = new byte[16];
				ifs.Read(tmpBuffer, 0, 16);
				
				streamPosition += 16;
				stepFrame ++;
								
				tmpDWordOffset = utils.GetBytesFromFourcc((uint)(moviFrameOffset[frameCount] - (_aviContainer.MoviOffset - 4)));
				tmpDWordLength = utils.GetBytesFromFourcc((uint)(moviFrameLength[frameCount]));

				for (int j=0; j<4; j++)
				{
					tmpBuffer[8+j] = tmpDWordOffset[j];
					tmpBuffer[12+j] = tmpDWordLength[j];
				}

				ofs.Write(tmpBuffer, 0, 16);
				
				
	    		frameCount ++;

				
	    		if (stepFrame >= updateEachFrames)
	    		{
					int percent = Convert.ToInt32(((streamPosition - _aviContainer.IdxOffset) / _aviContainer.IdxSize) * 100);
					if (percent == 100)
					{
						percent = 99;
					}
					
		    		// Update status
					StatusChangedEvent(this, new StatusChangedEventArgs("Write IDX chunk", percent));
					
					stepFrame = 0;
				}
				
				
				// Cancelled operation by user
				if (_cancelOperation)
				{
					ofs.Close();
					ofs.Dispose();
					ofs = null;
					ifs.Close();
					ifs.Dispose();
					ifs = null;
					_cancelOperation = false;
					StatusChangedEvent(this, new StatusChangedEventArgs("", 100));
					return false;
				}
				
			}

			
			return true;
		}
		
		
		
		
		
		
		
		
		
		private bool WriteAfterIDX()
		{
			
			int bufferSize = copyBufferSize;
			byte[] tmpBytes;
			
			StatusChangedEvent(this, new StatusChangedEventArgs("Write area after IDX", 0));
			
			while (streamPosition < _aviFileSize)
			{
				if ((streamPosition + bufferSize) > _aviFileSize)
				{
					bufferSize = (int)(_aviFileSize - streamPosition);
				}
				
				tmpBytes = new byte[bufferSize];
				ifs.Read(tmpBytes, 0, bufferSize);
				ofs.Write(tmpBytes, 0, bufferSize);
				streamPosition += bufferSize;
				
				
				// Cancelled operation by user
				if (_cancelOperation)
				{
					ofs.Close();
					ofs.Dispose();
					ofs = null;
					ifs.Close();
					ifs.Dispose();
					ifs = null;
					_cancelOperation = false;
					StatusChangedEvent(this, new StatusChangedEventArgs("", 100));
					return false;
				}
		
			}
			
			return true;
		
		}
		

		
		
		
		
		
	}	
		
		
}
