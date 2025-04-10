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
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 캡슐개별질량확인ViewModel : ViewModelBase
    {
        private 캡슐개별질량확인 _mainWnd;

        #region [Property]
        
        private decimal _maxStandard;
        public decimal maxStandard
        {
            get { return _maxStandard; }
            set
            {
                _maxStandard = value;
                OnPropertyChanged("maxStandard");
            }
        }

        private decimal _minStandard;
        public decimal minStandard
        {
            get { return _minStandard; }
            set
            {
                _minStandard = value;
                OnPropertyChanged("minStandard");
            }
        }

        private decimal _InputMax;
        public decimal InputMax
        {
            get { return _InputMax; }
            set
            {
                _InputMax = value;
                OnPropertyChanged("InputMax");
            }
        }

        private decimal _InputMin;
        public decimal InputMin
        {
            get { return _InputMin; }
            set
            {
                _InputMin = value;
                OnPropertyChanged("InputMin");
            }
        }

        private bool _isEnable;
        public bool isEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("isEnable");
            }
        }

        #endregion

        #region [Constructor]
        public 캡슐개별질량확인ViewModel()
        {
            
        }
        #endregion

        #region [BizRule]
        #endregion

        #region [Command]
        public ICommand LoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadCommandAsync"] = false;
                            CommandCanExecutes["LoadCommandAsync"] = false;
                            IsBusy = true;

                            if (arg != null && arg is 캡슐개별질량확인)
                            {
                                _mainWnd = arg as 캡슐개별질량확인;

                                var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                if (inputValues.Count > 0 && !string.IsNullOrWhiteSpace(inputValues[0].Raw.ACTVAL))
                                {
                                    var inputValue = inputValues[0];
                                    if (inputValue.Raw.NOTE != null)
                                    {
                                        DataSet ds = new DataSet();
                                        DataTable dt = new DataTable();
                                        var bytearray = inputValue.Raw.NOTE;
                                        string xml = System.Text.Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);

                                        ds.ReadXmlFromString(xml);
                                        dt = ds.Tables["DATA"];
                                        
                                        foreach (var row in dt.Rows)
                                        {
                                            minStandard = Convert.ToDecimal(row["하한값"]);
                                            maxStandard = Convert.ToDecimal(row["상한값"]);
                                        }
                                            
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", _mainWnd.CurrentInstruction.Raw.IRTGUID));
                                }
                            }

                            IsBusy = false;
                            CommandResults["LoadCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
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
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            IsBusy = true;
                            bool isCheckStandard = false;

                            if (InputMax <= 0 && InputMin <= 0) return;

                            if(minStandard > InputMin || maxStandard < InputMax || minStandard > InputMax || maxStandard < InputMin)
                            {
                                if (await OnMessageAsync("입력값이 기준값을 벗어났습니다. 기록을 진행하시겟습니까?", true) == false)
                                {
                                    IsBusy = false;
                                    CommandResults["LoadCommandAsync"] = true; 
                                    return;
                                }
                                else
                                {
                                    isCheckStandard = true;
                                }
                            }

                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            if (isCheckStandard)
                            {
                                authHelper = new iPharmAuthCommandHelper();

                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                enumRoleType inspectorRole = enumRoleType.ROLE001;
                                if (_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE != null && Enum.TryParse<enumRoleType>(_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                                {
                                }

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "기록값 일탈에 대해 서명후 기록을 진행합니다 ",
                                    "Deviation Sign",
                                    true,
                                    "OM_ProductionOrder_Deviation",
                                    "",
                                    this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                    this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                {
                                    IsBusy = false;
                                    return;
                                }
                                _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");
                            }

                            //이미지 저장시 서명화면으로 인해 이미지가 잘 안보임. 그에 따른 이미지 데이터만 먼저 생성해 놓도록 함.
                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "Image attached";
                            
                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (isCheckStandard)
                            {

                                var bizrule = new BR_PHR_REG_InstructionComment();

                                bizrule.IN_Comments.Add(
                                    new BR_PHR_REG_InstructionComment.IN_Comment
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        COMMENTTYPE = "CM008",
                                        COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation")
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

                            IsBusy = false;

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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


                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("기준값(%)"));
                            dt.Columns.Add(new DataColumn("하한값"));
                            dt.Columns.Add(new DataColumn("입력값"));
                            dt.Columns.Add(new DataColumn("상한값"));

                            DataRow row = dt.NewRow();
                            row["기준값(%)"] = "N/A";
                            row["하한값"] = "N/A";
                            row["입력값"] = "N/A";
                            row["상한값"] = "N/A";

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
        #endregion

        #region [User Define Function]
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
