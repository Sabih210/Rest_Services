
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using PITB_Service.Models;
using Microsoft.Extensions.Configuration;

namespace ChallengeSpekit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocStoreManagementAPIController : ControllerBase
    {

        #region Common 
       Common.Common _commonFunctions = new Common.Common();
        BB_DAO _bbDao = new BB_DAO();
        #endregion


        [HttpGet]
        [Route("GetDocumentByTopic/{_topic}", Name = "GetDocumentByTopic")]
        public DocStoreManagementResponse GetDocumentByTopic(string _topic)
        {
            DataTable dSet_Docs = null;
            DocStoreManagementResponse _res = new DocStoreManagementResponse();
            string l_Qry_Chk_API = @"SELECT D.DOCUMENTID,D.DOCUMENT_NAME FROM DSM_FOLDER F INNER JOIN DSM_DOCUMENT D ON F.FOLDERID=D.FOLDERID
INNER JOIN DSM_TOPIC T ON D.DOCUMENTID=T.DOCUMENTID
WHERE UPPER(T.TOPIC) LIKE UPPER('%{0}%') AND F.IS_DELETED=0 AND D.IS_DELETED=0 AND T.IS_DELETED=0";
            dSet_Docs = _bbDao.execute_select(string.Format(l_Qry_Chk_API, _topic));
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
        [Route("GetFolder/{_folderId}", Name = "GetFolder")]
        public FolderResponse GetFolder(string _folderId)
        {
            DataTable dt_folders = null;
            string l_Qry_Chk_API = string.Empty;
            FolderResponse _res = new FolderResponse();

            if (!string.IsNullOrEmpty(_folderId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_FOLDER F WHERE FOLDERID='{0}' and IS_DELETED='0'";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_FOLDER F WHERE IS_DELETED='0'";


            dt_folders = _bbDao.execute_select(string.Format(l_Qry_Chk_API, _folderId));


            _res.ResponseCode = "0";
            List<Folder> _foldersList = new List<Folder>();

            for (int i = 0; i < dt_folders.Rows.Count; i++)
            {
                _foldersList.Add(new Folder { FolderID = dt_folders.Rows[i]["FOLDERID"].ToString(), FolderName = dt_folders.Rows[i]["FOLDER_NAME"].ToString() });
            }

            _res.Folders = _foldersList;
            return _res;

        }

        [HttpPost]
        [Route("PerformActionOnFolder/{_folderRquest}", Name = "PerformActionOnFolder")]
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
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _folderRquest._id));
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
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _folderRquest._id));
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
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API));
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
                _resCode = "Data Could not be update. Please contact to administrator.";
            }

            return _resCode;

        }

        #endregion

        #region Document
        [HttpGet]
        [Route("GetDocument/{_documentId}", Name = "GetDocument")]
        public DocumentResponse GetDocument(string _documentId)
        {
            DataTable dt_documents = null;
            string l_Qry_Chk_API = string.Empty;
            DocumentResponse _res = new DocumentResponse();

            if (!string.IsNullOrEmpty(_documentId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_DOCUMENT D WHERE DOCUMENTID ='{0}' and IS_DELETED=0";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_DOCUMENT D WHERE IS_DELETED='0'";


            dt_documents = _bbDao.execute_select(string.Format(l_Qry_Chk_API, _documentId));


            _res.ResponseCode = "0";
            List<Document> _documentsList = new List<Document>();

            for (int i = 0; i < dt_documents.Rows.Count; i++)
            {
                _documentsList.Add(new Document { DocumentID = dt_documents.Rows[i]["DOCUMENTID"].ToString(), DocumentName = dt_documents.Rows[i]["DOCUMENT_NAME"].ToString() });
            }

            _res.Documents = _documentsList;

            return _res;

        }

        [HttpPost]
        [Route("PerformActionOnDocument/{_documentRequest}", Name = "PerformActionOnDocument")]
        public string PerformActionOnDocument(DocumentRequest _documentRequest)
        {
            int _reslt = 0;
            string l_Qry_Chk_API = string.Empty;
            string _resCode = string.Empty;


            if (string.IsNullOrEmpty(_documentRequest._id) && string.IsNullOrEmpty(_documentRequest._action) && string.IsNullOrEmpty(_documentRequest._documentName) && string.IsNullOrEmpty(_documentRequest._folderid))
            {
                _resCode = "Define Criteria which action you want to perform or contact administrator.";
                return _resCode;

            }

            if (_documentRequest._action.ToString().ToUpper() == "UPDATE" && !string.IsNullOrEmpty(_documentRequest._id) && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("DOCUMENTID", "DSM_DOCUMENT", _documentRequest._id, _documentRequest._folderid);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_DOCUMENT D SET document_name='" + _documentRequest._documentName + "', updated_on=sysdate(),updated_by='API' WHERE DOCUMENTID='{0}' and IS_DELETED=0";
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _documentRequest._id));
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

            else if (_documentRequest._action.ToString().ToUpper() == "DELETE" && !string.IsNullOrEmpty(_documentRequest._id) && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("DOCUMENTID", "DSM_DOCUMENT", _documentRequest._id, _documentRequest._folderid);

                if (_alreadyExists)
                {
                    l_Qry_Chk_API = @"UPDATE DSM_DOCUMENT D SET is_deleted=1, updated_on=sysdate(),updated_by='API' WHERE DOCUMENTID='{0}' and folderid='{1}' and IS_DELETED=0";
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _documentRequest._id, _documentRequest._folderid));
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

            else if (_documentRequest._action.ToString().ToUpper() == "INSERT" && !string.IsNullOrEmpty(_documentRequest._action) && !string.IsNullOrEmpty(_documentRequest._documentName) && !string.IsNullOrEmpty(_documentRequest._folderid))
            {
                bool _alreadyExists = _commonFunctions.CheckDocumentAlreadyExists("UPPER(DOCUMENT_NAME)", "DSM_DOCUMENT", _documentRequest._documentName.ToUpper(), _documentRequest._folderid);

                if (!_alreadyExists)
                {
                    int _documentid = _commonFunctions.GetNextId("DOCUMENTID", "DSM_DOCUMENT");
                    l_Qry_Chk_API = @"insert into DSM_DOCUMENT (DOCUMENTID, FOLDERID, DOCUMENT_NAME, CREATED_ON, CREATED_BY, UPDATED_ON, UPDATED_BY, IS_DELETED)
        values (" + _documentid + "," + _documentRequest._folderid + ", '" + _documentRequest._documentName + "', SYSDATE(), 'API', SYSDATE(), 'API', 0);";
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API));
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

        #region Topic
        [HttpGet]
        [Route("GetTopic/{_topicId}", Name = "GetTopic")]
        public TopicResponse GetTopic(string _topicId)
        {
            DataTable dt_topics = null;
            string l_Qry_Chk_API = string.Empty;
            TopicResponse _res = new TopicResponse();

            if (!string.IsNullOrEmpty(_topicId))
                l_Qry_Chk_API = @"SELECT * FROM DSM_TOPIC T WHERE TOPICID ='{0}' and IS_DELETED=0";
            else
                l_Qry_Chk_API = @"SELECT * FROM DSM_TOPIC T WHERE IS_DELETED='0'";


            dt_topics = _bbDao.execute_select(string.Format(l_Qry_Chk_API, _topicId));


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
        [Route("PerformActionOnTopic/{_topicRequest}", Name = "PerformActionOnTopic")]
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
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _topicRequest._id));
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
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API, _topicRequest._id, _topicRequest._folderid));
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
        values (" + _topicid + "," + _topicRequest._folderid + ", " + _topicRequest._documentid + ", '" + _topicRequest._topicName + "', sysdate(), 'API', sysdate(), 'API', 0);";
                    _reslt = _bbDao.execute_dml(string.Format(l_Qry_Chk_API));
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
        [Route("_GetDate/{_topic}", Name = "_GetDate")]
        public DocStoreManagementResponse _GetDate(string _topic)
        {
            DataTable dSet_Docs = null;
            DocStoreManagementResponse _res = new DocStoreManagementResponse();
            string l_Qry_Chk_API = @"SELECT D.DOCUMENTID,D.DOCUMENT_NAME FROM DSM_FOLDER F INNER JOIN DSM_DOCUMENT D ON F.FOLDERID=D.FOLDERID
        INNER JOIN DSM_TOPIC T ON D.DOCUMENTID=T.DOCUMENTID
        WHERE UPPER(T.TOPIC) LIKE UPPER('%{0}%')";
            dSet_Docs = _bbDao.execute_select(string.Format(l_Qry_Chk_API, _topic));
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


    }
}