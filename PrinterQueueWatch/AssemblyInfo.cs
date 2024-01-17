using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Review the values of the assembly attributes

[assembly: AssemblyTitle("Printer Monitor Component")]
[assembly: AssemblyDescription("A component to monitor a printer queue")]
[assembly: AssemblyCompany("Merrion Computing Ltd")]
[assembly: AssemblyProduct("MCL PrintQueueWatch")]
[assembly: AssemblyCopyright("(c) 2002 - 2012 Merrion Computing Ltd")]
[assembly: AssemblyTrademark("")]
[assembly: CLSCompliant(true)]

// Version information for an assembly consists of the following four values:
// 
// Major Version
// Minor Version 
// Build Number
// Revision
// 
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("2.0.8.*")]

// \\ --[Assembly security]----------------------------------------------------
// \\ Permission requests
// \\   UIPermissions
// \\      All windows allowed (This means that this control could be spoofed, but this is not an issue)
// \\
// \\ Assembly cannot be used by COM clients
[assembly: ComVisible(false)]
