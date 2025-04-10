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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 레시피확인ViewModel : ViewModelBase
    {
        #region [Property]
        public 레시피확인ViewModel()
        {
            _BR_PHR_SEL_CODE = new BR_PHR_SEL_CODE();
            _BR_BRS_SEL_EquipmentRecipe = new BR_BRS_SEL_EquipmentRecipe();
            _BR_BRS_SEL_EquipmentRecipeStepType = new BR_BRS_SEL_EquipmentRecipeStepType();
            _filteredComponents = new BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATACollection();
            _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL = new BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL();
        }

        레시피확인 _mainWnd;
        private DataTable _dt;
        public DataTable dt
        {
            get { return _dt; }
            set
            {
                _dt = value;
                OnPropertyChanged("dt");
            }
        }

        private string _EQPTID;
        public string EQPTID
        {
            get { return _EQPTID; }
            set
            {
                _EQPTID = value;
                OnPropertyChanged("EQPTID");
            }
        }

        private string _EQPTNAME;
        public string EQPTNAME
        {
            get { return _EQPTNAME; }
            set
            {
                _EQPTNAME = value;
                OnPropertyChanged("EQPTNAME");
            }
        }

        private bool _SEARCH_ENABLE;
        public bool SEARCH_ENABLE
        {
            get { return _SEARCH_ENABLE; }
            set
            {
                _SEARCH_ENABLE = value;
                OnPropertyChanged("SEARCH_ENABLE");
            }
        }

        private string _RECIPENAME;
        public string RECIPENAME
        {
            get { return _RECIPENAME; }
            set
            {
                _RECIPENAME = value;
                OnPropertyChanged("RECIPENAME");
            }
        }

        private bool _RECORD_ENABLE;
        public bool RECORD_ENABLE
        {
            get { return _RECORD_ENABLE; }
            set
            {
                _RECORD_ENABLE = value;
                OnPropertyChanged("RECORD_ENABLE");
            }
        }
        #endregion

        #region [Bizrule]
        private BR_PHR_SEL_CODE _BR_PHR_SEL_CODE;

        private BR_BRS_SEL_EquipmentRecipe _BR_BRS_SEL_EquipmentRecipe;
        public BR_BRS_SEL_EquipmentRecipe BR_BRS_SEL_EquipmentRecipe
        {
            get { return _BR_BRS_SEL_EquipmentRecipe; }
            set
            {
                _BR_BRS_SEL_EquipmentRecipe = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentRecipe");
            }
        }

        private BR_BRS_SEL_EquipmentRecipeStepType _BR_BRS_SEL_EquipmentRecipeStepType;
        public BR_BRS_SEL_EquipmentRecipeStepType BR_BRS_SEL_EquipmentRecipeStepType
        {
            get { return _BR_BRS_SEL_EquipmentRecipeStepType; }
            set
            {
                _BR_BRS_SEL_EquipmentRecipeStepType = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentRecipeStepType");
            }
        }

        private BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL;
        public BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL
        {
            get { return _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL; }
            set
            {
                _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL");
            }
        }

        private BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATACollection filteredComponents
        {
            get { return _filteredComponents; }
            set
            {
                _filteredComponents = value;
                NotifyPropertyChanged();
            }
        }
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
                            if (arg != null && arg is 레시피확인)
                            {
                                _mainWnd = arg as 레시피확인;
                                RECORD_ENABLE = false;
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


        public ICommand SearchEquipmentCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SearchEquipmentCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SearchEquipmentCommandAsync"] = false;
                            CommandCanExecutes["SearchEquipmentCommandAsync"] = false;

                            ///
                            if (arg != null && arg is string)
                            {
                                string id = arg as string;

                                // 설비 체크
                                _BR_PHR_SEL_CODE.INDATAs.Clear();
                                _BR_PHR_SEL_CODE.OUTDATAs.Clear();
                                _BR_PHR_SEL_CODE.INDATAs.Add(new BR_PHR_SEL_CODE.INDATA()
                                {
                                    TYPE = "Equipment",
                                    LANGID = "ko-KR",
                                    CODE = id
                                });

                                if (await _BR_PHR_SEL_CODE.Execute() && _BR_PHR_SEL_CODE.OUTDATAs.Count > 0)
                                {
                                    EQPTID = _BR_PHR_SEL_CODE.OUTDATAs[0].CODE;
                                    EQPTNAME = _BR_PHR_SEL_CODE.OUTDATAs[0].NAME.Replace("[" + _EQPTID + "]", "");
                                    SEARCH_ENABLE = true;
                                }
                                else
                                {
                                    SEARCH_ENABLE = false;
                                }

                            }
                            ///

                            CommandResults["SearchEquipmentCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SearchEquipmentCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SearchEquipmentCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("SearchEquipmentCommandAsync") ?
                       CommandCanExecutes["SearchEquipmentCommandAsync"] : (CommandCanExecutes["SearchEquipmentCommandAsync"] = true);
               });
            }
        }



        public ICommand GetEquipmentRecipeAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["GetEquipmentRecipeAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["GetEquipmentRecipeAsync"] = false;
                            CommandCanExecutes["GetEquipmentRecipeAsync"] = false;
                            string rcpName = null;
                            ///

                            filteredComponents.Clear();
                            _BR_BRS_SEL_EquipmentRecipe.INDATAs.Clear();
                            _BR_BRS_SEL_EquipmentRecipe.OUTDATAs.Clear();
                            _BR_BRS_SEL_EquipmentRecipe.INDATAs.Add(new BR_BRS_SEL_EquipmentRecipe.INDATA
                            {
                                EQPTID = EQPTID                              
                            });

                            if (await _BR_BRS_SEL_EquipmentRecipe.Execute())
                            {

                                _BR_BRS_SEL_EquipmentRecipeStepType.INDATAs.Clear();
                                _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs.Clear();
                                _BR_BRS_SEL_EquipmentRecipeStepType.INDATAs.Add(new BR_BRS_SEL_EquipmentRecipeStepType.INDATA
                                {
                                    EQPTID = EQPTID
                                });

                                //여러 태그를 한번에 조회했을때 타임아웃 에러가 나서 로직 수정
                                //if (await _BR_BRS_SEL_EquipmentRecipeStepType.Execute())
                                //{
                                //    dt = new DataTable();

                                //    // ROWSEQ가 0은 레시피명
                                //    foreach (var item in _BR_BRS_SEL_EquipmentRecipe.OUTDATAs.Where(o => o.ROWSEQ == 0).OrderBy(o => o.COLUMNSEQ))
                                //    {
                                //        if (item.COLUMNSEQ == 0)
                                //        {
                                //            RECIPENAME = item.ACTVAL;
                                //        }
                                //    }

                                //    rcpName = _mainWnd.CurrentInstruction.Raw.TARGETVAL;
                                //    if (rcpName.Trim() == RECIPENAME)
                                //    {
                                //        RECORD_ENABLE = true;
                                //    }
                                //    else
                                //    {
                                //        RECORD_ENABLE = false;
                                //        OnMessage("레시피 이름: " + RECIPENAME + ", Target : " + rcpName + "\r\n레시피이름과 Target이 다릅니다.");
                                //        return;
                                //    }

                                //    // ROWSEQ가 1부터는 스탭. dt 컬럼 추출하기 위해 ROWSEQ 가 1인 데이터들 조회
                                //    foreach (var item in _BR_BRS_SEL_EquipmentRecipe.OUTDATAs.Where(o => o.ROWSEQ == 1).OrderBy(o => o.COLUMNSEQ))
                                //    {
                                //        // Column 규칙 설비명.2.B1_태크명
                                //        // EX) OBFG4001.2.B1_AirVolumeSetpoint
                                //        //     OBFG4001.2.B2_AirVolumeSetpoint
                                //        // TAGID에서 컬럼명만 가져와서 dt에 저장
                                //        dt.Columns.Add(item.COLUMNNAME);
                                //    }

                                //    // ROWSEQ가 0은 레시피명. ROWSEQ가 1부터는 스탭
                                //    foreach (var item in _BR_BRS_SEL_EquipmentRecipe.OUTDATAs.Where(o => o.ROWSEQ != 0).OrderBy(o => o.ROWSEQ).ThenBy(o => o.COLUMNSEQ))
                                //    {
                                //        if (item.COLUMNSEQ == 0)
                                //        {
                                //            dt.Rows.Add();
                                //        }
                                //        for (int i = 0; i < dt.Columns.Count; i++)
                                //        {
                                //            //각 설비군마다 StepType에 따른 값 변경
                                //            if (dt.Columns[i].ColumnName == item.COLUMNNAME && dt.Columns[i].ColumnName == _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[0].COLUMNNAME)
                                //            {
                                //                //StepType에 등록된 값이 있는 경우면 등록된 값으로 보여줌, 등록되지 않은 값이 올라올 수도 있음. EX) -1
                                //                if (_BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs.Where(o => o.ACTIVAL == item.ACTVAL).Count() > 0)
                                //                {
                                //                    dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[Convert.ToInt32(item.ACTVAL)].STEPNAME;
                                //                }
                                //                else
                                //                {
                                //                    dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = item.ACTVAL;
                                //                }
                                //                break;
                                //            }
                                //            else if (dt.Columns[i].ColumnName == item.COLUMNNAME)
                                //            {
                                //                dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = item.ACTVAL;
                                //                break;
                                //            }
                                //        }
                                //    }

                                //    // 팀장님 요청사항. ROW의 데이터가 전부 0이면 해당 ROW 삭제. 사유는 실제 사용하는 태그 값은 매번 다르기 때문에 태그 등록을 많이 해둠.                                    
                                //    for (int i = dt.Rows.Count - 1; 0 <= i; i--)
                                //    {
                                //        bool activalFlag = false;
                                //        for (int j = 0; j < dt.Columns.Count; j++)
                                //        {
                                //            if (dt.Rows[i][j].ToString() != "0" && dt.Rows[i][j].ToString() != _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[0].STEPNAME && dt.Rows[i][j].ToString() != "")
                                //            {
                                //                activalFlag = true;
                                //                break;
                                //            }
                                //        }
                                //        if (activalFlag)
                                //        {
                                //            continue;
                                //        }
                                //        else
                                //        {
                                //            dt.Rows.RemoveAt(i);
                                //        }
                                //    }
                                //}

                                if (await _BR_BRS_SEL_EquipmentRecipeStepType.Execute())
                                {
                                    for (int i = 0; i < _BR_BRS_SEL_EquipmentRecipe.OUTDATAs.Count; i++)
                                    {
                                        _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.INDATAs.Clear();
                                        _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs.Clear();
                                        _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.INDATAs.Add(new BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.INDATA
                                        {
                                            EQPTID = _BR_BRS_SEL_EquipmentRecipe.OUTDATAs[i].EQPTID,
                                            TAGID = _BR_BRS_SEL_EquipmentRecipe.OUTDATAs[i].TAGID,
                                            COLUMNNAME = _BR_BRS_SEL_EquipmentRecipe.OUTDATAs[i].COLUMNNAME,
                                            COLUMNSEQ = _BR_BRS_SEL_EquipmentRecipe.OUTDATAs[i].COLUMNSEQ,
                                            ROWSEQ = _BR_BRS_SEL_EquipmentRecipe.OUTDATAs[i].ROWSEQ
                                        });

                                        if (await _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.Execute())
                                        {
                                            var temp = new BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATA
                                            {
                                                EQPTID = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].EQPTID,
                                                TAGID = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].TAGID,
                                                ACTVAL = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].ACTVAL,
                                                COLUMNNAME = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].COLUMNNAME,
                                                COLUMNSEQ = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].COLUMNSEQ,
                                                ROWSEQ = _BR_BRS_SEL_EquipmentRecipe_TAG_ACTVAL.OUTDATAs[0].ROWSEQ
                                            };
                                            filteredComponents.Add(temp);
                                        }                                        
                                    }

                                    dt = new DataTable();

                                    // ROWSEQ가 0은 레시피명
                                    foreach (var item in filteredComponents.Where(o => o.ROWSEQ == 0).OrderBy(o => o.COLUMNSEQ))
                                    {
                                        if (item.COLUMNSEQ == 0)
                                        {
                                            RECIPENAME = item.ACTVAL;
                                        }
                                    }

                                    rcpName = _mainWnd.CurrentInstruction.Raw.TARGETVAL;
                                    if (rcpName.Trim() == RECIPENAME)
                                    {
                                        RECORD_ENABLE = true;
                                    }
                                    else
                                    {
                                        RECORD_ENABLE = false;
                                        OnMessage("레시피 이름: " + RECIPENAME + ", Target : " + rcpName + "\r\n레시피이름과 Target이 다릅니다.");
                                        return;
                                    }

                                    // ROWSEQ가 1부터는 스탭. dt 컬럼 추출하기 위해 ROWSEQ 가 1인 데이터들 조회
                                    foreach (var item in filteredComponents.Where(o => o.ROWSEQ == 1).OrderBy(o => o.COLUMNSEQ))
                                    {
                                        // Column 규칙 설비명.2.B1_태크명
                                        // EX) OBFG4001.2.B1_AirVolumeSetpoint
                                        //     OBFG4001.2.B2_AirVolumeSetpoint
                                        // TAGID에서 컬럼명만 가져와서 dt에 저장
                                        dt.Columns.Add(item.COLUMNNAME);
                                    }

                                    // ROWSEQ가 0은 레시피명. ROWSEQ가 1부터는 스탭
                                    foreach (var item in filteredComponents.Where(o => o.ROWSEQ != 0).OrderBy(o => o.ROWSEQ).ThenBy(o => o.COLUMNSEQ))
                                    {
                                        if (item.COLUMNSEQ == 0)
                                        {
                                            dt.Rows.Add();
                                        }
                                        for (int i = 0; i < dt.Columns.Count; i++)
                                        {
                                            //각 설비군마다 StepType에 따른 값 변경
                                            if (dt.Columns[i].ColumnName == item.COLUMNNAME && dt.Columns[i].ColumnName == _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[0].COLUMNNAME)
                                            {
                                                //StepType에 등록된 값이 있는 경우면 등록된 값으로 보여줌, 등록되지 않은 값이 올라올 수도 있음. EX) -1
                                                if (_BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs.Where(o => o.ACTIVAL == item.ACTVAL).Count() > 0)
                                                {
                                                    dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[Convert.ToInt32(item.ACTVAL)].STEPNAME;
                                                }
                                                else
                                                {
                                                    dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = item.ACTVAL;
                                                }
                                                break;
                                            }
                                            else if (dt.Columns[i].ColumnName == item.COLUMNNAME)
                                            {
                                                dt.Rows[Convert.ToInt32(item.ROWSEQ) - 1][i] = item.ACTVAL;
                                                break;
                                            }
                                        }
                                    }

                                    // 팀장님 요청사항. ROW의 데이터가 전부 0이면 해당 ROW 삭제. 사유는 실제 사용하는 태그 값은 매번 다르기 때문에 태그 등록을 많이 해둠.                                    
                                    for (int i = dt.Rows.Count - 1; 0 <= i; i--)
                                    {
                                        bool activalFlag = false;
                                        for (int j = 0; j < dt.Columns.Count; j++)
                                        {
                                            if (dt.Rows[i][j].ToString() != "0" && dt.Rows[i][j].ToString() != _BR_BRS_SEL_EquipmentRecipeStepType.OUTDATAs[0].STEPNAME && dt.Rows[i][j].ToString() != "")
                                            {
                                                activalFlag = true;
                                                break;
                                            }
                                        }
                                        if (activalFlag)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            dt.Rows.RemoveAt(i);
                                        }
                                    }
                                }                                  
                            }
                            ///

                            CommandResults["GetEquipmentRecipeAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["GetEquipmentRecipeAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["GetEquipmentRecipeAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("GetEquipmentRecipeAsync") ?
                       CommandCanExecutes["GetEquipmentRecipeAsync"] : (CommandCanExecutes["GetEquipmentRecipeAsync"] = true);
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

                            CommandResults["ConfirmCommand"] = false;

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();

                            ///
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                var authHelper = new iPharmAuthCommandHelper();
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

                            var dateTimeInstructions = _mainWnd.Instructions.Where(o =>
                            {
                                return string.Compare(o.Raw.REF_IRTGUID, _mainWnd.CurrentInstruction.Raw.IRTGUID) == 0 &&
                                    ((enumVariableType)Enum.Parse(typeof(enumVariableType), o.Raw.IRTTYPE, false)) == enumVariableType.IT004;
                            }).OrderBy(o => o.Raw.IRTSEQ).ToList();

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);                            

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
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

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            _mainWnd.Close();

                            //
                            CommandResults["NoRecordConfirmCommand"] = true;
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

        public byte[] imageToByteArray()
        {
            try
            {                
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);              

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);

                stream.Read(datas, 0, datas.Length);

                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }      
        #endregion
    }
}
