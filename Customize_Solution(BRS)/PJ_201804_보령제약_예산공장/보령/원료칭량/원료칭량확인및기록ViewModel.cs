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
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace 보령
{
    public class 원료칭량확인및기록ViewModel : ViewModelBase
    {
        #region [Property]
        public 원료칭량확인및기록ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderDispense_Result = new BR_BRS_SEL_ProductionOrderDispense_Result();
            _XMLDataSet = new ObservableCollection<CampaignOrderXML>();
        }

        원료칭량확인및기록 _mainWnd;

        #region Campaign Production
        private BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection _OrderList;
        public BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection OrderList
        {
            get { return _OrderList; }
            set
            {
                _OrderList = value;
                OnPropertyChanged("OrderList");
            }
        }
        private bool _CanSelectOrder;
        public bool CanSelectOrder
        {
            get { return _CanSelectOrder; }
            set
            {
                _CanSelectOrder = value;
                OnPropertyChanged("CanSelectOrder");
            }
        }

        private ObservableCollection<CampaignOrderXML> _XMLDataSet;
        #endregion

        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_ProductionOrderDispense_Result _BR_BRS_SEL_ProductionOrderDispense_Result;
        public BR_BRS_SEL_ProductionOrderDispense_Result BR_BRS_SEL_ProductionOrderDispense_Result
        {
            get { return _BR_BRS_SEL_ProductionOrderDispense_Result; }
            set
            {
                _BR_BRS_SEL_ProductionOrderDispense_Result = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderDispense_Result");
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

                            ///

                            if (arg != null && arg is 원료칭량확인및기록)
                            {
                                _mainWnd = arg as 원료칭량확인및기록;

                                #region Campaign Order
                                OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                                CanSelectOrder = OrderList.Count > 0 ? true : false;
                                #endregion

                                if (OrderList.Count == 1)
                                    SerachWeighingResultCommandAsync.Execute(null);
                            }

                            ///

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

        public ICommand SerachWeighingResultCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SerachWeighingResultCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SerachWeighingResultCommandAsync"] = false;
                            CommandCanExecutes["SerachWeighingResultCommandAsync"] = false;

                            ///
                            _BR_BRS_SEL_ProductionOrderDispense_Result.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderDispense_Result.OUTDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderDispense_Result.INDATAs.Add(new BR_BRS_SEL_ProductionOrderDispense_Result.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            });

                            await _BR_BRS_SEL_ProductionOrderDispense_Result.Execute();

                            if(_BR_BRS_SEL_ProductionOrderDispense_Result.OUTDATAs.Count > 0)
                            {
                                await SetXMLDataTable();
                            }
                            ///

                            CommandResults["SerachWeighingResultCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SerachWeighingResultCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SerachWeighingResultCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("SerachWeighingResultCommandAsync") ?
                       CommandCanExecutes["SerachWeighingResultCommandAsync"] : (CommandCanExecutes["SerachWeighingResultCommandAsync"] = true);
               });
            }
        }

        public ICommand ConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;

                            IsBusy = true;
                            ///

                            if (_OrderList.Count == _XMLDataSet.Count)
                            {
                                iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();
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

                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "원료칭량확인및기록",
                                    "원료칭량확인및기록",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                dt.Columns.Add(new DataColumn("오더번호"));
                                //-------------------------------------------------------------------------------------------------------
                                dt.Columns.Add(new DataColumn("원료코드"));
                                dt.Columns.Add(new DataColumn("원료명"));
                                dt.Columns.Add(new DataColumn("규격"));
                                dt.Columns.Add(new DataColumn("하한"));
                                dt.Columns.Add(new DataColumn("기준"));
                                dt.Columns.Add(new DataColumn("상한"));
                                dt.Columns.Add(new DataColumn("칭량용기"));
                                dt.Columns.Add(new DataColumn("GrossWeight"));
                                dt.Columns.Add(new DataColumn("NetWeight"));
                                dt.Columns.Add(new DataColumn("시험번호"));
                                dt.Columns.Add(new DataColumn("저울번호"));
                                dt.Columns.Add(new DataColumn("작업자"));
                                dt.Columns.Add(new DataColumn("칭량순번"));
                                ds.Tables.Add(dt);

                                if (_XMLDataSet.Count > 0)
                                {
                                    foreach (var item in _XMLDataSet)
                                    {
                                        foreach (DataRow row in item.XML.Rows)
                                            dt.Rows.Add(row.ItemArray);
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
                                }
                            }
                            else
                                OnMessage("모든 오더정보가 조회되지 않았습니다.");

                            CommandResults["ConfirmCommand"] = false;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["ConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommand"] = true;
                            IsBusy = false;
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

        private async Task SetXMLDataTable()
        {
            try
            {
                var dt = new DataTable("DATA");

                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                dt.Columns.Add(new DataColumn("오더번호"));
                //-------------------------------------------------------------------------------------------------------
                dt.Columns.Add(new DataColumn("원료코드"));
                dt.Columns.Add(new DataColumn("원료명"));
                dt.Columns.Add(new DataColumn("규격"));
                dt.Columns.Add(new DataColumn("하한"));
                dt.Columns.Add(new DataColumn("기준"));
                dt.Columns.Add(new DataColumn("상한"));
                dt.Columns.Add(new DataColumn("칭량용기"));
                dt.Columns.Add(new DataColumn("GrossWeight"));
                dt.Columns.Add(new DataColumn("NetWeight"));
                dt.Columns.Add(new DataColumn("시험번호"));
                dt.Columns.Add(new DataColumn("저울번호"));
                dt.Columns.Add(new DataColumn("작업자"));
                dt.Columns.Add(new DataColumn("칭량순번"));

                foreach (var item in _BR_BRS_SEL_ProductionOrderDispense_Result.OUTDATAs)
                {
                    var row = dt.NewRow();

                    //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                    row["오더번호"] = item.POID != null ? item.POID : "";
                    //-------------------------------------------------------------------------------------------------------
                    row["원료코드"] = item.MTRLID != null ? item.MTRLID.ToString() : "";
                    row["원료명"] = item.MTRLNAME != null ? item.MTRLNAME.ToString() : "";
                    row["규격"] = item.STANDARD != null ? item.STANDARD.ToString() : "";
                    row["하한"] = item.LOWER != null ? item.LOWER.ToString() : "";
                    row["기준"] = item.TARGET != null ? item.TARGET.ToString() : "";
                    row["상한"] = item.UPPER != null ? item.UPPER.ToString() : "";
                    row["칭량용기"] = item.DSP_VESSEL != null ? item.DSP_VESSEL.ToString() : "";
                    row["GrossWeight"] = item.TOTALWEIGHT != null ? item.TOTALWEIGHT.ToString() : "";
                    row["NetWeight"] = item.MSUBLOTQTY != null ? item.MSUBLOTQTY.ToString() : "";
                    row["시험번호"] = item.MLOTID != null ? item.MLOTID.ToString() : "";
                    row["저울번호"] = item.SCALEID != null ? item.SCALEID.ToString() : "";
                    row["작업자"] = item.USERID != null ? item.USERID.ToString() : "";
                    row["칭량순번"] = item.WEIGHINGSEQ != null ? item.WEIGHINGSEQ.ToString() : "";

                    dt.Rows.Add(row);
                }

                // 없으면 추가 있으면 XML만 변경
                CampaignOrderXML delRow = null;
                foreach (var item in _XMLDataSet)
                {
                    if (item.PoId == _mainWnd.CurrentOrder.ProductionOrderID)
                        delRow = item;
                }
                if(delRow != null)
                    _XMLDataSet.Remove(delRow);

                _XMLDataSet.Add(new CampaignOrderXML
                {
                    PoId = _mainWnd.CurrentOrder.ProductionOrderID,
                    XML = dt
                });

                return;
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
    }
}
