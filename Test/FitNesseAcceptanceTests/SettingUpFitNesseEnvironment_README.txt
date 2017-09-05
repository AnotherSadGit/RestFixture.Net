Setting Up the FitNesse Test Environment for RestFixture.Net
============================================================
Simon Elms, 28 Aug 2017

This document assumes you'll be running the tests against a copy of RestFixture.Net built from 
source.  The class paths in the various test and suite pages will need to be edited if you're 
running the tests against downloaded RestFixture.Net DLLs.

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

3) Create Environment Variables
-------------------------------
Create the environment variables that specify various paths needed to run FitNesse and the 
RestFixture.Net tests.  

The environment variables can be defined as User variables or as System variables: 

a) Create User variables if FitNesse will only be used by one user on a shared computer;

b) Create System variables if multiple users will be using FitNesse.

Either User or System variables can be used on a computer that is used by only one person.

WARNING: When creating the environment variables DO NOT include a trailing backslash, "\", in the 
path.  eg Set the path to "C:\Fitnesse", NOT "C:\FitNesse\".

The environment variables to create are:

a) FITNESSE_HOME:  The path to the FitNesse root folder (the folder containing the 
fitnesse-standalone.jar or the fitnesse.jar file);

b) FITSHARP_HOME:  The path to the FitSharp folder, which contains the FitSharp Runner executables 
and FitSharp DLLs.  Normally this would be a sub-folder under the FitNesse root folder but this 
environment variable allows FitSharp to be located elsewhere;

c) RESTFIXTURE_HOME:  The path to the folder containing RestFixture.Net.dll and the other 
RestFixture.Net DLLs.  This may be a folder that the RestFixture.Net DLLs have been downloaded into 
or it may be a bin folder if RestFixture.Net has been built from source;

d) RESTFIXTURE_SERVER_HOME:  The path to the folder containing FitNesseTestServer.dll and the 
other RestFixture.Net test server DLLs. 

4) Start FitNesse, Configured to Run the RestFixture.Net Tests
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