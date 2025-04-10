using LGCNS.iPharmMES.Common;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using C1.Silverlight.Excel;
using System.Globalization;
using System.Collections.Generic;

namespace Board
{
    public class FacilitiesOperationStatusViewModel : ViewModelBase
    {
        #region property

        private FacilitiesOperationStatusView _mainWnd;

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

        private string _SelectedEquipmentCode;
        public string SelectedEquipmentCode
        {
            get { return _SelectedEquipmentCode; }
            set
            {
                _SelectedEquipmentCode = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedEquipmentName;
        public string SelectedEquipmentName
        {
            get { return _SelectedEquipmentName; }
            set
            {
                _SelectedEquipmentName = value;
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

        private BR_PHR_RPT_Equipment_WorkTime _BR_PHR_RPT_Equipment_WorkTime;
        public BR_PHR_RPT_Equipment_WorkTime BR_PHR_RPT_Equipment_WorkTime
        {
            get { return _BR_PHR_RPT_Equipment_WorkTime; }
            set
            {
                _BR_PHR_RPT_Equipment_WorkTime = value;
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

                            if (arg == null || !(arg is FacilitiesOperationStatusView))
                                return;
                            _mainWnd = arg as FacilitiesOperationStatusView;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "주요설비가동현황"
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

                            BR_PHR_RPT_Equipment_WorkTime.INDATAs.Clear();
                            BR_PHR_RPT_Equipment_WorkTime.OUTDATAs.Clear();

                            BR_PHR_RPT_Equipment_WorkTime.INDATAs.Add(new BR_PHR_RPT_Equipment_WorkTime.INDATA()
                            {
                                FROMPRODDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TOPRODDATE      = PeriodEDDTTM.ToString("yyyy-MM-dd"),
                                EQPTID          = SelectedEquipmentCode
                            });

                            if (!await BR_PHR_RPT_Equipment_WorkTime.Execute()) throw new Exception();
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
                                C1.Silverlight.Excel.XLSheet sheet = book.Sheets[0];
                                customExcel.InitHeaderExcel(book, sheet, _mainWnd.dgEQWorkTime); 
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

        public FacilitiesOperationStatusViewModel()
        {
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
            _BR_PHR_RPT_Equipment_WorkTime = new BR_PHR_RPT_Equipment_WorkTime();
        }

        #endregion
    }
}
