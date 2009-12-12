
using System;
using System.Collections.Generic;
using themonospot_Base_Main;

namespace themonospot_Base_Main
{
    /// <summary>
    /// Interface for all Parser Objects for Themonospot
    /// </summary>
    public interface IParser
    {
        List<clsInfoItem> VideoItems 
        {
            get;
        }
        
        List<clsInfoItem> AudioItems
        {
            get;
        }
        
        void OpenFile(string FileName);
        
        
    }
}
