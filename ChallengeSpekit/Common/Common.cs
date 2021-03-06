using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ChallengeSpekit.Models;

namespace ChallengeSpekit.Common
{
    public class Common
    {
        DAO _bbDao = new DAO();

        //This method will get the next id from provided table
        public int GetNextId(string _columnName,string _tableName) {
            DataTable dt = new DataTable();
             DAO _bbDao = new DAO();
            int _id = 0;
           
            dt = _bbDao.execute_select(string.Format("SELECT "+_columnName+" ID FROM "+_tableName+""));
            if (dt.Rows.Count > 0)
            {
                dt = _bbDao.execute_select(string.Format("SELECT  MAX(" + _columnName + ") + 1 ID FROM  " + _tableName + ""));

                _id = Convert.ToInt32(dt.Rows[0]["ID"]);

            }
            else
            {
                _id = 1;
            }
            return _id;
        }
        //This method will check if data is already exists
        public bool CheckDataAlreadyExists(string _columnName, string _tableName,string _data)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
          
            dt = _bbDao.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='"+ _data+"' and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }
           
            return _alreadyExists;
        }
        //This method will check if document is already exists
        public bool CheckDocumentAlreadyExists(string _columnName, string _tableName, string _documentName,string _folderId)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
        
            dt = _bbDao.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='" + _documentName + "' and folderid='"+ _folderId+ "'  and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }

            return _alreadyExists;
        }
        //This method will also check if data is already exists but in provided table having topic,folder and document ids
        public bool CheckDataAlreadyExists(string _columnName, string _tableName, string _topicId, string _folderId,string _documentId)
        {
            DataTable dt = new DataTable();

            bool _alreadyExists = false;
         
            dt = _bbDao.execute_select(string.Format("SELECT *  FROM " + _tableName + " WHERE " + _columnName + "='" + _topicId + "' and folderid='" + _folderId + "' and documentid='" + _documentId + "'  and is_deleted=0"));
            if (dt.Rows.Count > 0)
            {
                _alreadyExists = true;

            }

            return _alreadyExists;
        }
    }
}