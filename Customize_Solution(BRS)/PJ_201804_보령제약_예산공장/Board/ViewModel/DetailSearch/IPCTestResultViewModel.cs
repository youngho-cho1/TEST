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
    public class IPCTestResultViewModel : ViewModelBase
    {
        #region property

        private IPCTestResultView _mainWnd;

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

        private string _IPCCode;
        public string IPCCode
        {
            get{return _IPCCode;}
            set
            {
                _IPCCode = value;
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

        private BR_PHR_RPT_ProductionOrderTestSpecificationResult _BR_PHR_RPT_ProductionOrderTestSpecificationResult;
        public BR_PHR_RPT_ProductionOrderTestSpecificationResult BR_PHR_RPT_ProductionOrderTestSpecificationResult
        {
            get { return _BR_PHR_RPT_ProductionOrderTestSpecificationResult; }
            set
            {
                _BR_PHR_RPT_ProductionOrderTestSpecificationResult = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_SEL_TestSpecification_CBO _BR_PHR_SEL_TestSpecification_CBO;
        public BR_PHR_SEL_TestSpecification_CBO BR_PHR_SEL_TestSpecification_CBO
        {
            get { return _BR_PHR_SEL_TestSpecification_CBO; }
            set
            {
                _BR_PHR_SEL_TestSpecification_CBO = value;
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

                            if (arg == null || !(arg is IPCTestResultView))
                                return;
                            _mainWnd = arg as IPCTestResultView;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();
                            BR_PHR_SEL_TestSpecification_CBO.INDATAs.Clear();
                            BR_PHR_SEL_TestSpecification_CBO.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "IPC검사결과"
                            });

                            if (!await BR_PHR_GET_DEFAULT_DATE.Execute()) throw new Exception();

                            PeriodSTDTTM = DateTime.Parse(_BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(0, 4) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(4, 2) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(6, 2));
                            PeriodEDDTTM = DateTime.Parse(_BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(0, 4) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(4, 2) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(6, 2));

                            BR_PHR_SEL_TestSpecification_CBO.INDATAs.Add(new BR_PHR_SEL_TestSpecification_CBO.INDATA()
                            {
                                LANGID = "ko-kr",
                                CMCODE = null
                            });
                            if (!await BR_PHR_SEL_TestSpecification_CBO.Execute()) throw new Exception();
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

                            BR_PHR_RPT_ProductionOrderTestSpecificationResult.INDATAs.Clear();
                            BR_PHR_RPT_ProductionOrderTestSpecificationResult.OUTDATAs.Clear();

                            BR_PHR_RPT_ProductionOrderTestSpecificationResult.INDATAs.Add(new BR_PHR_RPT_ProductionOrderTestSpecificationResult.INDATA()
                            {
                                FROMPRODDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TOPRODDATE      = PeriodEDDTTM.ToString("yyyy-MM-dd"),
                                TIID            = IPCCode,
                                BATCHNO         = BATCHNO,
                                POID            = POID
                            });

                            if (!await BR_PHR_RPT_ProductionOrderTestSpecificationResult.Execute()) throw new Exception();
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
                                customExcel.InitHeaderExcel(book, Firsheet, _mainWnd.dgProductionOrder);
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

        public IPCTestResultViewModel()
        {
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
            _BR_PHR_SEL_TestSpecification_CBO = new BR_PHR_SEL_TestSpecification_CBO();
            _BR_PHR_RPT_ProductionOrderTestSpecificationResult = new BR_PHR_RPT_ProductionOrderTestSpecificationResult();
        }
    }
}
