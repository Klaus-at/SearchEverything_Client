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
/// $Log: FTPControlSocket.cs,v $
/// Revision 1.1  2003/05/17 12:33:13  bruceb
/// first release
/// 
/// KSchmid: added support for ETP
///             - lastcommand
///             - IsConnected()
///             - QUERY handling in ReadReply()
///
/// 
/// </summary>
namespace com.enterprisedt.net.ftp
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	/// <summary>  
	/// Supports client-side FTP operations	
	/// </summary>
	/// <author>              
	/// Bruce Blackshaw
	/// </author>
	/// <version>         
	/// $Revision: 1.1 $
	/// </version>
	public class FTPControlSocket
	{
        // last sent FTP Command
        private string lastCommand;

		/// <summary>   
		/// Set the TCP timeout on the underlying control socket.
		/// </summary>
		/// <param>
		/// The length of the timeout, in milliseconds
		/// </param>
		internal int Timeout
		{
			set
			{		
				timeout = value;
				if (controlSock == null)
					throw new SystemException("Failed to set timeout - no control socket");
				SetSocketTimeout(controlSock, timeout);
			}			
		}

		/// <summary>  
		/// Set the logging stream, replacing stdout		
		/// </summary>
		internal StreamWriter LogStream
		{
			set
			{
				if (value != null)
					this.log = value;
			}	
		}
		
		/// <summary>  
		/// Revision control id
		/// </summary>
		private static string cvsId = "@(#)$Id: FTPControlSocket.cs,v 1.1 2003/05/17 12:33:13 bruceb Exp $";
		
		/// <summary>   
		/// Standard FTP end of line sequence
		/// </summary>
		internal const string EOL = "\r\n";
		
		/// <summary>   
		/// The control port number for FTP
		/// </summary>
		internal const int CONTROL_PORT = 21;
		
		/// <summary>   Controls if responses sent back by the
		/// server are sent to assigned output stream
		/// </summary>
		private bool debugResponses = false;
		
		/// <summary>  
		/// Output stream debug is written to, stdout by default
		/// </summary>
		private TextWriter log = System.Console.Out;

		/// <summary>  
		/// Timeout value
		/// </summary>
		private int timeout = -1;
		
		/// <summary>  
		/// The underlying socket.
		/// </summary>
		private Socket controlSock = null;

		/// <summary>  
		/// The control socket's output stream
		/// </summary>
		private StreamWriter writer = null;

		/// <summary>  
		/// The control socket's input stream
		/// </summary>
		private StreamReader reader = null;
				
		/// <summary>   
		/// Constructor. Performs TCP connection and
		/// sets up reader/writer. Allows different control
		/// port to be used
		/// </summary>
		/// <param name="remoteHost">  Remote hostname
		/// </param>
		/// <param name="controlPort"> port for control stream
		/// </param>
		/// <param name="timeout">      the length of the timeout, in seconds
		/// </param>
		/// <param name="log">         the new logging stream
		/// 
		/// </param>
		public FTPControlSocket(string remoteHost, int controlPort, 
								StreamWriter log, int timeout)
		{
			// resolve remote host & take first entry
			IPHostEntry remoteHostEntry = Dns.Resolve(remoteHost);
			IPAddress[] ipAddresses = remoteHostEntry.AddressList;

			Initialize(ipAddresses[0], controlPort, log, timeout);
		}
		
		/// <summary>   
		/// Constructor. Performs TCP connection and
		/// sets up reader/writer. Allows different control
		/// port to be used
		/// </summary>
		/// <param name="remoteAddr">  Remote inet address
		/// </param>
		/// <param name="controlPort"> port for control stream
		/// </param>
		/// <param name="log">         the new logging stream
		/// </param>
		/// <param name="timeout">      the length of the timeout, in milliseconds
		/// </param>
		public FTPControlSocket(System.Net.IPAddress remoteAddr, 
								int controlPort, StreamWriter log, 
								int timeout)
		{
			Initialize(remoteAddr, controlPort, log, timeout);
		}

        public bool IsConnected()
        {
            return controlSock.Connected;
        }

		/// <summary>   
		/// Common constructor code. Performs TCP connection and
		/// sets up reader/writer. Allows different control
		/// port to be used
		/// </summary>
		/// <param name="remoteAddr">  Remote inet address
		/// </param>
		/// <param name="controlPort"> port for control stream
		/// </param>
		/// <param name="log">         the new logging stream
		/// </param>
		/// <param name="timeout">      the length of the timeout, in milliseconds
		/// </param>
		internal void Initialize(System.Net.IPAddress remoteAddr, 
								 int controlPort, StreamWriter log, 
								 int timeout)
		{
			LogStream = log;
			
			// ensure we get debug from initial connection sequence
			DebugResponses(true);

			// establish socket connection & set timeouts
			IPEndPoint ipe = new IPEndPoint(remoteAddr, controlPort);
			controlSock = 
				new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Timeout = timeout;
			controlSock.Connect(ipe);

			InitStreams();
			ValidateConnection();
			
			// switch off debug - user can switch on from this point
			DebugResponses(false);
		}
				
		/// <summary>   
		/// Checks that the standard 220 reply is returned
		/// following the initiated connection
		/// </summary>
		private void ValidateConnection()
		{			
			string reply = ReadReply();
			ValidateReply(reply, "220");
		}
		
		
		/// <summary>  
		/// Obtain the reader/writer stream for this connection
		/// </summary>
		private void InitStreams()
		{						
			NetworkStream stream = new NetworkStream(controlSock, true);
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
		}
												
		/// <summary>  
		/// Quit this FTP session and clean up.
		/// </summary>
		public void Logout()
		{			
			log.Flush();
			log = null;
	
			// owns socket so will close it
			writer.Close();		
			reader.Close();
		}
		
		
		/// <summary>  
		/// Request a data socket be created on the
		/// server, connect to it and return our
		/// connected socket.
		/// </summary>
		/// <param name="connectMode">  
		/// connection mode to connect with, either active or passive
		/// </param>
		/// <returns>  
		/// connected data socket
		/// </returns>
		internal Socket CreateDataSocket(FTPConnectMode connectMode)
		{		
			// active mode (PORT)
			if (connectMode == FTPConnectMode.ACTIVE)
			{
				return CreateDataSocketActive();
			}
			else // PASV
			{
				return CreateDataSocketPASV();
			}
		}
		
		
		/// <summary>  
		/// Create a listening socket which waits for a connection
		/// </summary>
		/// <returns>  
		/// not connected data socket		
		/// </returns>
		private Socket CreateDataSocketActive()
		{			
			// create listening socket at a system allocated port
			Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			// choose any port
			IPHostEntry localHostEntry = Dns.Resolve(Dns.GetHostName());
			IPEndPoint localEndPoint = new IPEndPoint(localHostEntry.AddressList[0], 0);
			sock.Bind(localEndPoint);

			// queue up to 5 connections
			sock.Listen(5);

			// get the listen port
			int port = ((IPEndPoint)sock.LocalEndPoint).Port;
			IPAddress addr = ((IPEndPoint)sock.LocalEndPoint).Address;

			// find out ip & port we are listening on			
			SetDataPort((IPEndPoint)sock.LocalEndPoint);
			return sock;
		}						
		
		/// <summary>  
		/// Sets the data port on the server, i.e. sends a PORT
		/// command		
		/// </summary>
		/// <param name="ep">local endpoint
		/// </param>
		private void SetDataPort(IPEndPoint ep)
		{			
			byte[] hostBytes = BitConverter.GetBytes(ep.Address.Address);
			
			// This is a .NET 1.1 API
			// byte[] hostBytes = ep.Address.GetAddressBytes();
			
			byte[] portBytes = ToByteArray((ushort)ep.Port);
			
			// assemble the PORT command
			string cmd = new StringBuilder("PORT ").
				Append((short)hostBytes[0]).Append(",").
				Append((short)hostBytes[1]).Append(",").
				Append((short)hostBytes[2]).Append(",").
				Append((short)hostBytes[3]).Append(",").
				Append((short)portBytes[0]).Append(",").
				Append((short)portBytes[1]).ToString();
			
			// send command and check reply
			string reply = SendCommand(cmd);
			ValidateReply(reply, "200");
		}
		
		/// <summary>  
		/// Convert a short into a byte array
		/// </summary>
		/// <param name="val">  value to convert
		/// </param>
		/// <returns>  a byte array
		/// 
		/// </returns>
		protected internal byte[] ToByteArray(ushort val)
		{			
			byte[] bytes = new byte[2];
			bytes[0] = (byte) (val >> 8); // bits 1- 8
			bytes[1] = (byte) (val & 0x00FF); // bits 9-16
			return bytes;
		}						
		
		/// <summary>  
		/// Request a data socket be created on the
		/// server, connect to it and return our
		/// connected socket.
		/// </summary>
		/// <returns>  connected data socket
		/// 
		/// </returns>
		private Socket CreateDataSocketPASV()
		{			
			// PASSIVE command - tells the server to listen for
			// a connection attempt rather than initiating it
			string reply = SendCommand("PASV");
			ValidateReply(reply, "227");
			
			// The reply to PASV is in the form:
			// 227 Entering Passive Mode (h1,h2,h3,h4,p1,p2).
			// where h1..h4 are the IP address to connect and
			// p1,p2 the port number
			// Example:
			// 227 Entering Passive Mode (128,3,122,1,15,87).
			// NOTE: PASV command in IBM/Mainframe returns the string
			// 227 Entering Passive Mode 128,3,122,1,15,87	(missing 
			// brackets)
			
			// extract the IP data string from between the brackets
			int startIP = reply.IndexOf((System.Char) '(');
			int endIP = reply.IndexOf((System.Char) ')');
			
			// allow for IBM missing brackets around IP address
			if (startIP < 0 && endIP < 0)
			{
				startIP = reply.ToUpper().LastIndexOf("MODE") + 4;
				endIP = reply.Length;
			}
			
			string ipData = reply.Substring(startIP + 1, (endIP) - (startIP + 1));
			int[] parts = new int[6];
			
			int len = ipData.Length;
			int partCount = 0;
			StringBuilder buf = new StringBuilder();
			
			// loop thru and examine each char
			for (int i = 0; i < len && partCount <= 6; i++)
			{				
				char ch = ipData[i];
				if (System.Char.IsDigit(ch))
					buf.Append(ch);
				else if (ch != ',')
				{
					throw new FTPException("Malformed PASV reply: " + reply);
				}
				
				// get the part
				if (ch == ',' || i + 1 == len)
				{
					// at end or at separator
					try
					{
						parts[partCount++] = System.Int32.Parse(buf.ToString());
						buf.Length = 0;
					}
					catch (System.FormatException ex)
					{
						throw new FTPException("Malformed PASV reply: " + reply);
					}
				}
			}
			
			// assemble the IP address
			// we try connecting, so we don't bother checking digits etc
			string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
			
			// assemble the port number
			int port = (parts[4] << 8) + parts[5];

			// create socket and return
			IPHostEntry remoteHostEntry = Dns.Resolve(ipAddress);
			IPEndPoint ipe = new IPEndPoint(remoteHostEntry.AddressList[0], port);
			Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			SetSocketTimeout(sock, timeout);
			sock.Connect(ipe);
			return sock;
		}
				
		
		/// <summary>  
		/// Send a command to the FTP server and
		/// return the server's reply
		/// </summary>
		/// <returns>  reply to the supplied command
		/// 
		/// </returns>
		internal string SendCommand(string command)
		{			
			if (debugResponses)
				log.WriteLine("---> " + command);
			
			// send it
			writer.Write(command + EOL);
			writer.Flush();
            lastCommand = command;
			
			// and read the result
			return ReadReply();
		}
		
		/// <summary>  
		/// Read the FTP server's reply to a previously
		/// issued command. RFC 959 states that a reply
		/// consists of the 3 digit code followed by text.
		/// The 3 digit code is followed by a hyphen if it
		/// is a muliline response, and the last line starts
		/// with the same 3 digit code.		
		/// </summary>
		/// <returns>  
		/// reply string
		/// </returns>
		internal string ReadReply()
		{			
			string firstLine = reader.ReadLine();
			if (firstLine == null || firstLine.Length == 0)
				throw new IOException("Unexpected null reply received");
			
			StringBuilder reply = new StringBuilder(firstLine);
			
			if (debugResponses)
				log.WriteLine(reply.ToString());
			
			string replyCode = reply.ToString().Substring(0, 3);
			
            // special ETP query reply handling
            if (lastCommand != null && lastCommand.Substring(0, 5) == "QUERY")
            {
                Int32 qryNrOfMatches;
                Int32 qryOffset;
                Int32 qryNumFolders;
                Int32 qryNumFiles;
                bool result;
                Int32 i;

                char[] ReplySeperators = new char[] { ' ' };
                string[] replyParts = firstLine.Split(ReplySeperators);
                result = Int32.TryParse(replyParts[1], 0, null, out qryOffset);
                result = Int32.TryParse(replyParts[2], 0, null, out qryNrOfMatches);
                result = Int32.TryParse(replyParts[3], 0, null, out qryNumFolders);
                result = Int32.TryParse(replyParts[4], 0, null, out qryNumFiles);
                i = qryNrOfMatches;
                if (i > 0)
                    reply.AppendLine();
                while (i > 0)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        throw new IOException("Unexpected null reply received");

                    if (debugResponses)
                        log.WriteLine(line);

                    reply.AppendLine(line);
                    i--;
                }
                return reply.ToString();
            }


			// check for multiline response and build up
			// the reply
			if (reply[3] == '-')
			{				
				bool complete = false;
				while (!complete)
				{
					string line = reader.ReadLine();
					if (line == null)
						throw new IOException("Unexpected null reply received");
					
					if (debugResponses)
						log.WriteLine(line);
					
					if (line.Length > 3 && line.Substring(0, 3).Equals(replyCode) && line[3] == ' ')
					{
						reply.Append(line.Substring(3));
						complete = true;
					}
					else
					{
						// not the last line
						reply.Append(" ");
						reply.Append(line);
					}
				} // end while
			}
			// end if
			return reply.ToString();
		}
		
		
		/// <summary>  Validate the response the host has supplied against the
		/// expected reply. If we get an unexpected reply we throw an
		/// exception, setting the message to that returned by the
		/// FTP server
		/// 
		/// </summary>
		/// <param name="reply">             the entire reply string we received
		/// </param>
		/// <param name="expectedReplyCode"> the reply we expected to receive
		/// 
		/// 
		/// </param>
		internal FTPReply ValidateReply(string reply, string expectedReplyCode)
		{			
			// all reply codes are 3 chars long
			string replyCode = reply.Substring(0, 3);
			string replyText = reply.Substring(4);
			FTPReply replyObj = new FTPReply(replyCode, replyText);
			
			if (replyCode.Equals(expectedReplyCode))
				return replyObj;
			
			// if unexpected reply, throw an exception
			throw new FTPException(replyText, replyCode);
		}
		
		/// <summary>  
		/// Validate the response the host has supplied against the
		/// expected reply. If we get an unexpected reply we throw an
		/// exception, setting the message to that returned by the
		/// FTP server
		/// </summary>
		/// <param name="reply">              
		/// the entire reply string we received
		/// </param>
		/// <param name="expectedReplyCodes"> 
		/// array of expected replies
		/// </param>
		/// <returns>  
		/// an object encapsulating the server's reply
		/// </returns>
		internal FTPReply ValidateReply(string reply, string[] expectedReplyCodes)
		{			
			// all reply codes are 3 chars long
			string replyCode = reply.Substring(0, 3);
			string replyText = reply.Substring(4);
			
			FTPReply replyObj = new FTPReply(replyCode, replyText);
			
			for (int i = 0; i < expectedReplyCodes.Length; i++)
				if (replyCode.Equals(expectedReplyCodes[i]))
					return replyObj;
			
			// got this far, not recognised
			throw new FTPException(replyText, replyCode);
		}
		
		
		/// <summary>  
		/// Switch debug of responses on or off
		/// </summary>
		/// <param name="on"> true if you wish to have responses to
		/// stdout, false otherwise
		/// 
		/// </param>
		internal void DebugResponses(bool on)
		{
			debugResponses = on;
		}

		/// <summary>  
		/// Helper method to set a socket's timeout value
		/// </summary>
		/// <param name="sock">socket to set timeout for
		/// </param>
		/// <param name="timeout">timeout value to set
		/// </param>
		private void SetSocketTimeout(Socket sock, int timeout)
		{
			if (timeout > 0) 
			{
				sock.SetSocketOption(SocketOptionLevel.Socket, 
					SocketOptionName.ReceiveTimeout, timeout);
				sock.SetSocketOption(SocketOptionLevel.Socket, 
					SocketOptionName.SendTimeout, timeout);				
			}
		}
	}
}