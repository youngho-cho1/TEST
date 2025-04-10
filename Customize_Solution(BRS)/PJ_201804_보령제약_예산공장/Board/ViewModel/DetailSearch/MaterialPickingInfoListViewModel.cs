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
    public class MaterialPickingInfoListViewModel : ViewModelBase
    {
        #region property

        private MaterialPickingInfoList _mainWnd;

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

        private string _MATERIALBARCODE;
        public string MATERIALBARCODE
        {
            get { return _MATERIALBARCODE; }
            set
            {
                _MATERIALBARCODE = value;
                NotifyPropertyChanged();
            }
        }

        private string _MTRLNAME;
        public string MTRLNAME
        {
            get { return _MTRLNAME; }
            set
            {
                _MTRLNAME = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Data

        private BR_BRS_SEL_MaterialPickingInfoList _BR_BRS_SEL_MaterialPickingInfoList;
        public BR_BRS_SEL_MaterialPickingInfoList BR_BRS_SEL_MaterialPickingInfoList
        {
            get { return _BR_BRS_SEL_MaterialPickingInfoList; }
            set
            {
                _BR_BRS_SEL_MaterialPickingInfoList = value;
                NotifyPropertyChanged();
            }
        }

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

                            if (arg == null || !(arg is MaterialPickingInfoList))
                                return;
                            _mainWnd = arg as MaterialPickingInfoList;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "자재Picking정보"
                            });

                            if (!await BR_PHR_GET_DEFAULT_DATE.Execute()) throw new Exception();

                            PeriodSTDTTM = DateTime.Parse(_BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(0, 4) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(4, 2) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].FROMDATE.Substring(6, 2));
                            PeriodEDDTTM = DateTime.Parse(_BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(0, 4) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(4, 2) + "-" +
                                                          _BR_PHR_GET_DEFAULT_DATE.OUTDATAs[0].TODATE.Substring(6, 2));


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

                            BR_BRS_SEL_MaterialPickingInfoList.INDATAs.Clear();
                            BR_BRS_SEL_MaterialPickingInfoList.OUTDATAs.Clear();

                            BR_BRS_SEL_MaterialPickingInfoList.INDATAs.Add(new BR_BRS_SEL_MaterialPickingInfoList.INDATA()
                            {
                                FROMDATE = PeriodSTDTTM,
                                TODATE = PeriodEDDTTM,
                                BATCHNO = BATCHNO,
                                MATERIALBARCODE = MATERIALBARCODE,
                                MTRLNAME = MTRLNAME
                            });

                            if (!await BR_BRS_SEL_MaterialPickingInfoList.Execute()) throw new Exception();
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

        public MaterialPickingInfoListViewModel()
        {
            _BR_BRS_SEL_MaterialPickingInfoList = new BR_BRS_SEL_MaterialPickingInfoList();
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
        }
    }
}
