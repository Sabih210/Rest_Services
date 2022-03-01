//using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

using Microsoft.Extensions.Configuration;


using MySql.Data.MySqlClient;



public class DAO
{


    string constr = "server=127.0.0.1;user id=DESKTOP-VV4QA8G;password=root;port=3306;database=doc_store_management;";


    //This method is genrically used for select commands
    public DataTable execute_select(string _cmd)
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
    //This method is genrically used for DML commands
    public int execute_dml(string _cmd)
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

    public int execute_dml(string _documentcmd, string _topiccmd)
    {
        MySqlTransaction tr = null;
        MySqlConnection con = new MySqlConnection(constr);
        int _isDocInserted = 0;
        int _isTopicInserted = 0;
        int _isInserted = 0;
        try
        {
            
            MySqlCommand cmd = new MySqlCommand(_documentcmd);

            MySqlDataAdapter sda = new MySqlDataAdapter();

            cmd.Connection = con;
            cmd.Connection.Open();
            tr = cmd.Connection.BeginTransaction();
            


            _isDocInserted = cmd.ExecuteNonQuery();

            cmd = new MySqlCommand(_topiccmd);
            cmd.Connection = con;
            _isTopicInserted = cmd.ExecuteNonQuery();           
          
            tr.Commit();
            cmd.Connection.Close();

            if (_isTopicInserted==1 && _isDocInserted==1) {
                _isInserted = 1;
            }
            return _isInserted;
        }
        catch (MySqlException ex)
        {
            tr.Rollback();
            _isInserted = 0;
            return _isInserted;
        }
        finally
        {
            con.Close();
           
            
        }
    }






}
