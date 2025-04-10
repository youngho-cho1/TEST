using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
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
    public class 중요간섭상황기록ViewModel : ViewModelBase
    {
        #region Properties
        public 중요간섭상황기록ViewModel()
        {
            _ErrorHistorys = new ErrorHistory.OUTDATACollection();
        }

        중요간섭상황기록 _mainWnd;

        private ErrorHistory.OUTDATACollection _ErrorHistorys;
        public ErrorHistory.OUTDATACollection ErrorHistorys
        {
            get { return _ErrorHistorys; }
            set
            {
                _ErrorHistorys = value;
                OnPropertyChanged("ErrorHistorys");
            }
        }
        #endregion
        #region Command
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {                             
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is 중요간섭상황기록)
                            {
                                _mainWnd = arg as 중요간섭상황기록;

                                IsBusy = true;

                                ErrorHistorys.Clear();

                                // 이전 기록 조회
                                if (_mainWnd.CurrentInstruction.Raw.ACTVAL == _mainWnd.TableTypeName && _mainWnd.CurrentInstruction.Raw.NOTE != null)
                                {
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable();
                                    var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                                    string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);

                                    ds.ReadXmlFromString(xml);
                                    if (ds.Tables[0].TableName == "DATA")
                                    {
                                        dt = ds.Tables[0];
                                        foreach (var row in dt.Rows)
                                        {
                                            ErrorHistorys.Add(new ErrorHistory.OUTDATA
                                            {
                                                STRTDTTM = Convert.ToDateTime(row["발생시각"].ToString()),
                                                ENDDTTM = Convert.ToDateTime(row["조치완료시간"].ToString()),
                                                OBJECT = row["간섭내용"].ToString(),
                                                MODULE = row["발생위치"].ToString(),
                                                DESCRIPTION = row["조치사항"].ToString()
                                            });
                                        }
                                    }
                                }
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
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ComfirmCommandAsync"] = false;
                            CommandCanExecutes["ComfirmCommandAsync"] = false;

                            ///

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "중요간섭상황기록",
                                "중요간섭상황기록",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // DataSet 생성
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("발생시각"));
                            dt.Columns.Add(new DataColumn("조치완료시간"));
                            dt.Columns.Add(new DataColumn("간섭내용"));
                            dt.Columns.Add(new DataColumn("발생위치"));
                            dt.Columns.Add(new DataColumn("조치사항"));

                            foreach (var item in ErrorHistorys)
                            {
                                var row = dt.NewRow();

                                row["발생시각"] = item.STRTDTTM != null ? item.STRTDTTM.ToString("yyyy-MM-dd HH:mm")  : "";
                                row["조치완료시간"] = item.ENDDTTM != null ? item.ENDDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                row["간섭내용"] = item.OBJECT ?? "";
                                row["발생위치"] = item.MODULE ?? "";
                                row["조치사항"] = item.DESCRIPTION ?? "";

                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);


                            IsBusy = false;
                            ///

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ?
                        CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region User Define
        public class ErrorHistory : BizActorRuleBase
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
                private DateTime _STRTDTTM = DateTime.Now;
                [BizActorOutputItemAttribute()]
                public DateTime STRTDTTM
                {
                    get
                    {
                        return this._STRTDTTM;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._STRTDTTM = value;
                            this.CheckIsOriginal("STRTDTTM", value);
                            this.OnPropertyChanged("STRTDTTM");
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
                private DateTime _ENDDTTM = DateTime.Now;
                [BizActorOutputItemAttribute()]
                public DateTime ENDDTTM
                {
                    get
                    {
                        return this._ENDDTTM;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._ENDDTTM = value;
                            this.CheckIsOriginal("ENDDTTM", value);
                            this.OnPropertyChanged("ENDDTTM");
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
                private string _OBJECT;
                [BizActorOutputItemAttribute()]
                public string OBJECT
                {
                    get
                    {
                        return this._OBJECT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._OBJECT = value;
                            this.CheckIsOriginal("OBJECT", value);
                            this.OnPropertyChanged("OBJECT");
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
                private string _MODULE;
                [BizActorOutputItemAttribute()]
                public string MODULE
                {
                    get
                    {
                        return this._MODULE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MODULE = value;
                            this.CheckIsOriginal("MODULE", value);
                            this.OnPropertyChanged("MODULE");
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
                private string _DESCRIPTION;
                [BizActorOutputItemAttribute()]
                public string DESCRIPTION
                {
                    get
                    {
                        return this._DESCRIPTION;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._DESCRIPTION = value;
                            this.CheckIsOriginal("DESCRIPTION", value);
                            this.OnPropertyChanged("DESCRIPTION");
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
            public ErrorHistory()
            {
                _OUTDATAs = new OUTDATACollection();
            }
        }
        #endregion
    }
}
