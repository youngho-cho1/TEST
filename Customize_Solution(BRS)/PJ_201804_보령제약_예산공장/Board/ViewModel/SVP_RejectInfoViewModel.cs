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
    public class SVP_RejectInfoViewModel : ViewModelBase
    {
        #region ##### property ##### 
        private SVP_RejectInfoViewModel _mainWnd;
        private SVP_RejectInfo _mainWnd2;

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

        private string _BatchNo;
        public string BatchNo
        {
            get { return _BatchNo; }
            set
            {
                _BatchNo = value;
                NotifyPropertyChanged();
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                NotifyPropertyChanged();
            }
        }

        private string _MtrlId;
        public string MtrlId
        {
            get { return _MtrlId; }
            set
            {
                _MtrlId = value;
                NotifyPropertyChanged();
            }
        }

        private string _MtrlName;
        public string MtrlName
        {
            get { return _MtrlName; }
            set
            {
                _MtrlName = value;
                NotifyPropertyChanged();
            }
        }
        #endregion ##### property #####

        #region [BizRule]

        private BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO;
        public BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO
        {
            get { return _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO; }
            set
            {
                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO");
            }
        }

        #endregion

        public SVP_RejectInfoViewModel()
        {
            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO = new BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO();
        }

        public ICommand LoadedCommandAsync
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

                            _mainWnd = arg as SVP_RejectInfoViewModel;
                            _mainWnd2 = arg as SVP_RejectInfo;

                            PeriodEDDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            PeriodSTDTTM = PeriodEDDTTM.AddDays(-7);
                            
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
                    using (await AwaitableLocks["SearchCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SearchCommand"] = false;
                            CommandCanExecutes["SearchCommand"] = false;

                            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATAs.Clear();
                            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.OUTDATAs.Clear();

                            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATAs.Add(new BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATA()
                            {
                                FROMDTTM = PeriodSTDTTM,
                                TODTTM = PeriodEDDTTM,
                                MTRLID = MtrlId != "" ? MtrlId : null,
                                MTRLNAME = MtrlName != "" ? MtrlName : null,
                                BATCHNO = BatchNo != "" ? BatchNo : null,
                                USERNAME = UserName != "" ? _UserName : null
                            });

                            await _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.Execute();

                            CommandResults["SearchCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SearchCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SearchCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SearchCommand") ?
                        CommandCanExecutes["SearchCommand"] : (CommandCanExecutes["SearchCommand"] = true);
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

                            Custom_C1ExportExcel customExcel = new Custom_C1ExportExcel();

                            customExcel.SaveBook(book =>
                            {   
                                C1.Silverlight.Excel.XLSheet sheet = book.Sheets[0];
                                
                                //sheet[0, 0].Value = "작업자";
                                //sheet.MergedCells.Add(0, 0, 6, 1);

                                //sheet[0, 1].Value = "검사일자";
                                //sheet.MergedCells.Add(0, 1, 6, 1);

                                //sheet[0, 2].Value = "검사수량(Vial)";
                                //sheet.MergedCells.Add(0, 2, 6, 1);

                                //sheet[0, 3].Value = "총 양품수율";
                                //sheet.MergedCells.Add(0, 3, 6, 1);

                                //sheet[0, 4].Value = "총 불량수율";
                                //sheet.MergedCells.Add(0, 4, 6, 1);

                                //sheet[0, 5].Value = "치명결점수량";
                                //sheet.MergedCells.Add(0, 5, 6, 1);

                                //sheet[0, 6].Value = "중결점수량";
                                //sheet.MergedCells.Add(0, 6, 6, 1);

                                //sheet[0, 7].Value = "경결점수량";
                                //sheet.MergedCells.Add(0, 7, 6, 1);

                                //sheet[0, 8].Value = "불량유형및수량";
                                //sheet.MergedCells.Add(0, 8, 1, 21);

                                //sheet[1, 8].Value = "내용물";
                                //sheet.MergedCells.Add(1, 8, 1, 8);
                                //sheet[2, 8].Value = "이물";
                                //sheet.MergedCells.Add(2, 8, 1, 7);
                                //sheet[2, 15].Value = "충전량불량";
                                //sheet.MergedCells.Add(2, 15, 2, 1);
                                //sheet[1, 16].Value = "용기";
                                //sheet.MergedCells.Add(1, 16, 1, 4);
                                //sheet[1, 20].Value = "캡";
                                //sheet.MergedCells.Add(1, 20, 1, 3);
                                //sheet[1, 23].Value = "고무전";
                                //sheet.MergedCells.Add(1, 23, 1, 3);
                                //sheet[1, 26].Value = "Cake상태불량";
                                //sheet.MergedCells.Add(1, 26, 1, 1);
                                //sheet[1, 27].Value = "바이알내부기벽/고무전약액묻음";
                                //sheet.MergedCells.Add(1, 27, 1, 1);
                                //sheet[1, 28].Value = "기타불량";
                                //sheet.MergedCells.Add(1, 28, 5, 1);
                                //sheet[0, 29].Value = "비고";
                                //sheet.MergedCells.Add(0, 29, 6, 1);



                                //sheet[2, 6].Value = "흰티(치명결점)";
                                //sheet[2, 7].Value = "검은티(치명결점)";
                                //sheet[2, 8].Value = "유색(치명결점)";
                                //sheet[2, 9].Value = "금속성(치명결점)";
                                //sheet[2, 10].Value = "유리조각(치명결점)";
                                //sheet[2, 11].Value = "섬유(6> 1mm)(치명결점)";
                                //sheet[2, 12].Value = "섬유(≤ 1mm)(중결점)";

                                //sheet[1, 5].Value = "충전량불량(중결점)";
                                //sheet.MergedCells.Add(2, 16, 2, 1);

                                //sheet[1, 17].Value = "충전량불량(중결점)";
                                //sheet.MergedCells.Add(2, 8, 1, 6);

                                //sheet[0, 7].Value = "바이알손상(치명결점)";
                                //sheet[0, 7].Value = "내부오염(중결점)";
                                //sheet[0, 7].Value = "바이알흠집(경결점)";
                                //sheet[0, 7].Value = "성형불량(경결점)";
                                //sheet[0, 7].Value = "캡씰링불량(중결점)";
                                //sheet[0, 7].Value = "이종캡(중결점)";
                                //sheet[0, 7].Value = " 캡외관불량(경결점)";
                                //sheet[0, 7].Value = "고무전없음(치명결점)";
                                //sheet[0, 7].Value = "이종고무전(치명결점)";
                                //sheet[0, 7].Value = "고무전이물(중결점)";
                                //sheet[0, 7].Value = "Cake상태불량";
                                //sheet[0, 7].Value = "바이알내부기벽/고무전약액묻음";
                                //sheet[0, 7].Value = "기타불량";
                                //sheet[0, 7].Value = "비고";

                                //sheet.MergedCells.Add(new XlCellRange(fromrow, torow, fromcolum, tocolum))

                                customExcel.InitMutiHeaderExcel(book, sheet, _mainWnd2.dgDetail);
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
    }
}
