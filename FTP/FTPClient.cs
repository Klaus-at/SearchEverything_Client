/// <summary>
/// C# FTP client library.
///  
/// Copyright (C) 2000-2003  Enterprise Distributed Technologies Ltd
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
/// Bug fixes, suggestions and comments should be sent to ftp@enterprisedt.com
/// 
/// Change Log:
/// 
/// $Log: FTPClient.cs,v $
/// Revision 1.1  2003/05/17 12:33:13  bruceb
/// first release
///
/// 
/// </summary>
namespace com.enterprisedt.net.ftp
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Collections;

	/// <summary>
	/// Enumerates the connect modes that are possible, active & PASV
	/// </summary>
	public enum FTPConnectMode 
	{
		/// <member>   
		/// Represents active connect mode
		/// </member>
		ACTIVE = 1,

		/// <member>   
		/// Represents PASV connect mode
		/// </member>
		PASV = 2
	}

	/// <summary>  
	/// Enumerates the transfer types possible. We support only the two common types, 
	/// ASCII and Image (often called binary).
	/// </summary>
	public enum FTPTransferType 
	{
		/// <member>   
		/// Represents ASCII transfer type
		/// </member>
		ASCII = 1,

		/// <member>   
		/// Represents Image (or binary) transfer type
		/// </member>
		BINARY = 2
	}

	/// <summary>  
	/// Supports client-side FTP. Most common
	/// FTP operations are present in this class.
	/// </summary>
	/// <author>       
	/// Bruce Blackshaw
	/// </author>
	/// <version>      
	/// $Revision: 1.1 $
	/// </version>
	public class FTPClient
	{
		/// <summary>   
		/// Set the TCP timeout on the underlying socket.
		/// 
		/// If a timeout is set, then any operation which
		/// takes longer than the timeout value will be
		/// killed with a java.io.InterruptedException. We
		/// set both the control and data connections
		/// 
		/// </summary>
		public int Timeout
		{
			set
			{				
				this.timeout = value;
				control.Timeout = value;
			}			
		}

		/// <summary>  
		/// Set the connect mode to ACTIVE or PASV
		/// </summary>
		public FTPConnectMode ConnectMode
		{
			set
			{
				connectMode = value;
			}			
		}

		/// <summary>  
		/// Gets the latest valid reply from the server
		/// </summary>
		/// <returns>  
		/// reply object encapsulating last valid server response
		/// </returns>
		public FTPReply LastValidReply
		{
			get
			{
				return lastValidReply;
			}			
		}

		/// <summary>  
		/// Set the logging stream, replacing stdout
		/// </summary>
		public StreamWriter LogStream
		{
			set
			{
				control.LogStream = value;
			}			
		}

		/// <summary>  
		/// Get the current transfer type
		/// </summary>
		/// <returns>  
		/// the current type of the transfer, i.e. BINARY or ASCII
		/// </returns>
		/// <summary>  
		/// Set the transfer type
		/// </summary>
		public FTPTransferType TransferType
		{
			get
			{
				return transferType;
			}			
			set
			{				
				// determine the character to send
				string typeStr = ASCII_CHAR;
				if (value.Equals(FTPTransferType.BINARY))
					typeStr = BINARY_CHAR;
				
				// send the command
				string reply = control.SendCommand("TYPE " + typeStr);
				lastValidReply = control.ValidateReply(reply, "200");
				
				// record the type
				transferType = value;
			}			
		}
		
		/// <summary>  
		/// Revision control id
		/// </summary>
		private static string cvsId = "@(#)$Id: FTPClient.cs,v 1.1 2003/05/17 12:33:13 bruceb Exp $";
		
		/// <summary>  
		/// Format to interpret MTDM timestamp
		/// </summary>
		private static string dtFormat = "yyyyMMddHHmmss";

		/// <summary>  
		/// The char sent to the server to set BINARY
		/// </summary>
		private static string BINARY_CHAR = "I";

		/// <summary>  
		/// The char sent to the server to set ASCII
		/// </summary>
		private static string ASCII_CHAR = "A";
		
		/// <summary>  
		/// Socket responsible for controlling the connection
		/// </summary>
		private FTPControlSocket control = null;
		
		/// <summary>  
		/// Socket responsible for transferring the data
		/// </summary>
		private Socket data = null;
		
		/// <summary>  
		/// Socket timeout for both data and control. In milliseconds
		/// </summary>
		private int timeout = 5000;
		
		/// <summary>  
		/// Record of the transfer type - make the default ASCII
		/// </summary>
		private FTPTransferType transferType = FTPTransferType.ASCII;
		
		/// <summary>  Record of the connect mode - make the default PASV (as this was
		/// the original mode supported)
		/// </summary>
		private FTPConnectMode connectMode = FTPConnectMode.PASV;
		
		/// <summary>  
		/// Holds the last valid reply from the server on the control socket
		/// </summary>
		private FTPReply lastValidReply;
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteHost"> 
		/// the remote hostname 
		/// </param>
		public FTPClient(string remoteHost)
		{
			control = new FTPControlSocket(remoteHost, FTPControlSocket.CONTROL_PORT, null, 0);
            
		}
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteHost"> the remote hostname
		/// </param>
		/// <param name="controlPort"> port for control stream
		/// 
		/// </param>
		public FTPClient(string remoteHost, int controlPort)
		{
			control = new FTPControlSocket(remoteHost, controlPort, null, 0);
		}
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteAddr"> the address of the
		/// remote host
		/// 
		/// </param>
		public FTPClient(System.Net.IPAddress remoteAddr)
		{
			control = new FTPControlSocket(remoteAddr, FTPControlSocket.CONTROL_PORT, null, 0);
		}
		
		/// <summary>  
		/// Constructor. Creates the control
		/// socket. Allows setting of control port (normally
		/// set by default to 21).
		/// </summary>
		/// <param name="remoteAddr"> the address of the
		/// remote host
		/// </param>
		/// <param name="controlPort"> port for control stream
		/// 
		/// </param>
		public FTPClient(System.Net.IPAddress remoteAddr, int controlPort)
		{
			control = new FTPControlSocket(remoteAddr, controlPort, null, 0);
		}
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteHost"> 
		/// the remote hostname
		/// </param>
		/// <param name="log">      
		/// log stream for logging to
		/// </param>		
		/// <param name="timeout">      
		/// the length of the timeout, in milliseconds
		/// </param>
		public FTPClient(string remoteHost, StreamWriter log, int timeout)
		{
			control = new FTPControlSocket(remoteHost, FTPControlSocket.CONTROL_PORT, log, timeout);
		}
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteHost"> 
		/// the remote hostname
		/// </param>
		/// <param name="controlPort"> 
		/// port for control stream
		/// </param>
		/// <param name="log">      
		/// log stream for logging to
		/// </param>		
		/// <param name="timeout">      
		/// the length of the timeout, in milliseconds
		/// </param>
		public FTPClient(string remoteHost, int controlPort, StreamWriter log, int timeout)
		{
			control = new FTPControlSocket(remoteHost, controlPort, log, timeout);
		}
		
		/// <summary>  
		/// Constructor. Creates the control socket
		/// </summary>
		/// <param name="remoteAddr"> 
		/// the address of the remote host
		/// </param>
		/// <param name="log">      
		/// log stream for logging to
		/// </param>				
		/// <param name="timeout">      
		/// the length of the timeout, in seconds
		/// </param>
		public FTPClient(IPAddress remoteAddr, 
				 StreamWriter log, int timeout)
		{
			control = 
				new FTPControlSocket(remoteAddr, FTPControlSocket.CONTROL_PORT, 
									 log, timeout);
		}
		
		/// <summary>  
		/// Constructor. Creates the control
		/// socket. Allows setting of control port (normally
		/// set by default to 21).
		/// </summary>
		/// <param name="remoteAddr"> 
		/// the address of the remote host
		/// </param>
		/// <param name="controlPort">
		/// port for control stream
		/// </param>
		/// <param name="log">      
		/// log stream for logging to
		/// </param>		
		/// <param name="timeout">      
		/// the length of the timeout, in seconds		
		/// </param>
		public FTPClient(System.Net.IPAddress remoteAddr, int controlPort, StreamWriter log, int timeout)
		{
			control = new FTPControlSocket(remoteAddr, controlPort, log, timeout);
		}

        public bool IsConnected()
        {
            return control.IsConnected();
        }
		
		/// <summary>  
		/// Login into an account on the FTP server. This
		/// call completes the entire login process
		/// </summary>
		/// <param name="user">      
		/// user name
		/// </param>
		/// <param name="password">  
		/// user's password
		/// </param>
		public void Login(string user, string password)
		{			
			string response = control.SendCommand("USER " + user);
            lastValidReply = control.ValidateReply(response, new string[] { "230", "331" });
            if (lastValidReply.ReplyCode == "331")
            {
                response = control.SendCommand("PASS " + password);
                lastValidReply = control.ValidateReply(response, "230");
            }
		}
				
		/// <summary>  
		/// Supply the user name to log into an account
		/// on the FTP server. Must be followed by the
		/// password() method - but we allow for
		/// </summary>
		/// <param name="user">      
		/// user name
		/// </param>
		public void User(string user)
		{			
			string reply = control.SendCommand("USER " + user);
			
			// we allow for a site with no password - 230 response
			string[] validCodes = new string[]{"230", "331"};
			lastValidReply = control.ValidateReply(reply, validCodes);
		}		
		
		/// <summary>  
		/// Supplies the password for a previously supplied
		/// username to log into the FTP server. Must be
		/// preceeded by the user() method
		/// </summary>
		/// <param name="password">  
		/// user's password
		/// </param>
		public void Password(string password)
		{			
			string reply = control.SendCommand("PASS " + password);
			
			// we allow for a site with no passwords (202)
			string[] validCodes = new string[]{"230", "202"};
			lastValidReply = control.ValidateReply(reply, validCodes);
		}
				
		/// <summary>  
		/// Issue arbitrary ftp commands to the FTP server.
		/// </summary>
		/// <param name="command">    
		/// ftp command to be sent to server
		/// </param>
		/// <param name="validCodes"> 
		/// valid return codes for this command		
		/// </param>
		public void Quote(string command, string[] validCodes)
		{			
			string reply = control.SendCommand(command);
			
			// allow for no validation to be supplied
			if (validCodes != null && validCodes.Length > 0)
				lastValidReply = control.ValidateReply(reply, validCodes);
		}

        public void Query(string searchString, Int32 offsetItems, Int32 isCaseSensitive, Int32 matchWholeWord, Int32 matchPath, Int32 maxItemsReturn)
        {
            string command = String.Format("QUERY {0} {1} {2} {3} {4} {5}", offsetItems, maxItemsReturn, isCaseSensitive, matchWholeWord, matchPath, searchString);
            string reply = control.SendCommand(command);

            // allow for no validation to be supplied
            string[] validCodes = new string[] { "200" };
            lastValidReply = control.ValidateReply(reply, validCodes);
        }		

		
		/// <summary>  
		/// Put a local file onto the FTP server. It
		/// is placed in the current directory.
		/// </summary>
		/// <param name="localPath">  
		/// path of the local file
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory		
		/// </param>
		public void Put(string localPath, string remoteFile)
		{			
			Put(localPath, remoteFile, false);
		}
		
		/// <summary>  
		/// Put a stream of data onto the FTP server. It
		/// is placed in the current directory.
		/// </summary>
		/// <param name="srcStream">  
		/// input stream of data to put
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory		
		/// </param>
		public void Put(Stream srcStream, string remoteFile)
		{			
			Put(srcStream, remoteFile, false);
		}		
		
		/// <summary>  
		/// Put a local file onto the FTP server. It
		/// is placed in the current directory. Allows appending
		/// if current file exists
		/// </summary>
		/// <param name="localPath">  
		/// path of the local file
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise		
		/// </param>
		public void Put(string localPath, string remoteFile, bool append)
		{			
			// get according to set type
			if (TransferType == FTPTransferType.ASCII)
			{
				PutASCII(localPath, remoteFile, append);
			}
			else
			{
				PutBinary(localPath, remoteFile, append);
			}
			ValidateTransfer();
		}
		
		/// <summary>  
		/// Put a stream of data onto the FTP server. It
		/// is placed in the current directory. Allows appending
		/// if current file exists
		/// </summary>
		/// <param name="srcStream">  
		/// input stream of data to put
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise 
		/// </param>
		public void Put(Stream srcStream, string remoteFile, bool append)
		{			
			// get according to set type
			if (TransferType == FTPTransferType.ASCII)
			{
				PutASCII(srcStream, remoteFile, append);
			}
			else
			{
				PutBinary(srcStream, remoteFile, append);
			}
			ValidateTransfer();
		}
		
		/// <summary>  
		/// Validate that the put() or get() was successful
		/// </summary>
		private void  ValidateTransfer()
		{			
			// check the control response
			string[] validCodes = new string[]{"226", "250"};
			string reply = control.ReadReply();
			lastValidReply = control.ValidateReply(reply, validCodes);
		}

		/// <summary>  
		/// Get the network stream associated with the data socket
		/// </summary>
		/// <returns>  
		/// Network stream ready for reading/writing
		/// </returns>
		private NetworkStream GetDataStream() 
		{
			Socket sock = data;

			// in active mode, we must accept the FTP server's connection
			if (connectMode == FTPConnectMode.ACTIVE) 
			{
				sock = data.Accept();
			}
			// ensure network stream owns the socket
			return new NetworkStream(sock, true);
		}
		
		/// <summary>  
		/// Request the server to set up the put
		/// </summary>
		/// <param name="remoteFile"> name of remote file in
		/// current directory
		/// </param>
		/// <param name="append">     true if appending, false otherwise
		/// 
		/// </param>
		private void InitPut(string remoteFile, bool append)
		{			
			// set up data channel
			data = control.CreateDataSocket(connectMode);
			
			// send the command to store
			string cmd = append ? "APPE ":"STOR ";
			string reply = control.SendCommand(cmd + remoteFile);
			
			// Can get a 125 or a 150
			string[] validCodes = new string[]{"125", "150"};
			lastValidReply = control.ValidateReply(reply, validCodes);
		}
		
		
		/// <summary>  
		/// Put as ASCII, i.e. read a line at a time and write
		/// inserting the correct FTP separator
		/// </summary>
		/// <param name="localPath">  
		/// full path of local file to read from
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file we are writing to
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise
		/// </param>
		private void PutASCII(string localPath, string remoteFile, 
						      bool append)
		{			
			// create an inputstream & pass to common method
			Stream srcStream = 
				new FileStream(localPath, FileMode.Open, FileAccess.Read);
			PutASCII(srcStream, remoteFile, append);
		}
		
		/// <summary>  
		/// Put as ASCII, i.e. read a line at a time and write
		/// inserting the correct FTP separator
		/// </summary>
		/// <param name="srcStream">  
		/// input stream of data to put
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file we are writing to
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise
		/// </param>
		private void PutASCII(Stream srcStream, string remoteFile, bool append)
		{			
			// need to read line by line ...
			StreamReader reader = new StreamReader(srcStream);
			
			InitPut(remoteFile, append);
			
			// get an character output stream to write to ... AFTER we
			// have the ok to go ahead AND AFTER we've successfully opened a
			// stream for the local file
			StreamWriter writer = new StreamWriter(GetDataStream());
			
			// write line by line, writing \r\n as required by RFC959 after
			// each line
			string line = null;
			while ((line = reader.ReadLine()) != null)
			{
				writer.Write(line, 0, line.Length);
				writer.Write(FTPControlSocket.EOL, 0, FTPControlSocket.EOL.Length);
			}
			reader.Close();

			// closing the writer will close the data socket
			writer.Flush();
			writer.Close();
		}
		
		
		/// <summary>  
		/// Put as binary, i.e. read and write raw bytes
		/// </summary>
		/// <param name="localPath">  
		/// full path of local file to read from
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file we are writing to
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise
		/// </param>
		private void PutBinary(string localPath, string remoteFile, bool append)
		{			
			// open input stream to read source file ... do this
			// BEFORE opening output stream to server, so if file not
			// found, an exception is thrown
			Stream srcStream = 
				new FileStream(localPath, FileMode.Open, FileAccess.Read);
			PutBinary(srcStream, remoteFile, append);
		}
		
		/// <summary>  
		/// Put as binary, i.e. read and write raw bytes
		/// </summary>
		/// <param name="srcStream">  
		/// input stream of data to put
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file we are writing to
		/// </param>
		/// <param name="append">     
		/// true if appending, false otherwise
		/// </param>
		private void PutBinary(Stream srcStream, string remoteFile, bool append)
		{			
			BufferedStream reader = new BufferedStream(srcStream);
			
			InitPut(remoteFile, append);
			
			// get an output stream
			BinaryWriter writer = new BinaryWriter(GetDataStream());
			
			byte[] buf = new byte[512];
			
			// read a chunk at a time and write to the data socket
			int count = 0;
			while ((count = reader.Read(buf, 0, buf.Length)) > 0) 
			{
				writer.Write(buf, 0, count);
			}
			reader.Close();
			writer.Flush();
			writer.Close();						   
		}
		
		
		/// <summary>  
		/// Put data onto the FTP server. It
		/// is placed in the current directory.
		/// </summary>
		/// <param name="bytes">       
		/// array of bytes
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory
		/// </param>
		public void Put(byte[] bytes, string remoteFile)
		{			
			Put(bytes, remoteFile, false);
		}
		
		/// <summary>  
		/// Put data onto the FTP server. It
		/// is placed in the current directory. Allows
		/// appending if current file exists
		/// </summary>
		/// <param name="bytes">       
		/// array of bytes
		/// </param>
		/// <param name="remoteFile"> name of remote file in
		/// current directory
		/// </param>
		/// <param name="append">     true if appending, false otherwise
		/// 
		/// </param>
		public void Put(byte[] bytes, string remoteFile, bool append)
		{			
			InitPut(remoteFile, append);
			
			// get an output stream
			BinaryWriter writer = new BinaryWriter(GetDataStream());
			
			// write array
			writer.Write(bytes, 0, bytes.Length);
			
			// flush and clean up
			writer.Flush();
			writer.Close();
						
			ValidateTransfer();
		}
		
		
		/// <summary>  
		/// Get data from the FTP server. Uses the currently
		/// set transfer mode.
		/// </summary>
		/// <param name="localPath">  
		/// local file to put data in
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory		
		/// </param>
		public void Get(string localPath, string remoteFile)
		{			
			// get according to set type
			if (TransferType == FTPTransferType.ASCII)
			{
				GetASCII(localPath, remoteFile);
			}
			else
			{
				GetBinary(localPath, remoteFile);
			}
			ValidateTransfer();
		}
		
		/// <summary>  
		/// Get data from the FTP server. Uses the currently
		/// set transfer mode.
		/// </summary>
		/// <param name="destStream"> 
		/// data stream to write data to
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory
		/// </param>
		public void Get(Stream destStream, string remoteFile)
		{
			
			// get according to set type
			if (TransferType == FTPTransferType.ASCII)
			{
				GetASCII(destStream, remoteFile);
			}
			else
			{
				GetBinary(destStream, remoteFile);
			}
			ValidateTransfer();
		}
		
		
		/// <summary>  
		/// Request to the server that the get is set up
		/// </summary>
		/// <param name="remoteFile"> 
		/// name of remote file
		/// </param>
		private void InitGet(string remoteFile)
		{			
			// set up data channel
			data = control.CreateDataSocket(connectMode);
			
			// send the retrieve command
			string reply = control.SendCommand("RETR " + remoteFile);
			
			// Can get a 125 or a 150
			string[] validCodes1 = new string[]{"125", "150"};
			lastValidReply = control.ValidateReply(reply, validCodes1);
		}
		
		
		/// <summary>  
		/// Get as ASCII, i.e. read a line at a time and write
		/// using the correct newline separator for the OS
		/// </summary>
		/// <param name="localPath">  
		/// full path of local file to write to
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file		
		/// </param>
		private void GetASCII(string localPath, string remoteFile)
		{			
			// B.McKeown:
			// Call initGet() before creating the FileOutputStream.
			// This will prevent being left with an empty file if a FTPException
			// is thrown by initGet().
			InitGet(remoteFile);
			
			// B. McKeown: Need to store the local file name so the file can be
			// deleted if necessary.
			FileInfo localFile = new FileInfo(localPath);
			
			// create the buffered stream for writing
			//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
			StreamWriter writer = new StreamWriter(localPath);
			
			// get an character input stream to read data from ... AFTER we
			// have the ok to go ahead AND AFTER we've successfully opened a
			// stream for the local file
			StreamReader reader = new StreamReader(GetDataStream());
						
			// read/write a line at a time
			IOException storedEx = null;
			string line = null;
			try
			{
				while ((line = reader.ReadLine()) != null)
				{
					writer.Write(line, 0, line.Length);
					writer.WriteLine();
				}
			}
			catch (IOException ex)
			{
				storedEx = ex;
			}
			finally
			{
				writer.Close();

				// if an error occurred, deleted the local file
				if (storedEx != null && File.Exists(localFile.FullName))
					File.Delete(localFile.FullName);
			}
			
			try
			{
				reader.Close();
			}
			catch (IOException ignore)
			{
			}
			
			// if we failed to write the file, rethrow the exception
			if (storedEx != null)
				throw storedEx;
		}
		
		/// <summary>  
		/// Get as ASCII, i.e. read a line at a time and write
		/// using the correct newline separator for the OS
		/// </summary>
		/// <param name="destStream"> 
		/// data stream to write data to
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file
		/// </param>
		private void GetASCII(Stream destStream, string remoteFile)
		{			
			InitGet(remoteFile);
			
			// create the buffered stream for writing
			StreamWriter writer = new StreamWriter(destStream);
			
			// get an character input stream to read data from ... AFTER we
			// have the ok to go ahead
			StreamReader reader = new StreamReader(GetDataStream());
						
			// read/write a line at a time
			IOException storedEx = null;
			string line = null;
			try
			{
				while ((line = reader.ReadLine()) != null)
				{
					writer.Write(line, 0, line.Length);
					writer.WriteLine();
				}
			}
			catch (IOException ex)
			{
				storedEx = ex;
			}
			finally
			{
				writer.Close();
			}
			
			try
			{
				reader.Close();
			}
			catch (IOException ignore)
			{
			}
			
			// if we failed to write the file, rethrow the exception
			if (storedEx != null)
				throw storedEx;
		}
		
		
		/// <summary>  
		/// Get as binary file, i.e. straight transfer of data
		/// </summary>
		/// <param name="localPath">  
		/// full path of local file to write to
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file
		/// </param>
		private void GetBinary(string localPath, string remoteFile)
		{			
			// B.McKeown:
			// Call initGet() before creating the FileOutputStream.
			// This will prevent being left with an empty file if a FTPException
			// is thrown by initGet().
			InitGet(remoteFile);
			
			// B. McKeown: Need to store the local file name so the file can be
			// deleted if necessary.
			FileInfo localFile = new FileInfo(localPath);
			
			// create the buffered output stream for writing the file
			BinaryWriter writer = new BinaryWriter(new FileStream(localPath, FileMode.OpenOrCreate));
			
			// get an input stream to read data from ... AFTER we have
			// the ok to go ahead AND AFTER we've successfully opened a
			// stream for the local file
			BinaryReader reader = new BinaryReader(GetDataStream());
						
			// do the retrieving
			int chunksize = 4096;
			byte[] chunk = new byte[chunksize];
			int count;
			IOException storedEx = null;
			
			// read from socket & write to file in chunks
			try
			{
				while ((count = reader.Read(chunk, 0, chunk.Length)) > 0)
				{
					writer.Write(chunk, 0, count);
				}
			}
			catch (IOException ex)
			{
				storedEx = ex;
			}
			finally
			{
				writer.Close();

				// if an error occurred, deleted the local file
				if (storedEx != null && File.Exists(localFile.FullName))
					File.Delete(localFile.FullName);
			}
			
			// close streams
			try
			{
				reader.Close();
			}
			catch (IOException ignore)
			{
			}
			
			// if we failed to write the file, rethrow the exception
			if (storedEx != null)
				throw storedEx;
		}
		
		/// <summary>  
		/// Get as binary file, i.e. straight transfer of data
		/// </summary>
		/// <param name="destStream"> 
		/// stream to write to
		/// </param>
		/// <param name="remoteFile"> 
		/// name of remote file
		/// </param>
		private void GetBinary(Stream destStream, string remoteFile)
		{			
			InitGet(remoteFile);
			
			// create the buffered output stream for writing the file
			BinaryWriter writer = new BinaryWriter(destStream);
			
			// get an input stream to read data from ... AFTER we have
			// the ok to go ahead AND AFTER we've successfully opened a
			// stream for the local file
			BinaryReader reader = new BinaryReader(GetDataStream());
						
			// do the retrieving
			int chunksize = 4096;
			byte[] chunk = new byte[chunksize];
			int count;
			IOException storedEx = null;
			
			// read from socket & write to file in chunks
			try
			{
				while ((count = reader.Read(chunk, 0, chunk.Length)) > 0)
				{
					writer.Write(chunk, 0, count);
				}
			}
			catch (IOException ex)
			{
				storedEx = ex;
			}
			finally
			{
				writer.Close();
			}
			
			// close streams
			try
			{
				reader.Close();
			}
			catch (IOException ignore)
			{
			}
			
			// if we failed to write to the stream, rethrow the exception
			if (storedEx != null)
				throw storedEx;
		}
		
		
		/// <summary>  
		/// Get data from the FTP server. Transfers in
		/// whatever mode we are in. Retrieve as a byte array. Note
		/// that we may experience memory limitations as the
		/// entire file must be held in memory at one time.
		/// </summary>
		/// <param name="remoteFile"> 
		/// name of remote file in current directory
		/// </param>
		public byte[] Get(string remoteFile)
		{			
			InitGet(remoteFile);
			
			// get an input stream to read data from
			BinaryReader reader = new BinaryReader(GetDataStream());
						
			// do the retrieving
			int chunksize = 4096;
			byte[] chunk = new byte[chunksize]; // read chunks into
			MemoryStream temp = new MemoryStream(chunksize); // temp swap buffer
			int count; // size of chunk read
			
			// read from socket & write to file
			while ((count = reader.Read(chunk, 0, chunk.Length)) > 0)
			{
				temp.Write(chunk, 0, count);
			}
			temp.Close();
						
			// close streams
			try
			{
				reader.Close();
			}
			catch (IOException ignore)
			{
			}
			
			ValidateTransfer();
			
			return temp.ToArray();
		}
		
		
		/// <summary>  
		/// Run a site-specific command on the
		/// server. Support for commands is dependent
		/// on the server
		/// </summary>
		/// <param name="command">  
		/// the site command to run
		/// </param>
		/// <returns> true if command ok, false if
		/// command not implemented
		/// </returns>
		public bool Site(string command)
		{
			
			// send the retrieve command
			string reply = control.SendCommand("SITE " + command);
			
			// Can get a 200 (ok) or 202 (not impl). Some
			// FTP servers return 502 (not impl)
			string[] validCodes = new string[]{"200", "202", "502"};
			lastValidReply = control.ValidateReply(reply, validCodes);
			
			// return true or false? 200 is ok, 202/502 not
			// implemented
			if (reply.Substring(0, (3) - (0)).Equals("200"))
				return true;
			else
				return false;
		}
		
				
		/// <summary>  
		/// List current directory's contents as an array of strings of
		/// filenames.
		/// </summary>
		/// <returns>  
		/// an array of current directory listing strings
		/// </returns>
		public string[] Dir()
		{			
			return Dir(null, false);
		}
		
		/// <summary>  
		/// List a directory's contents as an array of strings of filenames.
		/// </summary>
		/// <param name="dirname"> name of directory(<b>not</b> a file mask)
		/// </param>
		/// <returns>  an array of directory listing strings
		/// 
		/// </returns>
		public string[] Dir(string dirname)
		{			
			return Dir(dirname, false);
		}		
		
		/// <summary>  
		/// List a directory's contents as an array of strings. A detailed
		/// listing is available, otherwise just filenames are provided.
		/// The detailed listing varies in details depending on OS and
		/// FTP server. Note that a full listing can be used on a file
		/// name to obtain information about a file
		/// </summary>
		/// <param name="dirname"> 
		/// name of directory (<b>not</b> a file mask)
		/// </param>
		/// <param name="full">    
		/// true if detailed listing required false otherwise
		/// </param>
		/// <returns>  
		/// an array of directory listing strings
		/// </returns>
		public string[] Dir(string dirname, bool full)
		{			
			// set up data channel
			data = control.CreateDataSocket(connectMode);
			
			// send the retrieve command
			string command = full?"LIST ":"NLST ";
			if (dirname != null)
				command += dirname;
			
			// some FTP servers bomb out if NLST has whitespace appended
			command = command.Trim();
			string reply = control.SendCommand(command);
			
			// check the control response. wu-ftp returns 550 if the
			// directory is empty, so we handle 550 appropriately 
			string[] validCodes1 = new string[]{"125", "150", "550"};
			lastValidReply = control.ValidateReply(reply, validCodes1);
			
			// an empty array of files for 550
			string[] result = new string[0];
			
			// a normal reply ... extract the file list
			if (!lastValidReply.ReplyCode.Equals("550"))
			{
				// get an character input stream to read data from .
				StreamReader reader = new StreamReader(GetDataStream());
				
				// read a line at a time
				ArrayList lines = new ArrayList();
				string line = null;
				while ((line = reader.ReadLine()) != null)
				{
					lines.Add(line);
				}
				try
				{
					reader.Close();
				}
				catch (IOException ignore)
				{
				}
				
				// check the control response
				string[] validCodes2 = new string[]{"226", "250"};
				reply = control.ReadReply();
				lastValidReply = control.ValidateReply(reply, validCodes2);
				
				// empty array is default
				if (lines.Count > 0)
					result = (string[]) lines.ToArray(typeof(string));
			}
			return result;
		}
						
		/// <summary>  
		/// Switch debug of responses on or off
		/// </summary>
		/// <param name="on"> true if you wish to have responses to
		/// the log stream, false otherwise
		/// 
		/// </param>
		public void DebugResponses(bool on)
		{
			control.DebugResponses(on);
		}
		
		
		/// <summary>  
		/// Delete the specified remote file
		/// </summary>
		/// <param name="remoteFile"> name of remote file to
		/// delete
		/// 
		/// </param>
		public void Delete(string remoteFile)
		{			
			string reply = control.SendCommand("DELE " + remoteFile);
			lastValidReply = control.ValidateReply(reply, "250");
		}
		
		
		/// <summary>  
		/// Rename a file or directory
		/// </summary>
		/// <param name="from"> name of file or directory to rename
		/// </param>
		/// <param name="to">   intended name
		/// 
		/// </param>
		public void Rename(string from, string to)
		{
			
			string reply = control.SendCommand("RNFR " + from);
			lastValidReply = control.ValidateReply(reply, "350");
			
			reply = control.SendCommand("RNTO " + to);
			lastValidReply = control.ValidateReply(reply, "250");
		}
		
		
		/// <summary>  
		/// Delete the specified remote working directory
		/// </summary>
		/// <param name="dir"> 
		/// name of remote directory to delete
		/// </param>
		public void Rmdir(string dir)
		{			
			string reply = control.SendCommand("RMD " + dir);
			
			// some servers return 257, technically incorrect but
			// we cater for it ...
			string[] validCodes = new string[]{"250", "257"};
			lastValidReply = control.ValidateReply(reply, validCodes);
		}
				
		/// <summary>  
		/// Create the specified remote working directory
		/// </summary>
		/// <param name="dir"> 
		/// name of remote directory to create		
		/// </param>
		public void Mkdir(string dir)
		{			
			string reply = control.SendCommand("MKD " + dir);
			lastValidReply = control.ValidateReply(reply, "257");
		}
				
		/// <summary>  
		/// Change the remote working directory to
		/// that supplied
		/// </summary>
		/// <param name="dir"> name of remote directory to
		/// change to
		/// 
		/// </param>
		public void Chdir(string dir)
		{			
			string reply = control.SendCommand("CWD " + dir);
			lastValidReply = control.ValidateReply(reply, "250");
		}
		
		/// <summary>  
		/// Get modification time for a remote file
		/// </summary>
		/// <param name="remoteFile">  
		/// name of remote file
		/// </param>
		/// <returns>   
		/// modification time of file as a date
		/// </returns>
		public DateTime ModTime(string remoteFile)
		{			
			string reply = control.SendCommand("MDTM " + remoteFile);
			lastValidReply = control.ValidateReply(reply, "213");
			
			// parse the reply string ...
			DateTime ts = DateTime.ParseExact(lastValidReply.ReplyText, dtFormat, null);
			return ts;
		}
		
		/// <summary>  
		/// Get the current remote working directory
		/// </summary>
		/// <returns>   
		/// the current working directory
		/// </returns>
		public string Pwd()
		{			
			string reply = control.SendCommand("PWD");
			lastValidReply = control.ValidateReply(reply, "257");
			
			// get the reply text and extract the dir
			// listed in quotes, if we can find it. Otherwise
			// just return the whole reply string
			string text = lastValidReply.ReplyText;
			int start = text.IndexOf((System.Char) '"');
			int end = text.LastIndexOf((System.Char) '"');
			if (start >= 0 && end > start)
				return text.Substring(start + 1, (end) - (start + 1));
			else
				return text;
		}
		
		/// <summary>  
		/// Get the type of the OS at the server
		/// </summary>
		/// <returns>   
		/// the type of server OS		
		/// </returns>
		public string System()
		{			
			string reply = control.SendCommand("SYST");
			lastValidReply = control.ValidateReply(reply, "215");
			return lastValidReply.ReplyText;
		}
		
		/// <summary>  
		/// Get the help text for the specified command
		/// </summary>
		/// <param name="command"> name of the command to get help on
		/// </param>
		/// <returns> help text from the server for the supplied command
		/// 
		/// </returns>
		public string Help(string command)
		{			
			string reply = control.SendCommand("HELP " + command);
			string[] validCodes = new string[]{"211", "214"};
			lastValidReply = control.ValidateReply(reply, validCodes);
			return lastValidReply.ReplyText;
		}
		
		/// <summary>  
		/// Quit the FTP session
		/// </summary>
		public void Quit()
		{			
			try
			{
				string reply = control.SendCommand("QUIT");
				string[] validCodes = new string[]{"221", "226"};
				lastValidReply = control.ValidateReply(reply, validCodes);
			}
			finally
			{
				// ensure we clean up the connection
				control.Logout();
				control = null;
			}
		}
	}
}