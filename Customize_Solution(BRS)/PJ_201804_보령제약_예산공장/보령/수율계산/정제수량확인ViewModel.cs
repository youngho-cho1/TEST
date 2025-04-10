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
using System.Windows.Threading;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 정제수량확인ViewModel : ViewModelBase
    {
        #region [Property]
        private 정제수량확인 _mainWnd;
        public 정제수량확인ViewModel()
        {
            _BR_PHR_SEL_ProductionOrder_OrderSummary = new BR_PHR_SEL_ProductionOrder_OrderSummary();
            _BR_BRS_REG_Compress_GoodQty = new BR_BRS_REG_Compress_GoodQty();
            _BR_BRS_SEL_TabletPressGoodCount = new BR_BRS_SEL_TabletPressGoodCount();
        }
        private decimal _totalCount = 0;
        private bool _IsEditable = false;
        private string _comment = string.Empty;

        public bool IsEditable
        {
            get { return _IsEditable; }
            set
            {
                _IsEditable = value;
                OnPropertyChanged("IsEditable");
            }
        }
        private string _PRODUCTUOM;
        public string PRODUCTUOM
        {
            get { return _PRODUCTUOM; }
            set
            {
                _PRODUCTUOM = value;
                OnPropertyChanged("PRODUCTUOM");
            }
        }

        #endregion
        #region [BizRule]
        private BR_PHR_SEL_ProductionOrder_OrderSummary _BR_PHR_SEL_ProductionOrder_OrderSummary;
        private BR_BRS_SEL_TabletPressGoodCount _BR_BRS_SEL_TabletPressGoodCount;
        public BR_BRS_SEL_TabletPressGoodCount BR_BRS_SEL_TabletPressGoodCount
        {
            get { return _BR_BRS_SEL_TabletPressGoodCount; }
            set { _BR_BRS_SEL_TabletPressGoodCount = value; }
        }
        private BR_BRS_REG_Compress_GoodQty _BR_BRS_REG_Compress_GoodQty;
        #endregion
        #region [Command]

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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is 정제수량확인)
                            {
                                _mainWnd = arg as 정제수량확인;

                                _BR_PHR_SEL_ProductionOrder_OrderSummary.INDATEs.Add(new BR_PHR_SEL_ProductionOrder_OrderSummary.INDATE
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    ISUSE = "Y"
                                });

                                if (await _BR_PHR_SEL_ProductionOrder_OrderSummary.Execute())
                                {
                                    PRODUCTUOM = _BR_PHR_SEL_ProductionOrder_OrderSummary.OUTDATAs[0].NOTATION;
                                }

                                await GetGoodQuantity();
                            }
                            ///

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

        public ICommand RequestCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RequestCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RequestCommand"] = false;
                            CommandCanExecutes["RequestCommand"] = false;

                            ///

                            await GetGoodQuantity(true);

                            ///

                            CommandResults["RequestCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RequestCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RequestCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RequestCommand") ?
                        CommandCanExecutes["RequestCommand"] : (CommandCanExecutes["RequestCommand"] = true);
                });
            }
        }
        public ICommand ChageCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {

                    try
                    {
                        CommandResults["ChageCommandAsync"] = false;
                        CommandCanExecutes["ChageCommandAsync"] = false;

                        ///

                        // 강제진행 권한이 있는 유저가 서명 시 기능활성화
                        var authHelper = new iPharmAuthCommandHelper();
                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                        enumRoleType inspectorRole = enumRoleType.ROLE001;
                        if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "기록값 변경 시 코멘트 작성 필요합니다. ",
                                "양품수량 기록값 변경",
                                true,
                                "OM_ProductionOrder_Deviation",
                                "",
                                this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                        {
                            return;
                        }

                        _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                        _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");

                        IsEditable = true;

                        _comment = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation");

                        CommandResults["ChageCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChageCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["ChageCommandAsync"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChageCommandAsync") ?
                        CommandCanExecutes["ChageCommandAsync"] : (CommandCanExecutes["ChageCommandAsync"] = true);
                }
                    );

            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["ConfirmCommandAsync"] = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        if (BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Count < 1) throw new Exception("조회된 결과가 없습니다.");

                        var authHelper = new iPharmAuthCommandHelper();
                        // 전자서명 요청
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
                        else
                        {
                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "정제수량확인",
                                "정제수량확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }
                        }


                        string userid = string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")) ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI");
                       
                        var ds = new DataSet();
                        var dt = new DataTable("DATA");
                        ds.Tables.Add(dt);

                        dt.Columns.Add(new DataColumn("설비코드"));
                        dt.Columns.Add(new DataColumn("태그종류"));
                        dt.Columns.Add(new DataColumn("수량"));

                        _totalCount = 0;    // 2021.10.05 박희돈 양품수량 합계 전 초기화


                        foreach (var item in BR_BRS_SEL_TabletPressGoodCount.OUTDATAs)
                        {
                            decimal chk = 0;

                            if (decimal.TryParse(item.TAGVALUE, out chk))
                            {
                                if (item.TAGDESC == "양품 수량")
                                {
                                    _totalCount += chk;
                                }
                            }

                            var row = dt.NewRow();

                            row["설비코드"] = string.IsNullOrWhiteSpace(item.EQPTID) ? "" : item.EQPTID;
                            row["태그종류"] = string.IsNullOrWhiteSpace(item.TAGDESC) ? "" : item.TAGDESC;
                            row["수량"] = string.Format("{0:#,0} {1}", chk, _PRODUCTUOM);

                            dt.Rows.Add(row);
                        }

                        // 정제 양품 수량 기록 비즈룰, 수율계산에 사용됨
                        _BR_BRS_REG_Compress_GoodQty.INDATAs.Clear();
                        _BR_BRS_REG_Compress_GoodQty.INDATAs.Add(new BR_BRS_REG_Compress_GoodQty.INDATA
                        {
                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                            GOODQTY = _totalCount.ToString(),
                            USERID = userid
                        });

                        if (await _BR_BRS_REG_Compress_GoodQty.Execute())
                        {
                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (IsEditable)
                            {
                                var bizrule = new BR_PHR_REG_InstructionComment();

                                bizrule.IN_Comments.Add(
                                    new BR_PHR_REG_InstructionComment.IN_Comment
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        COMMENTTYPE = "CM008",
                                        COMMENT = _comment
                                    }
                                    );
                                bizrule.IN_IntructionResults.Add(
                                    new BR_PHR_REG_InstructionComment.IN_IntructionResult
                                    {
                                        RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                        ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                        IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                        IRTRSTGUID = _mainWnd.CurrentInstruction.Raw.IRTRSTGUID,
                                        IRTSEQ = (int)_mainWnd.CurrentInstruction.Raw.IRTSEQ
                                    }
                                    );

                                await bizrule.Execute();
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                        }

                        ///
                        CommandResults["ConfirmCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ConfirmCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                }
                    );
            }
        }

        #endregion
        #region [User Define]
        private async Task GetGoodQuantity(bool RequestFlag = false)
        {
            try
            {
                var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Clear();

                if (RequestFlag == false && bytearray != null)
                {
                    string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                    DataSet ds = new DataSet();
                    ds.ReadXmlFromString(xml);

                    if (ds.Tables.Count == 1 && ds.Tables[0].TableName == "DATA")
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Add(new BR_BRS_SEL_TabletPressGoodCount.OUTDATA
                            {
                                EQPTID = row["설비코드"].ToString(),
                                TAGDESC = row["태그종류"].ToString(),
                                TAGVALUE = row["수량"].ToString().Replace(_PRODUCTUOM, "")
                            });
                        }
                    }
                }
                else
                {
                    BR_BRS_SEL_TabletPressGoodCount.INDATAs.Clear();
                    BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Clear();
                    BR_BRS_SEL_TabletPressGoodCount.INDATAs.Add(new BR_BRS_SEL_TabletPressGoodCount.INDATA
                    {
                        POID = _mainWnd.CurrentOrder.ProductionOrderID ?? "",
                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID ?? "",
                        EQCLID = _mainWnd.CurrentInstruction.Raw.EQCLID ?? ""
                    });

                    //2024.05.21 김도연 : 설비 통신 초기화하고 MES로 전달할 경우, 태그값이 0으로 올라옴
                    await BR_BRS_SEL_TabletPressGoodCount.Execute();

                    if (_BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Count < 1)
                    {
                        BR_BRS_SEL_TabletPressGoodCount.OUTDATAs.Add(new BR_BRS_SEL_TabletPressGoodCount.OUTDATA
                        {
                            EQPTID = "N/A",
                            TAGDESC = "양품 수량",
                            TAGVALUE = "0"
                        });
                    }
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
