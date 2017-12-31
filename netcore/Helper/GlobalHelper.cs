using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;
using MongoDB.Bson.Serialization;

namespace Arthur_Clive.Helper
{
    /// <summary>Global helper method</summary>
    public class GlobalHelper
    {
        /// <summary>Get current directory of project</summary>
        public static string GetCurrentDir()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>xml file</summary>
        public static XElement ReadXML()
        {
            try
            {
                var dir = GetCurrentDir();
                var xmlStr = File.ReadAllText(Path.Combine(dir, "AmazonKeys.xml"));
                return XElement.Parse(xmlStr);
            }
            catch(Exception ex)
            {
                LoggerDataAccess.CreateLog("GlobalHelper", "ReadXML", ex.Message);
                return null;
            }
        }

        /// <summary>Send gift through email after payment success</summary>
        /// <param name="orderId"></param>
        public static string SendGift(long orderId)
        {
            try
            {
                var checkOrder = MongoHelper.CheckForDatas("OrderId", orderId, null, null, "OrderDB", "OrderInfo");
                if (checkOrder != null)
                {
                    var orderInfo = BsonSerializer.Deserialize<OrderInfo>(checkOrder);
                    List<string> productInfoList = new List<string>();
                    foreach (var product in orderInfo.ProductDetails)
                    {
                        productInfoList.Add(product.ProductSKU);
                    }
                    var productInfoString = String.Join(":", productInfoList);
                    var sendGift = EmailHelper.SendGift(orderId, productInfoString);
                }
                return "Success";
            }
            catch(Exception ex)
            {
                LoggerDataAccess.CreateLog("GlobalHelper", "SendGift", ex.Message);
                return null;
            }
        }

        /// <summary>Get string between to characters</summary>
        /// <param name="text"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        public static List<string> StringBetweenTwoCharacters(string text, string startString, string endString)
        {
            try
            {
                List<string> matched = new List<string>();
                int indexStart = 0, indexEnd = 0;
                bool exit = false;
                while (!exit)
                {
                    indexStart = text.IndexOf(startString);
                    indexEnd = text.IndexOf(endString);
                    if (indexStart != -1 && indexEnd != -1)
                    {
                        matched.Add(text.Substring(indexStart + startString.Length,
                            indexEnd - indexStart - startString.Length));
                        text = text.Substring(indexEnd + endString.Length);
                    }
                    else
                        exit = true;
                }
                return matched;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("GlobalHelper", "StringBetweenTwoCharacters", ex.Message);
                return null;
            }
        }
    }
}
