# SearchEverything Client

This application depends on voidtools.com "Search Everything" to work. It connects to one or more ETP servers and passes filesearch queries to them, then merges the results and translates the serverpath to a local path.

**Requirements:**  
+ Microsoft .NET Framework 2.0  
+ voidtools.com SearchEverything 1.2.1.371 for Windows [SearchEverything Download](http://www.voidtools.com/download.php)  

The application can be configured in the settings file, the most important parameters are:

### **Pathmapping**
each line contains a mapping between a local ETP server filepath and the filepath the user sees.  
Format is: `<ETPUri>;<serverpath>;<userpath>`  
matching is done "first found", so the order matters, username and password can be ommited from the URIs  

**Example:**  
`etp://vsvr1:21/;F:\Usr\Pilz\;G:\`  
`etp://vsvr1:21/;F:\Usr\Software\;I:\`  
`etp://vsvr1:21/;F:\Usr\UserHome\;J:\`  map server path F:\user\UserHome to J:\  
`etp://vsvr1:21/;F:\Usr\NCHomag\;H:\`  map server path F:\user\NCHomag to H:\  
`#etp://vsvr1:21021/;F:\Administration\;\\vsvr1\Administration\`  comment line  

### **ServerList**
each line starting with etp:// is interpreted as a connection-URI for a SearchEverything ETP server.  
Uri is in the form `etp://[(<username>|<username>:<password>)@]<hostname_or_ip>[:<port>]/`
this Uri must also be used in the PathMappings (except that username/password may be omitted there)

**Example:**  
`etp://etp@vsvr1:21/`  connect to ETP server vsvr1 at port 21, username etp, no password  
`#etp://etp:etp@vsvr1:21021/`  comment line, not active  

## Other Settings: _default value_  
**HideNonMappedFolders:** _true_   
When true, only search results where the path is mapped by PathMappings are displayed  

**HideInaccessible:** _true_  
When true, hide results, where fileaccess is not possible (for whatever reason).  

###Settings which can be changed from within the client:
**MatchWholeWord:** _false_, search for full words only  
**MatchCase:** _false_, search case-sensitive  
**MatchPath:** _false_, search not only in filenames, but also path-expressions  
**MaxResultsPerServer:** _1000_, only the first _n_ results from each server are queried, values > 1000 considerably slow down the search  
**ShowRealIcons:** _true_, display shell icons for the search results, instead of generic file and folder icons

