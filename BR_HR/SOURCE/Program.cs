using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServerAgent;
using System.Collections.Generic;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace BR_HR
{
    class Program
    {
        static void Main(string[] args)
        {
            oraConnet();
        }

        static void oraConnet()
        {
            try
            {
                //string _strCon = "Data Source=BR_HR;User id=PHARM_WMS;Password=PHARM_WMS1#;Integrated Security=no;";
                
                DataTable dtcon = GetConinfo();

                string _strCon = $@"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={dtcon.Rows[0]["HRDB_IP"] as string})(PORT={dtcon.Rows[0]["HRDB_PORT"] as string})))
                                    (CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=BREHR))); User Id = PHARM_WMS; Password = PHARM_WMS1#;";

                RemoteCall rc = new RemoteCall(dtcon.Rows[0]["SVRNAME"] as string
                                             , dtcon.Rows[0]["PROTOCOL"] as string
                                             , dtcon.Rows[0]["PORT"] as string
                                             , "SERVICE");

                string Date = dtcon.Rows[0]["DATE"] as string;

                string C_CD = dtcon.Rows[0]["COMPANY"] as string;

                OracleConnection s_con = new OracleConnection(_strCon);
                s_con.Open();

                OracleCommand s_cmd = new OracleCommand();
                s_cmd.Connection = s_con;

                string Cmd = string.Empty;

                Cmd = "";
                //Cmd = Cmd + Environment.NewLine + "SELECT DISTINCT ORG_ID, ORG_NM FROM BORYUNGEHR.VW_PHARM_WMS_INSA_NEW WHERE 1=1 AND ORG_ID = '75120'";
                Cmd = Cmd + Environment.NewLine + "SELECT C_CD AS COMPANY_CODE";
                Cmd = Cmd + Environment.NewLine + "     , COMP_NM AS COMPANY_NAME";
                Cmd = Cmd + Environment.NewLine + "     , EMP_ID AS EMPNO";
                Cmd = Cmd + Environment.NewLine + "     , EMP_NM AS USER_NAME";
                Cmd = Cmd + Environment.NewLine + "     , ISUSE AS USEYN";
                Cmd = Cmd + Environment.NewLine + "     , ORG_ID AS DEPT_CODE";
                Cmd = Cmd + Environment.NewLine + "     , ORG_NM AS DEPT_NAME";
                Cmd = Cmd + Environment.NewLine + "     , CASE WHEN ISUSE = 'N' THEN TO_CHAR(MOD_YMDHMS, 'YYYY-MM-DD HH:MM:SS')";
                Cmd = Cmd + Environment.NewLine + "       ELSE NULL END AS RETIRE_DATE";
                Cmd = Cmd + Environment.NewLine + "     , GRADE_CD AS POS_CODE";
                Cmd = Cmd + Environment.NewLine + "     , GRADE_NM AS POS_NAME";
                Cmd = Cmd + Environment.NewLine + "     , MAIL_ADDR AS EMAIL";
                Cmd = Cmd + Environment.NewLine + "  FROM BORYUNGEHR.VW_PHARM_WMS_INSA";
                Cmd = Cmd + Environment.NewLine + " WHERE 1=1";

                if (Date != "0")
                {
                    Cmd = Cmd + Environment.NewLine + "   AND MOD_YMDHMS >= SYSDATE" + Date;
                }
                Cmd = Cmd + Environment.NewLine + "   AND C_CD = '" + C_CD + "'";
                Debug.WriteLine(Cmd);

                //using (OracleDataAdapter adapter = new OracleDataAdapter(Cmd, s_con))
                //{
                //    DataSet dsSet = new DataSet();
                //    adapter.Fill(dsSet);
                //}

                s_cmd.CommandText = Cmd;
                OracleDataReader s_dr = s_cmd.ExecuteReader();
                
                DataSet dsin = new DataSet();
                DataTable dsinD = new DataTable("INDATA");

                dsinD.Columns.Add("ROWNO", typeof(int));
                dsinD.Columns.Add("COMPANY_CODE", typeof(string));
                dsinD.Columns.Add("COMPANY_NAME", typeof(string));
                dsinD.Columns.Add("DEPT_CODE", typeof(string));
                dsinD.Columns.Add("DEPT_NAME", typeof(string));
                dsinD.Columns.Add("USERID", typeof(string));
                dsinD.Columns.Add("USER_NAME", typeof(string));
                dsinD.Columns.Add("EMPNO", typeof(string));
                dsinD.Columns.Add("EMAIL", typeof(string));
                dsinD.Columns.Add("MOBILE_NUMBER", typeof(string));
                dsinD.Columns.Add("POS_CODE", typeof(string));
                dsinD.Columns.Add("POS_NAME", typeof(string));
                dsinD.Columns.Add("DUTY_CODE", typeof(string));
                dsinD.Columns.Add("DUTY_NAME", typeof(string));
                dsinD.Columns.Add("USEYN", typeof(string));
                dsinD.Columns.Add("RETIRE_DATE", typeof(string));

                int s1 =0;

                while (s_dr.Read())
                {
                    DataRow Dr = dsinD.NewRow();
                    Dr["COMPANY_CODE"] = s_dr["COMPANY_CODE"] as string;
                    Dr["COMPANY_NAME"] = s_dr["COMPANY_NAME"] as string;
                    Dr["DEPT_CODE"] = s_dr["DEPT_CODE"] as string;
                    Dr["DEPT_NAME"] = s_dr["DEPT_NAME"] as string;
                    Dr["USERID"] = s_dr["EMPNO"] as string;
                    Dr["USER_NAME"] = s_dr["USER_NAME"] as string;
                    Dr["EMPNO"] = s_dr["EMPNO"] as string;
                    Dr["EMAIL"] = s_dr["EMAIL"] as string;
                    Dr["MOBILE_NUMBER"] = null;
                    Dr["POS_CODE"] = s_dr["POS_CODE"] as string;
                    Dr["POS_NAME"] = s_dr["POS_NAME"] as string;
                    Dr["DUTY_CODE"] = null;
                    Dr["DUTY_NAME"] = null;
                    Dr["USEYN"] = s_dr["USEYN"] as string;
                    Dr["RETIRE_DATE"] = s_dr["RETIRE_DATE"] as string;
                    Dr["ROWNO"] = s1;
                    dsinD.Rows.Add(Dr);
                    s1++;
                }

                dsin.Tables.Add(dsinD);

                rc.ExecuteService("BR_BRS_REG_GW_IF_EMPLOYEE", dsin, "INDATA", "");


                WriteLog("BR_BRS_REG_GW_IF_EMPLOYEE", "Success");
                
                

                s_con.Close();
            }
            catch (Exception ex)
            {
                WriteLog("BR_BRS_REG_GW_IF_EMPLOYEE", ex.Message);

            }

        }

        static DataTable GetConinfo()
        {
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml("Connectioninfos.txt");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ds.Tables[0];
        }

        static void WriteLog(string svcid, string msg)
        {
            Console.WriteLine(msg);

            try
            {
                string src = "WMSTEST";
                if (!EventLog.SourceExists(src)) EventLog.CreateEventSource(src, "BatchJob WMS Log");

                EventLog aLog = new EventLog();
                aLog.Source = src;
                aLog.WriteEntry(msg, EventLogEntryType.Information);
            }
            catch
            {
            }
        }
    }
}
