//using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using TPE.BO;
using TPE.DAO;
using Tracer;
using MySql.Data.MySqlClient;


namespace PITB_Service.Models
{
    public static class BB_DAO
    {
        static string  constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;


        public static DataTable execute_select(string _cmd)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(_cmd))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }

            return dt;

        }

        public static int execute_dml(string _cmd)
        {
            int _isInserted = 0;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(_cmd))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {

                        cmd.Connection = con;
                        cmd.Connection.Open();
                        _isInserted = cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                      
                    }
                }
            }

            return _isInserted;

        }

        public static DataSet LoadResultFromBB(TPEDatabaseContext pBBDatabaseContext, string r_Cmnd)
        {
            int rowAffected = -1;
            string strMethodName = "LoadResult";
            IDatabase m_BBSystemDatabaseFactory = DatabaseFactory.CreateDatabase(ConfigurationManager.ConnectionStrings["BB_Connection"].ConnectionString);
            DbConnection connection = null;
            bool flag = false;
            DbConnection pConn = null;
            DataSet dSet = null;
            try
            {

                DbCommand cmd = m_BBSystemDatabaseFactory.Command;
                connection = m_BBSystemDatabaseFactory.Connection;
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;

                pConn = m_BBSystemDatabaseFactory.Connection;


                DbDataReader dbReader = null;
                if (pConn.State != ConnectionState.Open)
                {
                    pConn.Open();
                }
                else
                {
                    if (pBBDatabaseContext != null)
                    {
                        cmd.Transaction = pBBDatabaseContext.DbTransaction;
                    }
                }
                //  cmd.Connection = pConn;

                cmd.CommandText = r_Cmnd;

                //AuditLog.Log(3, ReplaceString(cmd.CommandText, cmd.Parameters), strMethodName, DateTime.Now);

                //dbReader = cmd.ExecuteReader();
                DataAdapter adapter = m_BBSystemDatabaseFactory.GetAdapter(cmd);
                dSet = new DataSet();
                adapter.Fill(dSet);


                if (dSet == null)
                {
                    AuditTrace.Notify(3, "No record Found.", strMethodName, DateTime.Now);
                    throw new Exception("No Record Found.");
                }
            }
            catch (Exception ex)
            {

                AuditTrace.Notify(1, "No record found:" + ex.Message, strMethodName, DateTime.Now);
                //ThrowException("Exception:" + ex.Message);
            }
            finally
            {
                if (pBBDatabaseContext == null)
                {
                    pConn.Dispose();
                    pConn.Close();
                }
            }
            return dSet;
        }

        public static bool InsertLogData(TPEDatabaseContext pBBDatabaseContext, string r_Cmnd)
        {
            bool isrowAffected = false;
            string strMethodName = "InsertLogData";
            IDatabase m_BBSystemDatabaseFactory = DatabaseFactory.CreateDatabase(Constants.WebConfiguration.Instance.BBSystemConnectionString);
            DbConnection connection = null;
            DbConnection pConn = null;

            try
            {

                DbCommand cmd = m_BBSystemDatabaseFactory.Command;
                connection = m_BBSystemDatabaseFactory.Connection;
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                pConn = m_BBSystemDatabaseFactory.Connection;
                if (pConn.State != ConnectionState.Open)
                {
                    pConn.Open();
                }
                else
                {
                    if (pBBDatabaseContext != null)
                    {
                        cmd.Transaction = pBBDatabaseContext.DbTransaction;
                    }
                }
                cmd.CommandText = r_Cmnd;
                isrowAffected = m_BBSystemDatabaseFactory.ExecuteNonQuery(cmd);

            }
            catch (Exception ex)
            {

                AuditTrace.Notify(1, "No record found:" + ex.Message, strMethodName, DateTime.Now);
                //ThrowException("Exception:" + ex.Message);
            }
            finally
            {
                if (pBBDatabaseContext == null)
                {
                    pConn.Dispose();
                    pConn.Close();
                }
            }
            return isrowAffected;
        }


        public static string SP_CD_IS_GL_FIELD_API(string P_COMPANY_ID,
string P_CD_TYPE_ID,
string P_GL_CODE_1,
string P_GL_CODE_2,
string P_GL_CODE_3,
string P_GL_CODE_4,
string P_GL_CODE_5,
string P_AMOUNT,
string P_CNIC,
string P_MOBILE_NUMBER,
string P_CORE_ACCOUNT_NUMBER,
string P_IBAN_ACCOUNT_NUMBER,
string P_IS_IBFT,
out string l_STATUS)
        {

            IDatabase m_BBSystemDatabaseFactory = DatabaseFactory.CreateDatabase(Constants.WebConfiguration.Instance.BBSystemConnectionString);
            DbConnection connection = null;

            using (new FuncTrace("TPE.KernelExtended", "CD_CheckAccountOpen"))
            {
                DbCommand cmd = m_BBSystemDatabaseFactory.Command;
                connection = m_BBSystemDatabaseFactory.Connection;
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "BB_GENERIC_CD.CD_IS_GL_FIELD_API";

                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_COMPANY_ID : ", P_COMPANY_ID);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_CD_TYPE_ID : ", P_CD_TYPE_ID);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_GL_CODE_1 : ", P_GL_CODE_1);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_GL_CODE_2 : ", P_GL_CODE_2);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_GL_CODE_3 : ", P_GL_CODE_3);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_GL_CODE_4 : ", P_GL_CODE_4);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_GL_CODE_5 : ", P_GL_CODE_5);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_AMOUNT : ", P_AMOUNT);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_CNIC : ", P_CNIC);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_MOBILE_NUMBER : ", P_MOBILE_NUMBER);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_CORE_ACCOUNT_NUMBER : ", P_CORE_ACCOUNT_NUMBER);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_IBAN_ACCOUNT_NUMBER : ", P_IBAN_ACCOUNT_NUMBER);
                AuditTrace.Notify("CD_IS_GL_FIELD_API | P_IS_IBFT : ", P_IS_IBFT);

                DbParameter prmCDTYPEID = m_BBSystemDatabaseFactory.GetParameter("P_CD_TYPE_ID", DbType.String);
                prmCDTYPEID.Value = SafeConvert.DbValue(P_CD_TYPE_ID);
                cmd.Parameters.Add(SafeConvert.DbValue(prmCDTYPEID));

                DbParameter prmCOMPANYID = m_BBSystemDatabaseFactory.GetParameter("P_COMPANY_ID", DbType.String);
                prmCOMPANYID.Value = SafeConvert.DbValue(P_COMPANY_ID);
                cmd.Parameters.Add(SafeConvert.DbValue(prmCOMPANYID));


                DbParameter prmGLCode1 = m_BBSystemDatabaseFactory.GetParameter("P_GL_CODE_1", DbType.String);
                prmGLCode1.Value = SafeConvert.DbValue(P_GL_CODE_1);
                cmd.Parameters.Add(SafeConvert.DbValue(prmGLCode1));

                DbParameter prmGLCode2 = m_BBSystemDatabaseFactory.GetParameter("P_GL_CODE_2", DbType.String);
                prmGLCode2.Value = SafeConvert.DbValue(P_GL_CODE_2);
                cmd.Parameters.Add(SafeConvert.DbValue(prmGLCode2));

                DbParameter prmGLCode3 = m_BBSystemDatabaseFactory.GetParameter("P_GL_CODE_3", DbType.String);
                prmGLCode3.Value = SafeConvert.DbValue(P_GL_CODE_3);
                cmd.Parameters.Add(SafeConvert.DbValue(prmGLCode3));

                DbParameter prmGLCode4 = m_BBSystemDatabaseFactory.GetParameter("P_GL_CODE_4", DbType.String);
                prmGLCode4.Value = SafeConvert.DbValue(P_GL_CODE_4);
                cmd.Parameters.Add(SafeConvert.DbValue(prmGLCode4));

                DbParameter prmGLCode5 = m_BBSystemDatabaseFactory.GetParameter("P_GL_CODE_5", DbType.String);
                prmGLCode5.Value = SafeConvert.DbValue(P_GL_CODE_5);
                cmd.Parameters.Add(SafeConvert.DbValue(prmGLCode5));

                DbParameter prmAmount = m_BBSystemDatabaseFactory.GetParameter("P_AMOUNT", DbType.String);
                prmAmount.Value = SafeConvert.DbValue(P_AMOUNT);
                cmd.Parameters.Add(SafeConvert.DbValue(prmAmount));

                DbParameter prmCNIC = m_BBSystemDatabaseFactory.GetParameter("P_CNIC", DbType.String);
                prmCNIC.Value = SafeConvert.DbValue(P_CNIC);
                cmd.Parameters.Add(SafeConvert.DbValue(prmCNIC));

                

                DbParameter prmMobileNumber = m_BBSystemDatabaseFactory.GetParameter("P_MOBILE_NUMBER", DbType.String);
                prmMobileNumber.Value = SafeConvert.DbValue(P_MOBILE_NUMBER);
                cmd.Parameters.Add(SafeConvert.DbValue(prmMobileNumber));

                DbParameter prmCoreAccountNumber = m_BBSystemDatabaseFactory.GetParameter("P_CORE_ACCOUNT_NUMBER", DbType.String);
                prmCoreAccountNumber.Value = SafeConvert.DbValue(P_CORE_ACCOUNT_NUMBER);
                cmd.Parameters.Add(SafeConvert.DbValue(prmCoreAccountNumber));

                DbParameter prmIBAN = m_BBSystemDatabaseFactory.GetParameter("P_IBAN_ACCOUNT_NUMBER", DbType.String);
                prmIBAN.Value = SafeConvert.DbValue(P_IBAN_ACCOUNT_NUMBER);
                cmd.Parameters.Add(SafeConvert.DbValue(prmIBAN));

                DbParameter prmIsIBFT = m_BBSystemDatabaseFactory.GetParameter("P_IS_IBFT", DbType.String);
                prmIsIBFT.Value = SafeConvert.DbValue(P_IS_IBFT);
                cmd.Parameters.Add(SafeConvert.DbValue(prmIsIBFT));

                DbParameter prmRetMessage = m_BBSystemDatabaseFactory.GetParameter("l_STATUS", DbType.String);
                prmRetMessage.Direction = ParameterDirection.Output;
                prmRetMessage.DbType = DbType.String;
                prmRetMessage.Size = 200;
                cmd.Parameters.Add(SafeConvert.DbValue(prmRetMessage));

                bool l_resp = false;
                try
                {
                    l_resp = m_BBSystemDatabaseFactory.ExecuteNonQuery(cmd);
                    AuditTrace.Notify("CD_IS_GL_FIELD_API | l_resp : ", l_resp);
                    AuditTrace.Notify("CD_IS_GL_FIELD_API | prmRetMessage : ", prmRetMessage.Value);
                  


                }
                catch (Exception ex)
                {
                    AuditTrace.Exception("CD_IS_GL_FIELD_API get Exception: ", ex.Message);
                    //  l_resCode = Constants.ResponseCode.INTERNAL_ERROR;
                }
                finally
                {
                    connection.Dispose();
                    connection.Close();
                    AuditTrace.Notify("CD_IS_GL_FIELD_API Finally Exits");
                }

                string resCode = string.Empty;
                resCode = SafeConvert.ToString(l_resp);
                l_STATUS = SafeConvert.ToString(prmRetMessage.Value);
             

                AuditTrace.Notify("CD_IS_GL_FIELD_API | RetValue : ", resCode);

                AuditTrace.Notify("CD_IS_GL_FIELD_API | O_GL_ACCOUNT : ", l_STATUS);


                AuditTrace.Notify("CD_IS_GL_FIELD_API | Exit ");
                return resCode;
            }
        }


        public static string CHECK_CORE_ACCOUNT_IBAN(string P_CORE_ACCOUNT_NUMBER,out string OP_CORE_ACCOUNT_NUMBER)
        {

            IDatabase m_BBSystemDatabaseFactory = DatabaseFactory.CreateDatabase(Constants.WebConfiguration.Instance.BBSystemConnectionString);
            DbConnection connection = null;
            string l_iban = string.Empty;
            using (new FuncTrace("TPE.KernelExtended", "CHECK_CORE_ACCOUNT_IBAN"))
            {
                DbCommand cmd = m_BBSystemDatabaseFactory.Command;
                connection = m_BBSystemDatabaseFactory.Connection;
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "BB_GENERIC_CD.F_GET_CHECK_CORE_IBAN";

                AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | P_COMPANY_ID : ", P_CORE_ACCOUNT_NUMBER);
               
                DbParameter prmCoreIban = m_BBSystemDatabaseFactory.GetParameter("P_CORE_ACCOUNT_NUMBER", DbType.String);
                prmCoreIban.Value = SafeConvert.DbValue(P_CORE_ACCOUNT_NUMBER);
                cmd.Parameters.Add(SafeConvert.DbValue(prmCoreIban));


                DbParameter prmRetIBAN = m_BBSystemDatabaseFactory.GetParameter("L_RET_IBAN", DbType.String);
                prmRetIBAN.Direction = ParameterDirection.Output;
                prmRetIBAN.DbType = DbType.String;
                prmRetIBAN.Size = 200;
                cmd.Parameters.Add(SafeConvert.DbValue(prmRetIBAN));

                bool l_resp = false;
                try
                {
                    l_resp = m_BBSystemDatabaseFactory.ExecuteNonQuery(cmd);
                    AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | l_resp : ", l_resp);
                    AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | prmRetMessage : ", prmRetIBAN.Value);



                }
                catch (Exception ex)
                {
                    AuditTrace.Exception("CHECK_CORE_ACCOUNT_IBAN get Exception: ", ex.Message);
                    //  l_resCode = Constants.ResponseCode.INTERNAL_ERROR;
                }
                finally
                {
                    connection.Dispose();
                    connection.Close();
                    AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN Finally Exits");
                }

                string resCode = string.Empty;
                resCode = Constants.ResponseCode.SUCCESS;
                l_iban = OP_CORE_ACCOUNT_NUMBER = SafeConvert.ToString(prmRetIBAN.Value);


                AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | RetValue : ", resCode);

                AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | l_iban : ", l_iban);


                AuditTrace.Notify("CHECK_CORE_ACCOUNT_IBAN | Exit ");
                return resCode;
            }
        }


    }
}