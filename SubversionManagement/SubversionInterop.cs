using System;
using System.Diagnostics;
using System.Xml;

namespace SubversionReporter.SubversionManagement
{
    internal class SubversionInterop
    {
        private readonly string _subversionPath;
        private const string SubversionLogCommand = " --verbose --xml log ";
        
        public SubversionInterop(string subversionPath)
        {
            _subversionPath = subversionPath;
        }

        public XmlDocument RetrieveLogs(string svnRepositoryPath)
        {
            var subversionArguments = ProduceSubversionArgumentString(svnRepositoryPath);
            return RetrieveDataFromSubversion(subversionArguments);
        }

        private XmlDocument RetrieveDataFromSubversion(string subversionArguments)
        {
            var subversionClient = new Process();
            subversionClient.StartInfo.CreateNoWindow = true;
            subversionClient.StartInfo.UseShellExecute = false;
            subversionClient.StartInfo.RedirectStandardOutput = true;
            subversionClient.StartInfo.RedirectStandardInput = true;
            subversionClient.StartInfo.RedirectStandardError = true;

            subversionClient.StartInfo.FileName = _subversionPath;
            subversionClient.StartInfo.Arguments = subversionArguments;

            Console.WriteLine("Starting subversion client...");

            subversionClient.Start();

            Console.WriteLine("Collecting subversion client output...");

            string output = subversionClient.StandardOutput.ReadToEnd();

            subversionClient.WaitForExit();
            subversionClient.Close();
            subversionClient.Dispose();

            Console.WriteLine("Subversion output collected.");

            return new XmlDocument {InnerXml = output};
            
        }

        private static string ProduceSubversionArgumentString(string svnRepositoryPath)
        {
            return SubversionLogCommand + svnRepositoryPath;
        }
    }
}
