using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.ServiceModel;

namespace WMS
{
    public class PartWashingViewModel :ViewModelBase
    {
        #region [Property]

        PartWashing _minaWnd;
        TooidInputAdd pop;
        public WashingMessageBox MPop;
        private DispatcherTimer _repeater;
        private int _repeaterInterval = 3000;   // 100ms -> 500ms -> 1000ms

        private string _WSGUID = string.Empty;
        private string _EquipRecipeNo = string.Empty;
        private string _EquipRecipeName = string.Empty;
        private string _EqiuptID = string.Empty;
        private string _EquiptName = string.Empty;
        private string _RecipeNo = string.Empty;
        private string _RecipeName = string.Empty;
        private string _Status = string.Empty;
        private bool _isStart = false;
        private bool _isLoad = false;
        private bool _isForceExit = false;

        private bool _IsBusyA;
        [Browsable(false)]
        public bool IsBusyA
        {
            get
            {
                return _IsBusyA;
            }
            set
            {
                if (_IsBusyA != value)
                {
                    _IsBusyA = value;
                    OnPropertyChanged("IsBusyA");
                }
            }
        }

        private bool _isBusyB;
        [Browsable(false)]
        public bool isBusyB
        {
            get
            {
                return _isBusyB;
            }
            set
            {
                if (_isBusyB != value)
                {
                    _IsBusyA = value;
                    OnPropertyChanged("isBusyB");
                }
            }
        }

        private string _tbheader;
        public string tbheader
        {
            get { return _tbheader; }
            set
            {
                _tbheader = value;
                OnPropertyChanged("tbheader");
            }
        }

        private string _clrHeader;
        public string clrheader
        {
            get { return _clrHeader; }
            set
            {
                _clrHeader = value;
                OnPropertyChanged("clrheader");
            }
        }

        private string _tbGridcount;
        public string tbGridcount
        {
            get { return _tbGridcount; }
            set
            {
                _tbGridcount = value;
                OnPropertyChanged("tbGridcount");
            }
        }

        private string _tbFooter;
        public string tbFooter
        {
            get { return _tbFooter; }
            set
            {
                _tbFooter = value;
                OnPropertyChanged("tbFooter");
            }
        }

        private string _tbFooter1;
        public string tbFooter1
        {
            get { return _tbFooter1; }
            set
            {
                _tbFooter1 = value;
                OnPropertyChanged("tbFooter1");
            }
        }

        private string _clrFooter;
        public string clrFooter
        {
            get { return _clrFooter; }
            set
            {
                _clrFooter = value;
                OnPropertyChanged("clrFooter");
            }
        }

        private string _clrFooter2;
        public string clrFooter2
        {
            get { return _clrFooter2; }
            set
            {
                _clrFooter2 = value;
                OnPropertyChanged("clrFooter2");
            }
        }

        private string _txtToolid;
        public string txtToolid
        {
            get { return _txtToolid; }
            set
            {
                _txtToolid = value;
                OnPropertyChanged("txtToolid");
            }
        }

        private bool _rdHeader;
        public bool rdHeader
        {
            get { return _rdHeader; }
            set
            {
                _rdHeader = value;
                OnPropertyChanged("rdHeader");
            }
        }

        private string _txtbtnsave;
        public string txtbtnsave
        {
            get { return _txtbtnsave; }
            set
            {
                _txtbtnsave = value;
                OnPropertyChanged("txtbtnsave");
            }
        }

        private string _txtStatus;
        public string txtStatus
        {
            get { return _txtStatus; }
            set
            {
                _txtStatus = value;
                OnPropertyChanged("txtStatus");
            }
        }

        private ObservableCollection<Toolinfo> _SEL_Toolinfo;
        public ObservableCollection<Toolinfo> SEL_Toolinfo
        {
            get { return _SEL_Toolinfo; }
            set
            {
                _SEL_Toolinfo = value;
                OnPropertyChanged("SEL_Toolinfo");
            }
        }

        private Toolinfo _Selecteditem;
        public Toolinfo Seleteditem
        {
            get { return _Selecteditem; }
            set
            {
                _Selecteditem = value;
                OnPropertyChanged("Seleteditem");
            }
        }

        private ObservableCollection<Toolinfo> _SEL_Toolinfo2;
        public ObservableCollection<Toolinfo> SEL_Toolinfo2
        {
            get { return _SEL_Toolinfo2; }
            set
            {
                _SEL_Toolinfo2 = value;
                OnPropertyChanged("SEL_Toolinfo2");
            }
        }

        private double _Dryheight1;
        public double Dryheight1
        {
            get { return _Dryheight1; }
            set
            {
                _Dryheight1 = value;
                OnPropertyChanged("Dryheight1");
            }
        }

        private double _Dryheight2;
        public double Dryheight2
        {
            get { return _Dryheight2; }
            set
            {
                _Dryheight2 = value;
                OnPropertyChanged("Dryheight2");
            }
        }

        private string _CleanDateFrom;
        public string CleanDateFrom
        {
            get { return _CleanDateFrom; }
            set
            {
                _CleanDateFrom = value;
                OnPropertyChanged("CleanDateFrom");
            }
        }

        private string _CleanDateTo;
        public string CleanDateTo
        {
            get { return _CleanDateTo; }
            set
            {
                _CleanDateTo = value;
                OnPropertyChanged("CleanDateTo");
            }
        }

        private bool _IsWashing;
        public bool IsWashing
        {
            get { return _IsWashing; }
            set
            {
                _IsWashing = value;
                OnPropertyChanged("IsWashing");
            }
        }

        private bool _isDryComplete;
        public bool isDryComplete
        {
            get { return _isDryComplete; }
            set
            {
                _isDryComplete = value;
                OnPropertyChanged("isDryComplete");
            }
        }

        private string _tbDryGridcount;
        public string tbDryGridcount
        {
            get { return _tbDryGridcount; }
            set
            {
                _tbDryGridcount = value;
                OnPropertyChanged("tbDryGridcount");
            }
        }

        private DryHistory _SelectDryHistory;
        public DryHistory SelectDryHistory
        {
            get { return _SelectDryHistory; }
            set
            {
                _SelectDryHistory = value;
                OnPropertyChanged("SelectDryHistory");
            }
        }

        private ObservableCollection<DryHistory> _DryHistory;
        public ObservableCollection<DryHistory> DryHistory
        {
            get { return _DryHistory; }
            set
            {
                _DryHistory = value;
                OnPropertyChanged("DryHistory");
            }
        }

        private DryDetail _SelectDryDetail;
        public DryDetail SelectDryDetail
        {
            get { return _SelectDryDetail; }
            set
            {
                _SelectDryDetail = value;
                OnPropertyChanged("SelectDryDetail");
            }
        }

        private ObservableCollection<DryDetail> _DryDetail;
        public ObservableCollection<DryDetail> DryDetail
        {
            get { return _DryDetail; }
            set
            {
                _DryDetail = value;
                OnErrorChanged("DryDetail");
            }
        }

        private string _tbDry2Gridcount;
        public string tbDry2Gridcount
        {
            get { return _tbDry2Gridcount; }
            set
            {
                _tbDry2Gridcount = value;
                OnPropertyChanged("tbDry2Gridcount");
            }
        }

        private string _txtPartWasher;
        public string txtPartWasher
        {
            get { return _txtPartWasher; }
            set
            {
                _txtPartWasher = value;
                OnPropertyChanged("txtPartWasher");
            }
        }

        private string _txtCleanRecipe;
        public string txtCleanRecipe
        {
            get { return _txtCleanRecipe; }
            set
            {
                _txtCleanRecipe = value;
                OnPropertyChanged("txtCleanRecipe");
            }
        }

        private string _txtDryStartDate;
        public string txtDryStartDate
        {
            get { return _txtDryStartDate; }
            set
            {
                _txtDryStartDate = value;
                OnPropertyChanged("txtDryStartDate");
            }
        }

        private string _txtDryEndDate;
        public string txtDryEndDate
        {
            get { return _txtDryEndDate; }
            set
            {
                _txtDryEndDate = value;
                OnPropertyChanged("txtDryEndDate");
            }
        }

        private bool _isWashEble;
        public bool isWashEble
        {
            get { return _isWashEble; }
            set
            {
                _isWashEble = value;
                OnPropertyChanged("isWashEble");
            }
        }

        private bool _isDryEble;
        public bool isDryEble
        {
            get { return _isDryEble; }
            set
            {
                _isDryEble = value;
                OnPropertyChanged("isDryEble");
            }
        }

        private bool _isDateago;
        public bool isDateago
        {
            get { return _isDateago; }
            set
            {
                _isDateago = value;
                OnPropertyChanged("isDateago");
            }
        }

        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_UsedToolInfo _BR_BRS_SEL_UsedToolInfo;
        public BR_BRS_SEL_UsedToolInfo BR_BRS_SEL_UsedToolInfo
        {
            get { return _BR_BRS_SEL_UsedToolInfo; }
            set
            {
                _BR_BRS_SEL_UsedToolInfo = value;
                OnPropertyChanged("BR_BRS_SEL_UsedToolInfo");
            }
        }

        private BR_BRS_REG_PartWashingHistory _BR_BRS_REG_PartWashingHistory;
        public BR_BRS_REG_PartWashingHistory BR_BRS_REG_PartWashingHistory
        {
            get { return _BR_BRS_REG_PartWashingHistory; }
            set
            {
                _BR_BRS_REG_PartWashingHistory = value;
                OnPropertyChanged("BR_BRS_REG_PartWashingHistory");
            }
        }

        private BR_BRS_SEL_PartWasherInfo _BR_BRS_SEL_PartWasherInfo;
        public BR_BRS_SEL_PartWasherInfo BR_BRS_SEL_PartWasherInfo
        {
            get { return _BR_BRS_SEL_PartWasherInfo; }
            set
            {
                _BR_BRS_SEL_PartWasherInfo = value;
                OnPropertyChanged("BR_BRS_SEL_PartWasherInfo");
            }
        }

        private BR_BRS_SEL_PartWashingHistory _BR_BRS_SEL_PartWashingHistory;
        public BR_BRS_SEL_PartWashingHistory BR_BRS_SEL_PartWashingHistory
        {
            get { return _BR_BRS_SEL_PartWashingHistory; }
            set
            {
                _BR_BRS_SEL_PartWashingHistory = value;
                OnPropertyChanged("BR_BRS_SEL_PartWashingHistory");
            }
        }

        private BR_BRS_SEL_PartWashingDetail _BR_BRS_SEL_PartWashingDetail;
        public BR_BRS_SEL_PartWashingDetail BR_BRS_SEL_PartWashingDetail
        {
            get { return _BR_BRS_SEL_PartWashingDetail; }
            set
            {
                _BR_BRS_SEL_PartWashingDetail = value;
                OnPropertyChanged("BR_BRS_SEL_PartWashingDetail");
            }
        }

        private BR_BRS_REG_PartWashingComplete _BR_BRS_REG_PartWashingComplete;
        public BR_BRS_REG_PartWashingComplete BR_BRS_REG_PartWashingComplete
        {
            get { return _BR_BRS_REG_PartWashingComplete; }
            set
            {
                _BR_BRS_REG_PartWashingComplete = value;
                OnPropertyChanged("BR_BRS_REG_PartWashingComplete");
            }
        }

        private BR_BRS_REG_PartWashingInitiate _BR_BRS_REG_PartWashingInitiate;
        public BR_BRS_REG_PartWashingInitiate BR_BRS_REG_PartWashingInitiate
        {
            get { return _BR_BRS_REG_PartWashingInitiate; }
            set
            {
                _BR_BRS_REG_PartWashingInitiate = value;
                OnPropertyChanged("BR_BRS_REG_PartWashingInitiate");
            }
        }

        private BR_BRS_REG_PartWashingStart _BR_BRS_REG_PartWashingStart;
        public BR_BRS_REG_PartWashingStart BR_BRS_REG_PartWashingStart
        {
            get { return _BR_BRS_REG_PartWashingStart; }
            set
            {
                _BR_BRS_REG_PartWashingStart = value;
                OnPropertyChanged("BR_BRS_REG_PartWashingStart");
            }
        }

        private BR_BRS_REG_PartWashingEnd _BR_BRS_REG_PartWashingEnd;
        public BR_BRS_REG_PartWashingEnd BR_BRS_REG_PartWashingEnd
        {
            get { return _BR_BRS_REG_PartWashingEnd; }
            set
            {
                _BR_BRS_REG_PartWashingEnd = value;
                OnPropertyChanged("BR_BRS_REG_PartWashingEnd");
            }
        }

        private BR_BRS_SEL_PartWashingInfo_RoomNo _BR_BRS_SEL_PartWashingInfo_RoomNo;
        public BR_BRS_SEL_PartWashingInfo_RoomNo BR_BRS_SEL_PartWashingInfo_RoomNo
        {
            get { return _BR_BRS_SEL_PartWashingInfo_RoomNo; }
            set
            {
                _BR_BRS_SEL_PartWashingInfo_RoomNo = value;
                OnPropertyChanged("BR_BRS_SEL_PartWashingInfo_RoomNo");
            }
        }

        private BR_BRS_CHK_PartWashingRecipe _BR_BRS_CHK_PartWashingRecipe;
        public BR_BRS_CHK_PartWashingRecipe BR_BRS_CHK_PartWashingRecipe
        {
            get { return _BR_BRS_CHK_PartWashingRecipe; }
            set
            {
                _BR_BRS_CHK_PartWashingRecipe = value;
                OnPropertyChanged("BR_BRS_CHK_PartWashingRecipe");
            }
        }

        #region [OnExecuteCompleted]

        void _BR_BRS_SEL_UsedToolInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_UsedToolInfo.OUTDATAs.Count > 0)
            {
                foreach (var s in _BR_BRS_SEL_UsedToolInfo.OUTDATAs)
                {
                    var match = _SEL_Toolinfo.Where(o => o.TOOLID == s.TOOLID).FirstOrDefault();
                    if (match != null)
                    {
                        OnMessage("동일한 도구가 등록되있습니다.");
                        return;
                    }
                }

                decimal Rc = _SEL_Toolinfo.Count;

                _SEL_Toolinfo2.Clear();

                foreach (var itemlist in _SEL_Toolinfo)
                {
                    _SEL_Toolinfo2.Add(
                       new Toolinfo
                       {
                           RowIndex = itemlist.RowIndex,
                           RowEditSec = itemlist.RowEditSec,
                           Seq = itemlist.Seq,
                           TOOLID = itemlist.TOOLID,
                           TOOLNAME = itemlist.TOOLNAME,
                           MTRLID = itemlist.MTRLID,
                           MTRLNAME = itemlist.MTRLNAME,
                           USEDDTTM = itemlist.USEDDTTM,
                           RECIPENO = itemlist.RECIPENO,
                           RECIPENAME = itemlist.RECIPENAME,
                           OPSGNAME = itemlist.OPSGNAME,
                           OPSGGUID = itemlist.OPSGGUID,
                           BATCHNO = itemlist.BATCHNO,
                           PROC_OFFTIME = itemlist.PROC_OFFTIME,
                           PRIORTY = itemlist.PRIORTY,
                           POID = itemlist.POID
                       });
                }

                _SEL_Toolinfo.Clear();

                foreach (var item in _BR_BRS_SEL_UsedToolInfo.OUTDATAs)
                {
                    _SEL_Toolinfo.Add(
                        new Toolinfo
                        {
                            RowIndex = "0",
                            RowEditSec = "INS",
                            Seq = (int)Rc + 1,
                            TOOLID = item.TOOLID,
                            TOOLNAME = item.TOOLNAME,
                            MTRLID = item.MTRLID,
                            MTRLNAME = item.MTRLNAME,
                            OPSGGUID = item.OPSGGUID,
                            OPSGNAME = item.OPSGNAME,
                            USEDDTTM = item.USEDDTTM,
                            RECIPENO = item.RECIPENO,
                            RECIPENAME = item.RECIPENAME,
                            BATCHNO = item.BATCHNO,
                            PROC_OFFTIME = item.PROC_OFFTIME,
                            PRIORTY = item.PRIORITY,
                            POID = item.POID
                        });
                }

                foreach (var list in _SEL_Toolinfo2)
                {
                    _SEL_Toolinfo.Add(
                        new Toolinfo
                        {
                            RowIndex = "0",
                            RowEditSec = "INS",
                            Seq = list.Seq,
                            TOOLID = list.TOOLID,
                            TOOLNAME = list.TOOLNAME,
                            MTRLID = list.MTRLID,
                            MTRLNAME = list.MTRLNAME,
                            OPSGGUID = list.OPSGGUID,
                            OPSGNAME = list.OPSGNAME,
                            USEDDTTM = list.USEDDTTM,
                            RECIPENO = list.RECIPENO,
                            RECIPENAME = list.RECIPENAME,
                            BATCHNO = list.BATCHNO,
                            PROC_OFFTIME = list.PROC_OFFTIME,
                            PRIORTY = list.PRIORTY,
                            POID = list.POID
                        });
                }

            }
        }

        void _BR_BRS_REG_PartWashingHistory_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_REG_PartWashingHistory.OUTDATAs.Count > 0)
            {
                _WSGUID = _BR_BRS_REG_PartWashingHistory.OUTDATAs.Where(o => o.WSGUID != null).Select(o => o.WSGUID).FirstOrDefault();
            }
        }

        void _BR_BRS_SEL_PartWasherInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_PartWasherInfo.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_SEL_PartWasherInfo.OUTDATAs)
                {
                    _EqiuptID = item.EQPTID.ToString();
                    _EquiptName = item.EQPTNAME.ToString();
                    _EquipRecipeNo = item.RECIPENO.ToString();
                    _EquipRecipeName = item.RECIPENAME.ToString();
                }
            }
        }

        void _BR_BRS_SEL_PartWashingHistory_OnExecuteCompleted(string ruleName)
        {
            if (_isLoad)
            {
                if (_BR_BRS_SEL_PartWashingHistory.OUTDATAs.Count > 0)
                {
                    foreach (var item in _BR_BRS_SEL_PartWashingHistory.OUTDATAs)
                    {
                        _EqiuptID = item.EQPTID;
                        _EquiptName = item.EQPTNAME;
                        _EquipRecipeNo = item.RECIPENO;
                        _EquipRecipeName = item.RECIPENAME;
                        _WSGUID = item.WSGUID;

                        LoadDetailTooladd(item.WSGUID);
                    }
                }
            }
            else
            {
                _DryHistory.Clear();
                if (_BR_BRS_SEL_PartWashingHistory.OUTDATAs.Count > 0)
                {
                    int Seq = 0;
                    foreach (var item in _BR_BRS_SEL_PartWashingHistory.OUTDATAs)
                    {
                        if (Seq != 0)
                        {
                            Seq--;
                        }
                        else
                        {
                            Seq = _BR_BRS_SEL_PartWashingHistory.OUTDATAs.Count;
                        }

                        _DryHistory.Add(new DryHistory
                        {
                            RowEditSec = item.RowEditSec,
                            RowIndex = (_DryHistory.Count + 1).ToString(),
                            RowLoadedFlag = item.RowLoadedFlag,
                            WSGUID = item.WSGUID,
                            EQPTID = item.EQPTID,
                            EQPTNAME = item.EQPTNAME,
                            RECIPENO = item.RECIPENO,
                            RECIPENAME = item.RECIPENAME,
                            EQSTONDTTM = item.EQSTONDTTM != null ? Convert.ToDateTime(item.EQSTONDTTM).ToString("yyyy-MM-dd HH:mm") : string.Empty,
                            EQSTOFFDTTM = item.EQSTOFFDTTM != null ? Convert.ToDateTime(item.EQSTOFFDTTM).ToString("yyyy-MM-dd HH:mm") : string.Empty,
                            STATUS = item.STATUS,
                            CNT = item.CNT,
                            SEQ = Seq.ToString(),
                            CLEANRECIPE = item.RECIPENO + " " + item.RECIPENAME
                        });
                    }
                    tbDryGridcount = _DryHistory.Count.ToString() + " 건";
                    if (_DryHistory.Count > 0)
                    {
                        _minaWnd.DryDataGrid.SelectedIndex = 0;
                    }
                }
            }
        }

        async void _BR_BRS_SEL_PartWashingDetail_OnExecuteCompleted(string ruleName)
        {
            if (_isLoad)
            {
                _DryHistory.Clear();
                if (_BR_BRS_SEL_PartWashingDetail.OUTDATAs.Count > 0)
                {
                    int Seq = 0;
                    foreach (var item in _BR_BRS_SEL_PartWashingDetail.OUTDATAs)
                    {
                        if (Seq != 0)
                        {
                            Seq--;
                        }
                        else
                        {
                            Seq = _BR_BRS_SEL_PartWashingHistory.OUTDATAs.Count;
                        }

                        _SEL_Toolinfo.Add(
                            new Toolinfo
                            {
                                RowIndex = "0",
                                RowEditSec = "INS",
                                Seq = Seq,
                                TOOLID = item.TOOLID,
                                TOOLNAME = item.TOOLNAME,
                                MTRLID = item.MTRLID,
                                MTRLNAME = item.MTRLID,
                                OPSGGUID = item.OPSGGUID,
                                OPSGNAME = item.OPSGGUID,
                                USEDDTTM = item.USEDDTTM,
                                RECIPENO = _EquipRecipeNo,
                                RECIPENAME = _EquipRecipeName,
                                PRIORTY = item.PRIORITY,
                                BATCHNO = ""

                            });
                    }

                    _tbGridcount = _SEL_Toolinfo.Count.ToString() + " 건";
                    if (_SEL_Toolinfo.Count > 0)
                    {
                        _minaWnd.CleanRecipeDataGrid.SelectedIndex = 0;

                        int? _PRIORTY = 0;

                        _PRIORTY = _SEL_Toolinfo.OrderBy(o => o.PRIORTY).Select(o => o.PRIORTY).FirstOrDefault();

                        if (_PRIORTY != null)
                        {
                            _RecipeNo = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENO).FirstOrDefault();
                            _RecipeName = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENAME).FirstOrDefault();
                            tbheader = _RecipeNo + " - " + _RecipeName;

                            _BR_BRS_CHK_PartWashingRecipe.INDATAs.Clear();
                            _BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Clear();

                            _BR_BRS_CHK_PartWashingRecipe.INDATAs.Add(new BR_BRS_CHK_PartWashingRecipe.INDATA
                            {
                                TOOLRECIPENO = _RecipeNo,
                                EQUIPMENTRECIPENO = _EquipRecipeNo
                            });

                            await _BR_BRS_CHK_PartWashingRecipe.Execute();
                        }
                    }
                }

                _isLoad = false;
            }
            else
            {
                _DryDetail.Clear();
                if (_BR_BRS_SEL_PartWashingDetail.OUTDATAs.Count > 0)
                {
                    foreach (var item in _BR_BRS_SEL_PartWashingDetail.OUTDATAs)
                    {
                        _DryDetail.Add(new DryDetail
                        {
                            SEQ = item.SEQ,
                            TOOLID = item.TOOLID,
                            TOOLNAME = item.TOOLNAME,
                            STATUS = item.STATUS,
                            DryingComplete = false,
                            Rewashing = false,
                            TOOLINFO = item.TOOLID + " - " + item.TOOLNAME,
                            RowEditSec = item.RowEditSec,
                            RowIndex = (_DryDetail.Count + 1).ToString(),
                            RowLoadedFlag = item.RowLoadedFlag
                        });
                    }
                    tbDry2Gridcount = _DryDetail.Count.ToString() + " 건";
                }
            }

        }

        void _BR_BRS_SEL_PartWashingInfo_RoomNo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_PartWashingInfo_RoomNo.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_SEL_PartWashingInfo_RoomNo.OUTDATAs)
                {
                    _EqiuptID = item.EQPTID;
                    _EquiptName = item.EQPTNAME;
                }
            }
        }

        void _BR_BRS_CHK_PartWashingRecipe_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Count > 0)
            {
                if (_BR_BRS_CHK_PartWashingRecipe.OUTDATAs[0].RESULT_CODE.ToString() == "NG")
                {
                    //OnMessage(_BR_BRS_CHK_PartWashingRecipe.OUTDATAs[0].RESULT_MSG.ToString());

                    clrFooter = "#FFEB4A4A";

                    MPop = new WashingMessageBox();
                    MPop.DataContext = this;
                    MPop.Message = _BR_BRS_CHK_PartWashingRecipe.OUTDATAs[0].RESULT_MSG.ToString();
                    MPop.MessageStaus(WashingMessageBox.MessageStatusType.Error);
                    MPop.ShowCloseButton = false;
                    MPop.CenterOnScreen();
                    MPop.Closed += (s, e) =>
                    {
                        MPop.isLoaded = false;
                        IsBusy = false;
                        MPop.Close();
                    };
                    IsBusy = true;
                    MPop.Show();
                    return;
                }
                else
                {
                    clrFooter = "#FF8AEB8A";
                }
            }
        }
        #endregion

        #endregion
        
        #region [Commnad]

        public ICommand LoadCommandAsync { get; set; }

        public ICommand AddCommnadAsync { get; set; }
        
        public ICommand InitiateCommnadAsync { get; set; }

        public ICommand KeyDownCommandAsync { get; set; }

        public ICommand SelectChangeCommand { get; set; }

        public ICommand RemoveCommandAsync { get; set; }

        public ICommand SaveCommnadAsync { get; set; }

        public ICommand SerchDryCommand { get; set; }

        public ICommand OpenDryCommnadAsync { get; set; }

        public ICommand SelectDryCommand { get; set; }

        public ICommand SaveDryCommnadAsync { get; set; }

        #endregion

        #region [Generator]

        public PartWashingViewModel()
        {
            _BR_BRS_SEL_UsedToolInfo = new BR_BRS_SEL_UsedToolInfo();
            _BR_BRS_SEL_UsedToolInfo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_UsedToolInfo_OnExecuteCompleted);
            _BR_BRS_REG_PartWashingHistory = new BR_BRS_REG_PartWashingHistory();
            _BR_BRS_REG_PartWashingHistory.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_REG_PartWashingHistory_OnExecuteCompleted);
            _SEL_Toolinfo = new ObservableCollection<Toolinfo>();
            _Selecteditem = new Toolinfo();
            _SEL_Toolinfo2 = new ObservableCollection<Toolinfo>();
            _BR_BRS_SEL_PartWasherInfo = new BR_BRS_SEL_PartWasherInfo();
            _BR_BRS_SEL_PartWasherInfo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_PartWasherInfo_OnExecuteCompleted);
            _BR_BRS_SEL_PartWashingHistory = new BR_BRS_SEL_PartWashingHistory();
            _BR_BRS_SEL_PartWashingHistory.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_PartWashingHistory_OnExecuteCompleted);
            _DryHistory = new ObservableCollection<DryHistory>();
            _SelectDryHistory = new DryHistory();
            _BR_BRS_SEL_PartWashingDetail = new BR_BRS_SEL_PartWashingDetail();
            _BR_BRS_SEL_PartWashingDetail.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_PartWashingDetail_OnExecuteCompleted);
            _BR_BRS_REG_PartWashingInitiate = new BR_BRS_REG_PartWashingInitiate();
            _BR_BRS_REG_PartWashingStart = new BR_BRS_REG_PartWashingStart();
            _BR_BRS_REG_PartWashingEnd = new BR_BRS_REG_PartWashingEnd();
            _BR_BRS_CHK_PartWashingRecipe = new LGCNS.iPharmMES.Common.BR_BRS_CHK_PartWashingRecipe();
            _BR_BRS_CHK_PartWashingRecipe.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_CHK_PartWashingRecipe_OnExecuteCompleted);
            _DryDetail = new ObservableCollection<DryDetail>();
            _SelectDryDetail = new DryDetail();
            LoadCommandAsync = new CommandBase(Loaded);
            AddCommnadAsync = new CommandBase(Added);
            InitiateCommnadAsync = new CommandBase(Initiate);
            KeyDownCommandAsync = new CommandBase(KeyDown);
            SelectChangeCommand = new CommandBase(SelectChange);
            RemoveCommandAsync = new CommandBase(Remove);
            SaveCommnadAsync = new CommandBase(Save);
            SerchDryCommand = new CommandBase(SerchDry);
            OpenDryCommnadAsync = new CommandBase(OpenDetail);
            SelectDryCommand = new CommandBase(SelectDry);
            SaveDryCommnadAsync = new CommandBase(SaveDry);

            _BR_BRS_REG_PartWashingComplete = new BR_BRS_REG_PartWashingComplete();
            _BR_BRS_SEL_PartWashingInfo_RoomNo = new BR_BRS_SEL_PartWashingInfo_RoomNo();
            _BR_BRS_SEL_PartWashingInfo_RoomNo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_PartWashingInfo_RoomNo_OnExecuteCompleted);

            tbheader = string.Empty;
            tbFooter = string.Empty;
            tbFooter1 = string.Empty;
            tbGridcount = string.Empty;
            clrFooter = "LightGray";
            clrheader = "LightGray";
            tbheader = string.Empty;
            rdHeader = true;
            txtToolid = "";
            txtStatus = "";
            txtbtnsave = "";
            CleanDateFrom = "";
            CleanDateTo = "";
            IsWashing = false;
            isDryComplete = false;
            tbDryGridcount = "";
            tbDry2Gridcount = "";
            txtPartWasher = "";
            txtCleanRecipe = "";
            txtDryStartDate = "";
            txtDryEndDate = "";
            isWashEble = false;
            isDryEble = true;
            isDateago = false;
        }

        #endregion

        #region [Command Excute]

        public async void Loaded(object param)
        {
            IsBusy = true;

            _minaWnd = param as PartWashing;

            _SEL_Toolinfo.Clear();
            OnPropertyChanged("SEL_Toolinfo");
            tbFooter = "";
            tbFooter1 = "";
            tbheader = "";
            tbGridcount = "0 건";
            clrFooter = "LightGray";
            clrheader = "LightGray";
            clrFooter2 = "LightGray";
            _WSGUID = null;

            _BR_BRS_SEL_PartWashingInfo_RoomNo.INDATAs.Clear();
            _BR_BRS_SEL_PartWashingInfo_RoomNo.OUTDATAs.Clear();
            _BR_BRS_SEL_PartWashingInfo_RoomNo.INDATAs.Add(new BR_BRS_SEL_PartWashingInfo_RoomNo.INDATA
            {
                ROOMNO = AuthRepositoryViewModel.Instance.RoomID
            });
            await _BR_BRS_SEL_PartWashingInfo_RoomNo.Execute();
            
            _EquipRecipeNo = "1";
            _EquipRecipeName = "";
            rdHeader = true;

            txtStatus = "세척대기";
            txtbtnsave = "Save";
            _Status = "";
            _isLoad = true;

            _BR_BRS_SEL_PartWashingHistory.INDATAs.Clear();
            _BR_BRS_SEL_PartWashingHistory.OUTDATAs.Clear();
            _BR_BRS_SEL_PartWashingHistory.INDATAs.Add(new BR_BRS_SEL_PartWashingHistory.INDATA
            {
                EQPTID = _EqiuptID,
                STATUS = "READY"
            });
            await _BR_BRS_SEL_PartWashingHistory.Execute();

            isDateago = true;
            CleanDateFrom = (await AuthRepositoryViewModel.GetDBDateTimeNow()).AddDays(-3).ToString("yyyy-MM-dd");
            CleanDateTo = (await AuthRepositoryViewModel.GetDBDateTimeNow()).ToString("yyyy-MM-dd");

            EQPTTIMER();
            pop = new TooidInputAdd();

            MPop = new WashingMessageBox();
            
            IsBusy = false;

        }

        public void Added(object param)
        {
            if (IsBusy != false) return;
            IsBusy = true;

            IsBusy = false;
            pop.DataContext = this;
            pop.txtToolIdS.Text = "";
            txtToolid = "";
            pop.Show();

        }

        public void Initiate(object param)
        {
            if (IsBusy != false) return;
            IsBusy = true;

            MPop = new WashingMessageBox();
            MPop.DataContext = this;
            MPop.Message = "초기화 하시겟습니까?.";
            MPop.MessageStaus(WashingMessageBox.MessageStatusType.initial);
            MPop.ShowCloseButton = false;            
            MPop.CenterOnScreen();
            MPop.btninitial.Click += async(s1, e1) =>
            {

                try
                {
                    var authHelper = new iPharmAuthCommandHelper();

                    authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_Washing");

                    if (await authHelper.ClickAsync(
                        Common.enumCertificationType.Function,
                        Common.enumAccessType.Create,
                        "Part Washing",
                        "Part Washing",
                        false,
                        "EM_Equipment_Washing",
                        "", null, null) == false)
                    {
                        IsBusy = false;
                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                    }

                    if (_WSGUID != null)
                    {
                        _BR_BRS_REG_PartWashingInitiate.INDATAs.Clear();
                        _BR_BRS_REG_PartWashingInitiate.HISTDATAs.Clear();

                        _BR_BRS_REG_PartWashingInitiate.INDATAs.Add(new BR_BRS_REG_PartWashingInitiate.INDATA
                        {
                            WSGUID = _WSGUID,
                            SIGNATUREGUID = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                            USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                        });

                        await _BR_BRS_REG_PartWashingInitiate.Execute();
                    }

                    _SEL_Toolinfo.Clear();
                    tbheader = "";
                    tbGridcount = "0 건";
                    _WSGUID = null;
                    MPop.isLoaded = false;
                    clrheader = "LightGray";
                    clrFooter = "LightGray";
                    MPop.Close();
                }
                catch (Exception ex)
                {
                    OnMessage(ex.Message);
                    IsBusy = false;
                }
                finally
                {
                    IsBusy = false;
                }
            };
            MPop.OKButton.Click += (s2, e2) =>
            {
                MPop.isLoaded = false;
                IsBusy = false;
                MPop.Close();
            };
            IsBusy = true;
            MPop.Show();

        }

        public void KeyDown(object parma)
        {
            if (IsBusy != false) return;
            IsBusy = true;

            ToolListAdd(parma.ToString());
            
            IsBusy = false;
        }

        public void SelectChange(object param)
        {
            if (IsBusy != false) return;

            IsBusy = true;
            if (param != null)
            {
                var tmparam = param as C1.Silverlight.DataGrid.C1DataGrid;

                _Selecteditem = tmparam.SelectedItem as Toolinfo;
            }
            IsBusy = false;
        }

        public async void Remove(object param)
        {
            if (IsBusy != false) return;

            try
            {
                IsBusy = true;

                if (param != null)
                {
                    var temp = param as Toolinfo;
                    var matched = _SEL_Toolinfo.Where(o => o.TOOLID == temp.TOOLID && o.Seq == temp.Seq && o.RowIndex == temp.RowIndex).FirstOrDefault();
                    if (matched != null) _SEL_Toolinfo.Remove(matched);
                    
                    _tbGridcount = _SEL_Toolinfo.Count.ToString() + " 건";
                    OnPropertyChanged("tbGridcount");

                    if (_SEL_Toolinfo.Count > 0)
                    {
                        _minaWnd.CleanRecipeDataGrid.SelectedIndex = 0;
                        
                        int? _PRIORTY = 0;

                        _PRIORTY = _SEL_Toolinfo.OrderBy(o => o.PRIORTY).Select(o => o.PRIORTY).FirstOrDefault();

                        rdHeader = false;
                        if (_PRIORTY != null)
                        {
                            _RecipeNo = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENO).FirstOrDefault();
                            _RecipeName = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENAME).FirstOrDefault();
                            tbheader = _RecipeNo + " - " + _RecipeName;

                            _BR_BRS_CHK_PartWashingRecipe.INDATAs.Clear();
                            _BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Clear();

                            _BR_BRS_CHK_PartWashingRecipe.INDATAs.Add(new BR_BRS_CHK_PartWashingRecipe.INDATA
                            {
                                TOOLRECIPENO = _RecipeNo,
                                EQUIPMENTRECIPENO = _EquipRecipeNo
                            });

                            await _BR_BRS_CHK_PartWashingRecipe.Execute();
                        }
                        rdHeader = true;
                    }
                    else
                    {
                        tbheader = "";
                        clrheader = "LightGray";
                        clrFooter = "LightGray";
                    }

                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
                IsBusy = false;
            }
            finally
            {

                IsBusy = false;
            }
        }

        public async void Save(object param)
        {
            if (IsBusy != false) return;

            try
            {
                IsBusy = true;

                var authHelper = new iPharmAuthCommandHelper();
                
                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_Washing");

                if (await authHelper.ClickAsync(
                    Common.enumCertificationType.Function,
                    Common.enumAccessType.Create,
                    "Part Washing",
                    "Part Washing",
                    false,
                    "EM_Equipment_Washing",
                    "", null, null) == false)
                {
                    IsBusy = false;
                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));                    
                }
                
                int? _PRIORTY = 0;

                _PRIORTY = _SEL_Toolinfo.OrderBy(o => o.PRIORTY).Select(o => o.PRIORTY).FirstOrDefault();

                if (_PRIORTY != null)
                {
                    _RecipeNo = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENO).FirstOrDefault();

                    _BR_BRS_CHK_PartWashingRecipe.INDATAs.Clear();
                    _BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Clear();

                    _BR_BRS_CHK_PartWashingRecipe.INDATAs.Add(new BR_BRS_CHK_PartWashingRecipe.INDATA
                    {
                        TOOLRECIPENO = _RecipeNo,
                        EQUIPMENTRECIPENO = _EquipRecipeNo
                    });

                    await _BR_BRS_CHK_PartWashingRecipe.Execute();
                }

                _BR_BRS_REG_PartWashingHistory.INDATAs.Clear();
                _BR_BRS_REG_PartWashingHistory.INDETAILs.Clear();
                _BR_BRS_REG_PartWashingHistory.OUTDATAs.Clear();

                _BR_BRS_REG_PartWashingHistory.INDATAs.Add(
                    new BR_BRS_REG_PartWashingHistory.INDATA
                    {
                        SECTION = _WSGUID != null ? "UPD" : "INS",
                        WSGUID = _WSGUID,
                        EQPTID = _EqiuptID,
                        RECIPE = _RecipeNo,
                        SIGNATUREGUID = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                        EQSHID = null,
                        STATUS = null,
                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                    });

                foreach (var item in _SEL_Toolinfo)
                {
                    _BR_BRS_REG_PartWashingHistory.INDETAILs.Add(
                        new BR_BRS_REG_PartWashingHistory.INDETAIL
                        {
                            SEQ = item.Seq,
                            TOOLID = item.TOOLID,
                            MTRLID = item.MTRLID,
                            OPSGGUID = item.OPSGGUID,
                            USEDDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow(),
                            POID = item.POID
                        });
                }

                await _BR_BRS_REG_PartWashingHistory.Execute();

                if (!_isForceExit)
                {
                    MPop = new WashingMessageBox();
                    MPop.DataContext = this;
                    MPop.Message = "설비에서 세척 작업을 시작하십시오.";
                    MPop.MessageStaus(WashingMessageBox.MessageStatusType.Normal);
                    MPop.ShowCloseButton = false;
                    MPop.CenterOnScreen();
                    MPop.Closed += (s, e) =>
                    {
                        MPop.isLoaded = false;
                        IsBusy = false;
                        MPop.Close();
                    };
                    IsBusy = true;
                    MPop.Show();
                }
                else
                {
                    _isForceExit = false;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
            }
            finally
            {
            }
        }

        public async void SerchDry(object param)
        {
            IsBusyA = true;

            _BR_BRS_SEL_PartWashingHistory.INDATAs.Clear();
            _BR_BRS_SEL_PartWashingHistory.OUTDATAs.Clear();
            _BR_BRS_SEL_PartWashingHistory.INDATAs.Add(new BR_BRS_SEL_PartWashingHistory.INDATA
            {
                EQPTID = _EqiuptID
            });
            await _BR_BRS_SEL_PartWashingHistory.Execute();

            IsBusyA = false;
        }

        public void SelectDry(object param)
        {
            IsBusyA = true;

            if (param != null)
            {
                var temparm = param as C1.Silverlight.DataGrid.C1DataGrid;
                _SelectDryHistory = temparm.SelectedItem as DryHistory;
            }

            IsBusyA = false;
        }

        public async void OpenDetail(object param)
        {
            try
            {
                IsBusyA = true;

                if (_SelectDryHistory.WSGUID != null)
                {
                    if (_SelectDryHistory.STATUS != "READY")
                    {
                        _BR_BRS_SEL_PartWashingDetail.INDATAs.Clear();
                        _BR_BRS_SEL_PartWashingDetail.OUTDATAs.Clear();
                        _BR_BRS_SEL_PartWashingDetail.INDATAs.Add(new BR_BRS_SEL_PartWashingDetail.INDATA
                        {
                            WSGUID = _SelectDryHistory.WSGUID
                        });
                        await _BR_BRS_SEL_PartWashingDetail.Execute();

                        txtPartWasher = _EqiuptID + " - " + _EquiptName;
                        txtCleanRecipe = _SelectDryHistory.CLEANRECIPE;
                        txtDryStartDate = _SelectDryHistory.EQSTONDTTM.Length > 0 ? Convert.ToDateTime(_SelectDryHistory.EQSTONDTTM).ToString("yyyy-MM-dd HH:mm") : string.Empty;
                        txtDryEndDate = _SelectDryHistory.EQSTOFFDTTM.Length > 0 ? Convert.ToDateTime(_SelectDryHistory.EQSTOFFDTTM).ToString("yyyy-MM-dd HH:mm") : string.Empty;
                    }
                    else
                    {
                        OnMessage("세척작업이 완료되지 않았습니다");
                    }
                }
                else
                {
                    OnMessage("선택된 Row가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
                IsBusyA = false;
            }
            finally
            {
                IsBusyA = false;
            }

        }

        public void CheckDuplte(object param, bool chk, string DryRe)
        {
            try
            {
                isBusyB = true;

                if (param != null)
                {
                    int d = 0;
                    int r = 0;

                    var tmparm = param as DryDetail;

                    if (tmparm.STATUS == "COMPLETE" || tmparm.STATUS == "START" || tmparm.STATUS == "REWASHING")
                    {
                        tmparm.DryingComplete = false;
                        tmparm.Rewashing = false;
                    }
                    else
                    {
                        if (DryRe == "DryingComplete" && chk)
                        {
                            if (tmparm.Rewashing)
                            {
                                tmparm.Rewashing = false;
                                tmparm.DryingComplete = true;
                            }
                            else
                            {
                                tmparm.DryingComplete = true;
                            }
                        }
                        else if (DryRe == "Rewashing" && chk)
                        {
                            if (tmparm.DryingComplete)
                            {
                                tmparm.DryingComplete = false;
                                tmparm.Rewashing = true;
                            }
                            else
                            {
                                tmparm.Rewashing = true;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                OnMessage(ex.ToString());
                isBusyB = false;
            }
            finally
            {
                isBusyB = false;
            }
        }

        public async void SaveDry(object param)
        {
            try
            {
                isBusyB = true;
                
                var authHelper = new iPharmAuthCommandHelper();
                
                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_Washing");

                if (await authHelper.ClickAsync(
                    Common.enumCertificationType.Function,
                    Common.enumAccessType.Create,
                    "Part Washing",
                    "Part Washing",
                    false,
                    "EM_Equipment_Washing",
                    "", null, null) == false)
                {
                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                }

                _BR_BRS_REG_PartWashingComplete.INDATAs.Clear();
                _BR_BRS_REG_PartWashingComplete.HISTDATAs.Clear();
                _BR_BRS_REG_PartWashingComplete.INDETAILs.Clear();

                _BR_BRS_REG_PartWashingComplete.INDATAs.Add(new BR_BRS_REG_PartWashingComplete.INDATA
                {
                    WSGUID = _SelectDryHistory.WSGUID,
                    SIGNATUREGUID = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                    USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                });

                if (_DryDetail.Count > 0)
                {
                    foreach (var item in _DryDetail)
                    {
                        if (item.DryingComplete == true || item.Rewashing == true)
                        {
                            _BR_BRS_REG_PartWashingComplete.INDETAILs.Add(new BR_BRS_REG_PartWashingComplete.INDETAIL
                            {
                                TOOLID = item.TOOLID,
                                STATUS = item.DryingComplete == true ? "COMPLETE" : (item.Rewashing == true ? "REWASHING" : string.Empty)
                            });
                        }
                    }

                    await _BR_BRS_REG_PartWashingComplete.Execute();
                }


                _BR_BRS_SEL_PartWashingHistory.INDATAs.Clear();
                _BR_BRS_SEL_PartWashingHistory.OUTDATAs.Clear();
                _BR_BRS_SEL_PartWashingHistory.INDATAs.Add(new BR_BRS_SEL_PartWashingHistory.INDATA
                {
                    EQPTID = _EqiuptID
                });
                await _BR_BRS_SEL_PartWashingHistory.Execute();

                _minaWnd.DryGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                _minaWnd.DryGrid.RowDefinitions[1].Height = new GridLength(0);

            }
            catch(Exception ex)
            {
                OnMessage(ex.ToString());
                isBusyB = false;
            }
            finally
            {
                isBusyB = false;
            }
        }

        #endregion

        #region[User Define Function]

        public async void ToolListAdd(string ToolID)
        {
            try
            {
                _BR_BRS_SEL_UsedToolInfo.INDATAs.Clear();
                _BR_BRS_SEL_UsedToolInfo.OUTDATAs.Clear();

                _BR_BRS_SEL_UsedToolInfo.INDATAs.Add(
                    new BR_BRS_SEL_UsedToolInfo.INDATA
                    {
                        TOOLID = ToolID
                    });
                await _BR_BRS_SEL_UsedToolInfo.Execute();


                _tbGridcount = _SEL_Toolinfo.Count.ToString() + " 건";
                OnPropertyChanged("tbGridcount");

                if (_SEL_Toolinfo.Count > 0)
                {
                    _minaWnd.CleanRecipeDataGrid.SelectedIndex = 0;

                    int? _PRIORTY = 0;

                    _PRIORTY = _SEL_Toolinfo.OrderBy(o => o.PRIORTY).Select(o => o.PRIORTY).FirstOrDefault();

                    rdHeader = false;
                    if (_PRIORTY != null)
                    {
                        _RecipeNo = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENO).FirstOrDefault();
                        _RecipeName = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENAME).FirstOrDefault();
                        tbheader = _RecipeNo + " - " + _RecipeName;

                        _BR_BRS_CHK_PartWashingRecipe.INDATAs.Clear();
                        _BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Clear();

                        _BR_BRS_CHK_PartWashingRecipe.INDATAs.Add(new BR_BRS_CHK_PartWashingRecipe.INDATA
                        {
                            TOOLRECIPENO = _RecipeNo,
                            EQUIPMENTRECIPENO = _EquipRecipeNo
                        });

                        await _BR_BRS_CHK_PartWashingRecipe.Execute();
                    }
                    rdHeader = true;
                    _WSGUID = null;
                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
                return;
            }
            
        }

        public void EQPTTIMER()
        {
            try
            {
                if (_repeater == null || _repeater.IsEnabled == false)
                {
                    _repeater = new DispatcherTimer();
                    _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
                    _repeater.Tick += async (s, e) =>
                    {
                        try
                        {
                            _BR_BRS_SEL_PartWasherInfo.INDATAs.Clear();
                            _BR_BRS_SEL_PartWasherInfo.OUTDATAs.Clear();

                            _BR_BRS_SEL_PartWasherInfo.INDATAs.Add(
                                new BR_BRS_SEL_PartWasherInfo.INDATA
                                    {
                                        EQPTID = _EqiuptID
                                    });

                            if (await _BR_BRS_SEL_PartWasherInfo.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent, Common.enumBizRuleInDataParsingType.Property, false) && _BR_BRS_SEL_PartWasherInfo.OUTDATAs.Count > 0)
                            {
                                if (_repeater != null && _repeater.IsEnabled)
                                {
                                    foreach (var item in _BR_BRS_SEL_PartWasherInfo.OUTDATAs)
                                    {
                                        _EqiuptID = item.EQPTID.ToString();
                                        _EquiptName = item.EQPTNAME.ToString();
                                        _EquipRecipeNo = item.RECIPENO.ToString();
                                        _EquipRecipeName = item.RECIPENAME.ToString();
                                        _Status = item.STATUS.ToString();
                                    }

                                    tbFooter = _EqiuptID + "/" + _EquiptName;
                                    tbFooter1 = "( " + _EquipRecipeNo + " - " + _EquipRecipeName + ")";

                                    int? _PRIORTY = 0;

                                    _PRIORTY = _SEL_Toolinfo.OrderBy(o => o.PRIORTY).Select(o => o.PRIORTY).FirstOrDefault();

                                    _BR_BRS_CHK_PartWashingRecipe.INDATAs.Clear();
                                    _BR_BRS_CHK_PartWashingRecipe.OUTDATAs.Clear();

                                    _BR_BRS_CHK_PartWashingRecipe.INDATAs.Add(new BR_BRS_CHK_PartWashingRecipe.INDATA
                                    {
                                        TOOLRECIPENO = _SEL_Toolinfo.Where(o => o.PRIORTY == _PRIORTY).Select(o => o.RECIPENO).FirstOrDefault(),
                                        EQUIPMENTRECIPENO = _EquipRecipeNo
                                    });

                                    await _BR_BRS_CHK_PartWashingRecipe.Execute();
                                    
                                    if (_Status == "START")
                                    {
                                        if (!_isForceExit)
                                        {
                                            if (MPop.isLoaded)
                                            {
                                                MPop.Message = "세척작업이 진행 중입니다." + Environment.NewLine + "기다려주십시오";
                                                MPop.MessageStaus(WashingMessageBox.MessageStatusType.Warring);
                                                _isStart = true;
                                            }
                                            else
                                            {
                                                MPop = new WashingMessageBox();
                                                MPop.DataContext = this;
                                                MPop.Message = "세척작업이 진행 중입니다." + Environment.NewLine + "기다려주십시오";
                                                MPop.MessageStaus(WashingMessageBox.MessageStatusType.Warring);
                                                MPop.ShowCloseButton = false;
                                                MPop.CenterOnScreen();
                                                MPop.Closed += (s1, e1) =>
                                                {
                                                    MPop.isLoaded = false;
                                                    IsBusy = false;
                                                    _isStart = false;
                                                    MPop.Close();
                                                };
                                                MPop.OKButton.Click += (s1, e1) =>
                                                {
                                                    MPop.isLoaded = false;
                                                    IsBusy = false;
                                                    _isStart = false;
                                                    _isForceExit = true;
                                                    MPop.Close();
                                                };
                                                IsBusy = true;
                                                _isStart = true;
                                                MPop.Show();
                                            }
                                        }
                                    }
                                    else if (_Status == "END")
                                    {
                                        if (MPop.isLoaded && _isStart)
                                        {
                                            MPop.Close();

                                            _isStart = false;
                                            _SEL_Toolinfo.Clear();
                                            tbGridcount = "0 건";
                                            tbheader = "";
                                            _WSGUID = null;

                                            IsBusy = false;
                                        }
                                    }                                    
                                }
                            }
                        }
                        catch (TimeoutException er)
                        {
                            _repeater.Stop();
                            _repeater = null;
                            OnMessage(er.Message);
                        }
                        catch (FaultException ef)
                        {
                            _repeater.Stop();
                            _repeater = null;
                            OnMessage(ef.Message);
                        }
                    };
                    _repeater.Start();
                }
                else
                {
                    _repeater.Stop();
                    Thread.Sleep(2000);
                    _repeater.Start();
                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
            }
        }

        public void EQPTTIMEROff()
        {
            if (_repeater != null)
            {
                _repeater.Stop();
                _repeater = null;
            }
        }

        public async void LoadDetailTooladd(string WSGUID)
        {
            _BR_BRS_SEL_PartWashingDetail.INDATAs.Clear();
            _BR_BRS_SEL_PartWashingDetail.OUTDATAs.Clear();

            _BR_BRS_SEL_PartWashingDetail.INDATAs.Add(new BR_BRS_SEL_PartWashingDetail.INDATA
            {
                WSGUID = WSGUID
            });

            await _BR_BRS_SEL_PartWashingDetail.Execute();
        }

        #endregion

    }

    public class Toolinfo : ViewModelBase
    {
        private string _RowIndex;
        public string RowIndex
        {
            get
            {
                return this._RowIndex;
            }
            set
            {
                this._RowIndex = value;
            }
        }

        private string _RowEditSec;
        public string RowEditSec
        {
            get
            {
                return this._RowEditSec;
            }
            set
            {
                this._RowEditSec = value;
            }
        }

        private int _Seq;
        public int Seq
        {
            get { return _Seq; }
            set { _Seq = value; }
        }

        private string _TOOLID;
        public string TOOLID
        {
            get
            {
                return this._TOOLID;
            }
            set
            {
                _TOOLID = value;
            }
        }

        private string _TOOLNAME;
        public string TOOLNAME
        {
            get
            {
                return this._TOOLNAME;
            }
            set
            {
                _TOOLNAME = value;
            }
        }

        private string _MTRLID;
        public string MTRLID
        {
            get
            {
                return this._MTRLID;
            }
            set
            {
                _MTRLID = value;
            }
        }

        private string _MTRLNAME;
        public string MTRLNAME
        {
            get
            {
                return this._MTRLNAME;
            }
            set
            {
                _MTRLNAME = value;
            }
        }

        private System.Nullable<System.DateTime> _USEDDTTM;
        public System.Nullable<System.DateTime> USEDDTTM
        {
            get
            {
                return this._USEDDTTM;
            }
            set
            {
                _USEDDTTM = value;
            }
        }

        private string _RECIPENO;
        public string RECIPENO
        {
            get
            {
                return this._RECIPENO;
            }
            set
            {
                _RECIPENO = value;
            }
        }

        private string _RECIPENAME;
        public string RECIPENAME
        {
            get
            {
                return this._RECIPENAME;
            }
            set
            {
                _RECIPENAME = value;                
            }
        }
        private string _OPSGGUID;
        public string OPSGGUID
        {
            get
            {
                return this._OPSGGUID;
            }
            set
            {
                this._OPSGGUID = value;
                this.OnPropertyChanged("OPSGGUID");
            }
        }
        private string _OPSGNAME;      
        public string OPSGNAME
        {
            get
            {
                return this._OPSGNAME;
            }
            set
            {
                this._OPSGNAME = value;
                this.OnPropertyChanged("OPSGNAME");
            }
        }
        private string _BATCHNO;
        public string BATCHNO
        {
            get
            {
                return this._BATCHNO;
            }
            set
            {
                this._BATCHNO = value;
                this.OnPropertyChanged("BATCHNO");
            }
        }

        private string _PROC_OFFTIME;
        public string PROC_OFFTIME
        {
            get
            {
                return this._PROC_OFFTIME;
            }
            set
            {
                this._PROC_OFFTIME = value;
                this.OnPropertyChanged("PROC_OFFTIME");
            }
        }

        private Int32? _PRIORTY;
        public Int32? PRIORTY
        {
            get { return _PRIORTY; }
            set
            {
                _PRIORTY = value;
                OnPropertyChanged("PRIORTY");
            }
        }

        private string _POID;
        public string POID
        {
            get { return _POID; }
            set
            {
                _POID = value;
                OnPropertyChanged("POID");
            }
        }

    }

    public class DryHistory : ViewModelBase
    {
        private bool _RowLoadedFlag;
        public bool RowLoadedFlag
        {
            get
            {
                return this._RowLoadedFlag;
            }
            set
            {
                this._RowLoadedFlag = value;
                this.OnPropertyChanged("_RowLoadedFlag");
            }
        }
        private string _RowIndex;
        public string RowIndex
        {
            get
            {
                return this._RowIndex;
            }
            set
            {
                this._RowIndex = value;
                this.OnPropertyChanged("RowIndex");
            }
        }
        private string _RowEditSec;
        public string RowEditSec
        {
            get
            {
                return this._RowEditSec;
            }
            set
            {
                this._RowEditSec = value;
                this.OnPropertyChanged("RowEditSec");
            }
        }
        private string _WSGUID;
        public string WSGUID
        {
            get
            {
                return this._WSGUID;
            }
            set
            {
                this._WSGUID = value;
                this.OnPropertyChanged("WSGUID");
            }
        }
        private string _EQPTID;
        public string EQPTID
        {
            get
            {
                return this._EQPTID;
            }
            set
            {
                this._EQPTID = value;
                this.OnPropertyChanged("EQPTID");
            }
        }
        private string _EQPTNAME;
        public string EQPTNAME
        {
            get
            {
                return this._EQPTNAME;
            }
            set
            {
                this._EQPTNAME = value;
                this.OnPropertyChanged("EQPTNAME");
            }
        }
        private string _RECIPENO;
        public string RECIPENO
        {
            get
            {
                return this._RECIPENO;
            }
            set
            {
                    this._RECIPENO = value;
                    this.OnPropertyChanged("RECIPENO");
            }
        }
        private string _RECIPENAME;
        public string RECIPENAME
        {
            get
            {
                return this._RECIPENAME;
            }
            set
            {
                this._RECIPENAME = value;
                this.OnPropertyChanged("RECIPENAME");
            }
        }
        private string _EQSTONDTTM;
        public string EQSTONDTTM
        {
            get
            {
                return this._EQSTONDTTM;
            }
            set
            {
                this._EQSTONDTTM = value;
                this.OnPropertyChanged("EQSTONDTTM");
            }
        }
        private string _EQSTOFFDTTM;
        public string EQSTOFFDTTM
        {
            get
            {
                return this._EQSTOFFDTTM;
            }
            set
            {
                this._EQSTOFFDTTM = value;
                this.OnPropertyChanged("EQSTOFFDTTM");
            }
        }
        private string _STATUS;
        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
                this.OnPropertyChanged("STATUS");
            }
        }
        private System.Nullable<short> _CNT;
        public System.Nullable<short> CNT
        {
            get
            {
                return this._CNT;
            }
            set
            {
                this._CNT = value;
                this.OnPropertyChanged("CNT");
            }
        }
        private string _SEQ;
        public string SEQ
        {
            get
            {
                return this._SEQ;
            }
            set
            {
                this._SEQ = value;
                this.OnPropertyChanged("SEQ");
            }
        }
        private string _CLEANRECIPE;
        public string CLEANRECIPE
        {
            get
            {
                return this._CLEANRECIPE;
            }
            set
            {
                this._CLEANRECIPE = value;
                this.OnPropertyChanged("CLEANRECIPE");
            }
        }
    }

    public class DryDetail : ViewModelBase
    {
        private int _Dry = 0;
        private int _Re = 0;
        private bool _RowLoadedFlag;
        public bool RowLoadedFlag
        {
            get
            {
                return this._RowLoadedFlag;
            }
            set
            {
                this._RowLoadedFlag = value;
                this.OnPropertyChanged("_RowLoadedFlag");
            }
        }
        private string _RowIndex;
        public string RowIndex
        {
            get
            {
                return this._RowIndex;
            }
            set
            {
                this._RowIndex = value;
                this.OnPropertyChanged("RowIndex");
            }
        }
        private string _RowEditSec;
        public string RowEditSec
        {
            get
            {
                return this._RowEditSec;
            }
            set
            {
                this._RowEditSec = value;
                this.OnPropertyChanged("RowEditSec");
            }
        }
        private string _TOOLID;
        public string TOOLID
        {
            get
            {
                return this._TOOLID;
            }
            set
            {
                this._TOOLID = value;
                this.OnPropertyChanged("TOOLID");
            }
        }
        private string _TOOLNAME;
        public string TOOLNAME
        {
            get
            {
                return this._TOOLNAME;
            }
            set
            {
                this._TOOLNAME = value;
                this.OnPropertyChanged("TOOLNAME");
            }
        }
        private string _STATUS;
        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
                this.OnPropertyChanged("STATUS");
            }
        }
        private System.Nullable<short> _SEQ;
        public System.Nullable<short> SEQ
        {
            get
            {
                return this._SEQ;
            }
            set
            {
                this._SEQ = value;
                this.OnPropertyChanged("SEQ");
            }
        }
        private bool _DryingComplete;
        public bool DryingComplete
        {
            get { return _DryingComplete; }
            set
            {
                _DryingComplete = value;
                OnPropertyChanged("DryingComplete");
            }
        }
        private bool _Rewashing;
        public bool Rewashing
        {
            get { return _Rewashing; }
            set
            {
                _Rewashing = value;
                OnPropertyChanged("Rewashing");
            }
        }
        private string _TOOLINFO;
        public string TOOLINFO
        {
            get { return _TOOLINFO; }
            set
            {
                _TOOLINFO = value;
                OnPropertyChanged("TOOLINFO");
            }
        }
    }
}
