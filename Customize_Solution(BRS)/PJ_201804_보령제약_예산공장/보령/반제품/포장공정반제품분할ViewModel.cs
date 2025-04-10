using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
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
using 보령.UserControls;

namespace 보령
{
    public class 포장공정반제품분할ViewModel : ViewModelBase
    {
        #region 0.Property
        private 포장공정반제품분할 _mainWnd;
        public 포장공정반제품분할ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT = new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();
            _BR_BRS_REG_MaterialSubLot_INV_Split = new BR_BRS_REG_MaterialSubLot_INV_Split();
            _BR_BRS_SEL_VESSEL_Info = new BR_BRS_SEL_VESSEL_Info();
            _BR_BRS_SEL_MaterialSubLotSplitHistory = new BR_BRS_SEL_MaterialSubLotSplitHistory();
            _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING = new BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING();
        }

        // 프린터
        private BR_PHR_SEL_System_Printer.OUTDATA _selectedPrint;
        public string curPrintName
        {
            get
            {
                if (_selectedPrint != null)
                    return _selectedPrint.PRINTERNAME;
                else
                    return "N/A";
            }
        }
        // 분할 방식
        private string _SPLIT_TYPE;
        public string SPLIT_TYPE
        {
            get { return _SPLIT_TYPE; }
            set
            {
                _SPLIT_TYPE = value;
                OnPropertyChanged("SPLIT_TYPE");
            }
        }

        // 분할 대상 반제품의 기존 무게
        private Weight _oriSplitLotNetWeight = new Weight();
        public string oriSplitLotNetWeight
        {
            get
            {
                return _oriSplitLotNetWeight.WeightUOMString;
            }
        }

        // 분할 대상 반제품의 기존 무게
        private Weight _oriMergeLotNetWeight = new Weight();
        public string oriMergeLotNetWeight
        {
            get
            {
                return _oriMergeLotNetWeight.WeightUOMString;
            }
        }

        // 반제품 내용물 무게 합산
        private Weight _oriNetWeight_sum = new Weight();
        public string oriNetWeight_sum
        {
            get
            {
                return _oriNetWeight_sum.WeightUOMString;
            }
        }

        // 분할 대상 반제품
        private BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA _SplitMaterialSubLot;
        public BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA SplitMaterialSubLot
        {
            get { return _SplitMaterialSubLot; }
            set
            {
                _SplitMaterialSubLot = value;
                OnPropertyChanged("SplitMaterialSubLot");
            }
        }
        // 병합 대상 반제품
        private BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA _MergeMaterialSubLot;
        public BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA MergeMaterialSubLot
        {
            get { return _MergeMaterialSubLot; }
            set
            {
                _MergeMaterialSubLot = value;
                OnPropertyChanged("MergeMaterialSubLot");
            }
        }


        private bool _CanSplitHLAB;
        /// <summary>
        /// ture : 분할버튼 활성화, false : 분할버튼 비활성화
        /// </summary>
        public bool CanSplitHLAB
        {
            get { return _CanSplitHLAB; }
            set
            {
                _CanSplitHLAB = value;
                OnPropertyChanged("CanSplitHLAB");
            }
        }

        private bool _CanWeightHLAB;
        /// <summary>
        /// ture : 용기무게변경 버튼 활성화, false : 용기무게변경 버튼 비활성화
        /// </summary>
        public bool CanWeightHLAB
        {
            get { return _CanWeightHLAB; }
            set
            {
                _CanWeightHLAB = value;
                OnPropertyChanged("CanWeightHLAB");
            }
        }
        #endregion
        #region 1.BizRule
        /// <summary>
        /// 포장반제품 조회
        /// </summary>
        private BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT;
        public BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT
        {
            get { return _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT; }
            set
            {
                _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT");
            }
        }
        /// <summary>
        /// 보관용기 사용시작 및 선별 반제품 생성
        /// </summary>
        private BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING;

        /// <summary>
        /// 프린터 조회
        /// </summary>
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        /// <summary>
        /// 라벨 출력
        /// </summary>
        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        /// <summary>
        /// 반제품 분할
        /// </summary>
        private BR_BRS_REG_MaterialSubLot_INV_Split _BR_BRS_REG_MaterialSubLot_INV_Split;
        /// <summary>
        /// 스캔한 용기 확인
        /// </summary>
        private BR_BRS_SEL_VESSEL_Info _BR_BRS_SEL_VESSEL_Info;
        private BR_BRS_SEL_MaterialSubLotSplitHistory _BR_BRS_SEL_MaterialSubLotSplitHistory;
        public BR_BRS_SEL_MaterialSubLotSplitHistory BR_BRS_SEL_MaterialSubLotSplitHistory
        {
            get { return _BR_BRS_SEL_MaterialSubLotSplitHistory; }
            set
            {
                _BR_BRS_SEL_MaterialSubLotSplitHistory = value;
                OnPropertyChanged("BR_BRS_SEL_MaterialSubLotSplitHistory");
            }
        }
        #endregion
        #region 2.Command
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
                            if (arg == null || !(arg is 포장공정반제품분할))
                                return;

                            _mainWnd = arg as 포장공정반제품분할;

                            // 프린터 조회
                            _BR_PHR_SEL_System_Printer.INDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.OUTDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = AuthRepositoryViewModel.Instance.RoomID,
                                IPADDRESS = Common.ClientIP
                            });

                            if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                            {
                                if (_BR_PHR_SEL_System_Printer.OUTDATAs.Count == 1)
                                    _selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                            }

                            OnPropertyChanged("curPrintName");

                            GetAvailableWIPList();
                            CanSplitHLAB = false;
                            CanWeightHLAB = false;
                            SPLIT_TYPE = "반제품분할";
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommand"] = false;
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
        } // 화면로드
        public ICommand ChangePrintCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangePrintCommand"] = false;
                        CommandCanExecutes["ChangePrintCommand"] = false;

                        ///
                        SelectPrinterPopup popup = new SelectPrinterPopup();

                        popup.Closed += (s, e) =>
                        {
                            if (popup.DialogResult.GetValueOrDefault())
                            {
                                if (popup.SourceGrid.SelectedItem != null && popup.SourceGrid.SelectedItem is BR_PHR_SEL_System_Printer.OUTDATA)
                                {
                                    _selectedPrint = popup.SourceGrid.SelectedItem as BR_PHR_SEL_System_Printer.OUTDATA;
                                    OnPropertyChanged("curPrintName");
                                }
                            }
                        };

                        popup.Show();
                        ///

                        CommandResults["ChangePrintCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangePrintCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ChangePrintCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangePrintCommand") ?
                        CommandCanExecutes["ChangePrintCommand"] : (CommandCanExecutes["ChangePrintCommand"] = true);
                });
            }
        } // 프린터 변경
        public ICommand LabelPrintCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LabelPrintCommandAsync"].EnterAsync())
                    {
                        try
                        {

                            CommandResults["LabelPrintCommandAsync"] = false;
                            CommandCanExecutes["LabelPrintCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (!string.IsNullOrWhiteSpace(curPrintName) && !curPrintName.Equals("N/A"))
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs)
                                {
                                    if (item.ISSELECTED)
                                    {
                                        _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                        _BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();
                                        _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                        {
                                            ReportPath = "/Reports/Label/LABEL_INPROCESS_WIP",
                                            PrintName = _selectedPrint.PRINTERNAME,
                                            USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                        });

                                        _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                        {
                                            ParamName = "MSUBLOTID",
                                            ParamValue = !string.IsNullOrWhiteSpace(item.MSUBLOTID) ? item.MSUBLOTID : ""
                                        });
                                        _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                        {
                                            ParamName = "VESSELID",
                                            ParamValue = !string.IsNullOrWhiteSpace(item.VESSELID) && item.VESSELID != "VINYL" ? item.VESSELID : ""
                                        });

                                        await _BR_PHR_SEL_PRINT_LabelImage.Execute();
                                    }
                                }
                            }
                            else
                                throw new Exception("출력할 프린터정보가 없습니다.");

                            IsBusy = false;
                            ///

                            CommandResults["LabelPrintCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LabelPrintCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LabelPrintCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LabelPrintCommandAsync") ?
                        CommandCanExecutes["LabelPrintCommandAsync"] : (CommandCanExecutes["LabelPrintCommandAsync"] = true);
                });
            }
        } // 선택한 반제품라벨 발행
        public ICommand CreateHALBCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["CreateHALBCommand"] = false;
                        CommandCanExecutes["CreateHALBCommand"] = false;

                        if (arg != null && arg is Button)
                        {
                            Button btn = arg as Button;
                            if (btn.Name == "btnCreateHLAB")
                            {
                                BarcodePopup popup = new BarcodePopup();
                                popup.tbMsg.Text = "용기번호를 입력하세요";
                                popup.Closed += async (s, e) =>
                                {
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(popup.tbText.Text))
                                        {
                                            if (popup.DialogResult.GetValueOrDefault())
                                            {
                                                string vesselid = popup.tbText.Text.ToUpper();
                                                BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA temp = null;
                                                string userId = !string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")) ? AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI") : AuthRepositoryViewModel.Instance.LoginedUserID;

                                                foreach (var item in BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs)
                                                {
                                                    if (item.VESSELID == vesselid)
                                                        temp = item;
                                                }
                                                if (temp == null)
                                                {

                                                    _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.INDATAs.Clear();
                                                    _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.OUTDATAs.Clear();

                                                    _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.INDATA
                                                    {
                                                        VESSELID = vesselid,
                                                        INSUSER = userId,
                                                        BATCHNO = _mainWnd.CurrentOrder.BatchNo
                                                    });

                                                    if (await _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.Execute() && _BR_BRS_REG_ProductionOrderOutput_INSPECTION_PACKING.OUTDATAs.Count > 0)
                                                    {
                                                        OnMessage(string.Format("보관용기 사용시작이 완료되었습니다.\n(보관용기 : {0})", vesselid));
                                                        GetAvailableWIPList();
                                                    }
                                                }
                                                else
                                                {
                                                    //OnMessage("이미 생성된 보관용기입니다. 다시 한번 확인해주세요.");
                                                    throw new Exception("이미 생성된 보관용기입니다. 다시 한번 확인해주세요.");
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("취소했습니다.");
                                            }
                                        }
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        OnException(ex.Message, ex);
                                    }
                                };
                                popup.Show();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommandResults["CreateHALBCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["CreateHALBCommand"] = true;
                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("CreateHALBCommand") ?
                        CommandCanExecutes["CreateHALBCommand"] : (CommandCanExecutes["CreateHALBCommand"] = true);
                });
            }
        } // 보관용기사용시작
        public ICommand ChangeHALBCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangeHALBCommand"] = false;
                        CommandCanExecutes["ChangeHALBCommand"] = false;

                        ///
                        if (arg != null && arg is Button)
                        {
                            Button btn = arg as Button;
                            if (btn.Name == "btnChangeSplitHALB" || btn.Name == "btnChangeMergeHLAB")
                            {
                                BarcodePopup popup = new BarcodePopup();
                                popup.tbMsg.Text = "용기번호를 입력하세요";
                                popup.Closed += (s, e) =>
                                {
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(popup.tbText.Text))
                                        {
                                            string vesselid = popup.tbText.Text;
                                            BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA temp = null;

                                            // 반제품 목록에 있는지 확인
                                            foreach (var item in BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs)
                                            {
                                                if (item.VESSELID == vesselid)
                                                    temp = item;
                                            }

                                            if (temp != null)
                                            {
                                                if (popup.DialogResult.GetValueOrDefault())
                                                {
                                                    temp.NET.SetWeight(temp.MSUBLOTQTY.GetValueOrDefault(), temp.UOM, temp.PRECISION.GetValueOrDefault());
                                                    temp.TARE.SetWeight(temp.TAREWEIGHT.GetValueOrDefault(), temp.UOM, temp.PRECISION.GetValueOrDefault());
                                                    temp.GROSS.SetWeight(temp.NET.Value + temp.TARE.Value, temp.UOM, temp.PRECISION.GetValueOrDefault());

                                                    switch (btn.Name)
                                                    {
                                                        case "btnChangeSplitHALB":
                                                            if (temp.NET.Value > 0)
                                                            {
                                                                if (_MergeMaterialSubLot != null)
                                                                {
                                                                    if (_MergeMaterialSubLot.VESSELID == vesselid)
                                                                    {
                                                                        OnMessage("병합대상인 용기입니다.");
                                                                        break;
                                                                    }
                                                                }

                                                                SplitMaterialSubLot = temp;
                                                                _oriSplitLotNetWeight.SetWeight(SplitMaterialSubLot.NET.WeightUOMString);
                                                                _oriNetWeight_sum.SetWeight(_oriSplitLotNetWeight.Value + _oriMergeLotNetWeight.Value, _oriSplitLotNetWeight.Uom, _oriSplitLotNetWeight.Precision);
                                                            }
                                                            else
                                                                OnMessage("내용물이 없는 반제품 입니다.");


                                                            break;
                                                        case "btnChangeMergeHLAB":
                                                            if (_SplitMaterialSubLot != null)
                                                            {
                                                                if (_SplitMaterialSubLot.VESSELID == vesselid)
                                                                {
                                                                    OnMessage("분할대상인 용기입니다.");
                                                                    break;
                                                                }
                                                            }

                                                            if (temp.NET.Value == 0)
                                                                SPLIT_TYPE = "반제품분할(빈용기)";
                                                            else
                                                                SPLIT_TYPE = "반제품분할(반제품)";

                                                            MergeMaterialSubLot = temp;
                                                            _oriMergeLotNetWeight.SetWeight(MergeMaterialSubLot.NET.WeightUOMString);
                                                            _oriNetWeight_sum.SetWeight(_oriSplitLotNetWeight.Value + _oriMergeLotNetWeight.Value, _oriMergeLotNetWeight.Uom, _oriMergeLotNetWeight.Precision);
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    if (_SplitMaterialSubLot != null && _MergeMaterialSubLot != null)
                                                    {
                                                       CanSplitHLAB = true;
                                                    }
                                                    if (_MergeMaterialSubLot != null)
                                                    {
                                                        CanWeightHLAB = true;
                                                    }

                                                    WeightPropertyChanged();
                                                }
                                            }
                                            else
                                                OnMessage("반제품을 조회하지 못했습니다.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        OnException(ex.Message, ex);
                                    }
                                };
                                popup.Show();
                            }
                        }
                        ///

                        CommandResults["ChangeHALBCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangeHALBCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ChangeHALBCommand"] = true;
                        WeightPropertyChanged();
                        IsBusy = false;
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("ChangeHALBCommand") ?
                       CommandCanExecutes["ChangeHALBCommand"] : (CommandCanExecutes["ChangeHALBCommand"] = true);
               });
            }
        } // 반제품 선택
        public ICommand ChangeWeightCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangeWeightCommand"] = false;
                        CommandCanExecutes["ChangeWeightCommand"] = false;

                        ///
                        if (arg != null && arg is Button)
                        {
                            Button btn = arg as Button;
                            LGCNS.iPharmMES.Common.KeyPadPopUp popup = new LGCNS.iPharmMES.Common.KeyPadPopUp(LGCNS.iPharmMES.Common.KeyPadPopUp.KeyPadType.Numeric);
                            popup.Closed += (s, e) =>
                            {
                                decimal val;
                                if (!string.IsNullOrWhiteSpace(popup.Value) && Decimal.TryParse(popup.Value, out val))
                                {
                                    if (popup.DialogResult.GetValueOrDefault())
                                    {
                                        if (btn.Name == "btnNetWeightChange")
                                        {
                                            // 2022.01.20 박희돈 Validation 삭제. 사유 : 작업자가 더 많이 분할 후 분할 양을 줄여야 하는 이슈 발생. 
                                            MergeMaterialSubLot.NET.Value = val;
                                            MergeMaterialSubLot.GROSS.Value = MergeMaterialSubLot.TARE.Value + val;

                                            SplitMaterialSubLot.NET.Value = _oriNetWeight_sum.Value - val;
                                            SplitMaterialSubLot.GROSS.Value = SplitMaterialSubLot.NET.Value + SplitMaterialSubLot.TARE.Value;
                                            //if (val >= _oriMergeLotNetWeight.Value)
                                            //{
                                            //    MergeMaterialSubLot.NET.Value = val;
                                            //    MergeMaterialSubLot.GROSS.Value = MergeMaterialSubLot.TARE.Value + val;

                                            //    SplitMaterialSubLot.NET.Value = _oriNetWeight_sum.Value - val;
                                            //    SplitMaterialSubLot.GROSS.Value = SplitMaterialSubLot.NET.Value + SplitMaterialSubLot.TARE.Value;
                                            //}
                                            //else
                                            //    OnMessage("병합 대상의 내용물 무게는 기존 무게보다 작을 수 없습니다.");
                                        }
                                        else if (btn.Name == "btnTareWeightChange")
                                        {
                                            MergeMaterialSubLot.TARE.Value = val;
                                            MergeMaterialSubLot.GROSS.Value = MergeMaterialSubLot.NET.Value + val;
                                        }
                                    }
                                }
                            };
                            popup.Show();
                        }
                        ///

                        CommandResults["ChangeWeightCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangeWeightCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ChangeWeightCommand"] = true;
                        WeightPropertyChanged();
                        IsBusy = false;
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("ChangeWeightCommand") ?
                       CommandCanExecutes["ChangeWeightCommand"] : (CommandCanExecutes["ChangeWeightCommand"] = true);
               });
            }
        } // 병합대상 반제품의 무게 정보 변경
        public ICommand SplictCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SplictCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SplictCommandAsync"] = false;
                            CommandCanExecutes["SplictCommandAsync"] = false;

                            ///

                            if (_MergeMaterialSubLot.NET.Value == _oriMergeLotNetWeight.Value)
                                throw new Exception("무게가 변경되지 않았습니다.");

                            // 전자서명 요청
                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("포장공정반제품분할"),
                                string.Format("포장공정반제품분할"),
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // 반제품 분할
                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_POs.Clear();
                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_SPLITs.Clear();
                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_MERGEs.Clear();

                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_POs.Add(new BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_PO
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                OUTPUTGUID = _SplitMaterialSubLot.OUTPUTGUID,
                                USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                            });
                            // Split WIP
                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_SPLITs.Add(new BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_SPLIT
                            {
                                MSUBLOTID = _SplitMaterialSubLot.MSUBLOTID,
                                VESSELID = _SplitMaterialSubLot.VESSELID,
                                MSUBLOTQTY = _SplitMaterialSubLot.NET.Value,
                                TAREWEIGHT = _SplitMaterialSubLot.TARE.Value
                            });
                            // Merge WIP
                            _BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_MERGEs.Add(new BR_BRS_REG_MaterialSubLot_INV_Split.INDATA_MERGE
                            {
                                MSUBLOTID = _MergeMaterialSubLot.MSUBLOTID,
                                VESSELID = _MergeMaterialSubLot.VESSELID,
                                MSUBLOTQTY = _MergeMaterialSubLot.NET.Value,
                                TAREWEIGHT = _MergeMaterialSubLot.TARE.Value
                            });

                            if (await _BR_BRS_REG_MaterialSubLot_INV_Split.Execute())
                            {
                                GetAvailableWIPList();
                                CanSplitHLAB = false;
                                SplitMaterialSubLot = null;
                                MergeMaterialSubLot = null;
                                _oriMergeLotNetWeight.Value = 0;
                                _oriSplitLotNetWeight.Value = 0;
                                _oriNetWeight_sum.Value = 0;
                            }
                            ///

                            CommandResults["SplictCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SplictCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SplictCommandAsync"] = true;
                            WeightPropertyChanged();
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SplictCommandAsync") ?
                        CommandCanExecutes["SplictCommandAsync"] : (CommandCanExecutes["SplictCommandAsync"] = true);
                });
            }
        } // 반제품 분할
        public ICommand SaveCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SaveCommand"] = false;
                            CommandCanExecutes["SaveCommand"] = false;

                            ///
                            // 변경 정보 팝업으로 조회

                            // 이력 조회
                            _BR_BRS_SEL_MaterialSubLotSplitHistory.INDATAs.Clear();
                            _BR_BRS_SEL_MaterialSubLotSplitHistory.OUTDATAs.Clear();
                            _BR_BRS_SEL_MaterialSubLotSplitHistory.INDATAs.Add(new BR_BRS_SEL_MaterialSubLotSplitHistory.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                            });

                            if (await _BR_BRS_SEL_MaterialSubLotSplitHistory.Execute())
                            {
                                // 팝업창 호출
                                포장공정반제품분할이력 popup = new 포장공정반제품분할이력();
                                popup.DataContext = this;

                                #region 분할이력 기록
                                popup.btnSave.Click += async (s, e) =>
                                {
                                    try
                                    {
                                        // 전자서명 요청
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
                                            string.Format("포장공정반제품분할"),
                                            string.Format("포장공정반제품분할"),
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "", null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        // XML 변환
                                        DataSet ds = new DataSet();
                                        DataTable dt = new DataTable("DATA");
                                        ds.Tables.Add(dt);

                                        dt.Columns.Add(new DataColumn("용기번호"));
                                        dt.Columns.Add(new DataColumn("분할전무게"));
                                        dt.Columns.Add(new DataColumn("분할후무게"));

                                        foreach (var item in _BR_BRS_SEL_MaterialSubLotSplitHistory.OUTDATAs)
                                        {
                                            if (item.ISSELECTED)
                                            {
                                                DataRow row = dt.NewRow();
                                                row["용기번호"] = item.VESSELID ?? "N/A";
                                                row["분할전무게"] = item.PREVWEIGHT ?? "N/A";
                                                row["분할후무게"] = item.CURWEIGHT ?? "N/A";
                                                dt.Rows.Add(row);
                                            }
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

                                        popup.DialogResult = true;

                                        if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                        else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                                    }
                                    catch (Exception ex)
                                    {
                                        OnException(ex.Message, ex);
                                    }
                                };
                                #endregion
                                #region 분할이력없음
                                popup.btnNoSplit.Click += async (s, e) =>
                                {
                                    try
                                    {
                                        // 전자서명 요청
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
                                            string.Format("포장공정반제품분할"),
                                            string.Format("포장공정반제품분할"),
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "", null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        // 지시문 결과 입력
                                        _mainWnd.CurrentInstruction.Raw.ACTVAL = "반제품 분할 없음";
                                        _mainWnd.CurrentInstruction.Raw.NOTE = null;

                                        var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                                        if (result != enumInstructionRegistErrorType.Ok)
                                        {
                                            throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                        }

                                        popup.DialogResult = true;

                                        if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                        else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                                    }
                                    catch (Exception ex)
                                    {
                                        OnException(ex.Message, ex);
                                    }
                                };
                                #endregion

                                popup.Show();
                            }
                            ///

                            CommandResults["SaveCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveCommand") ?
                        CommandCanExecutes["SaveCommand"] : (CommandCanExecutes["SaveCommand"] = true);
                });
            }
        } // 분할이력 저장
        #endregion
        #region 3.Function
        private async void GetAvailableWIPList()
        {
            try
            {
                _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATAs.Clear();
                _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs.Clear();
                _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATA
                {
                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                    ISSPLIT = "Y"
                });

                if (await BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.Execute())
                {
                    int precision = 3;
                    decimal net, tare;

                    foreach (var item in _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs)
                    {
                        precision = item.PRECISION.HasValue ? item.PRECISION.Value : 3;
                        tare = item.TAREWEIGHT.HasValue ? item.TAREWEIGHT.Value : -99999m;
                        net = item.MSUBLOTQTY.HasValue ? item.MSUBLOTQTY.Value : -99999m;

                        item.TARE.SetWeight(tare, item.UOM, precision);
                        item.NET.SetWeight(net, item.UOM, precision);
                        item.GROSS.SetWeight(tare + net, item.UOM, precision);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void WeightPropertyChanged()
        {
            OnPropertyChanged("SPLIT_TYPE");
            OnPropertyChanged("oriSplitLotNetWeight");
            OnPropertyChanged("oriMergeLotNetWeight");
            OnPropertyChanged("oriNetWeight_sum");
            OnPropertyChanged("MergeMaterialSubLot");
            OnPropertyChanged("SplitMaterialSubLot");
        }
        #endregion
    }
}
