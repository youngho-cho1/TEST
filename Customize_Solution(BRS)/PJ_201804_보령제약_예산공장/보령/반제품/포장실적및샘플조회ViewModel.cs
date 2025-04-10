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

namespace 보령
{
    public class 포장실적및샘플조회ViewModel : ViewModelBase
    {
        #region [Property]

        포장실적및샘플조회 _MaindWnd;

        private string _BatchNo;
        public string BatchNo
        {
            get { return _BatchNo; }
            set
            {
                _BatchNo = value;
                OnPropertyChanged("BatchNo");
            }
        }

        private string _OrderNo;
        public string OrderNo
        {
            get { return _OrderNo; }
            set
            {
                _OrderNo = value;
                OnPropertyChanged("OrderNo");
            }              
        }

        private string _Operation;
        public string Operation
        {
            get { return _Operation; }
            set
            {
                _Operation = value;
                OnPropertyChanged("Operation");
            }
        }

        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_PackingOutPut_From_TnT _BR_BRS_SEL_PackingOutPut_From_TnT;
        public BR_BRS_SEL_PackingOutPut_From_TnT BR_BRS_SEL_PackingOutPut_From_TnT
        {
            get { return _BR_BRS_SEL_PackingOutPut_From_TnT; }
            set
            {
                _BR_BRS_SEL_PackingOutPut_From_TnT = value;
                OnPropertyChanged("BR_BRS_SEL_PackingOutPut_From_TnT");
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
                        using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                        {
                            try
                            {
                                CommandCanExecutes["LoadedCommand"] = false;
                                CommandResults["LoadedCommand"] = false;
                                IsBusy = true;

                                if (arg != null)
                                {
                                    _MaindWnd = arg as 포장실적및샘플조회;

                                    BatchNo = _MaindWnd.CurrentOrder.BatchNo;
                                    OrderNo = _MaindWnd.CurrentOrder.ProductionOrderID;
                                    Operation = _MaindWnd.CurrentOrder.OrderProcessSegmentName;

                                    _BR_BRS_SEL_PackingOutPut_From_TnT.INDATAs.Clear();
                                    _BR_BRS_SEL_PackingOutPut_From_TnT.OUTDATAs.Clear();
                                    _BR_BRS_SEL_PackingOutPut_From_TnT.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_PackingOutPut_From_TnT.INDATA
                                    {
                                        POID = _MaindWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _MaindWnd.CurrentOrder.OrderProcessSegmentID,
                                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                    });

                                    await _BR_BRS_SEL_PackingOutPut_From_TnT.Execute();
                                }
                                CommandResults["LoadedCommand"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["LoadedCommand"] = false;
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

        public ICommand ConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        using ( await AwaitableLocks["ConfirmCommand"].EnterAsync())
                        {
                            try
                            {
                                CommandResults["ConfirmCommand"] = false;
                                CommandCanExecutes["ConfirmCommand"] = false;
                                IsBusy = true;
                                
                                if (_BR_BRS_SEL_PackingOutPut_From_TnT.OUTDATAs.Count == 0) 
                                {
                                    OnMessage("기록할 데이터가 없습니다.");
                                    return;
                                }

                                iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                                if (_MaindWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _MaindWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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
                                        "", _MaindWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }

                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role, 
                                    Common.enumAccessType.Create, 
                                    "포장실적및샘플조회", 
                                    "포장실적및샘플조회", 
                                    false, 
                                    "OM_ProductionOrder_SUI", 
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                dt.Columns.Add(new DataColumn("최종포장실적"));
                                dt.Columns.Add(new DataColumn("샘플수량합계"));
                                dt.Columns.Add(new DataColumn("보관검체"));
                                dt.Columns.Add(new DataColumn("실험검체"));
                                dt.Columns.Add(new DataColumn("일반검체"));
                                dt.Columns.Add(new DataColumn("안전검체"));

                                if (_BR_BRS_SEL_PackingOutPut_From_TnT.OUTDATAs.Count > 0)
                                {
                                    foreach (var item in _BR_BRS_SEL_PackingOutPut_From_TnT.OUTDATAs)
                                    {
                                        var row = dt.NewRow();
                                        row["최종포장실적"] = item.OUTPUTQTY != null ? item.OUTPUTQTY.ToString() : "";
                                        row["샘플수량합계"] = item.TOTAL_SAMPLE_QTY != null ? item.TOTAL_SAMPLE_QTY.ToString() : "";
                                        row["보관검체"] = item.STORAGE_SAMPLE_QTY != null ? item.STORAGE_SAMPLE_QTY.ToString() : "";
                                        row["실험검체"] = item.INSPECTION_SAMPLE_QTY != null ? item.INSPECTION_SAMPLE_QTY.ToString() : "";
                                        row["일반검체"] = item.GENERAL_SAMPLE_QTY != null ? item.GENERAL_SAMPLE_QTY.ToString() : "";
                                        row["안전검체"] = item.STABILITY_SAMPLE_QTY != null ? item.STABILITY_SAMPLE_QTY.ToString() : "";
                                        dt.Rows.Add(row);
                                    }

                                    ds.Tables.Add(dt);

                                    var xml = BizActorRuleBase.CreateXMLStream(ds);
                                    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);


                                    _MaindWnd.CurrentInstruction.Raw.ACTVAL = _MaindWnd.TableTypeName;
                                    _MaindWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                    var result = await _MaindWnd.Phase.RegistInstructionValue(_MaindWnd.CurrentInstruction, true);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _MaindWnd.CurrentInstruction.Raw.IRTGUID, result));
                                    }

                                    if (_MaindWnd.Dispatcher.CheckAccess()) _MaindWnd.DialogResult = true;
                                    else _MaindWnd.Dispatcher.BeginInvoke(() => _MaindWnd.DialogResult = true);
                                }

                                CommandResults["ConfirmCommand"] = false;
                            }
                            catch(Exception ex)
                            {
                                CommandCanExecutes["ConfirmCommand"] = false;
                                OnException(ex.Message, ex);
                            }
                            finally
                            {
                                CommandCanExecutes["ConfirmCommand"] = true;
                                IsBusy= false;
                            }
                        }
                    }, arg =>
                        {
                            return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                                CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
                        });
            }
        }
        #endregion

        #region [Generator]

        public 포장실적및샘플조회ViewModel()
        {
            _BR_BRS_SEL_PackingOutPut_From_TnT = new BR_BRS_SEL_PackingOutPut_From_TnT();
        }

        #endregion

    }
}
