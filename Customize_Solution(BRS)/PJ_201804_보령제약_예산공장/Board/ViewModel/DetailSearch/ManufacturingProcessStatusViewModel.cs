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
    public class ManufacturingProcessStatusViewModel : ViewModelBase
    {
          #region property

        private ManufacturingProcessStatusView _mainWnd;

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

        private bool _ISFERT;
        public bool ISFERT
        {
            get { return _ISFERT; }
            set
            {
                _ISFERT = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ISHALB;
        public bool ISHALB
        {
            get { return _ISHALB; }
            set
            {
                _ISHALB = value;
                NotifyPropertyChanged();
            }
        }

        private string _PRODID;
        public string PRODID
        {
            get { return _PRODID; }
            set
            {
                _PRODID = value;
                NotifyPropertyChanged();
            }
        }

        private string _BATCHNO;
        public string BATCHNO
        {
            get { return _BATCHNO; }
            set
            {
                _BATCHNO = value;
                NotifyPropertyChanged();
            }
        }

        private string _POID;
        public string POID
        {
            get { return _POID; }
            set
            {
                _POID = value;
                NotifyPropertyChanged();
            }
        }

        private string _WORKER;
        public string WORKER
        {
            get { return _WORKER; }
            set
            {
                _WORKER = value;
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

        private BR_PHR_RPT_ProductionOrder_WorkTime _BR_PHR_RPT_ProductionOrder_WorkTime;
        public BR_PHR_RPT_ProductionOrder_WorkTime BR_PHR_RPT_ProductionOrder_WorkTime
        {
            get { return _BR_PHR_RPT_ProductionOrder_WorkTime; }
            set
            {
                _BR_PHR_RPT_ProductionOrder_WorkTime = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_RPT_ProductionOrderDetail_WorkTime _BR_PHR_RPT_ProductionOrderDetail_WorkTime;
        public BR_PHR_RPT_ProductionOrderDetail_WorkTime BR_PHR_RPT_ProductionOrderDetail_WorkTime
        {
            get { return _BR_PHR_RPT_ProductionOrderDetail_WorkTime; }
            set
            {
                _BR_PHR_RPT_ProductionOrderDetail_WorkTime = value;
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

                            if (arg == null || !(arg is ManufacturingProcessStatusView))
                                return;
                            _mainWnd = arg as ManufacturingProcessStatusView;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "제조공수현황"
                            });

                            if (!await BR_PHR_GET_DEFAULT_DATE.Execute()) throw new Exception();

                            PeriodSTDTTM = DateTime.Parse(BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(0, 4) + "-" +
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(4, 2) + "-" + 
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(6, 2));
                            PeriodEDDTTM = DateTime.Parse(BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(0, 4) + "-" +
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(4, 2) + "-" + 
                                                          BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(6, 2));

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

                            BR_PHR_RPT_ProductionOrder_WorkTime.INDATAs.Clear();
                            BR_PHR_RPT_ProductionOrder_WorkTime.OUTDATAs.Clear();

                            BR_PHR_RPT_ProductionOrder_WorkTime.INDATAs.Add(new BR_PHR_RPT_ProductionOrder_WorkTime.INDATA()
                            {
                                ISFERT      = ISFERT ? "Y" : null,
                                ISHALB      = ISHALB ? "Y" : null,
                                FROMDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TODATE      = PeriodEDDTTM.ToString("yyyy-MM-dd"),
                                PRODID      = PRODID,
                                BATCHNO     = BATCHNO,
                                POID        = POID,
                                EMPNO       = null,
                                EMPNAME     = null
                            });

                            if (!await BR_PHR_RPT_ProductionOrder_WorkTime.Execute()) throw new Exception();
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

                            if (arg == null || !(arg is BR_PHR_RPT_ProductionOrder_WorkTime.OUTDATA))
                                return;
                            var rowdata = arg as BR_PHR_RPT_ProductionOrder_WorkTime.OUTDATA;

                            BR_PHR_RPT_ProductionOrderDetail_WorkTime.INDATAs.Add(new BR_PHR_RPT_ProductionOrderDetail_WorkTime.INDATA()
                            {
                                POID = rowdata.POID
                            });

                            if (!await BR_PHR_RPT_ProductionOrderDetail_WorkTime.Execute()) throw new Exception();

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
                                customExcel.InitMutiHeaderExcel(book, Firsheet, _mainWnd.dgProductionOrder);
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

        public ManufacturingProcessStatusViewModel()
        {
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
            _BR_PHR_RPT_ProductionOrder_WorkTime = new BR_PHR_RPT_ProductionOrder_WorkTime();
            _BR_PHR_RPT_ProductionOrderDetail_WorkTime = new BR_PHR_RPT_ProductionOrderDetail_WorkTime();

            ISFERT = true;
        } 
    }
}
