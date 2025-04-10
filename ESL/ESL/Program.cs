using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServerAgent;
using System.IO;

namespace ESL
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                Console.WriteLine("[Usage] ESL <svcid>");
                Environment.Exit(1);
            }

            string svcid = args[0];

            DataTable dtCon = GetConinfo();
            
            string Location = dtCon.Rows[0]["LOCATION"] as string;

            try
            {
                DataSet dsin = GetDataFromXMLfile(svcid, "IN");
                DataSet dsOut = GetDataFromXMLfile(svcid, "OUT");

                RemoteCall rc = new RemoteCall(dtCon.Rows[0]["SVRNAME"] as string
                                              , dtCon.Rows[0]["PROTOCOL"] as string
                                              , dtCon.Rows[0]["PORT"] as string
                                              , "SERVICE");

                DateTime starttime = DateTime.Now;
                WriteLog(svcid, string.Format("[{0:s}.{1:D3}] Start [{2}] service", starttime, starttime.Millisecond, svcid));

                try
                {
                    // 비즈룰 실행
                    dsOut = rc.ExecuteService(svcid, dsin, GetDtName(dsin), GetDtName(dsOut));

                    // XML 파일 생성
                    FileCreate(dsOut, Location, svcid);

                }
                catch(Exception ex)
                {
                    if (ex.Data.Contains("TYPE") && ex.Data["TYPE"].ToString() == "USER")
                        WriteLog(svcid, ex.Message + Environment.NewLine + ex.Data["LOC"]);
                    else
                        WriteLog(svcid, ex.Message + Environment.NewLine + ex.Data["LOC"]+ Environment.NewLine+ ex.StackTrace);

                    Environment.Exit(3);
                }

                DateTime endtime = DateTime.Now;
                WriteLog(svcid, string.Format("[{0:s}.{1:D3}] End [{2}] service", endtime, endtime.Millisecond, svcid));
            }
            catch (Exception ex)
            {
                WriteLog(svcid, ex.Message + Environment.NewLine);
            }
        }

        /// <summary>
        /// 서버 연결정보, 파일배포경로 등
        /// </summary>
        static DataTable GetConinfo()
        {
            DataSet ds = new DataSet();

            ds.ReadXml("Connectioninfo.txt");

            return ds.Tables[0];
        }
        /// <summary>
        /// 비즈룰 INDATA, OUTDATA 정보 조회
        /// </summary>
        static DataSet GetDataFromXMLfile(string svcid, string prefix)
        {
            DataSet ds = new DataSet();

            try
            {
                ds.ReadXml(svcid + @"\"+ prefix + "_PARA.txt");
            }
            catch(Exception ex)
            {
                WriteLog(svcid, ex.Message + Environment.NewLine);
            }

            return ds;
        }

        static string GetDtName(DataSet ds)
        {
            string dtNm = string.Empty;

            for(int i= 0; ds != null && i<ds.Tables.Count; i++)
            {
                dtNm += ds.Tables[i].TableName + ((i == ds.Tables.Count-1) ? "" : ",");
            }

            return dtNm;
        }

        /// <summary>
        /// CSV 파일 생성
        /// </summary>
        static void FileCreate(DataSet dsOUt, string FileLocation, string svcid)
        {
            try
            {
                foreach (DataTable dt in dsOUt.Tables)
                {
                    string Save_FileName = "import_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    StreamWriter sw = new StreamWriter(@"" + FileLocation + Save_FileName + ".csv", false, Encoding.UTF8);

                    // HEADER
                    for (int i = 0; i < dsOUt.Tables[0].Columns.Count; i++)
                    {
                        WriteItem(sw, dsOUt.Tables[0].Columns[i].ColumnName, true);
                        if (i < dsOUt.Tables[0].Columns.Count - 1)
                            sw.Write(",");
                    }
                    sw.Write("\r\n");

                    // ROW
                    foreach (DataRow dr in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            WriteItem(sw, dr[i]);
                            if (i < dt.Columns.Count-1)
                                sw.Write(",");
                        }
                        sw.Write("\r\n");
                    }

                    sw.Flush();
                    sw.Close();
                }

                WriteLog(svcid, "Success");
            }
            catch(Exception ex)
            {
                WriteLog(svcid, "Error :"+ ex.Message);
            }
        }
        /// <summary>
        /// csv 파일 내용작성
        /// </summary>
        static void WriteItem(TextWriter tw, object item, bool isHeader = false)
        {
            if (item == null)
                return;

            string s = item.ToString();

            //if (s.IndexOfAny("\",Wx0AWx0D".ToCharArray()) > -1)
            //    tw.Write(s.Replace("\"", "\"\"").ToLower());
            //else
            //    tw.Write(s.Replace("\"", "").ToLower());

            if(isHeader)
                tw.Write(string.Format("{0}", s.ToLower()));
            else
                tw.Write(string.Format("\"{0}\"", s));

            tw.Flush();
        }

        /// <summary>
        /// 이벤트 로그 기록
        /// </summary>
        static void WriteLog(string svcid, string msg)
        {
            System.IO.DirectoryInfo drtif = new DirectoryInfo(svcid + @"\");

            if (drtif.Exists != true)
                drtif.Create();

            string src = "ESL";

            if (!System.Diagnostics.EventLog.SourceExists(src))
                System.Diagnostics.EventLog.CreateEventSource(src, "MEStoESL BATCH Log");

            System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
            log.Source = src;
            log.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Information);
        }

    }
}
