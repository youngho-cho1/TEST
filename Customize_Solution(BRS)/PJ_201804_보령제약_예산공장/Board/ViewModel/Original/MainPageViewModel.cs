using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Board_Original
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Property
  
        private string _serverUri;
        public string serverUri
        {
            get { return _serverUri; }
            set { _serverUri = value; }
        }

        private HubConnection _connection;
        public HubConnection connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        private IHubProxy _hub;
        public IHubProxy hub
        {
            get { return _hub; }
            set { _hub = value; }
        }

        private string _ConnectionState;
        public string ConnectionState
        {
            get { return _ConnectionState; }
            set
            {
                _ConnectionState = value;
                NotifyPropertyChanged();
            }
        }

        private string _remainTime;
        public string remainTime
        {
            get { return _remainTime; }
            set
            {
                _remainTime = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isPause;
        public bool isPause
        {
            get { return _isPause; }
            set
            {
                _isPause = value;
                NotifyPropertyChanged();
            }
        }


        private Visibility _isWarnVisible;
        public Visibility isWarnVisible
        {
            get { return _isWarnVisible; }
            set
            {
                _isWarnVisible = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility[] _isVisible;
        public Visibility[] isVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _title;
        public string title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        private string _subTitle;
        public string subTitle
        {
            get { return _subTitle; }
            set
            {
                _subTitle = value;
                NotifyPropertyChanged();
            }
        }

        private int _countTime;
        public int countTime
        {
            get { return _countTime; }
            set
            {
                _countTime = value;
                NotifyPropertyChanged();
            }
        }

        private int _screenIndex;
        public int screenIndex
        {
            get { return _screenIndex; }
            set { _screenIndex = value; }
        }

        private BoardMain _mainWnd;

        private MemoryStream memStream = null;

        private int _initCount;

        private Timer _countTimer;

        private Timer _timer;

        private Page[] _pageBoard;
        public Page[] pageBoard
        {
            get { return _pageBoard; }
            set
            {
                _pageBoard = value;
                NotifyPropertyChanged();
            }
        }

        private StatusBoardInfo _summaryBoard;
        public StatusBoardInfo summaryBoard
        {
            get { return _summaryBoard; }
            set
            {
                _summaryBoard = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableDictionary<string, RoomStatus> _allOfRoomStatus;
        public ObservableDictionary<string, RoomStatus> allOfRoomStatus
        {
            get { return _allOfRoomStatus; }
            set
            {
                _allOfRoomStatus = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Property

        #region Data

        private BR_PHR_SEL_StatusBoard _BR_PHR_SEL_StatusBoard;
        public BR_PHR_SEL_StatusBoard BR_PHR_SEL_StatusBoard
        {
            get { return _BR_PHR_SEL_StatusBoard; }
            set { _BR_PHR_SEL_StatusBoard = value; }
        }

        private BR_PHR_SEL_System_Option_OPTIONTYPE _BR_PHR_SEL_System_Option_OPTIONTYPE;
        public BR_PHR_SEL_System_Option_OPTIONTYPE BR_PHR_SEL_System_Option_OPTIONTYPE
        {
            get { return _BR_PHR_SEL_System_Option_OPTIONTYPE; }
            set { _BR_PHR_SEL_System_Option_OPTIONTYPE = value; }
        }
        
        #endregion Data

        #region Command

        public ICommand LoadCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadCommand"] = false;
                            CommandCanExecutes["LoadCommand"] = false;

                            ///
                            if (arg == null || !(arg is BoardMain))
                                return;
                            _mainWnd = arg as BoardMain;

                            //Hub연결 및 그룹 등록

                            //System_Option에서 Hub주소 조회
                            _BR_PHR_SEL_System_Option_OPTIONTYPE.INDATAs.Clear();
                            _BR_PHR_SEL_System_Option_OPTIONTYPE.OUTDATAs.Clear();
                            _BR_PHR_SEL_System_Option_OPTIONTYPE.INDATAs.Add(new BR_PHR_SEL_System_Option_OPTIONTYPE.INDATA()
                            {
                                OPTIONTYPE = "OPTP006",
                                ISUSE = "Y"
                            });

                            await _BR_PHR_SEL_System_Option_OPTIONTYPE.Execute();
                            foreach (var outdata in _BR_PHR_SEL_System_Option_OPTIONTYPE.OUTDATAs)
                            {
                                if (outdata.OPTIONITEM == "SYS_HUB_SVR_URL")
                                {
                                    _serverUri = outdata.OPTIONVALUE;
                                    break;
                                }
                            }
                            //Local에서 테스트 할 경우 "http://localhost:10000/IUI"

                            //Hub에 건네줄 정보등록
                            IDictionary<string, string> _queryString = new Dictionary<string, string>();
                            _queryString.Add("GroupName", "BoardGroup");
                            _connection = new HubConnection(_serverUri, _queryString, true);
                            _connection.StateChanged += Connection_StateChanged;
                            _ConnectionState = "Connecting";
                            _hub = connection.CreateHubProxy("SignalRHub");

                            _hub.On<string, string, string>("Broadcast", (GroupName, messageKey, xmlData) =>
                            {
                                getServerData(xmlData);
                            });

                            _ConnectionState = "Connecting";
                            await connection.Start();

                            //Data초기화
                            await _BR_PHR_SEL_StatusBoard.Execute();
                            title = _summaryBoard.pageTitle;
                            subTitle = _summaryBoard.pageEngTitle;

                            //visible초기화
                            screenIndex = 0;
                            isVisible[screenIndex] = Visibility.Visible;
                            OnPropertyChanged("isVisible");

                            //타이머 설정
                            var current = DateTime.Now.TimeOfDay;
                            pageChangeTimer(current.Add(TimeSpan.FromSeconds(1)), TimeSpan.FromSeconds(1), InitCountingTime);
                            ///

                            CommandResults["LoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommand") ?
                        CommandCanExecutes["LoadCommand"] : (CommandCanExecutes["LoadCommand"] = true);
                });
            }
        }

        public ICommand nextClickCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["nextClickCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["nextClickCommand"] = false;
                            CommandCanExecutes["nextClickCommand"] = false;

                            ///

                            if (!isPause)
                                resetTimer();
                            else
                                restartTimer();

                            ///

                            CommandResults["nextClickCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["nextClickCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["nextClickCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("nextClickCommand") ?
                        CommandCanExecutes["nextClickCommand"] : (CommandCanExecutes["nextClickCommand"] = true);
                });
            }
        }

        public ICommand pauseClickCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["pauseClickCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["pauseClickCommand"] = false;
                            CommandCanExecutes["pauseClickCommand"] = false;

                            ///

                            if (_isPause)
                                return;
                            _isPause = true;
                            _countTimer.Dispose();
                            
                            ///

                            CommandResults["pauseClickCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["pauseClickCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["pauseClickCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("pauseClickCommand") ?
                        CommandCanExecutes["pauseClickCommand"] : (CommandCanExecutes["pauseClickCommand"] = true);
                });
            }
        }

        public ICommand settingPopupOpenCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["settingPopupOpenCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["settingPopupOpenCommand"] = false;
                            CommandCanExecutes["settingPopupOpenCommand"] = false;

                            ///
                            var popup = new BoardSettingPopup()
                            {
                                DataContext = new BoardSettingPopupViewModel()
                                {
                                    timerCount = _initCount
                                }
                            };
                            popup.Closed += (s,e) =>
                            {
                                _initCount = ((BoardSettingPopupViewModel)popup.DataContext).timerCount;
                                countTime = _initCount;
                                _mainWnd.isInBtn = false;
                            };
                            popup.Show();
                            ///

                            CommandResults["settingPopupOpenCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["settingPopupOpenCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["settingPopupOpenCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("settingPopupOpenCommand") ?
                        CommandCanExecutes["settingPopupOpenCommand"] : (CommandCanExecutes["settingPopupOpenCommand"] = true);
                });
            }
        }
        
        #endregion Command

        #region Constructor

        public MainPageViewModel()
        { 
            //보여질 화면 설정
            isWarnVisible = Visibility.Collapsed;
            _isVisible = new Visibility[4];
            for (int i = 0; i < 4; i++)
                isVisible[i] = Visibility.Collapsed;

            //타이머 화면 이동
            _initCount = 5;
            _countTime = _initCount;
            remainTime = countTime.ToString();

            //DataContext설정
            _allOfRoomStatus = new ObservableDictionary<string, RoomStatus>();
            _summaryBoard = new StatusBoardInfo();
            _pageBoard = new Page[3];
            for (int i = 0; i < 3; i++)
                pageBoard[i] = new Page();

            //BizRule 초기화
            _BR_PHR_SEL_StatusBoard = new BR_PHR_SEL_StatusBoard();
            _BR_PHR_SEL_StatusBoard.OnExecuteCompleted += new DelegateExecuteCompleted(bizCompleted);
            _BR_PHR_SEL_System_Option_OPTIONTYPE = new BR_PHR_SEL_System_Option_OPTIONTYPE();
        }

        #endregion

        #region Method

        //연결 재시도 
        public void Connection_StateChanged(StateChange obj)
        {
            if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                _isWarnVisible = Visibility.Visible;
                var current = DateTime.Now.TimeOfDay;
                SetTimer(current.Add(TimeSpan.FromSeconds(5)), TimeSpan.FromSeconds(4), StartCon);
                return;
            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                _isWarnVisible = Visibility.Collapsed;
                ConnectionState = "Connected";
            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connecting)
            {
                ConnectionState = "Connecting";
            }
            else
            {
                ConnectionState = "Reconnecting";
            }
            if (_timer != null)
                _timer.Dispose();
        }

        private async Task StartCon()
        {
            await connection.Start();
        }

        private void SetTimer(TimeSpan starTime, TimeSpan every, Func<Task> action)
        {
            var current = DateTime.Now;
            var timeToGo = starTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;
            }
            _timer = new Timer(x =>
            {
                action.Invoke();
            }, null, timeToGo, every);
        }

        //60초 카운트
        private async Task InitCountingTime()
        {
            _countTime -= 1;
            if (_countTime < 0)
            {
                resetTimer();
                _countTime = _initCount;
            }

            remainTime = _countTime.ToString();
        }

        private void pageChangeTimer(TimeSpan starTime, TimeSpan every, Func<Task> action)
        {
            var current = DateTime.Now;
            var timeToGo = starTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;
            }
            _countTimer = new Timer(x =>
            {
                action.Invoke();
            }, null, timeToGo, every);
        }

        public void bizCompleted(string temp)
        {
            //Page초기화
            foreach (var outdata in _BR_PHR_SEL_StatusBoard.StatusBoardPages)
            {
                if (outdata.PageDisplaySEQ != null)
                {
                    pageBoard[(int)outdata.PageDisplaySEQ - 1].pageTitle = outdata.PageName;
                    pageBoard[(int)outdata.PageDisplaySEQ - 1].pageEngTitle = "(" + outdata.PageNameEnglish + ")";
                }
            }

            //Process초기화
            foreach (var outdata in _BR_PHR_SEL_StatusBoard.StatusBoardPageProcesss)
            {
                if (outdata.PageDisplaySEQ != null && outdata.ProcessDisplaySEQ != null)
                {
                    pageBoard[(int)outdata.PageDisplaySEQ - 1].procList[(int)outdata.ProcessDisplaySEQ - 1].procName = outdata.ProcessName + "(" + outdata.ProcessNameEnglish + ")";
                }
            }

            //RoomStatus초기화
            foreach (var outdata in _BR_PHR_SEL_StatusBoard.StatusBoardPageProcessRooms)
            {
                if (!_allOfRoomStatus.ContainsKey(outdata.RoomGUID))
                {
                    _allOfRoomStatus.Add(outdata.RoomGUID, pageBoard[(int)outdata.PageDisplaySEQ - 1].procList[(int)outdata.ProcessDisplaySEQ - 1].roomList[0][(int)outdata.RoomDisplaySEQ - 1]);
                    refreshRoomStatus(outdata.RoomGUID, outdata);
                }

            }
        }

        public void refreshRoomStatus(string key, BR_PHR_SEL_StatusBoard.StatusBoardPageProcessRoom newRoom)
        {
            _allOfRoomStatus[key].roomGUID = newRoom.RoomGUID;
            _allOfRoomStatus[key].roomName = newRoom.EQPTNAME;
            _allOfRoomStatus[key].roomCode = newRoom.EQPTID;
            _allOfRoomStatus[key].statusText = newRoom.StatusText;
            _allOfRoomStatus[key].statusTime = newRoom.StatusTime;
            _allOfRoomStatus[key].prodActual = newRoom.ProductionActual;
            _allOfRoomStatus[key].prodPlan = newRoom.ProductionPlan;
            _allOfRoomStatus[key].isRoomVisible = newRoom.RoomGUID == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public void resetTimer()
        {
            isVisible[screenIndex] = Visibility.Collapsed;
            OnPropertyChanged("isVisible");

            screenIndex = (screenIndex + 1) % 4;
            isVisible[screenIndex] = Visibility.Visible;
            OnPropertyChanged("isVisible");

            _countTime = _initCount;
            remainTime = countTime.ToString();

            if (screenIndex == 0)
            {
                title = _summaryBoard.pageTitle;
                subTitle = _summaryBoard.pageEngTitle;
            }
            else
            {
                title = _pageBoard[screenIndex - 1].pageTitle;
                subTitle = _pageBoard[screenIndex - 1].pageEngTitle;
            }
        }

        public void restartTimer()
        {
            var current = DateTime.Now.TimeOfDay;
            //시간 타이머 실행
            pageChangeTimer(current.Add(TimeSpan.FromSeconds(1)), TimeSpan.FromSeconds(1), InitCountingTime);
            _isPause = false;
        }

        public void getServerData(string xmlData)
        {
            var ds = new DataSet();
            byte[] temp = Convert.FromBase64String(xmlData);

            using (memStream = new MemoryStream(temp))
            {
                ds.ReadXml(memStream);
                foreach (var rowData in ds.Tables["INDATA"].Rows)
                {
                    BR_PHR_SEL_StatusBoard.StatusBoardPageProcessRoom outdata = new BR_PHR_SEL_StatusBoard.StatusBoardPageProcessRoom()
                    {
                        PageGUID = rowData["PageGUID"] as string,
                        ProcessGUID = rowData["ProcessGUID"] as string,
                        RoomGUID = rowData["roomGUID"] as string,
                        EQPTNAME = rowData["EQPTNAME"] as string,
                        EQPTID = rowData["EQPTID"] as string,
                        StatusText = rowData["StatusText"] as string,
                        StatusTime = rowData["StatusTime"] as string,
                        ProductionActual = rowData["ProductionActual"] as string,
                        ProductionPlan = rowData["ProductionPlan"] as string,
                        PageDisplaySEQ = Int32.Parse((rowData["PageDisplaySEQ"] as string)),
                        ProcessDisplaySEQ = Int32.Parse((rowData["ProcessDisplaySEQ"] as string)),
                        RoomDisplaySEQ = Int32.Parse((rowData["RoomDisplaySEQ"] as string))
                    };

                    //key가 존재한다면 방이 등록되어 있는 것. //정보갱신
                    if (_allOfRoomStatus.ContainsKey(outdata.RoomGUID))
                    {
                        refreshRoomStatus(outdata.RoomGUID, outdata);
                    }
                    else
                    {//key가 존재 하지 않는다면 새로운 방이 생성된것. 기존에 방이 존재하던 위치에 새로운 방이 덮어 쓸 수 있으며, 새로운 위치에 방이 생성될 수 있음.
                        var unknownRoom = pageBoard[(int)outdata.PageDisplaySEQ - 1].procList[(int)outdata.ProcessDisplaySEQ - 1].roomList[0][(int)outdata.RoomDisplaySEQ - 1];
                        if (unknownRoom.roomGUID != null)
                        {
                            _allOfRoomStatus.Remove(unknownRoom.roomGUID);
                        }

                        _allOfRoomStatus.Add(outdata.RoomGUID, unknownRoom);
                        refreshRoomStatus(outdata.RoomGUID, outdata);
                    }

                    outdata.Dispose();
                }
            }

        }

        #endregion method
    }

    //작업실
    public class RoomStatus : ViewModelBase
    {
        private string _pageCode;
        public string pageCode
        {
            get { return _pageCode; }
            set
            {
                _pageCode = value;
                NotifyPropertyChanged();
            }
        }

        private int _pageSeq;
        public int pageSeq
        {
            get { return _pageSeq; }
            set
            {
                _pageSeq = value;
                NotifyPropertyChanged();
            }
        }

        private string _procCode;
        public string procCode
        {
            get { return _procCode; }
            set
            {
                _procCode = value;
                NotifyPropertyChanged();
            }
        }

        private int _procSeq;
        public int procSeq
        {
            get { return _procSeq; }
            set
            {
                _procSeq = value;
                NotifyPropertyChanged();
            }
        }

        private string _roomCode;
        public string roomCode
        {
            get { return _roomCode; }
            set
            {
                _roomCode = value;
                OnPropertyChanged("roomCode");
            }
        }

        private int _roomSeq;
        public int roomSeq
        {
            get { return _roomSeq; }
            set
            {
                _roomSeq = value;
                NotifyPropertyChanged();
            }
        }

        private string _procName;
        public string procName
        {
            get { return _procName; }
            set
            {
                _procName = value;
                NotifyPropertyChanged();
            }
        }

        private string _roomName;
        public string roomName
        {
            get { return _roomName; }
            set
            {
                _roomName = value;
                NotifyPropertyChanged();
            }
        }

        private string _statusText;
        public string statusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                NotifyPropertyChanged();
            }
        }

        private string _statusTime;
        public string statusTime
        {
            get { return _statusTime; }
            set
            {
                _statusTime = value;
                NotifyPropertyChanged();
            }
        }

        private string _prodActual;
        public string prodActual
        {
            get { return _prodActual; }
            set
            {
                _prodActual = value;
                NotifyPropertyChanged();
            }
        }

        private string _prodPlan;
        public string prodPlan
        {
            get { return _prodPlan; }
            set
            {
                _prodPlan = value;
                NotifyPropertyChanged();
            }
        }

        private string _roomGUID;
        public string roomGUID
        {
            get { return _roomGUID; }
            set
            {
                _roomGUID = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility _isRoomVisible;
        public Visibility isRoomVisible
        {
            get { return _isRoomVisible; }
            set
            {
                _isRoomVisible = value;
                NotifyPropertyChanged();
            }
        }

        public RoomStatus()
        {
            //prodActual = "N/A";
            //prodPlan = "N/A";
            roomGUID = null;
            isRoomVisible = Visibility.Collapsed;
        }
    }

    //공정 -공정명, 작업실
    public class Process : ViewModelBase
    {
        //공정명
        private string _procName;
        public string procName
        {
            get { return _procName; }
            set
            {
                _procName = value;
                NotifyPropertyChanged();
            }
        }

        private string _procEngName;
        public string procEngName
        {
            get { return _procEngName; }
            set
            {
                _procEngName = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<RoomStatus[]> _roomList;
        public ObservableCollection<RoomStatus[]> roomList
        {
            get { return _roomList; }
            set
            {
                _roomList = value;
                NotifyPropertyChanged();
            }
        }

        public Process()
        {
            _roomList = new ObservableCollection<RoomStatus[]>();
            roomList.Add(new RoomStatus[8]);
            for (int i = 0; i < 8; i++)
                roomList[0][i] = new RoomStatus();
        }
    }

    //페이지 - 페이지명, 공정명
    public class Page : ViewModelBase
    {
        private string _pageTitle;
        public string pageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged();
            }
        }

        private string _pageEngTitle;
        public string pageEngTitle
        {
            get { return _pageEngTitle; }
            set
            {
                _pageEngTitle = value;
                NotifyPropertyChanged();
            }
        }

        private Process[] _procList;
        public Process[] procList
        {
            get { return _procList; }
            set
            {
                _procList = value;
                NotifyPropertyChanged();
            }
        }

        public Page()
        {
            _procList = new Process[4];
            InitArray();
        }

        public void InitArray()
        {
            for (int i = 0; i < 4; i++)
                procList[i] = new Process();
        }
    }

    //Summary
    public class summaryBoard : ViewModelBase
    {
        private string _annualOutput;
        public string annualOutput
        {
            get { return _annualOutput; }
            set
            {
                _annualOutput = value;
                OnPropertyChanged("annualOutput");
            }
        }

        private string _monthlyPlan;
        public string monthlyPlan
        {
            get { return _monthlyPlan; }
            set
            {
                _monthlyPlan = value;
                OnPropertyChanged("monthlyPlan");
            }
        }

        private string _monthlyActual;
        public string monthlyActual
        {
            get { return _monthlyActual; }
            set
            {
                _monthlyActual = value;
                OnPropertyChanged("monthlyActual");
            }
        }

        private string _monthlySubActual;
        public string monthlySubActual
        {
            get { return _monthlySubActual; }
            set
            {
                _monthlySubActual = value;
                OnPropertyChanged("monthlySubActual");
            }
        }

        private string _monthlyProcessing;
        public string monthlyProcessing
        {
            get { return _monthlyProcessing; }
            set
            {
                _monthlyProcessing = value;
                OnPropertyChanged("monthlyProcessing");
            }
        }

        private string _monthlySubProcessing;
        public string monthlySubProcessing
        {
            get { return _monthlySubProcessing; }
            set
            {
                _monthlySubProcessing = value;
                OnPropertyChanged("monthlySubProcessing");
            }
        }

        public summaryBoard()
        {
        }
    }

    public class StatusBoardInfo : ViewModelBase
    {
        private string _pageTitle;
        public string pageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged("pageTitle");
            }
        }

        private string _pageEngTitle;
        public string pageEngTitle
        {
            get { return _pageEngTitle; }
            set
            {
                _pageEngTitle = value;
                OnPropertyChanged("pageEngTitle");
            }
        }

        private string _year;
        public string year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged("year");
            }
        }

        private string _month;
        public string month
        {
            get { return _month; }
            set
            {
                _month = value;
                OnPropertyChanged("month");
            }
        }

        private ObservableCollection<summaryBoard> _summaryList;
        public ObservableCollection<summaryBoard> summaryList
        {
            get { return _summaryList; }
            set
            {
                _summaryList = value;
                OnPropertyChanged("summaryList");
            }
        }

        public StatusBoardInfo()
        {
            _pageTitle = "생산 계획 및 실적 현황";
            _pageEngTitle = "(Plan and Actual)";
            _summaryList = new ObservableCollection<summaryBoard>();
            for (int i = 0; i < 3; i++)
                _summaryList.Add(new summaryBoard());
        }
    }

    //특정한 조건에 따라 글자 색을 바꿔준다.
    public class FontColorConverter : IValueConverter
    {
        private SolidColorBrush GREEN = new SolidColorBrush(Color.FromArgb(255, (byte)204, (byte)255, (byte)153));
        private SolidColorBrush RED = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)124, (byte)128));
        private SolidColorBrush YELLOW = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)255, (byte)102));
        private SolidColorBrush DEFAULT = new SolidColorBrush(Color.FromArgb(255, (byte)222, (byte)235, (byte)247));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is string)
            {
                string temp = value as string;
                if (temp == "N/A")
                    return GREEN;
                if (Int32.Parse(temp) >= 70)
                {
                    return GREEN;
                }
                else if (Int32.Parse(temp) >= 35 && Int32.Parse(temp) < 70)
                {
                    return RED;
                }
                else if (Int32.Parse(temp) < 35)
                {
                    return YELLOW;
                }
            }
            return GREEN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
