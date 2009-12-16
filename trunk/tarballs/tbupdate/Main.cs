using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace tbupdate
{
	class MainClass
	{
		
		public static Arguments AppArgs;
		public static string AppFolder;
		
		
		public static void Main(string[] args)
		{
			AppArgs = new Arguments(args);
			AppFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			
			if (args.Length == 0)
			{
				SendHelpMessage();
				return;
			}
			
			if (AppArgs["h"] == "true")
			{
				SendHelpMessage();
				return;
			}			
			
			
			if (AppArgs["n"] == null ||
			    AppArgs["mf"] == null ||
			    AppArgs["i"] == null ||
			    AppArgs["u"] == null)
			{
				Console.WriteLine("Missing some parameter !!!\r\n\r\n");
				SendHelpMessage();
				return;
			}

			
			string tbname = AppArgs["n"].ToString();
			string makefilePath = AppArgs["mf"].ToString();
			string installFilePath = AppArgs["i"].ToString();
			string uninstallFilePath = AppArgs["u"].ToString();
			
			string tbfilename = tbname + ".tar.gz";
			
			string originalPath = AppFolder + 
				Path.DirectorySeparatorChar + 
			    "original";
			
			string originalPathTB = originalPath + 
				Path.DirectorySeparatorChar + 
				tbfilename;			
			
			string firstTBPath = AppFolder + 
				Path.DirectorySeparatorChar + 
				tbfilename;


			if (!File.Exists(firstTBPath))
			{
				Console.WriteLine("tarball not founded...");
				return;
			}
			
			Console.WriteLine("extract sources");
			ExecuteCommandSync("tar", "xzf " + tbfilename);
			
			Console.WriteLine("backup original tarball");
			
			if (!Directory.Exists(originalPath))
			{
				Directory.CreateDirectory(originalPath);
			}
			
			if (File.Exists(originalPathTB))
			{
				File.Delete(originalPathTB);
			}
			
			File.Move(firstTBPath,originalPathTB);
			
			Console.WriteLine("modify Makefile.in");
			ModifyMakefile(AppFolder + Path.DirectorySeparatorChar + makefilePath,
			               installFilePath,
			               uninstallFilePath);
			
			ExecuteCommandSync("tar", "-zcf " + tbfilename + " " + tbname);
			
			Directory.Delete(AppFolder + 
				Path.DirectorySeparatorChar + 
				tbname,true);
			
			return;
		}
		
		
		
		private static void ExecuteCommandSync(string FileName, string CmdArgs)
		{
		     try
		     {
			    ProcessStartInfo procStartInfo = new ProcessStartInfo(FileName, CmdArgs);
		
			    // The following commands are needed to redirect the standard output.
			    // This means that it will be redirected to the Process.StandardOutput StreamReader.
			    procStartInfo.RedirectStandardOutput = true;
			    procStartInfo.UseShellExecute = false;
			    
				// Do not create the black window.
			    procStartInfo.CreateNoWindow = true;
			    
				// Now we create a process, assign its ProcessStartInfo and start it
			    Process proc = new Process();
			    proc.StartInfo = procStartInfo;
			    proc.Start();
			    
				// Get the output into a string
			    string result = proc.StandardOutput.ReadToEnd();
			    
				// Display the command output.
			    Console.WriteLine(result);
		      }
		      catch (Exception objException)
		      {
		      // Log the exception
				throw objException;
		      }
			
		}
		
		
		private static void ModifyMakefile(string filePath,
		                                   string pathInstallAdd,
		                                   string pathUninstallAdd)
		{
			string NewFileName = filePath + "_new";
			string InstallSection = "";
			string UninstallSection = "";
			string MakeFile = "";
			
			StreamReader SR;
			SR = new StreamReader(AppFolder + Path.DirectorySeparatorChar + pathInstallAdd);
			InstallSection = SR.ReadToEnd();
			SR.Close();
			SR = new StreamReader(AppFolder + Path.DirectorySeparatorChar + pathUninstallAdd);
			UninstallSection = SR.ReadToEnd();
			SR.Close();
			SR = new StreamReader(filePath);
			MakeFile = SR.ReadToEnd();
			
			StreamWriter SW = new StreamWriter(NewFileName);
			
			int INSTALLPF = MakeFile.IndexOf("install-programfilesDATA: $(programfiles_DATA)");
			INSTALLPF = MakeFile.IndexOf("done\n",INSTALLPF);
			
			SW.Write(MakeFile.Substring(0,INSTALLPF) + 
			         "done\r\n" + 
			         InstallSection);
			
			MakeFile = MakeFile.Substring(INSTALLPF + 4);
			
			int UNINSTALLPF = MakeFile.IndexOf("uninstall-programfilesDATA:");
			UNINSTALLPF = MakeFile.IndexOf("install-",UNINSTALLPF + 10);
			
			
			
			
			SW.Write(MakeFile.Substring(0,UNINSTALLPF) + 			        
			         UninstallSection + "\r\n");
			
			MakeFile = MakeFile.Substring(UNINSTALLPF);
			
			SW.Write(MakeFile);
			
			SW.Close();
			SW.Dispose();
			SW = null;
			SR.Close();
			SR.Dispose();
			SR = null;
			
			File.Delete(filePath);
			File.Move(NewFileName, filePath);
			return;
		}
		
		
		private static void SendHelpMessage()
		{
			Console.WriteLine("\r\n" + 
			                  "Use tbupdate -n <tarball name without tar.gz> \r\n" + 
			                  "             -mf <Makefile.in internal path> \r\n"+
			                  "             -i <install additions file path> \r\n" + 
			                  "             -u <uninstall additions file path> \r\n");
			
		}
		
		
	}
}