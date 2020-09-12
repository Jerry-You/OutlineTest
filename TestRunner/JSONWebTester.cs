using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TestRunner
{
    public class JSONWebTester
    {
        public string URL { get; set; }
        public string Data { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<Cookie> Cookies { get; set; }
        public dynamic Result { get; set; }
        public JSONWebTester()
        {
            Headers = new Dictionary<string, string>();
            Cookies = new List<Cookie>();
            //default
            Headers.Add("Content-Type", "application/json");
            Headers.Add("Accept", "application/json");

        }
        public dynamic GetResult() {

            dynamic result = null;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            foreach (var tp in Headers)
                request.Headers.Add(tp.Key, tp.Value);
            foreach (var tp in Cookies)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(tp);
            }
            var response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream ms = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                       result = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());                       
                    }
                }
            }
            

            return result;
            
              
        }
        public void SetCookieAuth(string name, string token, string domain)
        { }
        public void SetAuthHeader(string type, string token)
        { 
        }
        public bool CompareResult(string expect,bool suppressonerror,out string message)
        {
            message = "";
            bool returnme = true;
            List<string> difference = new List<string>();
            JToken jtoken = JsonConvert.DeserializeObject<JToken>(expect);
            returnme = CompareResult(expect, Result, difference,"\\", suppressonerror);
            message = string.Join(Environment.NewLine, difference.Where(f=>!string.IsNullOrEmpty(f)).ToArray());
            return true;            
        }
        private bool CompareResult(JToken expect, JToken result, List<string> differnt, string path,bool suppressonerror)
        {
            // compare1, make sure the type is the same, 
            ///
            /// left array, right array
            /// left primitive value, right primitive value
            /// left jvalue jvalue
            ///
            bool returnme = true;
            if (expect is JArray)
            {
                if (!(result is JArray))
                {
                    differnt.Add($"path at {path} is having difference, expected a collection");
                    return false;
                }
                int index = 0;
                JArray array = result as JArray;
                if ((expect as JArray).Count != array.Count)
                {
                    differnt.Add($"path at {path} is having difference, collection size is different between expected and actual result");
                    return false;

                }
                foreach (var subjoken in expect as JArray)
                {
                    returnme = returnme && CompareResult(subjoken,array[index],differnt,path+$"//[{index}]",suppressonerror);
                    index++;
                    if (suppressonerror)
                        break;
                }
            }
            else if (expect is JObject)
            {
                if (!(result is JObject))
                {
                    differnt.Add($"path at {path} is having difference, expected a nested object");
                    return false;
                }
                IDictionary<string,JToken> obj = (result as JObject) as IDictionary<string,JToken>;
                IDictionary<string, JToken> expectedobj = (expect as JObject) as IDictionary<string, JToken>;
                string[] keysnotinexpect = obj.Keys.Except(expectedobj.Keys).ToArray();
                string[] keysnotinresult = expectedobj.Keys.Except(obj.Keys).ToArray();
                if (keysnotinexpect.Length > 0)
                {
                    differnt.Add($"path at {path} is having difference, property {string.Join(",",keysnotinexpect)} are not expected");
                    return false;
                }
                if (keysnotinresult.Length > 0)
                {
                    differnt.Add($"path at {path} is having difference, property {string.Join(",", keysnotinresult)} are expected but can not be found in result");
                    return false;
                }
                foreach (var tps in expectedobj)
                {
                    returnme = returnme && CompareResult(tps.Value, obj[tps.Key], differnt, path + $"//[{tps.Key}]", suppressonerror);
                    if (suppressonerror)
                        break;
                }

            }
            else if (expect is JValue)
            {
                if (!(result is JValue))
                {
                    differnt.Add($"path at {path} is having difference, expected a primitive value");
                    return false;
                }
                if ((expect as JValue).ToString() != (result as JValue).ToString())
                {
                    differnt.Add($"path at {path} is having difference, expected {(expect as JValue).ToString()}, actual {(result as JValue).ToString()}");
                    return false;
                }
            }
            else 
            {
                differnt.Add($"path at {path} is having issue, unable to parse expected value");
                return false;
            }
            return returnme;
        }
        
    }
}
