using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Arthur_Clive.Data;
using Arthur_Clive.Logger;

namespace Arthur_Clive.Helper
{
    /// <summary>Helper method for PayUMoney service</summary>
    public class PayUHelper
    {
        /// <summary>Get hash value of random number</summary>
        /// <param name="text"></param>
        public static string Generatehash512(string text)
        {
            try
            {
                byte[] message = Encoding.UTF8.GetBytes(text);
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] hashValue;
                SHA512Managed hashString = new SHA512Managed();
                string hex = "";
                hashValue = hashString.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "Generatehash512", ex.Message);
                return null;
            }
        }

        /// <summary>Prepare post form for paymet through PayUMoney</summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public static string PreparePOSTForm(string url, Hashtable data)
        {
            try
            {
                string formID = "PostForm";
                StringBuilder strForm = new StringBuilder();
                strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
                foreach (DictionaryEntry key in data)
                {
                    strForm.Append("<input type=\"hidden\" name=\"" + key.Key + "\" value=\"" + key.Value + "\">");
                }
                strForm.Append("</form>");
                StringBuilder strScript = new StringBuilder();
                strScript.Append("<script language='javascript'>");
                strScript.Append("var v" + formID + " = document." + formID + ";");
                strScript.Append("v" + formID + ".submit();");
                strScript.Append("</script>");
                strForm.Append(strScript);
                return strForm.ToString();
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "PreparePOSTForm", ex.Message);
                return null;
            }
        }

        /// <summary>Get TxnId</summary>
        public static string GetTxnId()
        {
            try
            {
                Random random = new Random();
                string strHash = Generatehash512(random.ToString() + DateTime.Now);
                string txnId = strHash.ToString().Substring(0, 20);
                return txnId;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "GetTxnId", ex.Message);
                return null;
            }
        }

        /// <summary>Get hash string</summary>
        /// <param name="txnId"></param>
        /// <param name="model"></param>
        public static string GetHashString(string txnId, PaymentModel model)
        {
            try
            {
                string hashSequence = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|||||||||salt";
                hashSequence = hashSequence.Replace("key", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value);
                hashSequence = hashSequence.Replace("txnid", txnId);
                hashSequence = hashSequence.Replace("amount", Convert.ToDecimal(model.Amount).ToString("F2"));
                hashSequence = hashSequence.Replace("productinfo", model.ProductInfo);
                hashSequence = hashSequence.Replace("firstname", model.FirstName);
                hashSequence = hashSequence.Replace("email", model.Email);
                hashSequence = hashSequence.Replace("udf1", model.OrderId.ToString());
                hashSequence = hashSequence.Replace("udf2", model.UserName);
                hashSequence = hashSequence.Replace("salt", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("saltkey").First().Value);
                return hashSequence;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "GetHashString", ex.Message);
                return null;
            }
        }

        /// <summary>Get reverse hash string</summary>
        /// <param name="txnId"></param>
        /// <param name="model"></param>
        public static string GetReverseHashString(string txnId, PaymentModel model)
        {
            try
            {
                string hashSequence = "salt|status|||||||||udf2|udf1|email|firstname|productinfo|amount|txnid|key";
                hashSequence = hashSequence.Replace("key", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value);
                hashSequence = hashSequence.Replace("txnid", txnId);
                hashSequence = hashSequence.Replace("amount", Convert.ToDecimal(model.Amount).ToString("F2"));
                hashSequence = hashSequence.Replace("productinfo", model.ProductInfo);
                hashSequence = hashSequence.Replace("firstname", model.FirstName);
                hashSequence = hashSequence.Replace("email", model.Email);
                hashSequence = hashSequence.Replace("status", "success");
                hashSequence = hashSequence.Replace("udf1", model.OrderId.ToString());
                hashSequence = hashSequence.Replace("udf2", model.UserName);
                hashSequence = hashSequence.Replace("salt", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("saltkey").First().Value);
                return hashSequence;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "GetReverseHashString", ex.Message);
                return null;
            }
        }

        /// <summary>Generate hashtable data for payment gateway process</summary>
        /// <param name="model">Data to be included in the form</param>
        public static Hashtable GetHashtableData(PaymentModel model)
        {
            try
            {
                string SuccessUrl = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("successurl").First().Value;
                string FailureUrl = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("failureurl").First().Value;
                string CancelUrl = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("cancelurl").First().Value;
                string txnId = GetTxnId();
                string hashString = GetHashString(txnId, model);
                string hash = Generatehash512(hashString).ToLower();
                string action = GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("url").First().Value + "/_payment";
                Hashtable data = new Hashtable();
                data.Add("hash", hash);
                data.Add("txnid", txnId);
                data.Add("key", GlobalHelper.ReadXML().Elements("payu").Where(x => x.Element("current").Value.Equals("Yes")).Descendants("key").First().Value);
                string AmountForm = Convert.ToDecimal(model.Amount).ToString("F2");
                data.Add("amount", AmountForm);
                data.Add("firstname", model.FirstName);
                data.Add("email", model.Email);
                data.Add("phone", model.PhoneNumber);
                data.Add("productinfo", model.ProductInfo);
                data.Add("surl", SuccessUrl);
                data.Add("furl", FailureUrl);
                data.Add("lastname", model.LastName);
                data.Add("curl", CancelUrl);
                data.Add("address1", model.AddressLine1);
                data.Add("address2", model.AddressLine2);
                data.Add("city", model.City);
                data.Add("state", model.State);
                data.Add("country", model.Country);
                data.Add("zipcode", model.ZipCode);
                data.Add("udf1", model.OrderId);
                data.Add("udf2", model.UserName);
                data.Add("udf3", "");
                data.Add("udf4", "");
                data.Add("udf5", "");
                data.Add("pg", "");
                data.Add("service_provider", "PayUMoney");
                return data;
            }
            catch (Exception ex)
            {
                LoggerDataAccess.CreateLog("PayUHelper", "GetHashtableData", ex.Message);
                return null;
            }
        }
    }
}
