using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace SubversionReporter.SubversionManagement
{
    public class SubversionManager
    {
        private readonly string _subversionExePath;

        public SubversionManager(string subversionExePath)
        {
            _subversionExePath = subversionExePath;
        }

        public XmlDocument RetrieveSubversionLogs(string svnRepositoryPath)
        {
            var subversionRequestProcess = new SubversionInterop(_subversionExePath);
            return subversionRequestProcess.RetrieveLogs(svnRepositoryPath);
        }

        public string TransformSubversionLog(XmlDocument subversionLogData, string xslStyleSheetPath)
        {   
            var xsltDataXml = LoadXslTransformationStyle(xslStyleSheetPath);

            var xmlTr = new XmlNodeReader(subversionLogData);
            var outputBuilder = new StringBuilder();

            var transform = new XslCompiledTransform();
            transform.Load(xsltDataXml, new XsltSettings {EnableDocumentFunction = true, EnableScript = true}, new XmlUrlResolver());
            transform.Transform(xmlTr, null, new StringWriter(outputBuilder));

            return outputBuilder.ToString();
        }

        private static XmlTextReader LoadXslTransformationStyle(string xslStyleSheetPath)
        {
            var xsltSheet = File.ReadAllText(xslStyleSheetPath);
            TextReader xsltSheetDataTextReader = new StringReader(xsltSheet);
            return new XmlTextReader(xsltSheetDataTextReader);
        }
    }
}
