using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using SubversionReporter.SubversionManagement;

namespace SubversionReportProducer
{
    class Program
    {
        private static Arguments _commandLineArgs;
        private static string _svnPath;
        private static string _repositoryPath;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Subversion Report Producer\n");

                ConfigureApplicationContext(args);
                var manager = new SubversionManager(_svnPath);

                Console.WriteLine("Collecting subversion history...");

                var logs = manager.RetrieveSubversionLogs(_repositoryPath);

                Console.WriteLine("Transforming subversion history...");

                var transformedLogs = manager.TransformSubversionLog(logs, _commandLineArgs["style"]);

                Console.WriteLine("Done.");

                if (File.Exists(_commandLineArgs["outputLocation"]))
                {
                    File.Delete(_commandLineArgs["outputLocation"]);
                }

                File.WriteAllText(_commandLineArgs["outputLocation"], transformedLogs);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        #region private methods
        private static void ConfigureApplicationContext(IEnumerable<string> args)
        {
            _commandLineArgs = new Arguments(args);
            LoadConfiguration(ref _svnPath, "SubversionPath");
            LoadConfiguration(ref _repositoryPath, "RepositoryPath");
            ValidateArgs();
        }

        private static void LoadConfiguration(ref string storage, string settingName)
        {            
            // First look in config
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[settingName]))
            {
                storage = ConfigurationManager.AppSettings[settingName];
            }

            // Then overwrite it if a command line arg is specified
            if (!String.IsNullOrEmpty(_commandLineArgs[settingName]))
            {
                storage = _commandLineArgs[settingName];
            } 
        }

        private static void ValidateArgs()
        {
            if (_commandLineArgs["?"] != null || _commandLineArgs["help"] != null)
            {
                DisplayUsage();
            }

            if (String.IsNullOrEmpty(_commandLineArgs["outputLocation"]))
            {
                throw new ApplicationException("outputLocation not specified.");
            }
            
            Console.WriteLine("Output Location: " + _commandLineArgs["outputLocation"]);

            if (String.IsNullOrEmpty(_commandLineArgs["style"]))
            {
                throw new ApplicationException("style not specified.");
            }
            
            Console.WriteLine("Style: " + _commandLineArgs["style"]);

            if (String.IsNullOrEmpty(_repositoryPath))
            {
                throw new ApplicationException("SVN path not specified.");
            }
            
            Console.WriteLine("Repository Path: " + _repositoryPath);

            Console.WriteLine();
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("   --style=XSLFile --outputLocation=FileName --repositoryPath=SVNRepository");
            Console.WriteLine();
            Console.WriteLine("e.g.");
            Console.WriteLine("   .exe --style=ChangeLog.xsl --outputLocation=out.txt --repositoryPath=svn://repo/trunk");
        }
        #endregion

    }

}
