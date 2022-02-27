using PITB_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using Tracer;
using Newtonsoft.Json;
using System.Data;
using TPE.BO;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace PITB_Service.API
{
    public class NadraRequest : ApiController
    {
        [HttpGet]
        public string testAPI()
        {
            return DateTime.Now.ToString();
        }
        [HttpPost]
        public Transaction.ResponseData Inquiry(NadraRequestINQ.NadraRequestInq objRequest)
        {
            Transaction.ResponseData objResponse = new Models.Transaction.ResponseData();

            BO_TPE_NADRA_CHANNEL_TXN objTransaction = AddTransaction(objRequest);
            bool isRequestValid = false;
            string responseMessage = string.Empty;
            isRequestValid = DataValidation(objRequest, ref responseMessage);
            if (!isRequestValid)
            {
                AuditTrace.Exception("Channel authentication failed");
                objResponse.Status = ServiceConstants.TranStatus.Failed;
                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_REQUEST;
                objResponse.Response_Message = responseMessage;
                objTransaction.TPE_RESPONSE_CODE = objResponse.Response_Code;
                objTransaction.TPE_RESPONSE_MESSAGE = objResponse.Response_Message;
                objTransaction.Update();
                return objResponse;
            }
            //return responseMessage;
            bool isChannelValid = ValidateChannel(objRequest.channel, objRequest.password);
            if (!isChannelValid)
            {
                AuditTrace.Exception("Channel authentication failed");
                objResponse.Status = ServiceConstants.TranStatus.Failed;
                objResponse.Response_Code = ServiceConstants.ResponseCode.CHANNEL_AUTHENTICATION_FAILED;
                objResponse.Response_Message = ServiceConstants.ResponseMessage.CHANNEL_AUTHENTICATION_FAILED;
                objTransaction.TPE_RESPONSE_CODE = objResponse.Response_Code;
                objTransaction.TPE_RESPONSE_MESSAGE = objResponse.Response_Message;
                objTransaction.Update();
                return objResponse;
            }


            AuditTrace.Notify("***************Data recieved from PITB***************");

            string l_responseCode = ServiceConstants.ResponseCode.SUCCESS;
            string l_responseMessage = string.Empty;



            Transaction.ResponseMessage response = new Models.Transaction.ResponseMessage();
            Transaction.TransactionData data = new Models.Transaction.TransactionData();

            data.MESSAGEHEADER = new Models.Transaction.MessageHeader();
            data.MESSAGEHEADER.Channel = objRequest.channel;
            data.MESSAGEHEADER.SystemId = objRequest.channel;
            data.MESSAGEHEADER.SystemPassword = ServiceConstants.MessageHeader.SystemPassword;
            data.TRANSACTION_TYPE_NAME = ServiceConstants.TranType.BIOMETRIC_VERIFICATION;
            data.TRANSACTION_TYPE_TPE = ServiceConstants.TranType.BIOMETRIC_VERIFICATION;
            data.TPE_GUID = Guid.NewGuid().ToString();
            data.CNIC = objRequest.citizeN_NUMBER;
            data.Mobile_Number = objRequest.mobilE_NUMBER;
            data.BM_DEVICE_SERIAL = objRequest.bM_DEVICE_SERIAL;
            data.BM_DEVICE = objRequest.bM_DEVICE;
            data.BM_FORMAT = objRequest.bM_FORMAT;
            data.BM_ATTRIBUTE1 = objRequest.latitude;
            data.BM_ATTRIBUTE2 = objRequest.longitude;
            data.DEVICE_IMEI = objRequest.imei;
            //string bioXML = ConvertBioToXML(objRequest.BIOMETRIC_DATA_INFO);
            data.BIOMETRIC_DATA_INFO = new Transaction.BIOMETRIC_DATA_INFO();
            data.BIOMETRIC_DATA_INFO.BIOMETRIC_DETAILS = new List<Transaction.BIOMETRIC_DETAILS>();
            //foreach (PITB_Service.Models.BiometriC_DETAILS aaobj in objRequest.BIOMETRIC_DATA_INFO.biometriC_DETAILS)
            //{
            //    data.BIOMETRIC_DATA_INFO.BIOMETRIC_DETAILS.Add(new Transaction.BIOMETRIC_DETAILS { BIOMETRIC_DATA = aaobj.biometriC_DATA, BM_FINGER_INDEX = aaobj.bM_FINGER_INDEX });
            //}
            string xml = data.Serialize(objRequest.channel);

            try
            {
                response = data.ParseResponseTPE(data.PerformTPERequest(xml, objRequest.channel, string.Empty));
                objResponse.Response_Code = response.MessageBody.ResponseCode;
                objResponse.Response_Message = response.MessageBody.ResponseDescription;
                objResponse.TranID = response.MessageBody.TranID;
                if (objResponse.Response_Code == ServiceConstants.ResponseCode.SUCCESS)
                {

                }
            }
            catch (TimeoutException ex)
            {
                AuditTrace.Exception("PerformTransaction : Timeout from TPE ", ex.Message);
                objResponse.Status = ServiceConstants.TranStatus.Failed;
                objResponse.Response_Code = ServiceConstants.ResponseCode.TIME_OUT;
                objResponse.Response_Message = ServiceConstants.ResponseMessage.TIME_OUT;
            }

            return objResponse;
        }

        public string ConvertBioToXML(NadraRequestINQ.BIOMETRIC_DATA_INFO bioData)
        {
            string output = string.Empty;
            XmlSerializer xsSubmit = new XmlSerializer(typeof(NadraRequestINQ.BIOMETRIC_DATA_INFO));
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, bioData);
                output = sww.ToString();
            }

            var doc = XDocument.Parse(output);
            IEnumerable<XElement> emptyElements;

            emptyElements = from descendant in doc.Descendants()
                            where descendant.IsEmpty || string.IsNullOrWhiteSpace(descendant.Value)
                            select descendant;

            emptyElements.Remove();

            doc.Root.RemoveAttributes();

            return output = doc.ToString();
        }

        public bool ValidateChannel(string channel, string password)
        {
            List<BO_TPE_NADRA_CHANNEL> objChannel = BO_TPE_NADRA_CHANNEL.GetObjectsByCriteria<BO_TPE_NADRA_CHANNEL>(string.Empty,
                TPE.BO.Criteria.EqualsTo(BO_TPE_NADRA_CHANNEL.col_CHANNEL, channel));
            if (objChannel != null && objChannel.Count > 0)
            {
                if (objChannel[0].PASSWORD == password)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }

        }

        public BO_TPE_NADRA_CHANNEL_TXN AddTransaction(NadraRequestINQ.NadraRequestInq objRequest)
        {
            BO_TPE_NADRA_CHANNEL_TXN obj = new BO_TPE_NADRA_CHANNEL_TXN();
            obj.CHANNEL_CODE = objRequest.channel;
            obj.SERVICE_PVDR_TXN_ID = objRequest.rrn;
            obj.CITIZEN_NUMBER = objRequest.citizeN_NUMBER;
            obj.CONTACT_NUMBER = objRequest.contacT_NUMBER;
            obj.MOBILE_NUMBER = objRequest.mobilE_NUMBER;
            obj.LONGITUDE = objRequest.longitude;
            obj.LATITUDE = objRequest.latitude;
            obj.IMEI = objRequest.imei;
            obj.TEMPLATE_TYPE = objRequest.templatE_TYPE;
            obj.AREA_NAME = objRequest.areA_NAME;
            obj.CREATED_ON = DateTime.Now;
            obj.CREATED_BY = "SYSTEM";
            obj.UPDATED_ON = DateTime.Now;
            obj.UPDATED_BY = "SYSTEM";
            //obj.BIOMETRIC_DATA = 
            obj.Add();
            return obj;
        }


        public static bool DataValidation(NadraRequestINQ.NadraRequestInq data, ref string errString)
        {
            bool IsDataValid = true;


            ICollection<ValidationResult> _validate = null;
            if (!ValidateModelWithAnnotations(data, out _validate))
            {
                IsDataValid = false;
                errString = String.Join("\n", _validate.Select(o => o.ErrorMessage));
            }


            if (IsDataValid)
            {
                if (!ValidateModelWithAnnotations(data.channel, out _validate))
                {
                    IsDataValid = false;
                    errString = String.Join("\n", _validate.Select(o => o.ErrorMessage));
                }
            }
            return IsDataValid;
        }

        public static bool ValidateModelWithAnnotations<T>(T obj, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }

    }

    public class Plus_code
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }

    }
    public class Address_components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IList<string> types { get; set; }

    }
    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }

    }
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }
    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }
    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }
    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }

    }
    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }

    }
    public class Results
    {
        public IList<Address_components> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }

    }
    public class googleAPIResponse
    {
        public Plus_code plus_code { get; set; }
        public IList<Results> results { get; set; }
        public string status { get; set; }

    }


}