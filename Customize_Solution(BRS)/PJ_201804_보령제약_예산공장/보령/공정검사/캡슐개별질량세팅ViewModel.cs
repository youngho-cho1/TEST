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

namespace 보령
{
    public class 캡슐개별질량세팅ViewModel : ViewModelBase
    {
        private 캡슐개별질량세팅 _mainWnd;

        #region [Property]

        private ObservableCollection<standardInfo> _standardInfoList;
        public ObservableCollection<standardInfo> standardInfoList
        {
            get { return _standardInfoList; }
            set
            {
                _standardInfoList = value;
                OnPropertyChanged("standardInfoList");
            }
        }


        private string _targetStandard;
        public string targetStandard
        {
            get { return _targetStandard; }
            set
            {
                _targetStandard = value;
                OnPropertyChanged("targetStandard");
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

        private string _inPutStandard;
        public string inPutStandard
        {
            get { return _inPutStandard; }
            set
            {
                _inPutStandard = value;
                OnPropertyChanged("inPutStandard");
            }
        }
        
        #endregion

        #region [Constructor]
        public 캡슐개별질량세팅ViewModel()
        {
            _standardInfoList = new ObservableCollection<보령.캡슐개별질량세팅ViewModel.standardInfo>();
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

                            if (arg != null && arg is 캡슐개별질량세팅)
                            {
                                _mainWnd = arg as 캡슐개별질량세팅;
                                
                                if (string.IsNullOrEmpty(_mainWnd.CurrentInstruction.Raw.TARGETVAL))
                                {
                                    OnMessage("기준정보가 등록되지 않았습니다.");
                                    return;
                                }
                                else
                                {
                                    targetStandard = _mainWnd.CurrentInstruction.Raw.TARGETVAL;
                                }

                                isEnable = true;
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

                            if (string.IsNullOrEmpty(inPutStandard)) return;

                            int precision = 0;
                            
                            standardInfoList.Add(new standardInfo
                            {
                                targetStandard = Convert.ToDecimal(targetStandard),
                                minStandard = MathExt.Ceiling((Convert.ToDecimal(inPutStandard) * (1 - (Convert.ToDecimal(targetStandard) / 100))), 1).ToString(),
                                inputStandard = inPutStandard,
                                maxStandard = MathExt.Floor((Convert.ToDecimal(inPutStandard) * (1 + (Convert.ToDecimal(targetStandard) / 100))), 1).ToString()
                            });

                            isEnable = false;

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

        public ICommand SaveCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["SaveCommandAsync"] = false;
                            CommandCanExecutes["SaveCommandAsync"] = false;

                            if (standardInfoList.Count == 0)
                            {
                                throw new Exception(string.Format("입력값을 등록 후 기록해주세요."));
                            }

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
                            
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("기준값"));
                            dt.Columns.Add(new DataColumn("하한값"));
                            dt.Columns.Add(new DataColumn("입력값"));
                            dt.Columns.Add(new DataColumn("상한값"));

                            DataRow row = dt.NewRow();
                            row["기준값"] = standardInfoList[0].targetStandard;
                            row["하한값"] = standardInfoList[0].minStandard;
                            row["입력값"] = standardInfoList[0].inputStandard;
                            row["상한값"] = standardInfoList[0].maxStandard;

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

                            CommandResults["SaveCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveCommandAsync") ?
                        CommandCanExecutes["SaveCommandAsync"] : (CommandCanExecutes["SaveCommandAsync"] = true);
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

        public class standardInfo : BizActorDataSetBase
        {
            private string _minStandard;
            public string minStandard
            {
                get { return string.Format("{0:#,0}", _minStandard); }
                set
                {
                    _minStandard = string.Format("{0:#,0}", value);
                    OnPropertyChanged("minStandard");
                }
            }
            private string _maxStandard;
            public string maxStandard
            {
                get { return string.Format("{0:#,0}", _maxStandard); }
                set
                {
                    _maxStandard = string.Format("{0:#,0}", value);
                    OnPropertyChanged("maxStandard");
                }
            }

            private string _inputStandard;
            public string inputStandard
            {
                get { return string.Format("{0:#,0}", _inputStandard); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _inputStandard = chk.ToString();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("inPutStandard");
                }
            }

            private decimal _targetStandard;
            public decimal targetStandard
            {
                get { return _targetStandard; }
                set
                {
                    _targetStandard = value;
                    OnPropertyChanged("targetStandard");
                }
            }
            
        }
        #endregion
    }
}
