using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Arthur_Clive.Controllers;
using Arthur_Clive.Data;
using Arthur_Clive.Helper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace UnitTest_ArthurClive.Controller
{
    class Integrationtest_ArthurCliveController_Helper
    {
        public static List<string> ExtractFromString(string text, string startString, string endString)
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

        public static ResponceData DeserializedResponceData(dynamic data)
        {
            ActionResultModel deserializedResponce = new ActionResultModel();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponce.GetType());
            deserializedResponce = ser.ReadObject(ms) as ActionResultModel;
            ms.Close();
            return deserializedResponce.Value;
        }

        public static ResponceData_CategoryController_Get DeserializedResponceData_CategoryController_Get(dynamic data)
        {
            var split = data.Split(",");
            string alteredJsonString = "";
            var i = 0;
            foreach (var splitData in split)
            {
                i++;
                string replacedString = splitData;
                if (splitData.Contains("_id"))
                {
                    if (splitData.Contains("_v"))
                    {
                        replacedString = splitData.Replace(splitData, "\"_v\" : [{");
                    }
                    else
                    {
                        replacedString = splitData.Replace(splitData, "{");
                    }
                    alteredJsonString += replacedString;
                }
                else
                {
                    if (split.Length == i)
                    {
                        alteredJsonString += replacedString;
                    }
                    else
                    {
                        alteredJsonString += replacedString + ",";
                    }
                }
            }
            ActionResultModel_CategoryController_Get deserializedResponce = new ActionResultModel_CategoryController_Get();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(alteredJsonString));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponce.GetType());
            deserializedResponce = ser.ReadObject(ms) as ActionResultModel_CategoryController_Get;
            ms.Close();
            return deserializedResponce.Value;
        }

        public static ResponceData_ProductController_Get DeserializedResponceData_ProductController_Get(string data)
        {
            List<string> matchedString = ExtractFromString(data, "\"_v\" : ", " }, \"Content\" : null }");
            var split = matchedString[0].Split("}, { \"_id\" : ObjectId");
            long i = 0;
            foreach (var temp in split)
            {
                if (i == 0)
                {
                    split[i] = "[{" + temp.Substring(temp.IndexOf(',') + 1) + "},";
                }
                else if (i == split.Length - 1)
                {
                    split[i] = " {" + temp.Substring(temp.IndexOf(',') + 1);
                }
                else
                {
                    split[i] = " {" + temp.Substring(temp.IndexOf(',') + 1) + "},";
                }
                i++;
            }
            var alteredString = "";
            foreach (var temp in split)
            {
                var anotherSplit = temp.Split(",");
                string formedString = "";
                long k = 0;
                foreach (var xx in anotherSplit)
                {
                    if (xx.Contains("NumberLong"))
                    {
                        var extractedNumber = ExtractFromString(xx, "NumberLong(", ")")[0];
                        formedString += xx.Replace("NumberLong(" + extractedNumber + ")", extractedNumber) + ",";
                    }
                    else
                    {
                        if (k == anotherSplit.Length - 1)
                        {
                            formedString += xx;
                        }
                        else
                        {
                            formedString += xx + ",";
                        }
                    }
                    k++;
                }
                alteredString += formedString;
            }
            var replacedData = data.Replace(matchedString[0],alteredString);
            ActionResultModel_ProductController_Get deserializedResponce = new ActionResultModel_ProductController_Get();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(replacedData));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponce.GetType());
            deserializedResponce = ser.ReadObject(ms) as ActionResultModel_ProductController_Get;
            ms.Close();
            return deserializedResponce.Value;
        }

        public static IMongoDatabase db = MongoHelper._client.GetDatabase("Authentication");

        public async static void InsertRegiterModeldata(RegisterModel registerModel)
        {
            await db.GetCollection<RegisterModel>("Authentication").InsertOneAsync(registerModel);
        }

        public async static Task<ActionResult> GetCategories(CategoryController controller)
        {
            var result = await controller.Get() as ActionResult;
            return result;
        }
    }
}
