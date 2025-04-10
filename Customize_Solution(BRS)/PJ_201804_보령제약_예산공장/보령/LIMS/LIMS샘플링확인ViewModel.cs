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

namespace 보령
{
    public class LIMS샘플링확인ViewModel : ViewModelBase
    {
        #region [Property]

        LIMS샘플링확인 _mainWnd;
        
        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_ProductionOrder_SAMPLEQTY _BR_BRS_SEL_ProductionOrder_SAMPLEQTY;
        public BR_BRS_SEL_ProductionOrder_SAMPLEQTY BR_BRS_SEL_ProductionOrder_SAMPLEQTY
        {
            get { return _BR_BRS_SEL_ProductionOrder_SAMPLEQTY; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_SAMPLEQTY = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_SAMPLEQTY");
            }
        }

        #endregion

        #region [Constructor]

        public LIMS샘플링확인ViewModel()
        {
            _BR_BRS_SEL_ProductionOrder_SAMPLEQTY = new BR_BRS_SEL_ProductionOrder_SAMPLEQTY();
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
                                _mainWnd = arg as LIMS샘플링확인;

                                _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_SAMPLEQTY.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                await _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.Execute();
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
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;
                            IsBusy = true;

                            // 조회결과 확인
                            if (_BR_BRS_SEL_ProductionOrder_SAMPLEQTY.OUTDATAs.Count == 0)
                            {
                                OnMessage("기록할 데이터가 없습니다.");
                                return;
                            }

                            // 적합여부 확인
                            string strAccept = "ACCEPT";
                            var br = new BR_PHR_SEL_CommonCode_cmcdtype();
                            br.INDATAs.Add(new BR_PHR_SEL_CommonCode_cmcdtype.INDATA
                            {
                                CMCDTYPE = "PHR_QCSTATE",
                                CMCDIUSE = "Y",
                                CMCODE = "accept"
                            });
                            if (await br.Execute() && br.OUTDATAs.Count > 0)
                                strAccept = br.OUTDATAs[0].CMCDNAME;

                            foreach (var item in _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.OUTDATAs)
                            {
                                if (item.MSUBLOTSTAT != strAccept)
                                {
                                    OnMessage("시험결과가 부적합 입니다.");
                                    return;
                                }
                                //2023.06.26 박희돈 반제품 유효기간은 반제품보관상태확인에서 코멘트 작성 후 연장함으로 유효기간 체크로직 제외
                                //else if (item.MSUBLOTEXPIRYSTAT != strAccept)
                                //{
                                //    OnMessage("반제품 유효기간이 부적합 입니다.");
                                //    return;
                                //}
                            }

                            // 전자서명 요청
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
                                "LIMS샘플링확인", 
                                "LIMS샘플링확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            dt.Columns.Add(new DataColumn("MSUBLOTID"));
                            dt.Columns.Add(new DataColumn("MSUBLOTSTAT"));
                            dt.Columns.Add(new DataColumn("MSUBLOTEXPIRYSTAT"));
                            dt.Columns.Add(new DataColumn("OPSGNAME"));
                       
                            if (_BR_BRS_SEL_ProductionOrder_SAMPLEQTY.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrder_SAMPLEQTY.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["MSUBLOTID"] = item.MSUBLOTID != null ? item.MSUBLOTID.ToString() : "";
                                    row["MSUBLOTSTAT"] = item.MSUBLOTSTAT != null ? item.MSUBLOTSTAT.ToString() : "";
                                    row["MSUBLOTEXPIRYSTAT"] = item.MSUBLOTEXPIRYSTAT != null ? item.MSUBLOTEXPIRYSTAT.ToString() : "";
                                    row["OPSGNAME"] = item.OPSGNAME != null ? item.OPSGNAME.ToString() : "";
                                    dt.Rows.Add(row);
                                }

                                ds.Tables.Add(dt);

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
    }
}
