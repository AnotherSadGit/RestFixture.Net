Release Version of Jurassic
===========================
Simon Elms, 4 Sep 2017

At 4 Sep 2017 the current Nuget package of Jurassic is 2.2.1.  This has a bug:  It throws an 
exception when ScriptEngine.SetGlobalValue is used to assign null to a global variable.  

The bug was fixed in Jurassic commit 950b1d2435bd18e7c648afadb899c597d9a0d490, from  
24/05/2017 5:54:06 p.m.  However, there has been no new Nuget package with that fix.

I checked out the most recent Jurassic commit prior to the conversion to .NET Standard 2.0, 
0fcf4ec359e10c91f7223f35bf0d0ec0857d9ef5, from 13/07/2017 11:34:43 a.m, and built it in release 
configuration.  The Jurassic.dll from that build has been copied to this project.

NOTE: Jurassic has been on assembly version 3.0.0.0 for around a year.  That would appear to be 
the next release.  For my interim Jurassic.dll I reduced the assembly version to 2.3.0.0.

Once the Jurassic 3.0.0 Nuget package is released this Jurassic.dll can be removed and replaced 
with the Jurassic Nuget package.