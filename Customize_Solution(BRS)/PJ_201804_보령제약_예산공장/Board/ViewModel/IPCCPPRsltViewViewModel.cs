using LGCNS.iPharmMES.Common;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using C1.Silverlight.Excel;
using System.Globalization;
using System.Collections.Generic;
using LGCNS.EZMES.Common;
using C1.Silverlight;

namespace Board
{
    public class IPCCPPRsltViewViewModel : ViewModelBase
    {
        private IPCCPPRsltView _mainWnd;
        public IPCCPPRsltViewViewModel()
        {
            _mockup_OrderSummary_items = new Mockup_classes.Mockup_OrderSummary_items();
            _mockup_OperationCheck_items = new Mockup_classes.Mockup_OperationCheck_items();
            _mockup_OperationCheck1_items = new Mockup_classes.Mockup_OperationCheck_items();
            _mockup_CriticalParameter_items = new Mockup_classes.Mockup_CriticalParameter_items();
            _mockup_CriticalParameter1_items = new Mockup_classes.Mockup_CriticalParameter_items();

            _mockup_LineCBO_items = new Mockup_classes.Mockup_LineCBO_items();

            RetrieveOperationCheckCommand = new CommandBase(RetrieveOperationCheck);
            
            //조회조건 콤보박스 조회 추가
            _PLANDATEOPTION = new BR_PHR_SEL_CommonCode_CBO();
            _PLANDATEOPTION.OnExecuteCompleted += new DelegateExecuteCompleted(__PLANDATEOPTIONOnExecuteCompeted);
            _CLOSEDATEOPTION = new BR_PHR_SEL_CommonCode_CBO();
            _CLOSEDATEOPTION.OnExecuteCompleted += new DelegateExecuteCompleted(__CLOSEDATEOPTIONOnExecuteCompleted);
            _TreeSearchCondition = new SearchCondition();

            //TreeFilter
            _TreeFilterCondition = new FilterCondition();
            _TreeFilterCondition.isOrder = true;
            _TreeFilterCondition.isBatch = false;

            //_typeComboItemsSource = new ObservableCollection<FilterValue>();
            _FilterTypeCBO = new BR_PHR_SEL_CommonCode_CBO();
            _FilterTypeCBO.OnExecuteCompleted += new DelegateExecuteCompleted(_FilterTypeCBO_OnExecuteCompleted);

        }


        #region[Property]

        //비즈룰 개발 후 대체 필요
        private Mockup_classes.Mockup_OrderSummary_items _mockup_OrderSummary_items;
        public Mockup_classes.Mockup_OrderSummary_items mockup_OrderSummary_items
        {
            get { return _mockup_OrderSummary_items; }
            set { _mockup_OrderSummary_items = value;
                OnPropertyChanged("mockup_OrderSummary_items");
            }
        }

        private Mockup_classes.Mockup_OrderSummary_items _CheckedRows;
        public Mockup_classes.Mockup_OrderSummary_items CheckedRows
        {
            get { return _CheckedRows; }
            set { _CheckedRows = value;
                OnPropertyChanged("CheckedRows");
            }
        }


        private Mockup_classes.Mockup_OperationCheck_items _mockup_OperationCheck_items;
        public Mockup_classes.Mockup_OperationCheck_items mockup_OperationCheck_items
        {
            get { return _mockup_OperationCheck_items; }
            set
            {
                _mockup_OperationCheck_items = value;
                OnPropertyChanged("mockup_OperationCheck_items");
            }
        }


        private Mockup_classes.Mockup_OperationCheck_items _mockup_OperationCheck1_items;
        public Mockup_classes.Mockup_OperationCheck_items mockup_OperationCheck1_items
        {
            get { return _mockup_OperationCheck1_items; }
            set { _mockup_OperationCheck1_items = value;
                  OnPropertyChanged("mockup_OperationCheck1_items");
                }
        }

        private Mockup_classes.Mockup_CriticalParameter_items _mockup_CriticalParameter_items;
        public Mockup_classes.Mockup_CriticalParameter_items mockup_CriticalParameter_items
        {
            get { return _mockup_CriticalParameter_items; }
            set { _mockup_CriticalParameter_items = value; }
        }

        private Mockup_classes.Mockup_CriticalParameter_items _mockup_CriticalParameter1_items;
        public Mockup_classes.Mockup_CriticalParameter_items mockup_CriticalParameter1_items
        {
            get { return _mockup_CriticalParameter1_items; }
            set { _mockup_CriticalParameter1_items = value;
                    OnPropertyChanged("mockup_CriticalParameter1_items");
                }
        }

        private Mockup_classes.Mockup_LineCBO_items _mockup_LineCBO_items;
        public Mockup_classes.Mockup_LineCBO_items mockup_LineCBO_items
        {
            get { return _mockup_LineCBO_items; }
            set
            {
                _mockup_LineCBO_items = value;
                OnPropertyChanged("mockup_LineCBO_items");
            }
        }
        private BufferedObservableCollection<Mockup_classes.Mockup_OrderSummary_item> _TreeFilter =
         new BufferedObservableCollection<Mockup_classes.Mockup_OrderSummary_item>();
        public BufferedObservableCollection<Mockup_classes.Mockup_OrderSummary_item> TreeFilter
        {
            get { return _TreeFilter; }
            set
            {
                if (_TreeFilter != value)
                {
                    _TreeFilter = value;
                    OnPropertyChanged("TreeFilter");
                }
            }
        }
        


        public class SearchCondition : ViewModelBase
        {
            private bool _isAll = false;
            public bool isAll
            {
                get { return _isAll; }
                set { _isAll = value; }
            }

            private bool _isPlanned = true;
            public bool isPlanned
            {
                get { return _isPlanned; }
                set { _isPlanned = value; }
            }


            private bool _isPreparing = true;
            public bool isPreparing
            {
                get { return _isPreparing; }
                set { _isPreparing = value; }
            }


            private bool _isRequesting = true;
            public bool isRequesting
            {
                get { return _isRequesting; }
                set { _isRequesting = value; }
            }

            private bool _isApproved = true;
            public bool isApproved
            {
                get { return _isApproved; }
                set { _isApproved = value; }
            }

            private bool _isProcessing = true;
            public bool isProcessing
            {
                get { return _isProcessing; }
                set { _isProcessing = value; }
            }

            private bool _isCompleted = false;
            public bool isCompleted
            {
                get { return _isCompleted; }
                set { _isCompleted = value; }
            }
            private bool _isReviewing = false;
            public bool isReviewing
            {
                get { return _isReviewing; }
                set { _isReviewing = value; }
            }

            private bool _isHolding = false;
            public bool isHolding
            {
                get { return _isHolding; }
                set { _isHolding = value; }
            }

            private bool _isClosed = false;
            public bool isClosed
            {
                get { return _isClosed; }
                set { _isClosed = value; }
            }

            private bool _isCanceled = false;
            public bool isCanceled
            {
                get { return _isCanceled; }
                set { _isCanceled = value; }
            }

            private bool _isPeriod;
            public bool isPeriod
            {
                get { return _isPeriod; }
                set { _isPeriod = value; }
            }

            private System.Nullable<System.DateTime> _PeriodSTDTTM;
            public System.Nullable<System.DateTime> PeriodSTDTTM
            {
                get { return _PeriodSTDTTM; }
                set { _PeriodSTDTTM = value; }
            }

            private System.Nullable<System.DateTime> _PeriodEDDTTM;
            public System.Nullable<System.DateTime> PeriodEDDTTM
            {
                get { return _PeriodEDDTTM; }
                set { _PeriodEDDTTM = value; }
            }

            private string _PlanDateOtion;
            public string PlanDateOption
            {
                get { return _PlanDateOtion; }
                set { _PlanDateOtion = value; }
            }

            private string _ClosedDateOtion;
            public string ClosedDateOtion
            {
                get { return _ClosedDateOtion; }
                set { _ClosedDateOtion = value; }
            }

            private string _Location;
            public string Location
            {
                get { return _Location; }
                set
                {
                    _Location = value;
                    OnPropertyChanged("Location");
                }
            }

            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set { _MTRLID = value; }
            }

            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set { _MTRLNAME = value; }
            }
        }


        //SearchCondition
        string _PlanDateOption = null;
        string _isPLAN = null;
        string _isAPPR = null;
        string _isPROC = null;
        string _isPREP = null;
        string _isCOMP = null;
        string _isREQS = null;
        string _isCNCL = null;
        string _ISHOLD = null;
        string _MTRLID = null;
        System.Nullable<DateTime> _PeriodSTDTTM;
        System.Nullable<DateTime> _PeriodEDDTTM;


        private string PCNAME;
        private string CLIENTIP;
        private string ROOMID;
        private BR_PHR_SEL_System_PC_IP _BR_PHR_SEL_System_PC_IP;
        public BR_PHR_SEL_System_PC_IP BR_PHR_SEL_System_PC_IP
        {
            get { return _BR_PHR_SEL_System_PC_IP; }
            set { _BR_PHR_SEL_System_PC_IP = value; }
        }

        private SearchCondition _TreeSearchCondition;
        public SearchCondition TreeSearchCondition
        {
            get { return _TreeSearchCondition; }
            set { _TreeSearchCondition = value; }
        }

        public class FilterCondition
        {
            public bool isOrder { get; set; }

            public bool isBatch { get; set; }

            public string howFiltering { get; set; }

            public string content { get; set; }
        }

        private FilterCondition _TreeFilterCondition;
        public FilterCondition TreeFilterCondition
        {
            get
            {
                return _TreeFilterCondition;
            }
            set
            {
                _TreeFilterCondition = value;
                OnPropertyChanged("TreeFilterCondition");
            }
        }

        public class FilterValue
        {
            public string displayName { get; set; }
            public string selectedValue { get; set; }
        }


        public void AllGridCheck(bool chk)
        {
            if (mockup_OrderSummary_items.Count > 0)
            {
                foreach (var item in mockup_OrderSummary_items)
                {
                    if (chk)
                        item.CHECKBOX = true;
                    else
                        item.CHECKBOX = false;
                }
                OnPropertyChanged("mockup_OrderSummary_items");
            }
        }


        #endregion

        #region [Bizrule]

        private BR_PHR_SEL_CommonCode_CBO _FilterTypeCBO;

        public BR_PHR_SEL_CommonCode_CBO FilterTypeCBO
        {
            get { return _FilterTypeCBO; }
            set
            {
                _FilterTypeCBO = value;

            }
        }
        private BR_PHR_SEL_CommonCode_CBO _PLANDATEOPTION;
        public BR_PHR_SEL_CommonCode_CBO PLANDATEOPTION
        {
            get { return _PLANDATEOPTION; }
            set { _PLANDATEOPTION = value; }
        }
        private BR_PHR_SEL_CommonCode_CBO _CLOSEDATEOPTION;
        public BR_PHR_SEL_CommonCode_CBO CLOSEDATEOPTION
        {
            get { return _CLOSEDATEOPTION; }
            set { _CLOSEDATEOPTION = value; }
        }

        #endregion


        #region [Command]


        public ICommand RetrieveOperationCheckCommand { get; set; }
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["LoadedCommand"] = false;
                        CommandCanExecutes["LoadedCommand"] = false;

                        if (arg != null)
                            _mainWnd = arg as IPCCPPRsltView;
                            _TreeSearchCondition.PeriodSTDTTM = Convert.ToDateTime(DateTime.Now);
                            _TreeSearchCondition.PeriodEDDTTM = Convert.ToDateTime(DateTime.Now.AddDays(7));

                            _TreeSearchCondition.PlanDateOption = "1W";             

                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "일반동-고형제라인"                            
                            });
                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "일반동-유산균라인"
                            });
                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "일반동-주사제라인"
                            });
                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "세파동-고형제라인"
                            });
                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "세파동-주사제라인"
                            });
                            mockup_LineCBO_items.Add(new Mockup_classes.Mockup_LineCBO_item()
                            {
                                CHECKBOX = false,
                                LINENAME = "항암동-주사제라인"
                            });


                        _PLANDATEOPTION.INDATAs.Clear();
                        _PLANDATEOPTION.OUTDATAs.Clear();

                        _PLANDATEOPTION.INDATAs.Add(new BR_PHR_SEL_CommonCode_CBO.INDATA
                        {
                            CMCDTYPE = "PHR_PLANDATE"
                            ,
                            LANGID = LogInInfo.LangID
                        });

                        _CLOSEDATEOPTION.INDATAs.Clear();
                        _CLOSEDATEOPTION.OUTDATAs.Clear();

                        _CLOSEDATEOPTION.INDATAs.Add(new BR_PHR_SEL_CommonCode_CBO.INDATA
                        {
                            CMCDTYPE = "PHR_CLOSEDDATE"
                            ,
                            LANGID = LogInInfo.LangID
                        });

                         _PLANDATEOPTION.Execute();
                         _CLOSEDATEOPTION.Execute();

                        CommandResults["LoadedViewCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LoadedViewCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadedViewCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);

                });
            }
        }

        //조회버튼 클릭시 event
        public ICommand RetrieveOrderSummaryCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;
                        CommandResults["RetrieveOrderSummaryCommand"] = false;
                        CommandCanExecutes["RetrieveOrderSummaryCommand"] = false;

                        mockup_OrderSummary_items.Clear();

                        mockup_OrderSummary_items.Add(new Mockup_classes.Mockup_OrderSummary_item()     
                        {
                            PeriodSTDTTM = DateTime.Now,
                            PeriodEDDTTM = DateTime.Now,
                            EQPTID = "MES100",
                            EQPTNAME = "일반동-고형제라인",
                            POID = "ODH04006",
                            LOTID = "H04006",                
                            MTRLNAME = "아로나민골드정(내수용)",
                            ORDERTYPENAME = "일반생산",
                            POSTAT = "PROCESSING",
                            POSTATNAME = "생산 중"
                        });


                        mockup_OrderSummary_items.Add(new Mockup_classes.Mockup_OrderSummary_item()
                        {
                            PeriodSTDTTM = DateTime.Now,
                            PeriodEDDTTM = DateTime.Now,
                            EQPTID = "MES100",
                            EQPTNAME = "일반동-고형제라인",
                            POID = "ODH04008",
                            LOTID = "H04008",       
                            MTRLNAME = "나푸롤정",           
                            ORDERTYPENAME = "시험생산",
                            POSTAT = "COMPLETED",
                            POSTATNAME = "생산 완료"
                        });


                        mockup_OrderSummary_items.Add(new Mockup_classes.Mockup_OrderSummary_item()
                        {
                            PeriodSTDTTM = DateTime.Now,
                            PeriodEDDTTM = DateTime.Now,
                            EQPTID = "MES100",
                            EQPTNAME = "일반동-고형제라인",
                            POID = "ODH03005",
                            LOTID = "H03005",   
                            MTRLNAME = "판토메드주사",                
                            ORDERTYPENAME = "밸리데이션",
                            POSTAT = "COMPLETED",
                            POSTATNAME = "생산 완료"
                        });



                        CommandResults["RetrieveOrderSummaryCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["RetrieveOrderSummaryCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["RetrieveOrderSummaryCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RetrieveOrderSummaryCommand") ?
                    CommandCanExecutes["RetrieveOrderSummaryCommand"] : (CommandCanExecutes["RetrieveOrderSummaryCommand"] = true);
                });
            }
        }



        public ICommand RerieveFilterTypeCBOCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RerieveFilterTypeCBOCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RerieveFilterTypeCBOCommand"] = false;
                            CommandCanExecutes["RerieveFilterTypeCBOCommand"] = false;

                            ///
                            _FilterTypeCBO.OUTDATAs.Clear();
                            _FilterTypeCBO.INDATAs.Clear();

                            _FilterTypeCBO.INDATAs.Add(new BR_PHR_SEL_CommonCode_CBO.INDATA
                            {
                                LANGID = LogInInfo.LangID,
                                CMCDTYPE = "PHR_TREE_FILTER"
                            });

                            await _FilterTypeCBO.Execute();
                            ///

                            CommandResults["RerieveFilterTypeCBOCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RerieveFilterTypeCBOCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RerieveFilterTypeCBOCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RerieveFilterTypeCBOCommand") ?
                        CommandCanExecutes["RerieveFilterTypeCBOCommand"] : (CommandCanExecutes["RerieveFilterTypeCBOCommand"] = true);
                });
            }
        }
        public ICommand FilteringTreeCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["FilteringTreeCommand"] = false;
                        CommandCanExecutes["FilteringTreeCommand"] = false;

                        ///
                        if (mockup_OrderSummary_items.Count > 0)
                        {
                            if (_TreeFilterCondition != null)
                            {
                                if (_TreeFilterCondition.content == null)
                                {
                                    _TreeFilterCondition.content = "";
                                }
                                #region OrderNumber

                                if (_TreeFilterCondition.isOrder)
                                {
                                    switch (_TreeFilterCondition.howFiltering)
                                    {
                                        #region Contained

                                        case "Contained":
                                            var ContainedOrder = from a in this.mockup_OrderSummary_items
                                                                 where a.POID.Contains(_TreeFilterCondition.content)
                                                                 select a;

                                            TreeFilter.Clear();

                                            foreach (var r in ContainedOrder.ToList())
                                                TreeFilter.Add(r);

                                           // SetTreeviewData();
                                            break;

                                        #endregion

                                        #region ExactlyName

                                        case "ExactlyName":
                                            var ExactlyNameOrder = from a in this.mockup_OrderSummary_items
                                                                   where a.POID.Equals(_TreeFilterCondition.content)
                                                                   select a;

                                            TreeFilter.Clear();

                                            foreach (var r in ExactlyNameOrder.ToList())
                                                TreeFilter.Add(r);

                                          //  SetTreeviewData();
                                            break;

                                        #endregion

                                        #region StartWith

                                        case "StartWith":
                                            var StartWithOrder = from a in this.mockup_OrderSummary_items
                                                                 where a.POID.StartsWith(_TreeFilterCondition.content)
                                                                 select a;

                                            TreeFilter.Clear();

                                            foreach (var r in StartWithOrder.ToList())
                                                TreeFilter.Add(r);

                                         //   SetTreeviewData();
                                            break;

                                            #endregion
                                    }
                                }

                                #endregion

                                #region BatchNumber

                                else if (_TreeFilterCondition.isBatch)
                                {
                                    switch (_TreeFilterCondition.howFiltering)
                                    {
                                        #region Contained

                                        case "Contained":
                                            var ContainedBatch = from a in this.mockup_OrderSummary_items
                                                                 where a.LOTID.Contains(_TreeFilterCondition.content)
                                                                 select a;

                                            TreeFilter.Clear();

                                            foreach (var r in ContainedBatch.ToList())
                                                TreeFilter.Add(r);

                                       //     SetTreeviewData();
                                            break;

                                        #endregion

                                        #region ExactlyName

                                        case "ExactlyName":
                                            var ExactlyNameBatch = from a in this.mockup_OrderSummary_items
                                                                   where a.LOTID.Equals(_TreeFilterCondition.content)
                                                                   select a;

                                            TreeFilter.Clear();

                                            foreach (var r in ExactlyNameBatch.ToList())
                                                TreeFilter.Add(r);

                                      //      SetTreeviewData();
                                            break;

                                        #endregion

                                        #region StartWith

                                        case "StartWith":
                                            var StartWithBatch = from a in this.mockup_OrderSummary_items
                                                                 where a.LOTID.StartsWith(_TreeFilterCondition.content)
                                                                 select a;

                                            TreeFilter.Clear();

                                            foreach (var r in StartWithBatch.ToList())
                                                TreeFilter.Add(r);

                                          //  SetTreeviewData();
                                            break;

                                            #endregion
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            //2012-10-19 메세지 수정. MSGID : PHM10013 
                            string message = LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "PHM10013");
                            C1MessageBox.Show(message, "Warning", C1MessageBoxIcon.Warning); //PHM10013 필터 기능은 오더 조회 후 가능합니다.
                        }
                        ///

                        CommandResults["FilteringTreeCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["FilteringTreeCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["FilteringTreeCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("FilteringTreeCommand") ?
                        CommandCanExecutes["FilteringTreeCommand"] : (CommandCanExecutes["FilteringTreeCommand"] = true);
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
                                customExcel.InitHeaderExcel(book, Firsheet, _mainWnd.grdOperationCheck);
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

        public ICommand ClickExportExcelCommand1
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickExportExcelCommand1"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickExportExcelCommand1"] = false;
                            CommandCanExecutes["ClickExportExcelCommand1"] = false;

                            ///
                            Custom_C1ExportExcel customExcel = new Custom_C1ExportExcel();

                            customExcel.SaveBook(book =>
                            {
                                book.Sheets.Add();
                                C1.Silverlight.Excel.XLSheet Firsheet = book.Sheets[0];
                                customExcel.InitHeaderExcel(book, Firsheet, _mainWnd.grd_CriticalParameter);
                            });
                            ///

                            CommandResults["ClickExportExcelCommand1"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickExportExcelCommand1"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickExportExcelCommand1"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickExportExcelCommand1") ?
                        CommandCanExecutes["ClickExportExcelCommand1"] : (CommandCanExecutes["ClickExportExcelCommand1"] = true);
                });
            }
        }


        #endregion

        #region [※ Event]
        //OrderSummary그리드 상단 버튼 클릭시 결과조회
        public void RetrieveOperationCheck(object param)
        {

            mockup_OperationCheck_items.Clear();

            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "타정",
                ITEM = "마손도(%)",
                SPEC = "N/A",
                RANGEVALUE = "-0.3",
                RESULT = "0.3"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "타정",
                ITEM = "경도(kp)",
                SPEC = "N/A",
                RANGEVALUE = "12.0~20.0",
                RESULT = "18.0"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "타정",
                ITEM = "두께(mm)",
                SPEC = "N/A",
                RANGEVALUE = "5.6~6.1",
                RESULT = "5.6"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                ITEM = "항습도(%)",
                SPEC = "N/A",
                RANGEVALUE = "-0.3",
                RESULT = "2.80"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "타정",
                ITEM = "마손도(%)",
                SPEC = "N/A",
                RANGEVALUE = "-0.3",
                RESULT = "0.3"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "타정",
                ITEM = "경도(kp)",
                SPEC = "N/A",
                RANGEVALUE = "12.0~20.0",
                RESULT = "18.0"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "타정",
                ITEM = "두께(mm)",
                SPEC = "N/A",
                RANGEVALUE = "5.6~6.1",
                RESULT = "5.6"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                ITEM = "비중",
                SPEC = "N/A",
                RANGEVALUE = "1.005~1.015",
                RESULT = "1.000"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                ITEM = "성상",
                SPEC = "미황색의 액",
                RANGEVALUE = "N/A",
                RESULT = "적합"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                ITEM = "pH",
                SPEC = "N/A",
                RANGEVALUE = "10.00~11.00",
                RESULT = "10.49"
            });
            mockup_OperationCheck_items.Add(new Mockup_classes.Mockup_OperationCheck_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                ITEM = "온도(C)",
                SPEC = "N/A",
                RANGEVALUE = "19.2~23.2",
                RESULT = "22.9"
            });



            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합속도 (분)",
                SPEC = "5",
                RANGEVALUE = "N/A",
                RESULT = "5"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합시간(rpm)",
                SPEC = "20",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "N/A",
                ITEM = "체망규격 (mesh)",
                SPEC = "16",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Agliator (rpm)",
                SPEC = "105",
                RANGEVALUE = "N/A",
                RESULT = "105"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Chopper (rpm)",
                SPEC = "2,200",
                RANGEVALUE = "N/A",
                RESULT = "2,200"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "혼합시간 (초)",
                SPEC = "120",
                RANGEVALUE = "60~80",
                RESULT = "120"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정속도 (rpm)",
                SPEC = "50",
                RANGEVALUE = "20~80",
                RESULT = "50"

            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04008",
                LOTID = "H04008",
                MTRLNAME = "나푸롤정",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정압력 (KN)",
                SPEC = "N/A",
                RANGEVALUE = "10.0~50.0",
                RESULT = "22.1"

            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합속도 (분)",
                SPEC = "5",
                RANGEVALUE = "N/A",
                RESULT = "5"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합시간(rpm)",
                SPEC = "20",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "N/A",
                ITEM = "체망규격 (mesh)",
                SPEC = "16",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Agliator (rpm)",
                SPEC = "105",
                RANGEVALUE = "N/A",
                RESULT = "105"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Chopper (rpm)",
                SPEC = "2,200",
                RANGEVALUE = "N/A",
                RESULT = "2,200"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "혼합시간 (초)",
                SPEC = "120",
                RANGEVALUE = "60~80",
                RESULT = "120"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정속도 (rpm)",
                SPEC = "50",
                RANGEVALUE = "20~80",
                RESULT = "50"

            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH04006",
                LOTID = "H04006",
                MTRLNAME = "아로나민골드정(내수용)",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정압력 (KN)",
                SPEC = "N/A",
                RANGEVALUE = "10.0~50.0",
                RESULT = "22.1"

            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합속도 (분)",
                SPEC = "5",
                RANGEVALUE = "N/A",
                RESULT = "5"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "100401AG11/Drum Mixer",
                ITEM = "혼합시간(rpm)",
                SPEC = "20",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H04006",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "N/A",
                ITEM = "체망규격 (mesh)",
                SPEC = "16",
                RANGEVALUE = "N/A",
                RESULT = "20"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Agliator (rpm)",
                SPEC = "105",
                RANGEVALUE = "N/A",
                RESULT = "105"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "Chopper (rpm)",
                SPEC = "2,200",
                RANGEVALUE = "N/A",
                RESULT = "2,200"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "조제",
                EQCLID = "100801/High Speed Mixer",
                ITEM = "혼합시간 (초)",
                SPEC = "120",
                RANGEVALUE = "60~80",
                RESULT = "120"
            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정속도 (rpm)",
                SPEC = "50",
                RANGEVALUE = "20~80",
                RESULT = "50"

            });
            mockup_CriticalParameter_items.Add(new Mockup_classes.Mockup_CriticalParameter_item()
            {
                POID = "ODH03005",
                LOTID = "H03005",
                MTRLNAME = "판토메드주사",
                OPSGNAME = "타정",
                EQCLID = "200110/FETTE 45",
                ITEM = "타정압력 (KN)",
                SPEC = "N/A",
                RANGEVALUE = "10.0~50.0",
                RESULT = "22.1"

            });

            _mockup_OperationCheck1_items.Clear();
            _mockup_CriticalParameter1_items.Clear();

            foreach (var row in mockup_OrderSummary_items)
            {
                if (row.CHECKBOX == true)
                {
                    var temp = from o in mockup_OperationCheck_items
                               where o.POID == row.POID.ToString()
                               select o;
                    temp.ToList().ForEach(o => _mockup_OperationCheck1_items.Add(o));

                    var temp1 = from o in mockup_CriticalParameter_items
                                where o.POID == row.POID.ToString()
                                select o;
                    temp1.ToList().ForEach(o => _mockup_CriticalParameter1_items.Add(o));

                }
            }


        }
        public void _FilterTypeCBO_OnExecuteCompleted(string ruleName)
        {
            _TreeFilterCondition.howFiltering = _FilterTypeCBO.OUTDATAs[0].CMCODE;
            OnPropertyChanged("TreeFilterCondition");
        }
        void __PLANDATEOPTIONOnExecuteCompeted(string ruleName)
        {
            _TreeSearchCondition.PlanDateOption = _PLANDATEOPTION.OUTDATAs[0].CMCODE;
        }
        void __CLOSEDATEOPTIONOnExecuteCompleted(string ruleName)
        {
            _TreeSearchCondition.ClosedDateOtion = _CLOSEDATEOPTION.OUTDATAs[0].CMCODE;
        }

        public bool SearchTreeViewCondition()
        {

            _PlanDateOption = null;
            _isPLAN = null;
            _isAPPR = null;
            _isPROC = null;
            _isPREP = null;
            _isCOMP = null;
            _isREQS = null;
            _isCNCL = null;



            if (!_TreeSearchCondition.isPlanned && !_TreeSearchCondition.isApproved && !_TreeSearchCondition.isCompleted &&
                !_TreeSearchCondition.isProcessing && !_TreeSearchCondition.isPreparing && !_TreeSearchCondition.isRequesting && !_TreeSearchCondition.isCanceled)
            {
                //2012-10-19 메세지 수정. MSGID : PHM10070 
                string message = LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "PHM10070");
                C1MessageBox.Show(message, "Warning", C1MessageBoxIcon.Warning); //PHM10070 하나 이상의 Status를 선택 후 조회해주세요.
                return false;
            }
            if (_TreeSearchCondition.isPlanned)
            {
                _isPLAN = "PLAN";
                _PlanDateOption = _TreeSearchCondition.PlanDateOption;
            }
            if (_TreeSearchCondition.isApproved)
            {
                _isAPPR = "APPR";
            }
            if (_TreeSearchCondition.isCompleted)
            {
                _isCOMP = "COMP";
            }
            if (_TreeSearchCondition.isProcessing)
            {
                _isPROC = "PROC";
            }
            if (_TreeSearchCondition.isPreparing)
            {
                _isPREP = "PREP";
            }
            if (_TreeSearchCondition.isRequesting)
            {
                _isREQS = "REQS";
            }
            if (_TreeSearchCondition.isCanceled)
            {
                _isCNCL = "CNCL";
            }

            if (_TreeSearchCondition.isHolding)
            {
                _ISHOLD = "Y";
            }
            else
            {
                _ISHOLD = null;
            }

            if (_TreeSearchCondition.isPeriod)
            {
                _PeriodSTDTTM = _TreeSearchCondition.PeriodSTDTTM;
                _PeriodEDDTTM = _TreeSearchCondition.PeriodEDDTTM;

                if (_PeriodSTDTTM > _PeriodEDDTTM)
                {
                    string message = LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "PHM10554");
                    C1MessageBox.Show(message, "Warning", C1MessageBoxIcon.Warning); //PHM10554 선택된 날짜를 다시 확인해주세요.
                    return false;
                }
            }
            else
            {
                _PeriodSTDTTM = null;
                _PeriodEDDTTM = null;
            }


            if (_TreeSearchCondition.MTRLID == null || _TreeSearchCondition.MTRLID.Equals(""))
            {
                _MTRLID = null;
            }
            else
            {
                _MTRLID = _TreeSearchCondition.MTRLID;
            }

            return true;

        }
        #endregion

    }
}

