
using System;

namespace themonospot_Base_Main
{
    /// <summary>
    /// Description of clsInfoItem.
    /// </summary>
    public class clsInfoItem
    {
        public string ItemName = "";
        public string ItemValue = "";
        
        public clsInfoItem()
        { }
        
        public clsInfoItem(string Name, string Value)
        { 
            ItemName = Name;
            ItemValue = Value;
        }
    }
}
