Setting Up the FitNesse Test Environment for RestFixture.Net
============================================================
Simon Elms, 28 Aug 2017

This document assumes you'll be running the tests against a copy of RestFixture.Net built from 
source.  The class paths in the plugins.properties configuration file will need to be edited if 
you're running the tests against downloaded RestFixture.Net DLLs.

1) Download and Install FitNesse
--------------------------------
If not installed already.  The download location and the instructions for installation are at:
http://fitnesse.org/FitNesseDownload

NOTE: The FitNesse root folder is the folder that, after installation, contains the 
fitnesse-standalone.jar or the fitnesse.jar (depending on which version of FitNesse is installed 
- the standalone version or the version that integrates with Maven).  The FitNesse root folder 
is not the same as the FitNesseRoot folder, which is the root folder containing all test pages.

2) Download and Install FitSharp
--------------------------------
If not installed already.  

FitSharp is FitNesse for .NET.  The FitSharp binaries can be downloaded in a ZIP file from:
https://github.com/jediwhale/fitsharp/releases

Unpack the contents of the ZIP file into a sub-folder under the FitNesse root folder.  Suggested 
folder name: FitSharp.

3) Create FITNESSE_HOME Environment Variable
--------------------------------------------
The FITNESSE_HOME environment variable specifies the path to the FitNesse root folder (the folder 
containing the fitnesse-standalone.jar or the fitnesse.jar file, and other FitNesse application 
files).  

This allows the test project to be saved in a separate location from the FitNesse application (by 
default test files are saved under the FitNesse root folder).  In this way multiple test projects 
can share the same FitNesse instance with each test project being saved to its own source control 
repository.  

FITNESSE_HOME can be defined as a User variable or as a System variable: 

a) Create a User variable if FitNesse will only be used by one user on a shared computer;

b) Create a System variable if multiple users will be using FitNesse.

Either a User or a System variable can be used on a computer that is used by only one person.

WARNING: When creating FITNESSE_HOME DO NOT include a trailing backslash, "\", in the 
path.  eg Set the path to "C:\Fitnesse", NOT "C:\FitNesse\".

4) Set Class Paths in plugins.properties File
---------------------------------------------
Edit the plugins.properties file, which is in the same folder as the 
FitNesseStart_RestFixtureNet.cmd batch file.  Set the class paths in the file so FitNesse can 
find FitSharp and the fixture DLLs it needs.

The class paths are:

a) FitSharpDirectory: The path to the FitSharp folder, which contains the FitSharp Runner 
executables and FitSharp DLLs.  Normally this would be a sub-folder under the FitNesse root folder 
but this plugin property allows FitSharp to be located elsewhere;

b) RestFixtureDirectory: The path to the folder containing RestFixture.Net.dll and the other 
RestFixture.Net DLLs.  This may be a folder that the RestFixture.Net DLLs have been downloaded into 
or it may be a bin folder if RestFixture.Net has been built from source;

c) RestFixtureTestServerDirectory: The path to the folder containing FitNesseTestServer.dll and the 
other RestFixture.Net test server DLLs; 

d) FitNessePluginsDirectory: The path to the folder containing DLLs for fixture classes and other 
FitNesse plugins.  Normally the RestFixture.Net DLLs would be added to the plugins folder when it 
is being used to test other applications (as opposed to this case, where we're testing 
RestFixture.Net itself).

NOTES:
i) Backslashes in paths will need to be escaped by doubling them: "\" -> "\\";

ii) When setting the class paths DO NOT include trailing backslashes, "\", in the 
path.  eg Set the path to "C:\\Fitnesse\\FitSharp", NOT "C:\\Fitnesse\\FitSharp\\";

iii) Relative or absolute paths may be used.  Out of the box the plugins.properties file is 
configured with relative paths for RestFixtureDirectory and RestFixtureTestServerDirectory, 
assuming that you'll be running the tests against a copy of RestFixture.Net built from source 
and that you haven't moved the FitNesseAcceptanceTests folder (which this README file is in);

iv) The FITNESSE_HOME environment variable can be used to simplify paths that are under the 
FitNesse root folder.  Use FitNesse markup variable syntax when specifying the environment 
variable, eg "FitSharpDirectory=${FITNESSE_HOME}\\FitSharp"

5) Start FitNesse, Configured to Run the RestFixture.Net Tests
--------------------------------------------------------------
Run the batch file FitNesseStart_RestFixtureNet.cmd to start FitNesse.  The batch file configures 
FitNesse to listen on port 8090.  Edit the batch file if you want to use a different port.

--------------------------------------------------------------------
NOTE: Source Control Configuration
--------------------------------------------------------------------
One of the FitNesse releases in 2015 or 2016 introduced a new page format: *.wiki files replaced 
content.txt and properties.xml.  The new format can only read page properties, such as whether a 
page is a test page or a suite page, with LF line endings.  It cannot read properties with CRLF 
endings.

Therefore this Git repository has been configured to checkout and commit line endings as-is 
(core.autocrlf="false") rather than the default which is checkout Windows-style CRLF and commit 
Unix-style LF (core.autocrlf="true").  

If core.autocrlf were set to "true" then all the line endings in FitNesse pages would effectively 
be converted to CRLF and FitNesse would be unable to read the page properties.