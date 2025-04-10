using C1.Silverlight.Excel;
using LGCNS.iPharmMES.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Board
{
    public class WorkerProcessStatusViewModel : ViewModelBase
    {
         #region property

        private WorkerProcessStatusView _mainWnd;

        private DateTime _PeriodSTDTTM;
        public DateTime PeriodSTDTTM
        {
            get { return _PeriodSTDTTM; }
            set
            {
                _PeriodSTDTTM = value;
                NotifyPropertyChanged();
            }
        }
        
        private DateTime _PeriodEDDTTM;
        public DateTime PeriodEDDTTM
        {
            get { return _PeriodEDDTTM; }
            set
            {
                _PeriodEDDTTM = value;
                NotifyPropertyChanged();
            }
        }
        
        private string _DEPTCODE;
        public string DEPTCODE
        {
            get { return _DEPTCODE; }
            set
            {
                _DEPTCODE = value;
                NotifyPropertyChanged();
            }
        }

        private string _DEPTNAME;
        public string DEPTNAME
        {
            get { return _DEPTNAME; }
            set
            {
                _DEPTNAME = value;
                NotifyPropertyChanged();
            }
        }

        private string _Worker;
        public string Worker
        {
            get{return _Worker;}
            set
            {
                _Worker = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Data

        private BR_PHR_GET_DEFAULT_DATE _BR_PHR_GET_DEFAULT_DATE;
        public BR_PHR_GET_DEFAULT_DATE BR_PHR_GET_DEFAULT_DATE
        {
            get { return _BR_PHR_GET_DEFAULT_DATE; }
            set
            {
                _BR_PHR_GET_DEFAULT_DATE = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_GET_PROD_DEPT _BR_PHR_GET_PROD_DEPT;
        public BR_PHR_GET_PROD_DEPT BR_PHR_GET_PROD_DEPT
        {
            get { return _BR_PHR_GET_PROD_DEPT; }
            set
            {
                _BR_PHR_GET_PROD_DEPT = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_RPT_Employee_WorkTime _BR_PHR_RPT_Employee_WorkTime;
        public BR_PHR_RPT_Employee_WorkTime BR_PHR_RPT_Employee_WorkTime
        {
            get { return _BR_PHR_RPT_Employee_WorkTime; }
            set
            {
                _BR_PHR_RPT_Employee_WorkTime = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_RPT_Employee_WorkTime_by_EmpNo _BR_PHR_RPT_Employee_WorkTime_by_EmpNo;
        public BR_PHR_RPT_Employee_WorkTime_by_EmpNo BR_PHR_RPT_Employee_WorkTime_by_EmpNo
        {
            get{return _BR_PHR_RPT_Employee_WorkTime_by_EmpNo;}
            set
            {
                _BR_PHR_RPT_Employee_WorkTime_by_EmpNo = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Command

        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///

                            if (arg == null || !(arg is WorkerProcessStatusView))
                                return;
                            _mainWnd = arg as WorkerProcessStatusView;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();
                            BR_PHR_GET_PROD_DEPT.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "작업자공수현황"
                            });

                            if (!await BR_PHR_GET_DEFAULT_DATE.Execute()) return;

                            PeriodSTDTTM = DateTime.Parse(BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(0, 4) + "-" +
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(4, 2) + "-" + 
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(6, 2));
                            PeriodEDDTTM = DateTime.Parse(BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(0, 4) + "-" +
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(4, 2) + "-" + 
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(6, 2));

                            if (!await BR_PHR_GET_PROD_DEPT.Execute()) return;
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        public ICommand BtnSearchCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BtnSearchCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["BtnSearchCommand"] = false;
                            CommandCanExecutes["BtnSearchCommand"] = false;

                            ///

                            BR_PHR_RPT_Employee_WorkTime.INDATAs.Clear();
                            BR_PHR_RPT_Employee_WorkTime.OUTDATAs.Clear();

                            BR_PHR_RPT_Employee_WorkTime.INDATAs.Add(new BR_PHR_RPT_Employee_WorkTime.INDATA()
                            {
                                FROMPRODDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TOPRODDATE      = PeriodEDDTTM.ToString("yyyy-MM-dd"),
                                PRODDEPTCODE    = DEPTCODE,
                                EMPNAME         = Worker,
                                EMPNO           = null
                            });

                            if (!await BR_PHR_RPT_Employee_WorkTime.Execute()) return;
                            ///

                            CommandResults["BtnSearchCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BtnSearchCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BtnSearchCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BtnSearchCommand") ?
                        CommandCanExecutes["BtnSearchCommand"] : (CommandCanExecutes["BtnSearchCommand"] = true);
                });
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelectionChangedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SelectionChangedCommand"] = false;
                            CommandCanExecutes["SelectionChangedCommand"] = false;

                            ///

                            if (arg == null || !(arg is BR_PHR_RPT_Employee_WorkTime.OUTDATA))
                                return;
                            var rowdata = arg as BR_PHR_RPT_Employee_WorkTime.OUTDATA;

                            BR_PHR_RPT_Employee_WorkTime_by_EmpNo.INDATAs.Clear();
                            BR_PHR_RPT_Employee_WorkTime_by_EmpNo.OUTDATAs.Clear();

                            BR_PHR_RPT_Employee_WorkTime_by_EmpNo.INDATAs.Add(new BR_PHR_RPT_Employee_WorkTime_by_EmpNo.INDATA()
                            {
                                EMPNO = rowdata.EMPNO,
                                FROMPRODDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TOPRODDATE      = PeriodEDDTTM.ToString("yyyy-MM-dd")
                            });

                            if (!await BR_PHR_RPT_Employee_WorkTime_by_EmpNo.Execute()) return;

                            ///

                            CommandResults["SelectionChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SelectionChangedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SelectionChangedCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelectionChangedCommand") ?
                        CommandCanExecutes["SelectionChangedCommand"] : (CommandCanExecutes["SelectionChangedCommand"] = true);
                });
            }
        }

        public ICommand ClickExportExcelCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickExportExcelCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickExportExcelCommand"] = false;
                            CommandCanExecutes["ClickExportExcelCommand"] = false;

                            ///

                            Custom_C1ExportExcel customExcel = new Custom_C1ExportExcel();

                            customExcel.SaveBook(book =>
                            {
                                book.Sheets.Add();
                                C1.Silverlight.Excel.XLSheet Firsheet = book.Sheets[0];
                                C1.Silverlight.Excel.XLSheet Secsheet = book.Sheets[1];

                                customExcel.InitMutiHeaderExcel(book, Firsheet, _mainWnd.dgWorkTime);
                                customExcel.InitMutiHeaderExcel(book, Secsheet, _mainWnd.dgDetail);
                            });
                            ///

                            CommandResults["ClickExportExcelCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickExportExcelCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickExportExcelCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickExportExcelCommand") ?
                        CommandCanExecutes["ClickExportExcelCommand"] : (CommandCanExecutes["ClickExportExcelCommand"] = true);
                });
            }
        }

        #endregion

        #region Constructor

        public WorkerProcessStatusViewModel()
        {
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
            _BR_PHR_GET_PROD_DEPT = new BR_PHR_GET_PROD_DEPT();
            _BR_PHR_RPT_Employee_WorkTime = new BR_PHR_RPT_Employee_WorkTime();
            _BR_PHR_RPT_Employee_WorkTime_by_EmpNo = new BR_PHR_RPT_Employee_WorkTime_by_EmpNo();
        }

        #endregion
    }

    public class Custom_C1ExportExcel
    {
        public void SaveBook(Action<C1XLBook> action)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Excel Files (*.xlsx) | *.xlsx";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var book = new C1XLBook();
                    if (action != null)
                    {
                        action(book);
                    }
                    using (var stream = dlg.OpenFile())
                    {
                        book.Save(stream);
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }
        }

        private void InitHeaderStyle(XLStyle Style)
        {
            Style.Font = new XLFont("Malgun Gothic", 12, true, false, Colors.Black);
            Style.BackColor = Colors.LightGray;
            Style.AlignHorz = XLAlignHorzEnum.Center;
            Style.AlignVert = XLAlignVertEnum.Center;
            Style.BorderTop = XLLineStyleEnum.Thin;
            Style.BorderRight = XLLineStyleEnum.Thin;
            Style.BorderBottom = XLLineStyleEnum.Thin;
            Style.BorderLeft = XLLineStyleEnum.Thin;
            Style.BorderColorBottom = Colors.Black;
            Style.BorderColorLeft = Colors.Black;
            Style.BorderColorTop = Colors.Black;
            Style.BorderColorRight = Colors.Black;
        }

        private void InitDataStyle(XLStyle Style)
        {
            Style.Font = new XLFont("Malgun Gothic", 12, false, false, Colors.Black);
            Style.AlignHorz = XLAlignHorzEnum.Left;
            Style.AlignVert = XLAlignVertEnum.Center;
            Style.BorderTop = XLLineStyleEnum.Thin;
            Style.BorderRight = XLLineStyleEnum.Thin;
            Style.BorderBottom = XLLineStyleEnum.Thin;
            Style.BorderLeft = XLLineStyleEnum.Thin;
            Style.BorderColorBottom = Colors.Black;
            Style.BorderColorLeft = Colors.Black;
            Style.BorderColorTop = Colors.Black;
            Style.BorderColorRight = Colors.Black;
        }

        public void InitHeaderExcel(C1XLBook Book, C1.Silverlight.Excel.XLSheet Sheet, C1.Silverlight.DataGrid.C1DataGrid DataGrid)
        {
            int totalRows = DataGrid.Rows.Count;
            int totalCols = DataGrid.Columns.Count;

            var HeaderStyle = new XLStyle(Book);
            var DataStyle = new XLStyle(Book);

            InitHeaderStyle(HeaderStyle);
            InitDataStyle(DataStyle);
            //Header Setting
            for (int i = 0; i < totalCols; i++)
            {
                XLCell Headercell = Sheet[0, i];
                Headercell.Style = HeaderStyle;
                Headercell.Value = DataGrid.Columns[i].Header.ToString();
            }

            //Data Setting                                
            for (int col = 0; col < totalCols; col++)
            {
                Sheet.Columns[col].Width = C1XLBook.PixelsToTwips(DataGrid.Columns[col].ActualWidth + 3);

                for (int row = 0; row < totalRows; row++)
                {
                    XLCell Datacell = Sheet[row + 1, col];
                    Datacell.Style = DataStyle;
                    Datacell.Value = DataGrid[row, col].Value != null ? DataGrid[row, col].Value.ToString() : "";
                }
            }
        }

        public void InitMutiHeaderExcel(C1XLBook Book, C1.Silverlight.Excel.XLSheet Sheet, C1.Silverlight.DataGrid.C1DataGrid DataGrid)
        {
            int totalRows = DataGrid.Rows.Count;
            int totalCols = DataGrid.Columns.Count;

            var HeaderStyle = new XLStyle(Book);
            var DataStyle = new XLStyle(Book);

            XLCell FirHeadercell;
            XLCell SecHeadercell;
            XLCell Datacell;

            XLCellRange CellRange = null;
            InitHeaderStyle(HeaderStyle);
            InitDataStyle(DataStyle);

            //Header Setting
            for (int i = 0; i < totalCols; i++)
            {
                FirHeadercell = Sheet[0, i];
                SecHeadercell = Sheet[1, i];
                FirHeadercell.Style = HeaderStyle;
                SecHeadercell.Style = HeaderStyle;

                var headerList = DataGrid.Columns[i].Header as List<string>;
                if (headerList.Count == 1)
                {
                    FirHeadercell.Value = headerList[0];
                    Sheet.MergedCells.Add(0, i, 2, 1);
                }
                else if (headerList.Count == 2)
                {
                    FirHeadercell.Value = headerList[0];
                    SecHeadercell.Value = headerList[1];

                    if (i > 0)
                    {
                        int mergeCount = 0;
                        while (true)
                        {
                            if (Sheet[0, i - (mergeCount + 1)].Value.ToString() == FirHeadercell.Value.ToString())
                            {
                                mergeCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (mergeCount == 0)
                        {
                            CellRange = null;
                        }
                        else if (mergeCount == 1)
                        {
                            CellRange = Sheet.MergedCells.Add(0, i - mergeCount, 1, mergeCount + 1);
                        }
                        else
                        {
                            Sheet.MergedCells.Remove(CellRange);
                            CellRange = Sheet.MergedCells.Add(0, i - mergeCount, 1, mergeCount + 1);
                        }
                    }
                }

            }

            //Data Setting                                
            for (int col = 0; col < totalCols; col++)
            {
                Sheet.Columns[col].Width = C1XLBook.PixelsToTwips(DataGrid.Columns[col].ActualWidth + 3);

                for (int row = 2; row < totalRows; row++)
                {
                    Datacell = Sheet[row, col];
                    Datacell.Style = DataStyle;
                    Datacell.Value = DataGrid[row, col].Value != null ? DataGrid[row, col].Value.ToString() : "";
                }
            }
        }
    }
}