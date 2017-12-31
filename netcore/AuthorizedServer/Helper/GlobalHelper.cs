using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AuthorizedServer.Logger;
using MongoDB.Driver;

namespace AuthorizedServer.Helper
{
    /// <summary>Global helper for authorized controller </summary>
    public class GlobalHelper
    {
        /// <summary>Get current directory of project</summary>
        public static string GetCurrentDir()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>To read XML</summary>
        public static XElement ReadXML()
        {
            try
            {
                var dir = GetCurrentDir();
                var xmlStr = File.ReadAllText(Path.Combine(dir, "AmazonKeys.xml"));
                return XElement.Parse(xmlStr);
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("GlobalHelper", "ReadXML", ex.Message);
                return null;
            }
        }

        /// <summary>Get ip config from xml</summary>
        public static string GetIpConfig()
        {
            try
            {
                var result = ReadXML().Elements("ipconfig").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("authorizedserver2");
                return result.First().Value;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("GlobalHelper", "GetIpConfig", ex.Message);
                return null;
            }
        }
    }
}
