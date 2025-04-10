using System;
using System.Data;
using System.Data.OracleClient;
using System.Diagnostics;

namespace Oracle
{
    public class OracleConnetor
    {
        public void INSToOracleDB(DataSet ds, string inTableName, string outTableName)
        {
            string FunctionName = nameof(INSToOracleDB);
            string connectionString = string.Format("Data Source={0};User id={1};Password={2};Integrated Security=no;", (object)ds.Tables["INDATA"].Rows[0]["DATASOURCE"].ToString(), (object)ds.Tables["INDATA"].Rows[0]["USERID"].ToString(), (object)ds.Tables["INDATA"].Rows[0]["PASSWORD"].ToString());
            using (OracleConnection oracleConnection = new OracleConnection(connectionString))
            {
                try
                {
                    oracleConnection.Open();
                    OracleCommand oracleCommand = new OracleCommand(ds.Tables["INDATA"].Rows[0]["QUERY"].ToString());
                    oracleCommand.Connection = oracleConnection;
                    OracleConnetor.WriteLog(FunctionName, string.Format("Start\n연결정보 : {0}\n쿼리 : {1}", (object)connectionString, (object)ds.Tables["INDATA"].Rows[0]["QUERY"].ToString()));
                    int num = oracleCommand.ExecuteNonQuery();
                    OracleConnetor.WriteLog(FunctionName, string.Format("End\n영향받은 행의 수 : {0} ", (object)num));
                    oracleConnection.Close();
                }
                catch (Exception ex)
                {
                    OracleConnetor.WriteLog(FunctionName, ex.Message);
                }
            }
        }

        public DataSet SELToOracleDB(DataSet ds, string inTableName, string outTableName)
        {
            string FunctionName = nameof(SELToOracleDB);
            string connectionString = string.Format("Data Source={0};User id={1};Password={2};Integrated Security=no;", (object)ds.Tables["INDATA"].Rows[0]["DATASOURCE"].ToString(), (object)ds.Tables["INDATA"].Rows[0]["USERID"].ToString(), (object)ds.Tables["INDATA"].Rows[0]["PASSWORD"].ToString());
            DataTable table = ds.Tables["OUTDATA"];
            table.Rows.Clear();
            using (OracleConnection oracleConnection = new OracleConnection(connectionString))
            {
                try
                {
                    oracleConnection.Open();
                    OracleCommand oracleCommand = new OracleCommand(ds.Tables["INDATA"].Rows[0]["QUERY"].ToString());
                    oracleCommand.Connection = oracleConnection;
                    OracleConnetor.WriteLog(FunctionName, string.Format("Start\n연결정보 : {0}\n쿼리 : {1}", (object)connectionString, (object)ds.Tables["INDATA"].Rows[0]["QUERY"].ToString()));
                    OracleDataReader oracleDataReader = oracleCommand.ExecuteReader();
                    while (oracleDataReader.Read())
                    {
                        DataRow row = table.NewRow();
                        for (int index = 0; index < table.Columns.Count; ++index)
                            row[index] = (object)(oracleDataReader[index] as string);
                        table.Rows.Add(row);
                    }
                    oracleConnection.Close();
                }
                catch (Exception ex)
                {
                    OracleConnetor.WriteLog(FunctionName, ex.Message);
                }
                return ds;
            }
        }

        private static void WriteLog(string FunctionName, string msg)
        {
            string message = string.Format("[{0}]{1}", (object)FunctionName, (object)msg);
            Console.WriteLine(message);
            try
            {
                using (EventLog eventLog = new EventLog())
                {
                    if (!EventLog.SourceExists("OracleConnetor"))
                        EventLog.CreateEventSource("OracleConnetor", nameof(OracleConnetor));
                    eventLog.Source = nameof(OracleConnetor);
                    eventLog.WriteEntry(message);
                    eventLog.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
