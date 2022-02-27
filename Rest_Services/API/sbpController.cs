using PITB_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using Tracer;
using TPE.BO;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;

namespace PITB_Service.API
{
    public class sbpController : ApiController
    {
        [HttpGet]        
        [Route("api/sbp/p2p/customers")]
        public string customers(string uidType,string uidValue)
        {
            try
            {
                //BO_TPE_CONFIGURATION l_urlCustomerInfoByCNIC = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "RAAST_URL");

                //string url = l_urlCustomerInfoByCNIC.CONFIG_VALUE;

                string url = ConfigurationManager.AppSettings["RAAST_URL"].ToString();

                var re = Request;
                var headers = re.Headers;
                string requestID = headers.GetValues("X-Request-ID").First();
                string participentID = headers.GetValues("Participant-MemberID").First();
                string token = headers.GetValues("Authorization").First();
                Dictionary<string, string> lstheaders = new Dictionary<string, string>();
                lstheaders.Add("X-Request-ID", requestID);
                lstheaders.Add("Participant-MemberID", participentID);
                lstheaders.Add("Authorization", token);
                //string resource = re.DisposeRequestResources
                string _isError = string.Empty;
                //string res = WRJGetCustomerInfoByCNIC_Client(url, requestID, participentID, uidType, uidValue, token, out _isError);
                string res = WrapperGeneric(url, lstheaders, re.RequestUri.PathAndQuery.ToString(), out _isError);
                return res;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string WRJGetCustomerInfoByCNIC_Client(string url, string req_id, string _participant_Member_Id, string _type, string _cnic, string _token, out string _isError)
        {
            string ret = string.Empty;
            _isError = "0";
            GetCustomerInfoByCNICResponseError _error = new GetCustomerInfoByCNICResponseError();

            BO_TPE_CONFIGURATION l_urlCustomerInfoByCNICinitial = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "RAAST_URL_CUSTOMER_INFO_CNIC");
            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC", "l_urlAliasTitleFetch.CONFIG_VALUE:: " + l_urlCustomerInfoByCNICinitial.CONFIG_VALUE);

            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC", "WRJGetCustomerInfoByCNIC_Client token:: " + _token);



            string _urlCustomerInfoCNIC = l_urlCustomerInfoByCNICinitial.CONFIG_VALUE;

            try
            {

                BO_TPE_CONFIGURATION l_sslEnabled = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "SSL_ENABLED");
                AuditTrace.Notify("TPE.Raast.WRJGetCustomerInfoByCNIC", "l_sslEnabled:: " + l_sslEnabled.CONFIG_VALUE);
                if (l_sslEnabled.CONFIG_VALUE == "1")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else if (l_sslEnabled.CONFIG_VALUE == "2")
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                else if (l_sslEnabled.CONFIG_VALUE == "3")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                //   ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                // url = "https://apis1.hbl.com:8343/";

                var client = new RestSharp.RestClient();
                Uri _url = new Uri(url);
                client.BaseUrl = _url;
                var request = new RestSharp.RestRequest();
                //client.Timeout = 1;
                request.Method = 0;
                request.AddHeader("Participant-MemberID", _participant_Member_Id);
                request.AddHeader("X-Request-ID", req_id);
                request.AddHeader("ContentType", "application/json");
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", _token);
                request.AddHeader("Charset", "UTF-8");


                request.Resource = _urlCustomerInfoCNIC + _type + "&uidValue=" + _cnic;
                AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WebRequest Data:::", request.ToString());
                AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client _type:::", _type);
                AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client _cnic:::", _cnic);

                var response = client.Execute(request) as RestSharp.RestResponse;
                if (response.ErrorException != null)
                {
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Exception:::", response.ErrorException);
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Message:::", response.ErrorMessage);
                }

                if (response != null && ((response.StatusCode == HttpStatusCode.OK) &&
                 (response.ResponseStatus == RestSharp.ResponseStatus.Completed)))
                {
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client Content :::", response.Content.ToString());
                    ret = response.Content.ToString();

                }
                else
                {
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());

                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.Content.ToString());
                    _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(response.Content.ToString());
                    string jsonError = new JavaScriptSerializer().Serialize(_error);
                    ret = jsonError;
                    _isError = "1";
                }

            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception Response :::", ex.Response);
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception string :::", ex.ToString());

                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(data);
                        _isError = "1";
                        _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(sr.ReadToEnd());
                        string jsonError = new JavaScriptSerializer().Serialize(_error);
                        ret = jsonError;
                    }
                }
            }



            return ret;
        }

        public static string WrapperGeneric(string url,Dictionary<string,string> header, string queryAndPath , out string _isError)
        {
            string ret = string.Empty;
            _isError = "0";
            GetCustomerInfoByCNICResponseError _error = new GetCustomerInfoByCNICResponseError();

            try
            {

                //BO_TPE_CONFIGURATION l_sslEnabled = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "SSL_ENABLED");

                string l_sslEnabled = ConfigurationManager.AppSettings["SSL_ENABLED"].ToString();

                AuditTrace.Notify("TPE.Raast.WRJGetCustomerInfoByCNIC", "l_sslEnabled:: " + l_sslEnabled);
                if (l_sslEnabled == "1")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else if (l_sslEnabled == "2")
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                else if (l_sslEnabled == "3")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }

                var client = new RestSharp.RestClient();
                Uri _url = new Uri(url);
                client.BaseUrl = _url;
                var request = new RestSharp.RestRequest();
                request.Method = 0;
                foreach (KeyValuePair<string, string> item in header)
                {
                    request.AddHeader(item.Key.ToString(), item.Value.ToString());
                }

                request.Resource = queryAndPath;
                
                var response = client.Execute(request) as RestSharp.RestResponse;
                if (response.ErrorException != null)
                {
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Exception:::", response.ErrorException);
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Message:::", response.ErrorMessage);
                }

                if (response != null && ((response.StatusCode == HttpStatusCode.OK) &&
                 (response.ResponseStatus == RestSharp.ResponseStatus.Completed)))
                {
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client Content :::", response.Content.ToString());
                    ret = response.Content.ToString();

                }
                else
                {
                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());

                    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.Content.ToString());
                    _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(response.Content.ToString());
                    string jsonError = new JavaScriptSerializer().Serialize(_error);
                    ret = jsonError;
                    _isError = "1";
                }

            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception Response :::", ex.Response);
                    AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception string :::", ex.ToString());

                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(data);
                        _isError = "1";
                        _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(sr.ReadToEnd());
                        string jsonError = new JavaScriptSerializer().Serialize(_error);
                        ret = jsonError;
                    }
                }
            }



            return ret;
        }

        public static string WrapperWebClient(string url, Dictionary<string, string> header, string queryAndPath, out string _isError)
        {
            string ret = string.Empty;
            _isError = "0";
            GetCustomerInfoByCNICResponseError _error = new GetCustomerInfoByCNICResponseError();

            try
            {
                string l_sslEnabled = ConfigurationManager.AppSettings["SSL_ENABLED"].ToString();
                AuditTrace.Notify("TPE.Raast.WRJGetCustomerInfoByCNIC", "l_sslEnabled:: " + l_sslEnabled);
                if (l_sslEnabled == "1")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }
                else if (l_sslEnabled == "2")
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                else if (l_sslEnabled == "3")
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }

                var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.ServicePoint.Expect100Continue = false;
                    webRequest.Timeout = 20000;
                    foreach (KeyValuePair<string, string> item in header)
                    {
                        webRequest.Headers.Add(item.Key.ToString(), item.Value.ToString());
                    }                    
                    webRequest.ContentType = "application/json";
                    webRequest.Accept = "application/json";                    
                }

                HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
                Stream resStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                ret = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                _isError = "1";
                using (WebResponse response = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(data);
                        _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(sr.ReadToEnd());
                        string jsonError = new JavaScriptSerializer().Serialize(_error);
                        ret = jsonError;
                    }
                }
            }
            return ret;
        }


        //public string PerformTransaction()
        //{
        //    string res = string.Empty;
        //    try
        //    {
        //        BO_TPE_CONFIGURATION l_urlCustomerInfoByCNIC = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "RAAST_URL_WRAPPER");
        //        string url = l_urlCustomerInfoByCNIC.CONFIG_VALUE;
        //        var re = Request;
        //        var headers = re.Headers;
        //        string tran_id = "123";
        //        string participent_iD = headers.GetValues("Participant-MemberID").First(); ;
        //        string cnic = headers.GetValues("Participant-MemberID").First(); ;
        //        string l_type = headers.GetValues("Participant-MemberID").First(); ;
        //        string token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiJwMnAifQ.-K5pO9SHSPYLgCJGigcNNpz8P1feAXtbjasz4zWIG2Y";
        //        string _isError = string.Empty;
        //        res = WRJGetCustomerInfoByCNIC_Client(url, tran_id, participent_iD, l_type, cnic, token, out _isError);
        //    }
        //    catch (Exception ex)
        //    {
        //        res = "Error";
        //    }
        //    return res;
        //}

        //public string PerformRaastTransaction()
        //{
        //    var re = Request;
        //    var headers = re.Headers;

        //    if (headers.Contains("Custom"))
        //    {
        //        string token = headers.GetValues("Custom").First();
        //    }
        //}

        //public string CallRaast(Dictionary<string, string> headers, string resource, string body,int method)
        //{

        //    var request = new RestSharp.RestRequest();
        //    //client.Timeout = 1;
        //    request.Method = method;
        //    foreach (Dictionary<string, string> a in headers)
        //    {
        //        request.AddHeader(a.Keys, a.Values);
        //    }

        //    request.Resource = resource;            
        //    var response = client.Execute(request) as RestSharp.RestResponse;
        //}

        //public static string WRJGetCustomerInfoByCNIC_Client()
        //{
        //    var re = Request;
        //    var headers = re.Headers;
        //    string ret = string.Empty;
        //    _isError = "0";
        //    GetCustomerInfoByCNICResponseError _error = new GetCustomerInfoByCNICResponseError();

        //    BO_TPE_CONFIGURATION l_urlCustomerInfoByCNICinitial = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "RAAST_URL_CUSTOMER_INFO_CNIC_WRAPPER");
        //    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC", "l_urlAliasTitleFetch.CONFIG_VALUE:: " + l_urlCustomerInfoByCNICinitial.CONFIG_VALUE);

        //    AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC", "WRJGetCustomerInfoByCNIC_Client token:: " + _token);



        //    string _urlCustomerInfoCNIC = l_urlCustomerInfoByCNICinitial.CONFIG_VALUE;

        //    try
        //    {

        //        BO_TPE_CONFIGURATION l_sslEnabled = BO_TPE_CONFIGURATION.GetObjectByID<BO_TPE_CONFIGURATION>(string.Empty, "SSL_ENABLED");
        //        AuditTrace.Notify("TPE.Raast.WRJGetCustomerInfoByCNIC", "l_sslEnabled:: " + l_sslEnabled.CONFIG_VALUE);
        //        if (l_sslEnabled.CONFIG_VALUE == "1")
        //        {
        //            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        //        }
        //        else if (l_sslEnabled.CONFIG_VALUE == "2")
        //        {
        //            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //        }
        //        else if (l_sslEnabled.CONFIG_VALUE == "3")
        //        {
        //            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        //            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //        }
        //        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //        // url = "https://apis1.hbl.com:8343/";

        //        var client = new RestSharp.RestClient();
        //        Uri _url = new Uri(url);
        //        client.BaseUrl = _url;
        //        var request = new RestSharp.RestRequest();
        //        //client.Timeout = 1;
        //        request.Method = 0;
        //        request.AddHeader("Participant-MemberID", headers.GetValues("Participant-MemberID").First());
        //        request.AddHeader("X-Request-ID", headers.GetValues("X-Request-ID").First());
        //        request.AddHeader("ContentType", "application/json");
        //        request.AddHeader("accept", "application/json");
        //        request.AddHeader("Authorization", headers.GetValues("Authorization").First());
        //        request.AddHeader("Charset", "UTF-8");


        //        request.Resource = _urlCustomerInfoCNIC + _type + "&uidValue=" + _cnic;
        //        AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WebRequest Data:::", request.ToString());
        //        AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client _type:::", _type);
        //        AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client _cnic:::", _cnic);

        //        var response = client.Execute(request) as RestSharp.RestResponse;
        //        if (response.ErrorException != null)
        //        {
        //            AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Exception:::", response.ErrorException);
        //            AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC| Error Message:::", response.ErrorMessage);
        //        }

        //        if (response != null && ((response.StatusCode == HttpStatusCode.OK) &&
        //         (response.ResponseStatus == RestSharp.ResponseStatus.Completed)))
        //        {
        //            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());
        //            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client Content :::", response.Content.ToString());
        //            ret = response.Content.ToString();

        //        }
        //        else
        //        {
        //            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.StatusCode.ToString());

        //            AuditTrace.Notify("TPE.Raast.Raast_Custome_Info_CNIC| WRJGetCustomerInfoByCNIC_Client :::", response.Content.ToString());
        //            _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(response.Content.ToString());
        //            string jsonError = new JavaScriptSerializer().Serialize(_error);
        //            ret = jsonError;
        //            _isError = "1";
        //        }

        //    }
        //    catch (WebException ex)
        //    {
        //        using (WebResponse response = ex.Response)
        //        {
        //            AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception Response :::", ex.Response);
        //            AuditTrace.Exception("TPE.Raast.Raast_Custome_Info_CNIC Eception string :::", ex.ToString());

        //            var httpResponse = (HttpWebResponse)response;

        //            using (Stream data = response.GetResponseStream())
        //            {
        //                StreamReader sr = new StreamReader(data);
        //                _isError = "1";
        //                _error = new JavaScriptSerializer().Deserialize<GetCustomerInfoByCNICResponseError>(sr.ReadToEnd());
        //                string jsonError = new JavaScriptSerializer().Serialize(_error);
        //                ret = jsonError;
        //            }
        //        }
        //    }



        //    return ret;
        //}
    }

    public class GetCustomerInfoByCNICResponseError
    {
        public string errorCode { get; set; }
        public string description { get; set; }
    }

    public enum Method
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7,
        COPY = 8
    }
}