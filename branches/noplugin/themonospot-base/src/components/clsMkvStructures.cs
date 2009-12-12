
using System;

namespace themonospot_Base_Main
{
    /// <summary>
    /// Description of clsMkvStructures.
    /// </summary>
    public class clsMkvTrack
    {
        public int     Track_Number     = 0;
        public string  Track_UID        = "";
        public int     Track_Type       = 0;
        public string  Track_Lang       = "";
        public string  Track_Codec      = "";
        
        public clsMkvVideo Track_Video  = new clsMkvVideo();
        public clsMkvAudio Track_Audio  = new clsMkvAudio();
        
        
        public clsMkvTrack()
        {
        }
    }
    
    
    public class clsMkvVideo
    {
        public int PixelWidth           = 0;
        public int PixelHeight          = 0;
    }

    public class clsMkvAudio
    {
        public float Freq               = 0;
        public int   Channels           = 0;        
    }


}
