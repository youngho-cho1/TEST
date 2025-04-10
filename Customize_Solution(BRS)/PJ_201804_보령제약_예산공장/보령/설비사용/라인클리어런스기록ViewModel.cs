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
using C1.Silverlight.Data;
using ShopFloorUI;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    public class 라인클리어런스기록ViewModel : ViewModelBase
    {
        #region [Property]
        private 라인클리어런스기록 _mainWnd;

        private LINECLEARANCE.OUTDATACollection _LINECLEARANCEOUTDATA;
        public LINECLEARANCE.OUTDATACollection LINECLEARANCEOUTDATA
        {
            get { return _LINECLEARANCEOUTDATA; }
            set
            {
                _LINECLEARANCEOUTDATA = value;
                OnPropertyChanged("LINECLEARANCEOUTDATA");
            }
        }
        
        string _ProdTeamId = string.Empty;

        #endregion
        #region [BizRule]
        // 라인클리어런스 정보 조회
        //private BR_BRS_GET_Equipment_LineClearance _BR_BRS_GET_Equipment_LineClearance;
        //public BR_BRS_GET_Equipment_LineClearance BR_BRS_GET_Equipment_LineClearance
        //{
        //    get { return _BR_BRS_GET_Equipment_LineClearance; }
        //    set
        //    {
        //        _BR_BRS_GET_Equipment_LineClearance = value;
        //        OnPropertyChanged("BR_BRS_GET_Equipment_LineClearance");
        //    }
        //}

        private BR_BRS_GET_CommonCode_LineClearance _BR_BRS_GET_CommonCode_LineClearance;        
        public BR_BRS_GET_CommonCode_LineClearance BR_BRS_GET_CommonCode_LineClearance
        {
            get { return _BR_BRS_GET_CommonCode_LineClearance; }
            set
            {
                _BR_BRS_GET_CommonCode_LineClearance = value;
                OnPropertyChanged("BR_BRS_GET_CommonCode_LineClearance");
            }
        }

        #endregion
        #region [Command]
        //초기 데이터 세팅 및 그리드 정보 조회
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

                            ///
                            if (arg == null || !(arg is 라인클리어런스기록)) return;
                            else
                            {
                               

                                _mainWnd = arg as 라인클리어런스기록;

                                _mainWnd.BusyIn.IsBusy = true;


                                // 2022.06.17 김호연 라인클리어런스 UI에서 기록하도록 변경
                                // 기존 소스
                                /***********************************************************************************/
                                //_BR_BRS_GET_Equipment_LineClearance.INDATAs.Clear();
                                //_BR_BRS_GET_Equipment_LineClearance.OUTDATAs.Clear();

                                //var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                                //if (inputValues.Count > 0 && !string.IsNullOrWhiteSpace(inputValues[0].Raw.ACTVAL))
                                //{
                                //    _BR_BRS_GET_Equipment_LineClearance.INDATAs.Add(new BR_BRS_GET_Equipment_LineClearance.INDATA
                                //    {
                                //        EQPTID = inputValues[0].Raw.ACTVAL
                                //    });
                                //}
                                //else
                                //{
                                //    _BR_BRS_GET_Equipment_LineClearance.INDATAs.Add(new BR_BRS_GET_Equipment_LineClearance.INDATA
                                //    {
                                //        EQPTID = AuthRepositoryViewModel.Instance.RoomID
                                //    });
                                //}

                                //if (await _BR_BRS_GET_Equipment_LineClearance.Execute() == true)
                                //    _mainWnd.MainDataGrid.Refresh();
                                /***********************************************************************************/
                                // 신규 소스
                                /***********************************************************************************/
                                _BR_BRS_GET_CommonCode_LineClearance.INDATAs.Clear();
                                _BR_BRS_GET_CommonCode_LineClearance.OUTDATAs.Clear();
                                _BR_BRS_GET_CommonCode_LineClearance.INDATAs.Add(new BR_BRS_GET_CommonCode_LineClearance.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await _BR_BRS_GET_CommonCode_LineClearance.Execute() == true)
                                {
                                    _ProdTeamId = _BR_BRS_GET_CommonCode_LineClearance.OUTDATA_OrderTypes[0].PRODTEAMID;
                                    
                                    string contentsYes = string.Empty;
                                    string contentsNo = string.Empty;

                                    if ("고형제".Equals(_ProdTeamId))
                                    {
                                        contentsYes = "Yes";
                                        contentsNo = "No";
                                    }
                                    else
                                    {
                                        contentsYes = "적합";
                                        contentsNo = "부적합";
                                    }

                                    foreach (var outdata in _BR_BRS_GET_CommonCode_LineClearance.OUTDATAs)
                                    {
                                        var temp = new LINECLEARANCE.OUTDATA
                                        {
                                            ITEMNAME = outdata.CMCDNAME,
                                            RESULTYES = false,
                                            RESULTNO = false,
                                            RESULTNA = false,
                                            ContentsYes = contentsYes,
                                            ContentsNo = contentsNo
                                        };
                                        _LINECLEARANCEOUTDATA.Add(temp);
                                        _mainWnd.MainDataGrid.Refresh();
                                    }
                                }
                                /***********************************************************************************/

                                _mainWnd.BusyIn.IsBusy = false;
                            }
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _mainWnd.BusyIn.IsBusy = false;
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        // 조회 결과 기록
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///
                            _mainWnd.BusyIn.IsBusy = true;

                            // 2022.06.17 김호연 라인클리어런스 UI에서 기록하도록 변경
                            // 기존 소스
                            /***********************************************************************************/
                            //if (_BR_BRS_GET_Equipment_LineClearance.OUTDATAs.Count > 0)
                            /***********************************************************************************/
                            // 신규 소스
                            /***********************************************************************************/
                            if (_BR_BRS_GET_CommonCode_LineClearance.OUTDATAs.Count > 0)
                            /***********************************************************************************/
                            {
                                // 전자서명(기록값 변경)
                                var authHelper = new iPharmAuthCommandHelper();

                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
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

                                // 조회내용 기록
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "라인클리어런스기록",
                                    "라인클리어런스기록",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // XML 변환
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                // 2021.08.20 박희돈 순번 ebr에 안나오도록 변경
                                //dt.Columns.Add(new DataColumn("순번"));
                                dt.Columns.Add(new DataColumn("점검사항"));
                                dt.Columns.Add(new DataColumn("결과"));

                                // 2022.06.17 김호연 라인클리어런스 UI에서 기록하도록 변경
                                // 기존 소스
                                /***********************************************************************************/
                                //foreach (var item in _BR_BRS_GET_Equipment_LineClearance.OUTDATAs)
                                //{
                                //    var row = dt.NewRow();

                                //    // 2021.08.20 박희돈 순번 ebr에 안나오도록 변경
                                //    //row["순번"] = item.NO != null ? item.NO : "";
                                //    row["점검사항"] = item.ITEMNAME != null ? item.ITEMNAME : "";
                                //    row["결과"] = item.RESULT != null ? item.RESULT : "";

                                //    dt.Rows.Add(row);
                                //}
                                /***********************************************************************************/
                                // 신규 소스
                                /***********************************************************************************/
                                var temp = _mainWnd.MainDataGrid.ItemsSource as LINECLEARANCE.OUTDATACollection;

                                foreach (var item in temp)
                                {
                                    var row = dt.NewRow();

                                    row["점검사항"] = item.ITEMNAME != null ? item.ITEMNAME : "";
                                    if (!item.RESULTYES && !item.RESULTNO && !item.RESULTNA)
                                    {
                                        _mainWnd.BusyIn.IsBusy = false;
                                        OnMessage("결과를 모두 선택해 주세요");
                                        return;
                                    }
                                    if (item.RESULTYES)
                                    {
                                        if("고형제".Equals(_ProdTeamId))
                                        {
                                            row["결과"] = "YES";
                                        }
                                        else
                                        {
                                            row["결과"] = "적합";
                                        }
                                    }
                                    else if (item.RESULTNO)
                                    {
                                        if ("고형제".Equals(_ProdTeamId))
                                        {
                                            row["결과"] = "NO";
                                        }
                                        else
                                        {
                                            row["결과"] = "부적합";
                                        }
                                    }
                                    else
                                    {
                                        row["결과"] = "N/A";
                                    }
                                    dt.Rows.Add(row);
                                }
                                /***********************************************************************************/

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
                            }

                            _mainWnd.BusyIn.IsBusy = false;
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _mainWnd.BusyIn.IsBusy = false;
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }

        public ICommand NoRecordConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommandAsync"] = false;
                            CommandCanExecutes["NoRecordConfirmCommandAsync"] = false;
                            
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "SVP소분원료확인및무게측정",
                                "SVP소분원료확인및무게측정",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //XML 형식으로 저장
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            
                            dt.Columns.Add(new DataColumn("점검사항"));
                            dt.Columns.Add(new DataColumn("결과"));

                            var row = dt.NewRow();
                            row["점검사항"] = "N/A";
                            row["결과"] = "N/A";
                            dt.Rows.Add(row);

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

                            _mainWnd.Close();

                            //
                            CommandResults["NoRecordConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NoRecordConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommandAsync") ?
                        CommandCanExecutes["NoRecordConfirmCommandAsync"] : (CommandCanExecutes["NoRecordConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Constructor]
        public 라인클리어런스기록ViewModel()
        {
            //_BR_BRS_GET_Equipment_LineClearance = new BR_BRS_GET_Equipment_LineClearance();
            _BR_BRS_GET_CommonCode_LineClearance = new BR_BRS_GET_CommonCode_LineClearance();
            _LINECLEARANCEOUTDATA = new LINECLEARANCE.OUTDATACollection();
        }
        #endregion


        public class LINECLEARANCE : BizActorRuleBase
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
                private string _ITEMNAME;
                [BizActorOutputItemAttribute()]
                public string ITEMNAME
                {
                    get
                    {
                        return this._ITEMNAME;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._ITEMNAME = value;
                            this.CheckIsOriginal("ITEMNAME", value);
                            this.OnPropertyChanged("ITEMNAME");
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

                private bool _RESULTYES;
                [BizActorOutputItemAttribute()]
                public bool RESULTYES
                {
                    get
                    {
                        return this._RESULTYES;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._RESULTYES = value;
                            this.CheckIsOriginal("RESULTYES", value);
                            this.OnPropertyChanged("RESULTYES");
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

                private bool _RESULTNO;
                [BizActorOutputItemAttribute()]
                public bool RESULTNO
                {
                    get
                    {
                        return this._RESULTNO;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._RESULTNO = value;
                            this.CheckIsOriginal("RESULTNO", value);
                            this.OnPropertyChanged("RESULTNO");
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

                private bool _RESULTNA;
                [BizActorOutputItemAttribute()]
                public bool RESULTNA
                {
                    get
                    {
                        return this._RESULTNA;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._RESULTNA = value;
                            this.CheckIsOriginal("RESULTNA", value);
                            this.OnPropertyChanged("RESULTNA");
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

                private string _ContentsYes;
                [BizActorOutputItemAttribute()]
                public string ContentsYes
                {
                    get
                    {
                        return this._ContentsYes;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._ContentsYes = value;
                            this.CheckIsOriginal("ContentsYes", value);
                            this.OnPropertyChanged("ContentsYes");
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


                private string _ContentsNo;
                [BizActorOutputItemAttribute()]
                public string ContentsNo
                {
                    get
                    {
                        return this._ContentsNo;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._ContentsNo = value;
                            this.CheckIsOriginal("ContentsNo", value);
                            this.OnPropertyChanged("ContentsNo");
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
            public LINECLEARANCE()
            {
                _OUTDATAs = new OUTDATACollection();
            }
        }
    }
}