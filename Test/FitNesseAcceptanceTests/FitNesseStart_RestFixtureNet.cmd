:: Command line arguments:
::	-p: Port that FitNesse listens on.  Determines the port to use in the URI, 
::		eg -p 8091 => http://localhost:8091/FrontPage
::	-e: The number of days to keep page history files for.  
::		If the FitNesse project is going to be under source control then set -e 0 as FitNesse will 
::		not need to keep internal copies of old versions of its pages.
:: %FITNESSE_HOME% environment variable: Path to the folder that contains the common FitNesse 
::	files, including the fitnesse-standalone.jar and the FitSharp sub-folder containing the 
::	FitSharp executables and DLLs.
java.exe -jar %FITNESSE_HOME%\fitnesse-standalone.jar -p 8090 -e 0