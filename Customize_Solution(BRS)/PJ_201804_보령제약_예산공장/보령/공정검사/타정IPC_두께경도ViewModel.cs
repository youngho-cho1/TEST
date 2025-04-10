using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Net;
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
    public class 타정IPC_두께경도ViewModel : ViewModelBase
    {
        #region Property
        public 타정IPC_두께경도ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderIPCResult = new BR_BRS_SEL_ProductionOrderIPCResult();
            _BR_BRS_SEL_ProductionOrderIPCStandard = new BR_BRS_SEL_ProductionOrderIPCStandard();
            _IPCResults = new BR_BRS_SEL_ProductionOrderIPCResult.OUTDATACollection();
            _BR_BRS_REG_ProductionOrderTestResult = new BR_BRS_REG_ProductionOrderTestResult();
        }

        private 타정IPC_두께경도 _mainWnd;
        private string IPC_TSID = "타정IPC_두께경도";

        private IPCControlData _ThicknessIPCData;
        public IPCControlData ThicknessIPCData
        {
            get { return _ThicknessIPCData; }
            set
            {
                _ThicknessIPCData = value;
                OnPropertyChanged("ThicknessIPCData");
            }
        }
        private IPCControlData _LongitudeIPCData;
        public IPCControlData LongitudeIPCData
        {
            get { return _LongitudeIPCData; }
            set
            {
                _LongitudeIPCData = value;
                OnPropertyChanged("LongitudeIPCData");
            }
        }

        private BR_BRS_SEL_ProductionOrderIPCResult.OUTDATACollection _IPCResults;
        public BR_BRS_SEL_ProductionOrderIPCResult.OUTDATACollection IPCResults
        {
            get { return _IPCResults; }
            set
            {
                _IPCResults = value;
                OnPropertyChanged("IPCResults");
            }
        } 

        #endregion
        #region BizRule
        private BR_BRS_SEL_ProductionOrderIPCResult _BR_BRS_SEL_ProductionOrderIPCResult;
        private BR_BRS_SEL_ProductionOrderIPCStandard _BR_BRS_SEL_ProductionOrderIPCStandard;
        private BR_BRS_REG_ProductionOrderTestResult _BR_BRS_REG_ProductionOrderTestResult;
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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if(arg != null && arg is 타정IPC_두께경도)
                            {
                                _mainWnd = arg as 타정IPC_두께경도;

                                // IPC 기준정보 조회
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Add(new BR_BRS_SEL_ProductionOrderIPCStandard.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    TSID = IPC_TSID
                                });

                                if (await _BR_BRS_SEL_ProductionOrderIPCStandard.Execute())
                                {
                                    ThicknessIPCData = IPCControlData.SetIPCControlData(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0]);
                                    LongitudeIPCData = IPCControlData.SetIPCControlData(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1]);
                                    await GetIPCResult();
                                }                                                                                             
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


        public ICommand RegisterIPCCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RegisterIPCCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RegisterIPCCommandAsync"] = false;
                            CommandCanExecutes["RegisterIPCCommandAsync"] = false;

                            ///
                            
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Clear();
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Clear();
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Clear();

                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_IPC");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("IPC 결과를 기록합니다."),
                                string.Format("IPC 결과를 기록합니다."),
                                false,
                                "OM_ProductionOrder_IPC",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            string confirmguid = AuthRepositoryViewModel.Instance.ConfirmedGuid;
                            DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_IPC");

                            // 시험명세 기록
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                            {
                                POTSRGUID = Guid.NewGuid(),
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPTSGUID = new Guid(_ThicknessIPCData.OPTSGUID),
                                OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                TESTSEQ = null,
                                STRDTTM = curDttm,
                                ENDDTTM = curDttm,
                                EQPTID = null,
                                INSUSER = user,
                                INSDTTM = curDttm,
                                EFCTTIMEIN = curDttm,
                                EFCTTIMEOUT = curDttm,
                                MSUBLOTID = null,
                                REASON = null,
                                ISUSE = "Y",
                                ACTIVEYN = "Y",
                                SMPQTY = _ThicknessIPCData.SMPQTY,
                                SMPQTYUOMID = _ThicknessIPCData.SMPQTYUOMID,
                            });

                            // 전자서명 코멘트
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                            {
                                COMMENTTYPE = "CM001",
                                COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                TSTYPE = _ThicknessIPCData.TSTYPE,
                                LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                            });

                            // 시험상세결과 기록
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                            {
                                POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                OPTSIGUID = new Guid(_ThicknessIPCData.OPTSIGUID),
                                POTSIRGUID = Guid.NewGuid(),
                                ACTVAL = _ThicknessIPCData.GetACTVAL,
                                INSUSER = user,
                                INSDTTM = curDttm,
                                EFCTTIMEIN = curDttm,
                                EFCTTIMEOUT = curDttm,
                                COMMENTGUID = !string.IsNullOrWhiteSpace(confirmguid) ? new Guid(confirmguid) : (Guid?)null,
                                REASON = null,
                                ISUSE = "Y",
                                ACTIVEYN = "Y"
                            });
                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                            {
                                POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                OPTSIGUID = new Guid(_LongitudeIPCData.OPTSIGUID),
                                POTSIRGUID = Guid.NewGuid(),
                                ACTVAL = _LongitudeIPCData.GetACTVAL,
                                INSUSER = user,
                                INSDTTM = curDttm,
                                EFCTTIMEIN = curDttm,
                                EFCTTIMEOUT = curDttm,
                                COMMENTGUID = new Guid(confirmguid),
                                REASON = null,
                                ISUSE = "Y",
                                ACTIVEYN = "Y"
                            });

                            if (await _BR_BRS_REG_ProductionOrderTestResult.Execute())
                            {
                                ThicknessIPCData = IPCControlData.SetIPCControlData(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0]);
                                LongitudeIPCData = IPCControlData.SetIPCControlData(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1]);

                                await GetIPCResult();
                            }
                                                                                
                            ///

                            CommandResults["RegisterIPCCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RegisterIPCCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RegisterIPCCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("RegisterIPCCommandAsync") ?
                       CommandCanExecutes["RegisterIPCCommandAsync"] : (CommandCanExecutes["RegisterIPCCommandAsync"] = true);
               });
            }
        }

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
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

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

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("구분"));
                            dt.Columns.Add(new DataColumn("두께"));
                            dt.Columns.Add(new DataColumn("두께적합여부"));
                            dt.Columns.Add(new DataColumn("경도"));
                            dt.Columns.Add(new DataColumn("경도적합여부"));

                            foreach (var item in _IPCResults)
                            {
                                var row = dt.NewRow();
                                row["구분"] = item.GUBUN ?? "";
                                row["두께"] = item.RSLT1 ?? "";
                                row["두께적합여부"] = item.RSLT3 ?? "";
                                row["경도"] = item.RSLT2 ?? "";
                                row["경도적합여부"] = item.RSLT4 ?? "";
                                dt.Rows.Add(row);
                            }

                            if (dt.Rows.Count > 0)
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

        #endregion
        
        private async Task GetIPCResult()
        {
            try
            {
                _IPCResults.Clear();
                _BR_BRS_SEL_ProductionOrderIPCResult.INDATAs.Clear();
                _BR_BRS_SEL_ProductionOrderIPCResult.OUTDATAs.Clear();
                               
                _BR_BRS_SEL_ProductionOrderIPCResult.INDATAs.Add(new BR_BRS_SEL_ProductionOrderIPCResult.INDATA
                {
                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                    TSID = IPC_TSID
                });

                if (await _BR_BRS_SEL_ProductionOrderIPCResult.Execute() && _BR_BRS_SEL_ProductionOrderIPCResult.OUTDATAs.Count > 0)
                {
                    _IPCResults.Add(new BR_BRS_SEL_ProductionOrderIPCResult.OUTDATA
                    {
                        GUBUN = "기준",
                        RSLT1 = _ThicknessIPCData.Standard,
                        RSLT2 = _LongitudeIPCData.Standard
                    });
                    foreach (BR_BRS_SEL_ProductionOrderIPCResult.OUTDATA item in _BR_BRS_SEL_ProductionOrderIPCResult.OUTDATAs)
                    {
                        //적합여부 판단
                        if (item.RSLT1 != "N/A")
                        {
                            if (ThicknessIPCData.LSL.HasValue && ThicknessIPCData.USL.HasValue)
                            {
                                if (ThicknessIPCData.LSL.Value <= Convert.ToDecimal(item.RSLT1) && Convert.ToDecimal(item.RSLT1) <= ThicknessIPCData.USL.Value)
                                    item.RSLT3 = "적합";
                                else
                                    item.RSLT3 = "부적합";
                            }
                            else if (ThicknessIPCData.LSL.HasValue)
                            {
                                if (ThicknessIPCData.LSL.Value <= Convert.ToDecimal(item.RSLT1))
                                    item.RSLT3 = "적합";
                                else
                                    item.RSLT3 = "부적합";
                            }
                            else if (ThicknessIPCData.USL.HasValue)
                            {
                                if (Convert.ToDecimal(item.RSLT1) <= ThicknessIPCData.USL.Value)
                                    item.RSLT3 = "적합";
                                else
                                    item.RSLT3 = "부적합";
                            }
                        }
                        else
                        {
                            item.RSLT3 = item.RSLT1;
                        }

                        if (item.RSLT2 != "N/A")
                        {
                            if (LongitudeIPCData.LSL.HasValue && LongitudeIPCData.USL.HasValue)
                            {
                                if (LongitudeIPCData.LSL.Value <= Convert.ToDecimal(item.RSLT2) && Convert.ToDecimal(item.RSLT2) <= LongitudeIPCData.USL.Value)
                                    item.RSLT4 = "적합";
                                else
                                    item.RSLT4 = "부적합";
                            }
                            else if (LongitudeIPCData.LSL.HasValue)
                            {
                                if (LongitudeIPCData.LSL.Value <= Convert.ToDecimal(item.RSLT2))
                                    item.RSLT4 = "적합";
                                else
                                    item.RSLT4 = "부적합";
                            }
                            else if (LongitudeIPCData.USL.HasValue)
                            {
                                if (Convert.ToDecimal(item.RSLT2) <= LongitudeIPCData.USL.Value)
                                    item.RSLT4 = "적합";
                                else
                                    item.RSLT4 = "부적합";
                            }
                        }
                        else
                        {
                            item.RSLT4 = item.RSLT2;
                        }

                        _IPCResults.Add(item);
                    }
                        
                        
                    OnPropertyChanged("IPCResults");                        
                }                
               
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

    }
}
