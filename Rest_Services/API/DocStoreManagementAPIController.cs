using PITB_Service.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Data;

namespace PITB_Service.API
{
    public class DocStoreManagementAPIController : ApiController
    {

        #region Common 
        Rest_Services.Common.Common _commonFunctions = new Rest_Services.Common.Common();      

        #endregion


        [HttpGet]
        public DocStoreManagementResponse GetDocumentByTopic(string _topic)
        {
            DataTable dSet_Docs = null;
            DocStoreManagementResponse _res = new DocStoreManagementResponse();
            string l_Qry_Chk_API = @"SELECT D.DOCUMENTID,D.DOCUMENT_NAME FROM DSM_FOLDER F INNER JOIN DSM_DOCUMENT D ON F.FOLDERID=D.FOLDERID
INNER JOIN DSM_TOPIC T ON D.DOCUMENTID=T.DOCUMENTID
WHERE UPPER(T.TOPIC) LIKE UPPER('%{0}%') AND F.IS_DELETED=0 AND D.IS_DELETED=0 AND T.IS_DELETED=0";
            dSet_Docs = BB_DAO.execute_select(string.Format(l_Qry_Chk_API, _topic));
            _res.ResponseCode = "0";
            List<string> _docs = new List<string>();
            foreach (DataRow _docName in dSet_Docs.Rows) {

                _docs.Add(_docName["DOCUMENT_NAME"].ToString());
            }

            _res.Document = _docs;
            return _res;

        }

        #region Folder
        [HttpGet]
        public FolderResponse GetFolder(string _folderId)
        {
            DataTable dt_folders = null;
            string l_Qry_Chk_API = string.Empty;
            FolderResponse _res = new FolderResponse();

            if(!string.IsNullOrEmpty(_folderId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_FOLDER F WHERE FOLDERID='{0}' and IS_DELETED='0'";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_FOLDER F WHERE IS_DELETED='0'";
           
            
            dt_folders = BB_DAO.execute_select(string.Format(l_Qry_Chk_API, _folderId));


            _res.ResponseCode = "0";
            List<Folder> _foldersList = new List<Folder>();
           
            for (int i=0;i< dt_folders.Rows.Count;i++)
            {               
                _foldersList.Add(new Folder { FolderID = dt_folders.Rows[i]["FOLDERID"].ToString(), FolderName = dt_folders.Rows[i]["FOLDER_NAME"].ToString() });
            }

            _res.Folders = _foldersList;
            return _res;

        }

        [HttpPost]
        public string PerformActionOnFolder(FolderRequest _folderRquest)
        {
            int _reslt = 0;
            string l_Qry_Chk_API = string.Empty;
            string _resCode = string.Empty;

            if (string.IsNullOrEmpty(_folderRquest._id) && string.IsNullOrEmpty(_folderRquest._action) && string.IsNullOrEmpty(_folderRquest._folderName))
            {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }


            if (!string.IsNullOrEmpty(_folderRquest._action) && _folderRquest._action.ToString().ToUpper() == "UPDATE" && !string.IsNullOrEmpty(_folderRquest._id) && !string.IsNullOrEmpty(_folderRquest._folderName))
            {
                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("FOLDERID", "DSM_FOLDER", _folderRquest._id);

                if (_alreadyExists)
                {

                    l_Qry_Chk_API = @"UPDATE DSM_FOLDER F SET folder_name='" + _folderRquest._folderName + "' , updated_on=sysdate(),updated_by='API'  WHERE FOLDERID='{0}' and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _folderRquest._id));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else
                {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }
            }
            else if (_folderRquest._action.ToString().ToUpper() == "DELETE" && !string.IsNullOrEmpty(_folderRquest._id) && string.IsNullOrEmpty(_folderRquest._folderName) && !string.IsNullOrEmpty(_folderRquest._action))
            {
                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("FOLDERID", "DSM_FOLDER", _folderRquest._id);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_FOLDER F SET is_deleted=1, updated_on=sysdate(),updated_by='API' WHERE FOLDERID='{0}' and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _folderRquest._id));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else
                {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }

            }
            else if (_folderRquest._action.ToString().ToUpper() == "INSERT" && !string.IsNullOrEmpty(_folderRquest._folderName) && !string.IsNullOrEmpty(_folderRquest._action))
            {

                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("UPPER(FOLDER_NAME)", "DSM_FOLDER", _folderRquest._folderName.ToUpper());

                if (!_alreadyExists)
                {
                    int _folderid = _commonFunctions.GetNextId("FOLDERID", "DSM_FOLDER");
                    l_Qry_Chk_API = @"insert into DSM_FOLDER (FOLDERID, FOLDER_NAME, CREATED_ON, CREATED_BY, UPDATED_ON, UPDATED_BY, IS_DELETED)
values (" + _folderid + ", '" + _folderRquest._folderName + "', sysdate(), 'API',sysdate(), 'API', 0)";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }

                }
                else
                {
                    _resCode = "Data Already Exists.";
                }
            }
            else {
                _resCode = "Data Could not be update. Please contact to administrator.";
            }

            return _resCode;

        }

        #endregion

        #region Document
        [HttpGet]
        public DocumentResponse GetDocument(string _documentId)
        {
            DataTable dt_documents = null;
            string l_Qry_Chk_API = string.Empty;
            DocumentResponse _res = new DocumentResponse();

            if (!string.IsNullOrEmpty(_documentId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_DOCUMENT D WHERE DOCUMENTID ='{0}' and IS_DELETED=0";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_DOCUMENT D WHERE IS_DELETED='0'";


            dt_documents = BB_DAO.execute_select(string.Format(l_Qry_Chk_API, _documentId));


            _res.ResponseCode = "0";
            List<Document> _documentsList = new List<Document>();
           
            for (int i = 0; i < dt_documents.Rows.Count; i++)
            {
                _documentsList.Add(new Document { DocumentID = dt_documents.Rows[i]["DOCUMENTID"].ToString(),  DocumentName= dt_documents.Rows[i]["DOCUMENT_NAME"].ToString() });
            }

            _res.Documents = _documentsList;

            return _res;

        }

        [HttpPost]
        public string PerformActionOnDocument(DocumentRequest _documentRequest)
        {
            int _reslt = 0;
            string l_Qry_Chk_API = string.Empty;
            string _resCode = string.Empty;


            if (string.IsNullOrEmpty(_documentRequest._id) && string.IsNullOrEmpty(_documentRequest._action) && string.IsNullOrEmpty(_documentRequest._documentName) && string.IsNullOrEmpty(_documentRequest._folderid)) {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }

            if (_documentRequest._action.ToString().ToUpper() == "UPDATE" && !string.IsNullOrEmpty(_documentRequest._id) && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("DOCUMENTID", "DSM_DOCUMENT", _documentRequest._id, _documentRequest._folderid);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_DOCUMENT D SET document_name='" + _documentRequest._documentName + "', updated_on=sysdate(),updated_by='API' WHERE DOCUMENTID='{0}' and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _documentRequest._id));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }

            }

            else if (_documentRequest._action.ToString().ToUpper() == "DELETE" && !string.IsNullOrEmpty(_documentRequest._id) && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("DOCUMENTID", "DSM_DOCUMENT", _documentRequest._id, _documentRequest._folderid);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_DOCUMENT D SET is_deleted=1, updated_on=sysdate(),updated_by='API' WHERE DOCUMENTID='{0}' and folderid='{1}' and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _documentRequest._id, _documentRequest._folderid));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }else {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }

            }

            else if (_documentRequest._action.ToString().ToUpper() == "INSERT" && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName) && !string.IsNullOrEmpty(_documentRequest._folderid))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("UPPER(DOCUMENT_NAME)", "DSM_DOCUMENT", _documentRequest._documentName.ToUpper(), _documentRequest._folderid);

                if (!_alreadyExists)
                {
                    int _documentid = _commonFunctions.GetNextId("DOCUMENTID", "DSM_DOCUMENT");
                    l_Qry_Chk_API = @"insert into DSM_DOCUMENT (DOCUMENTID, FOLDERID, DOCUMENT_NAME, CREATED_ON, CREATED_BY, UPDATED_ON, UPDATED_BY, IS_DELETED)
values (" + _documentid + "," + _documentRequest._folderid + ", '" + _documentRequest._documentName + "', SYSDATE(), 'API', SYSDATE(), 'API', 0);";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else {
                    _resCode = "Data Already Exists.";
                }

            }
            else {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }


            return _resCode;

        }

        #endregion

        #region Topic
        [HttpGet]
        public TopicResponse GetTopic(string _topicId)
        {
            DataTable dt_topics = null;
            string l_Qry_Chk_API = string.Empty;
            TopicResponse _res = new TopicResponse();

            if (!string.IsNullOrEmpty(_topicId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_TOPIC T WHERE TOPICID ='{0}' and IS_DELETED=0";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_TOPIC T WHERE IS_DELETED='0'";


            dt_topics = BB_DAO.execute_select(string.Format(l_Qry_Chk_API, _topicId));


            _res.ResponseCode = "0";
            List<Topic> _topicsList = new List<Topic>();
           
            for (int i = 0; i < dt_topics.Rows.Count; i++)
            {
                _topicsList.Add(new Topic { TopicID = dt_topics.Rows[i]["TOPICID"].ToString(), TopicName = dt_topics.Rows[i]["TOPIC"].ToString() });
            }

            _res.Topics = _topicsList;

            return _res;

        }

        [HttpPost]
        public string PerformActionOnTopic(TopicRequest _topicRequest)
        {
            int _reslt = 0;
            string l_Qry_Chk_API = string.Empty;
            string _resCode = string.Empty;


            if (string.IsNullOrEmpty(_topicRequest._id) && string.IsNullOrEmpty(_topicRequest._action) && string.IsNullOrEmpty(_topicRequest._topicName) && string.IsNullOrEmpty(_topicRequest._folderid) && string.IsNullOrEmpty(_topicRequest._documentid))
            {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }

            if (_topicRequest._action.ToString().ToUpper() == "UPDATE" && !string.IsNullOrEmpty(_topicRequest._id) && !string.IsNullOrEmpty(_topicRequest._action) && !string.IsNullOrEmpty(_topicRequest._topicName))
            {
                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("TOPICID", "DSM_TOPIC", _topicRequest._id, _topicRequest._folderid, _topicRequest._documentid);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_TOPIC T SET topic='" + _topicRequest._topicName + "', updated_on=sysdate(),updated_by='API' WHERE TOPICID='{0}' and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _topicRequest._id));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else
                {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }

            }

            else if (_topicRequest._action.ToString().ToUpper() == "DELETE" && !string.IsNullOrEmpty(_topicRequest._id) && !string.IsNullOrEmpty(_topicRequest._action) && !string.IsNullOrEmpty(_topicRequest._topicName))
            {
                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("TOPICID", "DSM_TOPIC", _topicRequest._id);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_TOPIC T SET is_deleted=1, updated_on=sysdate(),updated_by='API' WHERE TOPICID='{0}'  and IS_DELETED=0";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API, _topicRequest._id, _topicRequest._folderid));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else
                {
                    _resCode = "Data Could not be found. Please contact to administrator.";
                }

            }

            else if (_topicRequest._action.ToString().ToUpper() == "INSERT" && !string.IsNullOrEmpty(_topicRequest._action) && !string.IsNullOrEmpty(_topicRequest._topicName) && !string.IsNullOrEmpty(_topicRequest._folderid))
            {
                bool _alreadyExists = _commonFunctions.CheckDataAlreadyExists("UPPER(TOPIC)", "DSM_TOPIC", _topicRequest._topicName.ToUpper(), _topicRequest._folderid, _topicRequest._documentid);

                if (!_alreadyExists)
                {
                    int _topicid = _commonFunctions.GetNextId("TOPICID", "DSM_TOPIC");
                    l_Qry_Chk_API = @"insert into DSM_TOPIC (TOPICID, FOLDERID, DOCUMENTID, TOPIC, CREATED_ON, CREATED_BY, UPDATED_ON, UPDATED_BY, IS_DELETED)
values ("+ _topicid + ","+ _topicRequest._folderid+ ", "+ _topicRequest._documentid+ ", '"+ _topicRequest._topicName+ "', sysdate(), 'API', sysdate(), 'API', 0);";
                    _reslt = BB_DAO.execute_dml(string.Format(l_Qry_Chk_API));
                    if (_reslt == 1)
                    {
                        _resCode = "Success";
                    }
                    else
                    {
                        _resCode = "Data Could not be update. Please contact to administrator.";
                    }
                }
                else
                {
                    _resCode = "Data Already Exists.";
                }

            }
            else
            {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }


            return _resCode;

        }

        #endregion

        [HttpGet]
        public DocStoreManagementResponse _GetDate(string _topic)
        {
            DataTable dSet_Docs = null;
            DocStoreManagementResponse _res = new DocStoreManagementResponse();
            string l_Qry_Chk_API = @"SELECT D.DOCUMENTID,D.DOCUMENT_NAME FROM DSM_FOLDER F INNER JOIN DSM_DOCUMENT D ON F.FOLDERID=D.FOLDERID
INNER JOIN DSM_TOPIC T ON D.DOCUMENTID=T.DOCUMENTID
WHERE UPPER(T.TOPIC) LIKE UPPER('%{0}%')";
            dSet_Docs = BB_DAO.execute_select(string.Format(l_Qry_Chk_API, _topic));
            _res.ResponseCode = "0";
            List<string> _docs = new List<string>();
            foreach (DataRow _docName in dSet_Docs.Rows)
            {

                _docs.Add(_docName["DOCUMENT_NAME"].ToString());
            }

            _res.Document = _docs;
            return _res;

            //  return System.DateTime.Now.ToString();
        }

        //        [HttpPost]
        //        public Transaction.CDResponseData PerformTransaction(CD_RequestData objRequest)
        //        {
        //            Transaction.CDResponseData objResponse = new Models.Transaction.CDResponseData();


        //            if (ModelState.IsValid)
        //            {
        //                AuditTrace.Notify("CD_RequestData validated successfully.");
        //            }
        //            else
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Parameters;
        //                //  objResponse.Status = ServiceConstants.TranStatus.Failed;
        //                return objResponse;
        //            }
        //            string o_l_STATUS = string.Empty;
        //            string response_proc = string.Empty;

        //            objRequest.PROGRAM_CODE = objRequest.PROGRAM_CODE.ToUpper();
        //            objRequest.REF_FIELD_1 = objRequest.REF_FIELD_1.ToUpper();
        //            objRequest.REF_FIELD_2 = objRequest.REF_FIELD_2.ToUpper();
        //            objRequest.REF_FIELD_3 = objRequest.REF_FIELD_3.ToUpper();
        //            objRequest.REF_FIELD_4 = objRequest.REF_FIELD_4.ToUpper();
        //            objRequest.BANK_CODE = objRequest.BANK_CODE.ToUpper();
        //            AuditTrace.Notify("QUERY CorporateDisbursementAPIController: ", objRequest.PROGRAM_CODE + "," + objRequest.REF_FIELD_1 + "," + objRequest.REF_FIELD_2 + "," + objRequest.REF_FIELD_3 + "," +
        //              objRequest.REF_FIELD_4
        //                );

        //            bool isInserted = false;
        //            string log_id = string.Empty;
        //            string _company_Type = string.Empty;
        //            string _Type_Id = string.Empty;
        //            string l_Qry_CD_LOGS = @"SELECT BB_CD_LOG_S.NEXTVAL FROM DUAL ";
        //            string l_Qry_Chk_API = @"SELECT CDT.CD_NAME,CDT.CD_TYPE_ID TYPE_ID FROM BB_CD_TYPE_DETAIL TD,BB_CD_TYPE CDT
        //                                     WHERE TD.CD_TYPE_ID=CDT.CD_TYPE_ID 
        //                AND TD.CD_COMPANY_ID='{0}'";
        //            string l_Qry_Chk_BankInfo = @"SELECT * FROM CBI_BANK_INFO A
        //WHERE UPPER(A.BANK_ACCRONYM) = UPPER('{0}') AND IS_ENABLED='1'";

        //            DataSet dSet = null;
        //            DataSet dSet_Chk_API = null;
        //            AuditTrace.Notify("QUERY CorporateDisbursementAPIController: ", objRequest.COMPANY_ID + "," + objRequest.AMOUNT + "," + objRequest.BANK_CODE + "," + objRequest.CNIC);
        //            AuditTrace.Notify("QUERY CorporateDisbursementAPIController: ", string.Format(l_Qry_Chk_API, objRequest.COMPANY_ID));

        //            AuditTrace.Notify("QUERY CorporateDisbursementAPIController: ", l_Qry_CD_LOGS);
        //            dSet_Chk_API = BB_DAO.LoadResultFromBB(null, string.Format(l_Qry_Chk_API, objRequest.COMPANY_ID));
        //            if (dSet_Chk_API != null && dSet_Chk_API.Tables[0].Rows.Count > 0)
        //            {
        //                _company_Type = dSet_Chk_API.Tables[0].Rows[0]["CD_NAME"].ToString();
        //                _Type_Id = dSet_Chk_API.Tables[0].Rows[0]["TYPE_ID"].ToString();

        //                AuditTrace.Notify("CD Successfully CNIC validation check on : ", _company_Type);
        //            }

        //            response_proc = BB_DAO.SP_CD_IS_GL_FIELD_API(objRequest.COMPANY_ID,
        //             _Type_Id,//type_id
        //objRequest.PROGRAM_CODE,
        //objRequest.REF_FIELD_1,
        //objRequest.REF_FIELD_2,
        //objRequest.REF_FIELD_3,
        //objRequest.REF_FIELD_4,
        //SafeConvert.ToString(objRequest.AMOUNT),
        //objRequest.CNIC,
        //objRequest.Mobile_Number,
        //objRequest.CORE_ACCOUNT_NUMBER,
        //objRequest.IBAN_ACCOUNT_NUMBER,
        //objRequest.IS_IBFT,
        //out o_l_STATUS);


        //            if (response_proc == "True" && o_l_STATUS != "SUCCESS")
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = o_l_STATUS;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if ((!String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER) && objRequest.CORE_ACCOUNT_NUMBER.Length > 14)
        //                && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "0"))
        //            {
        //                string _ibanCheckResponse = string.Empty;
        //                string _retIBAN = string.Empty;
        //                _ibanCheckResponse = BB_DAO.CHECK_CORE_ACCOUNT_IBAN(objRequest.CORE_ACCOUNT_NUMBER,
        //out _retIBAN);
        //                AuditTrace.Notify("CD Successfully CORE_ACCOUNT_NUMBER validation check on return IBAN response : ", _ibanCheckResponse + "::IBAN Return::" + _retIBAN);

        //                if (objRequest.CORE_ACCOUNT_NUMBER != _retIBAN)
        //                {
        //                    objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                    objResponse.Response_Message = ServiceConstants.ResponseMessage.CoreAccountNumber;
        //                    string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                    isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                    return objResponse;
        //                }
        //                else
        //                {
        //                    objRequest.CORE_ACCOUNT_NUMBER = objRequest.CORE_ACCOUNT_NUMBER.Substring(objRequest.CORE_ACCOUNT_NUMBER.Trim().Length - 14);
        //                    AuditTrace.Notify("CD Successfully CORE_ACCOUNT_NUMBER validation check on : ", objRequest.CORE_ACCOUNT_NUMBER);
        //                }
        //            }


        //            AuditTrace.Notify("CD Successfully CNIC validation check on : ", _company_Type);

        //            dSet = BB_DAO.LoadResultFromBB(null, l_Qry_CD_LOGS);


        //            if (dSet != null && dSet.Tables[0].Rows.Count > 0)
        //            {
        //                log_id = dSet.Tables[0].Rows[0]["NEXTVAL"].ToString();
        //                AuditTrace.Notify("CD Successfully CNIC validation check on : ", objRequest.CNIC);
        //            }


        //            string l_INSERT_LOG = @"INSERT INTO BB_CD_LOG(ID,CHANNEL_CODE,TRAN_TYPE,CD_COMPANY_ID,CD_TYPE_ID,GUID,TRACE_NO,CNIC,CREATED_ON,UPDATED_ON,CREATED_BY,UPDATED_BY,REQUEST_MESSAGE,RESPONSE_MESSAGE)
        //VALUES(" + log_id + ",'API','CD_ACCOUNT_OPEN'," + objRequest.COMPANY_ID + "," + "2" + ",'" + objRequest.ConversationID + "'," + objRequest.ConversationID + "," + objRequest.CNIC + ",SYSDATE,SYSDATE,'API','API','" + JsonConvert.SerializeObject(objRequest).ToString() + "','" + JsonConvert.SerializeObject(objResponse).ToString() + "')";
        //            AuditTrace.Notify("***************Data recieved from CD PITB***************");

        //            isInserted = BB_DAO.InsertLogData(null, l_INSERT_LOG);


        //            string l_responseCode = ServiceConstants.ResponseCode.SUCCESS;
        //            string l_responseMessage = string.Empty;


        //            objResponse.ConversationID = objRequest.ConversationID;


        //            if ((!String.IsNullOrEmpty(objRequest.CNIC) && objRequest.CNIC.Length == 13) && objRequest.Mobile_Number.Length == 11)
        //            {
        //                AuditTrace.Notify("CD Successfully CNIC validation check on : ", objRequest.CNIC);
        //            }

        //            else if ((!String.IsNullOrEmpty(objRequest.CNIC) && objRequest.CNIC.Length == 13) && (!String.IsNullOrEmpty(objRequest.Mobile_Number) && (objRequest.Mobile_Number.Length < 11 || objRequest.Mobile_Number.Length > 11)))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.MobNO;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }

        //            else if (response_proc == "True" && o_l_STATUS != "SUCCESS" && String.IsNullOrEmpty(objRequest.CNIC))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.CNIC;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //                return objResponse;
        //            }


        //            // INVALID MOBILE NO - INVALID FORMAT
        //            if (!String.IsNullOrEmpty(objRequest.Mobile_Number) && objRequest.Mobile_Number.Length == 11
        //                && !System.Text.RegularExpressions.Regex.IsMatch(objRequest.Mobile_Number, "^03[0-9]{9}$"))

        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.mobno;
        //                return objResponse;

        //            }

        //            if (!String.IsNullOrEmpty(objRequest.CNIC) && objRequest.CNIC.Length == 13
        //               && !System.Text.RegularExpressions.Regex.IsMatch(objRequest.CNIC, "^[0-9]{13}$"))

        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.CNIC;
        //                return objResponse;

        //            }
        //            if (!String.IsNullOrEmpty(objRequest.ISSUANCE_DATE)
        //             && !System.Text.RegularExpressions.Regex.IsMatch(objRequest.ISSUANCE_DATE, "^(([0-9])|([0-2][0-9])|([3][0-1]))\\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\\-\\d{4}$"))

        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.IssuanceDate;
        //                return objResponse;

        //            }

        //            if (String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER) && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "0"))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.CoreAccountNumber;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }

        //            if (String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "1"))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.IbanNumber;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if (String.IsNullOrEmpty(objRequest.BANK_CODE) && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "1"))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.BankCode;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }

        //            if ((!String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && !String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER)) && objRequest.IS_IBFT == "1")
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.core_account_should_be_null;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if ((!String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && !String.IsNullOrEmpty(objRequest.BANK_CODE) && !String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER)) && objRequest.IS_IBFT == "1")
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.core_account_should_be_null;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if ((!String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && !String.IsNullOrEmpty(objRequest.BANK_CODE) && !String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER)) && objRequest.IS_IBFT == "0")
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Iban_should_be_null;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if ((!String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && !String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER)) && objRequest.IS_IBFT == "0")
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Iban_should_be_null;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }
        //            if (!String.IsNullOrEmpty(objRequest.IS_IBFT) && !System.Text.RegularExpressions.Regex.IsMatch(objRequest.IS_IBFT, "^[01]$"))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.IsIBFT;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }

        //            if ((!String.IsNullOrEmpty(objRequest.CORE_ACCOUNT_NUMBER) && objRequest.CORE_ACCOUNT_NUMBER.Length > 14)
        //                && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "0"))
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.CoreAccountNumber;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                return objResponse;
        //            }


        //            if (!String.IsNullOrEmpty(objRequest.BANK_CODE) && !String.IsNullOrEmpty(objRequest.IBAN_ACCOUNT_NUMBER) && (!String.IsNullOrEmpty(objRequest.IS_IBFT) && objRequest.IS_IBFT == "1"))
        //            {
        //                DataSet dSet_Chk_Bank_Info;
        //                string _min_Length = string.Empty;
        //                string _max_Length = string.Empty;
        //                string _regex = string.Empty;
        //                dSet_Chk_Bank_Info = BB_DAO.LoadResultFromBB(null, string.Format(l_Qry_Chk_BankInfo, objRequest.BANK_CODE));

        //                if (dSet_Chk_Bank_Info != null && dSet_Chk_Bank_Info.Tables[0].Rows.Count > 0)
        //                {
        //                    _min_Length = dSet_Chk_Bank_Info.Tables[0].Rows[0]["MIN_LENGTH"].ToString();
        //                    _max_Length = dSet_Chk_Bank_Info.Tables[0].Rows[0]["MAX_LENGTH"].ToString();
        //                    _regex = dSet_Chk_Bank_Info.Tables[0].Rows[0]["REGEX"].ToString();
        //                    AuditTrace.Notify("CD Successfully IBAN length check on : ", _min_Length + "::max" + _max_Length);
        //                }

        //                if (dSet_Chk_Bank_Info == null || dSet_Chk_Bank_Info.Tables[0].Rows.Count <= 0)
        //                {
        //                    objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                    objResponse.Response_Message = ServiceConstants.ResponseMessage.BankCode;
        //                    string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                    isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                    return objResponse;
        //                }
        //                if (System.Text.RegularExpressions.Regex.IsMatch(objRequest.IBAN_ACCOUNT_NUMBER, _regex))
        //                {
        //                    AuditTrace.Notify("CD Successfully IBAN_ACCOUNT_NUMBER validation check on : ", objRequest.IBAN_ACCOUNT_NUMBER);
        //                }

        //                else
        //                {
        //                    objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                    objResponse.Response_Message = ServiceConstants.ResponseMessage.IbanNumber;
        //                    string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                    isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);

        //                    return objResponse;
        //                }

        //            }


        //            if (objRequest.AMOUNT <= 0)
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Amount;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //                return objResponse;
        //            }
        //            if (!String.IsNullOrEmpty(SafeConvert.ToString(objRequest.AMOUNT)) && objRequest.AMOUNT != 0 && SafeConvert.ToString(objRequest.AMOUNT).Length < 16)
        //            {
        //                AuditTrace.Notify("Successfully Amount validation check on : ", objRequest.AMOUNT);
        //            }
        //            else
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Amount;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //                return objResponse;
        //            }

        //            //if (!String.IsNullOrEmpty(objRequest.ConversationID) && objRequest.Mobile_Number.Length == 11)
        //            //{
        //            //    AuditTrace.Notify("ConversationID:", objRequest.ConversationID);
        //            //}
        //            //else
        //            //{

        //            //    objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //            //    objResponse.Response_Message = ServiceConstants.ResponseMessage.TransactionID;
        //            //    string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //            //    isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //            //    return objResponse;
        //            //}

        //            AuditTrace.Notify("QUERY CorporateDisbursementAPIController: ", l_Qry_CD_LOGS);



        //            if (!String.IsNullOrEmpty(objRequest.PROGRAM_CODE))
        //            {
        //                AuditTrace.Notify("Program:", objRequest.PROGRAM_CODE);
        //            }
        //            else
        //            {
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.INVALID_DATA;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.Program;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //                return objResponse;
        //            }


        //            string _encryptPass = ServiceConstants.MessageHeader.APIPassword;

        //            Cryptography.Cipher _cip = new Cryptography.Cipher();

        //            string _decPass = _cip.Decrypt(_encryptPass);


        //            Transaction.ResponseMessage response = new Models.Transaction.ResponseMessage();
        //            Transaction.TransactionData data = new Models.Transaction.TransactionData();

        //            data.MESSAGEHEADER = new Models.Transaction.MessageHeader();
        //            data.MESSAGEHEADER.MessageType = ServiceConstants.MessageHeader.MessageType;
        //            data.MESSAGEHEADER.Channel = ServiceConstants.MessageHeader.Channel;
        //            data.MESSAGEHEADER.SystemId = ServiceConstants.MessageHeader.SystemId;
        //            data.MESSAGEHEADER.SystemPassword = _decPass;
        //            data.TRANSACTION_TYPE_NAME = ServiceConstants.TranType.CD_ACCOUNT_OPEN;
        //            data.TRANSACTION_TYPE_TPE = ServiceConstants.TranType.CD_ACCOUNT_OPEN;
        //            data.ACCESS_KEY = objRequest.COMPANY_ID;
        //            data.CNIC = objRequest.CNIC;
        //            data.Mobile_Number = objRequest.Mobile_Number;
        //            data.AMOUNT = SafeConvert.ToString(objRequest.AMOUNT);
        //            data.ISSUANCE_DATE = objRequest.ISSUANCE_DATE;
        //            data.PROGRAM_CODE = objRequest.PROGRAM_CODE.ToUpper();
        //            data.CORE_ACCOUNT_NUMBER = objRequest.CORE_ACCOUNT_NUMBER;
        //            data.BANK_CODE = objRequest.BANK_CODE;
        //            data.IBAN = objRequest.IBAN_ACCOUNT_NUMBER;
        //            data.IS_IBFT = objRequest.IS_IBFT;
        //            data.CONVERSATION_ID = objRequest.ConversationID;
        //            data.REF_FIELD_1 = objRequest.REF_FIELD_1.ToUpper();//Program Code 2
        //            data.REF_FIELD_2 = objRequest.REF_FIELD_2.ToUpper();//Program Code 3
        //            data.REF_FIELD_3 = objRequest.REF_FIELD_3.ToUpper();//Program Code 4
        //            data.REF_FIELD_4 = objRequest.REF_FIELD_4.ToUpper();//Program Code 5
        //            data.REF_FIELD_5 = objRequest.REF_FIELD_5;
        //            data.TPE_GUID = SafeConvert.ToString(Guid.NewGuid());
        //            data.COMPANY_ID = objRequest.COMPANY_ID;
        //            data.COMPANY_TYPE_ID = _Type_Id;


        //            string xml = data.Serialize();

        //            try
        //            {
        //                response = data.ParseResponseTPE(data.PerformTPERequest(xml));
        //                objResponse.Response_Code = response.MessageBody.ResponseCode;
        //                objResponse.Response_Message = response.MessageBody.ResponseDescription;
        //                objResponse.TranID = response.MessageBody.TranID;
        //                if (objResponse.Response_Code == ServiceConstants.ResponseCode.SUCCESS)
        //                {
        //                    string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                    isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //                }
        //                else
        //                {

        //                }
        //            }
        //            catch (TimeoutException ex)
        //            {
        //                AuditTrace.Exception("PerformTransaction : Timeout from TPE ", ex.Message);
        //                objResponse.Response_Code = ServiceConstants.ResponseCode.TIME_OUT;
        //                objResponse.Response_Message = ServiceConstants.ResponseMessage.TIME_OUT;
        //                string l_Update_LOG = @"UPDATE BB_CD_LOG SET UPDATED_ON=SYSDATE, RESPONSE_MESSAGE = '" + JsonConvert.SerializeObject(objResponse).ToString() + "' WHERE ID=" + log_id + "";
        //                isInserted = BB_DAO.InsertLogData(null, l_Update_LOG);
        //            }

        //            return objResponse;
        //        }
    }
}