using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령
{
    public class 무균공정작업자입력ViewModel : ViewModelBase
    {
        #region [Property]
        public 무균공정작업자입력ViewModel()
        {
            _UserContain = new UserContainer.OUTDATACollection();
            _BR_PHR_SEL_PERSON = new BR_PHR_SEL_PERSON();
        }

        private 무균공정작업자입력 _mainWnd;

        private string _UserId;
        public string UserId
        {
            get { return _UserId; }
            set
            {
                _UserId = value;
                OnPropertyChanged("UserId");
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        private UserContainer.OUTDATACollection _UserContain;
        public UserContainer.OUTDATACollection UserContain
        {
            get { return this._UserContain; }
            set
            {
                this._UserContain = value;
                this.OnPropertyChanged("UserContain");
            }
        }
        #endregion

        #region [BizRule]

        BR_PHR_SEL_PERSON _BR_PHR_SEL_PERSON;
        public BR_PHR_SEL_PERSON BR_PHR_SEL_PERSON
        {
            get { return _BR_PHR_SEL_PERSON; }
            set
            {
                _BR_PHR_SEL_PERSON = value;
                OnPropertyChanged("BR_PHR_SEL_PERSON");
            }
        }
        
        #endregion

        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is 무균공정작업자입력)
                            {
                                _mainWnd = arg as 무균공정작업자입력;

                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y")
                                    && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP")
                                    && _mainWnd.CurrentInstruction.Raw.NOTE != null)
                                {
                                    var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                                    string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                                    DataSet ds = new DataSet();
                                    ds.ReadXmlFromString(xml);

                                    if (ds.Tables.Count == 1 && ds.Tables[0].TableName == "DATA")
                                    {
                                        foreach (DataRow row in ds.Tables[0].Rows)
                                        {
                                            _UserContain.Add(new UserContainer.OUTDATA
                                            {
                                                USERID = row["작업자ID"] != null ? row["작업자ID"].ToString() : "",
                                                USERNAME = row["작업자명"] != null ? row["작업자명"].ToString() : "",
                                                RowEditSec = "INS"
                                            });
                                        }
                                        OnPropertyChanged("UserList");
                                    }
                                }

                                _mainWnd.txtUserId.Focus();
                            }

                            CommandResults["LoadedCommandAsync"] = true;
                        }

                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }

        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///

                            var authHelper = new iPharmAuthCommandHelper();
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN == "Y" && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    true,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            //XML 생성. 비즈룰 INDATA생성
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("작업자ID"));
                            dt.Columns.Add(new DataColumn("작업자명"));

                            var row = dt.NewRow();
                            foreach (var item in UserContain)
                            {
                                row = dt.NewRow();
                                row["작업자ID"] = item.USERID;
                                row["작업자명"] = item.USERNAME;
                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }

        public ICommand NoRecordConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommand"] = false;
                            CommandCanExecutes["NoRecordConfirmCommand"] = false;

                            // 전자서명
                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    true,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            //XML 생성. 비즈룰 INDATA생성
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("작업자ID"));
                            dt.Columns.Add(new DataColumn("작업자명"));

                            var row = dt.NewRow();

                            row = dt.NewRow();
                            row["작업자ID"] = "N/A";
                            row["작업자명"] = "N/A";
                            dt.Rows.Add(row);

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NoRecordConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommand") ?
                        CommandCanExecutes["NoRecordConfirmCommand"] : (CommandCanExecutes["NoRecordConfirmCommand"] = true);
                });
            }
        }
        #endregion

        #region [etc]
        public ICommand CheckUserIdCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] = false;
                        CommandResults["CheckIBCInfoCommandAsync"] = false;

                        UserId = arg as string;

                        if (string.IsNullOrEmpty(UserId)) return;

                        _BR_PHR_SEL_PERSON.INDATAs.Clear();
                        _BR_PHR_SEL_PERSON.OUTDATAs.Clear();

                        foreach (var item in _UserContain)
                        {
                            if (UserId == item.USERID)
                            {
                                OnMessage("이미 등록된 작업자입니다.");
                                CommandResults["CheckIBCInfoCommandAsync"] = true;
                                return;
                            }
                        }

                        _BR_PHR_SEL_PERSON.INDATAs.Add(new BR_PHR_SEL_PERSON.INDATA
                        {
                            USERIUSE = "Y",
                            LANGID = "",
                            USERID = UserId
                        });

                        if (await _BR_PHR_SEL_PERSON.Execute())
                        {

                            if (_BR_PHR_SEL_PERSON.OUTDATAs.Count > 0)
                            {
                                _UserContain.Add(new UserContainer.OUTDATA
                                {
                                    USERID = _BR_PHR_SEL_PERSON.OUTDATAs[0].USERID,
                                    USERNAME = _BR_PHR_SEL_PERSON.OUTDATAs[0].USERNAME,
                                    RowEditSec = "INS"
                                });
                            }
                            else
                                OnMessage("사원 정보가 없습니다.");
                        }

                        CommandResults["CheckIBCInfoCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["CheckIBCInfoCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("CheckIBCInfoCommandAsync") ?
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] : (CommandCanExecutes["CheckIBCInfoCommandAsync"] = true);
                });
            }
        }

        public class UserContainer : BizActorRuleBase
        {
            public sealed partial class OUTDATACollection : BufferedObservableCollection<OUTDATA>
            {
            }
            private OUTDATACollection _OUTDATAs;
            [BizActorOutputSetAttribute()]
            public OUTDATACollection OUTDATAs
            {
                get
                {
                    return this._OUTDATAs;
                }
            }
            [BizActorOutputSetDefineAttribute(Order = "0")]
            [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
            public partial class OUTDATA : BizActorDataSetBase
            {
                public OUTDATA()
                {
                    RowLoadedFlag = false;
                }
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
                private string _USERID;
                [BizActorOutputItemAttribute()]
                public string USERID
                {
                    get
                    {
                        return this._USERID;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._USERID = value;
                            this.CheckIsOriginal("USERID", value);
                            this.OnPropertyChanged("USERID");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _USERNAME;
                [BizActorOutputItemAttribute()]
                public string USERNAME
                {
                    get
                    {
                        return this._USERNAME;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._USERNAME = value;
                            this.CheckIsOriginal("USERNAME", value);
                            this.OnPropertyChanged("USERNAME");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
            #endregion
     
}
