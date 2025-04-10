using C1.Silverlight.Data;
using Order;
using LGCNS.EZMES.Common;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Threading.Tasks;
using Common = LGCNS.iPharmMES.Common.Common;

namespace 보령
{
    public class 샘플수량수동입력ViewModel : ViewModelBase
    {
        #region 0.Property
        public 샘플수량수동입력ViewModel()
        {
            _BR_BRS_GET_MaterialSublot_Sample = new BR_BRS_GET_MaterialSublot_Sample();
            _BR_BRS_REG_MaterialSublot_Sample = new BR_BRS_REG_MaterialSublot_Sample();
            InitSample();
        }
        private 샘플수량수동입력 _mainWnd;
       
        private decimal _STORAGE_SAMPLE_QTY;
        public decimal STORAGE_SAMPLE_QTY
        {
            get { return _STORAGE_SAMPLE_QTY; }
            set
            {
                _STORAGE_SAMPLE_QTY = value;
                calTotalSample();
                NotifyPropertyChanged();
            }
        }

        private decimal _INSPECTION_SAMPLE_QTY;
        public decimal INSPECTION_SAMPLE_QTY
        {
            get { return _INSPECTION_SAMPLE_QTY; }
            set
            {
                _INSPECTION_SAMPLE_QTY = value;
                calTotalSample();
                NotifyPropertyChanged();
            }
        }

        private decimal _GENERAL_SAMPLE_QTY;
        public decimal GENERAL_SAMPLE_QTY
        {
            get { return _GENERAL_SAMPLE_QTY; }
            set
            {
                _GENERAL_SAMPLE_QTY = value;
                calTotalSample();
                NotifyPropertyChanged();
            }
        }

        private decimal _STABILITY_SAMPLE_QTY;
        public decimal STABILITY_SAMPLE_QTY
        {
            get { return _STABILITY_SAMPLE_QTY; }
            set
            {
                _STABILITY_SAMPLE_QTY = value;
                calTotalSample();
                NotifyPropertyChanged();
            }
        }

        private decimal _TOTAL_SAMPLE_QTY;
        public decimal TOTAL_SAMPLE_QTY
        {
            get { return _TOTAL_SAMPLE_QTY; }
            set
            {
                _TOTAL_SAMPLE_QTY = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _dateTime;
        public DateTime dateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region BizRule
        private BR_BRS_GET_MaterialSublot_Sample _BR_BRS_GET_MaterialSublot_Sample;
        private BR_BRS_REG_MaterialSublot_Sample _BR_BRS_REG_MaterialSublot_Sample;
        #endregion

        #region Command


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
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///

                            dateTime = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            if (arg != null && arg is 샘플수량수동입력)
                            {
                                _mainWnd = arg as 샘플수량수동입력;

                                _BR_BRS_GET_MaterialSublot_Sample.INDATAs.Clear();
                                _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs.Clear();

                                _BR_BRS_GET_MaterialSublot_Sample.INDATAs.Add(new BR_BRS_GET_MaterialSublot_Sample.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                await _BR_BRS_GET_MaterialSublot_Sample.Execute();

                                if (_BR_BRS_GET_MaterialSublot_Sample.OUTDATAs.Count > 0)
                                {
                                    STORAGE_SAMPLE_QTY = _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].STORAGE_SAMPLE_QTY.HasValue ? _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].STORAGE_SAMPLE_QTY.Value : 0;
                                    INSPECTION_SAMPLE_QTY = _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].INSPECTION_SAMPLE_QTY.HasValue ? _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].INSPECTION_SAMPLE_QTY.Value : 0;
                                    GENERAL_SAMPLE_QTY = _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].GENERAL_SAMPLE_QTY.HasValue ? _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].GENERAL_SAMPLE_QTY.Value : 0;
                                    STABILITY_SAMPLE_QTY = _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].STABILITY_SAMPLE_QTY.HasValue ? _BR_BRS_GET_MaterialSublot_Sample.OUTDATAs[0].STABILITY_SAMPLE_QTY.Value : 0;
                                }
                                else
                                    InitSample();
                            }
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommand"] = false;
                            InitSample();
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
        public ICommand ClickSaveCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickSaveCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickSaveCommand"] = false;
                            CommandCanExecutes["ClickSaveCommand"] = false;

                            ///
                            // 전자서명 등록
                            var authHelper = new iPharmAuthCommandHelper();
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }
                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("샘플수량수동입력"),
                                string.Format("샘플수량수동입력"),
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // 샘플수량 등록
                            string insuser = !string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")) ? AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI") : AuthRepositoryViewModel.Instance.LoginedUserID;
                            _BR_BRS_REG_MaterialSublot_Sample.INDATAs.Clear();
                            _BR_BRS_REG_MaterialSublot_Sample.INDATAs.Add(new BR_BRS_REG_MaterialSublot_Sample.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                STORAGE_SAMPLE_QTY = STORAGE_SAMPLE_QTY,
                                INSPECTION_SAMPLE_QTY = INSPECTION_SAMPLE_QTY,
                                GENERAL_SAMPLE_QTY = GENERAL_SAMPLE_QTY,
                                STABILITY_SAMPLE_QTY = STABILITY_SAMPLE_QTY,
                                USERID = insuser,
                                SAMPLE_OUTDTTM = dateTime.ToString("yyyy-MM-dd HH:mm:ss")
                            });

                            if(await _BR_BRS_REG_MaterialSublot_Sample.Execute())
                            {
                                // XML 등록
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("보관검체수량"));
                                dt.Columns.Add(new DataColumn("실험검체수량"));
                                dt.Columns.Add(new DataColumn("일반검체수량"));
                                dt.Columns.Add(new DataColumn("안전검체수량"));
                                dt.Columns.Add(new DataColumn("총샘플수량"));
                                dt.Columns.Add(new DataColumn("샘플불출일자"));

                                var row = dt.NewRow();
                                row["보관검체수량"] = STORAGE_SAMPLE_QTY;
                                row["실험검체수량"] = INSPECTION_SAMPLE_QTY;
                                row["일반검체수량"] = GENERAL_SAMPLE_QTY;
                                row["안전검체수량"] = STABILITY_SAMPLE_QTY;
                                row["총샘플수량"] = TOTAL_SAMPLE_QTY;
                                row["샘플불출일자"] = dateTime.ToString("yyyy-MM-dd HH:mm:ss") != null ? dateTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
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
                            }
                            ///

                            CommandResults["ClickSaveCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickSaveCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickSaveCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickSaveCommand") ?
                        CommandCanExecutes["ClickSaveCommand"] : (CommandCanExecutes["ClickSaveCommand"] = true);
                });
            }
        }

        public ICommand ClickCancelCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ClickCancelCommand"] = false;
                        CommandCanExecutes["ClickCancelCommand"] = false;

                        ///
                        _mainWnd.Close();
                        ///

                        CommandResults["ClickCancelCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ClickCancelCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ClickCancelCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickCancelCommand") ?
                        CommandCanExecutes["ClickCancelCommand"] : (CommandCanExecutes["ClickCancelCommand"] = true);
                });
            }
        }
        #endregion
        public void calTotalSample()
        {
            TOTAL_SAMPLE_QTY = _STORAGE_SAMPLE_QTY + _INSPECTION_SAMPLE_QTY + _GENERAL_SAMPLE_QTY + _STABILITY_SAMPLE_QTY;
        }

        public void InitSample()
        {
            STORAGE_SAMPLE_QTY = 0;
            INSPECTION_SAMPLE_QTY = 0;
            GENERAL_SAMPLE_QTY = 0;
            STABILITY_SAMPLE_QTY = 0;
            TOTAL_SAMPLE_QTY = 0;           
        }
    }
}
