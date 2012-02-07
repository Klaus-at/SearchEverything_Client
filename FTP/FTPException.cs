/// <summary> 
/// Java FTP client library.
///  
/// Copyright (C) 2000-2003 Enterprise Distributed Technologies Ltd
///  
/// www.enterprisedt.com
///  
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the GNU Lesser General Public
/// License as published by the Free Software Foundation; either
/// version 2.1 of the License, or (at your option) any later version.
///  
/// This library is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
/// Lesser General Public License for more details.
///  
/// You should have received a copy of the GNU Lesser General Public
/// License along with this library; if not, write to the Free Software
/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
///  
/// Bug fixes, suggestions and comments should be sent to bruce@enterprisedt.com
///  
/// Change Log:
///  
/// $Log: FTPException.cs,v $
/// Revision 1.1  2003/05/17 12:33:13  bruceb
/// first release
///
/// 
/// </summary>
namespace com.enterprisedt.net.ftp
{
	/// <summary>  
	/// FTP specific exceptions	
	/// </summary>
	/// <author>      
	/// Bruce Blackshaw
	/// </author>
	/// <version>     
	/// $Revision: 1.1 $
	/// </version>
	public class FTPException:System.Exception
	{
		/// <summary>   
		/// Get the reply code if it exists
		/// </summary>
		/// <returns>  
		/// reply if it exists, -1 otherwise
		/// </returns>
		public int ReplyCode
		{
			get
			{
				return replyCode;
			}
			
		}
		
		/// <summary>  
		/// Revision control id
		/// </summary>
		private static string cvsId = "@(#)$Id: FTPException.cs,v 1.1 2003/05/17 12:33:13 bruceb Exp $";
		
		
		/// <summary>  Integer reply code
		/// </summary>
		private int replyCode = - 1;
		
		/// <summary>   
		/// Constructor. Delegates to super.
		/// </summary>
		/// <param name="msg">  Message that the user will be
		/// able to retrieve
		/// 
		/// </param>
		public FTPException(string msg):base(msg)
		{
		}
		
		/// <summary>  
		/// Constructor. Permits setting of reply code
		/// </summary>
		/// <param name="msg">       
		/// message that the user will be able to retrieve
		/// </param>
		/// <param name="replyCode"> string form of reply code
		/// 
		/// </param>
		public FTPException(string msg, string replyCode):base(msg)
		{			
			// extract reply code if possible
			try
			{
				this.replyCode = System.Int32.Parse(replyCode);
			}
			catch (System.FormatException ex)
			{
				this.replyCode = - 1;
			}
		}
		
		
	}
}