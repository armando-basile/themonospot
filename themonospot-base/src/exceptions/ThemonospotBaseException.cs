
using System;

namespace ThemonospotBase
{
	
	
	public class ThemonospotBaseException: Exception
	{

		
		
		
#region Properties
	
		
		private string _message = "";
	
		/// <value>
		/// Describe error generate exception
		/// </value>
		public override string Message 
		{
			get {	return _message;	}
		}
		
		
		
		private string _method = "";
		
		/// <value>
		/// Identify function that generate exception
		/// </value>
		public string Method 
		{
			get {	return _method;		}
			set {	_method = value;	}
		}
		
		
		
#endregion Properties




		
		
		
		
		
		/// <summary>
		/// Empty constructor
		/// </summary>
		public ThemonospotBaseException()
		{
		}
		
		/// <summary>
		/// Set Method reference
		/// </summary>
		public ThemonospotBaseException(string method): this(method, "")
		{
		}
		
		
		/// <summary>
		/// Set Method and Message references
		/// </summary>
		public ThemonospotBaseException(string method, string errorMessage)
		{
			_method = method;
			_message = errorMessage;
		}
		
		
	}
}
