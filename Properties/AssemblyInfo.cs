using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if WINDOWS
[assembly: AssemblyTitle("WaveEngine.Components")]
#elif OUYA
[assembly: AssemblyTitle("WaveEngineOUYA.Components")]
#elif ANDROID
[assembly: AssemblyTitle("WaveEngineAndroid.Components")]
#elif IOS
[assembly: AssemblyTitle("WaveEngineiOS.Components")]
#elif METRO
[assembly: AssemblyTitle("WaveEngineMetro.Components")]
#elif WINDOWS_PHONE
[assembly: AssemblyTitle("WaveEngineWP.Components")]
#elif MAC
[assembly: AssemblyTitle("WaveEngineMac.Components")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Wave Corporation")]
[assembly: AssemblyCopyright("Copyright © Wave Corporation 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d4161402-ce18-494c-9f3a-cde368b1f964")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.3.0.0")]
