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
    public class PerformanceByWorkshopViewModel : ViewModelBase
    {
        #region property

        private PerformanceByWorkshopView _mainWnd;

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

        private BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom _BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom;
        public BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom
        {
            get { return _BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom; }
            set
            {
                _BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom = value;
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

                            if (arg == null || !(arg is PerformanceByWorkshopView))
                                return;
                            _mainWnd = arg as PerformanceByWorkshopView;

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Clear();
                            BR_PHR_GET_DEFAULT_DATE.OUTDATAs.Clear();

                            BR_PHR_GET_DEFAULT_DATE.INDATAs.Add(new BR_PHR_GET_DEFAULT_DATE.INDATA()
                            {
                                PROGRAMID = "작업실별실적조회"
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

                            BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom.INDATAs.Clear();
                            BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom.OUTDATAs.Clear();

                            BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom.INDATAs.Add(new BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom.INDATA()
                            {
                                ISFERT      = ISFERT ? "Y" : null,
                                ISHALB      = ISHALB ? "Y" : null,
                                FROMDATE    = PeriodSTDTTM.ToString("yyyy-MM-dd"),
                                TODATE      = PeriodEDDTTM.ToString("yyyy-MM-dd"),
                                ROOMID      = SelectedEquipmentCode
                            });

                            if (!await BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom.Execute()) throw new Exception();
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
                                customExcel.InitHeaderExcel(book, sheet, _mainWnd.dgProductionOrder);
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

        public PerformanceByWorkshopViewModel()
        {
            _BR_PHR_GET_DEFAULT_DATE = new BR_PHR_GET_DEFAULT_DATE();
            _BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom = new BR_PHR_RPT_ProductionOrder_Result_by_ProdRoom();

            ISFERT = true;
        }
    }
}
