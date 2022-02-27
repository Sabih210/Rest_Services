using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PITB_Service.Models;

namespace Rest_Services.Common
{
    public class Common
    {
        public int GetNextId(string _columnName,string _tableName) {
            DataTable dt = new DataTable();

            int _id = 0;
            string _riderId = string.Empty;
            dt = BB_DAO.execute_select(string.Format("SELECT "+_columnName+" ID FROM "+_tableName+""));
            if (dt.Rows.Count > 0)
            {
                dt = BB_DAO.execute_select(string.Format("SELECT  MAX(" + _columnName + ") + 1 ID FROM  " + _tableName + ""));

                _id = Convert.ToInt32(dt.Rows[0]["ID"]);

            }
            else
            {
                _id = 1;
            }
            return _id;
        }
        public bool CheckDataAlreadyExists(string _columnName, string _tableName,string _data)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
            string _riderId = string.Empty;
            dt = BB_DAO.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='"+ _data+"' and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }
           
            return _alreadyExists;
        }

        public bool CheckDocumentAlreadyExists(string _columnName, string _tableName, string _documentName,string _folderId)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
            string _riderId = string.Empty;
            dt = BB_DAO.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='" + _documentName + "' and folderid='"+ _folderId+ "'  and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }

            return _alreadyExists;
        }
        public bool CheckDataAlreadyExists(string _columnName, string _tableName, string _topicId, string _folderId,string _documentId)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
            string _riderId = string.Empty;
            dt = BB_DAO.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='" + _topicId + "' and folderid='" + _folderId + "' and documentid='" + _documentId + "'  and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }

            return _alreadyExists;
        }
    }
}