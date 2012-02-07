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
/// $Log: FTPReply.cs,v $
/// Revision 1.1  2003/05/17 12:33:13  bruceb
/// first release
///
/// Revision 1.1  2002/11/19 22:01:25  bruceb
/// changes for 1.2
///  
///  
/// </summary>
namespace com.enterprisedt.net.ftp
{
	/// <summary>  Encapsulates the FTP server reply
	///  
	/// </summary>
	/// <author>       Bruce Blackshaw
	/// </author>
	/// <version>      $Revision: 1.1 $
	/// 
	/// </version>
	public class FTPReply
	{
		/// <summary>  Getter for reply code
		///  
		/// </summary>
		/// <returns> server's reply code
		/// 
		/// </returns>
		public string ReplyCode
		{
			get
			{
				return replyCode;
			}			
		}

		/// <summary>  Getter for reply text
		/// 
		/// </summary>
		/// <returns> server's reply text
		/// 
		/// </returns>
		public string ReplyText
		{
			get
			{
				return replyText;
			}			
		}
		
		/// <summary>  Revision control id
		/// </summary>
		private static string cvsId = "@(#)$Id: FTPReply.cs,v 1.1 2003/05/17 12:33:13 bruceb Exp $";
		
		/// <summary>  Reply code
		/// </summary>
		private string replyCode;
		
		/// <summary>  Reply text
		/// </summary>
		private string replyText;
		
		
		/// <summary>  
		/// Constructor. Only to be constructed
		/// by this package, hence internal access
		/// </summary>
		/// <param name="replyCode"> the server's reply code
		/// </param>
		/// <param name="replyText"> the server's reply text
		/// 
		/// </param>
		internal FTPReply(string replyCode, string replyText)
		{
			this.replyCode = replyCode;
			this.replyText = replyText;
		}
		
		
	}
}