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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Linq;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace 보령
{
    public class 로드셀일일점검ViewModel : ViewModelBase
    {
        #region [Property]
        public 로드셀일일점검ViewModel()
        {
            _DailyCheckDatas = new ObservableCollection<LoadCellTagDailyCheck>();
            _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE = new BR_PHR_SEL_EquipmentCustomAttributeValue_CODE();
        }

        private 로드셀일일점검 _mainWnd;
        private string _EqptId;
        public string EqptId
        {
            get { return _EqptId; }
            set
            {
                _EqptId = value;
                OnPropertyChanged("EqptId");
            }
        }

        private string _MinVelue;
        public string MinVelue
        {
            get { return _MinVelue; }
            set
            {
                _MinVelue = value;
                OnPropertyChanged("MinVelue");
            }
        }

        private string _MaxVelue;
        public string MaxVelue
        {
            get { return _MaxVelue; }
            set
            {
                _MaxVelue = value;
                OnPropertyChanged("MaxVelue");
            }
        }

        private string _AverVelue;
        public string AverVelue
        {
            get { return _AverVelue; }
            set
            {
                _AverVelue = value;
                OnPropertyChanged("AverVelue");
            }
        }

        private ObservableCollection<LoadCellTagDailyCheck> _DailyCheckDatas;
        public ObservableCollection<LoadCellTagDailyCheck> DailyCheckDatas
        {
            get { return _DailyCheckDatas; }
            set
            {
                _DailyCheckDatas = value;
                OnPropertyChanged("DailyCheckDatas");
            }
        }
        #endregion

        #region [BizRule]
        private BR_PHR_SEL_EquipmentCustomAttributeValue_CODE _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE;
        public BR_PHR_SEL_EquipmentCustomAttributeValue_CODE BR_PHR_SEL_EquipmentCustomAttributeValue_CODE
        {
            get { return _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE; }
            set
            {
                _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE = value;
                OnPropertyChanged("BR_PHR_SEL_EquipmentCustomAttributeValue_CODE");
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
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is 로드셀일일점검)
                            {
                                _mainWnd = arg as 로드셀일일점검;

                                if (!string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.EQPTID))
                                {
                                    EqptId = _mainWnd.CurrentInstruction.Raw.EQPTID;
                                }
                                else
                                    _mainWnd.txtEqptId.Focus();

                                //2022.12.09 박희돈 기준정보 설정 추가
                                MinVelue = _mainWnd.CurrentInstruction.Raw.MINVAL;
                                MaxVelue = _mainWnd.CurrentInstruction.Raw.MAXVAL;
                                AverVelue = _mainWnd.CurrentInstruction.Raw.TARGETVAL;
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
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        public ICommand ConnectEqptCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConnectEqptCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConnectEqptCommand"] = false;
                            CommandCanExecutes["ConnectEqptCommand"] = false;

                            ///
                            if (arg is string && !string.IsNullOrWhiteSpace(arg as string))
                            {
                                EqptId = (arg as string).ToUpper().Trim();

                                _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.INDATAs.Clear();
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.OUTDATAs.Clear();
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.INDATA
                                {
                                    EQPTID = EqptId,
                                    EQATCODE = "LOADCELLTAG"
                                });

                                // 설비 태그 조회
                                if (await _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.Execute() == true && _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.OUTDATAs.Count > 0)
                                {
                                    DailyCheckDatas.Clear();
                                    var SortedList = _BR_PHR_SEL_EquipmentCustomAttributeValue_CODE.OUTDATAs.OrderBy(x => x.EQATVAL1).ToList();

                                    int seq = 1;
                                    foreach (var item in SortedList)
                                    {
                                        DailyCheckDatas.Add(new LoadCellTagDailyCheck
                                        {
                                            TAGID = item.EQATVAL1,
                                            LOADCELLNO = seq++,
                                            CLEANSTATUS = "",
                                            ZEROSTATUS = "",
                                            TAGVALUE = "",
                                            DAILYCHKSTATUS = "",
                                            MinVelue = MinVelue,
                                            AverVelue = AverVelue,
                                            MaxVelue = MaxVelue
                                        });
                                    }
                                    _mainWnd.gdDailyCheck.Refresh();
                                }
                                else
                                {
                                    EqptId = "";
                                    _mainWnd.txtEqptId.Focus();
                                }
                            }

                            IsBusy = false;
                            ///

                            CommandResults["ConnectEqptCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ConnectEqptCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConnectEqptCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConnectEqptCommand") ?
                        CommandCanExecutes["ConnectEqptCommand"] : (CommandCanExecutes["ConnectEqptCommand"] = true);
                });
            }
        }

        public ICommand DailyCheckCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["DailyCheckCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["DailyCheckCommand"] = false;
                            CommandCanExecutes["DailyCheckCommand"] = false;

                            ///
                            if (arg != null && arg is LoadCellTagDailyCheck)
                            {
                                var curLoadCell = (arg as LoadCellTagDailyCheck);

                                // 점검 팝업 OPEN
                                var popup = new 로드셀점검팝업();

                                popup.lblAlert.Content = curLoadCell.TAGID + " 의 측정값";
                                popup.TAGID = curLoadCell.TAGID;
                                popup.EQTPID = _EqptId;

                                popup.btnConfirm.Click += (s, e) =>
                                {
                                    curLoadCell.TAGVALUE = popup.txtTagValue.Content.ToString();
                                    curLoadCell.CLEANSTATUS = "양호";
                                    curLoadCell.ZEROSTATUS = "적합";

                                    if (curLoadCell.TAGVALUE.Equals("N/A"))
                                    {
                                        OnMessage("로드셀 값을 확인해주세요.");
                                        return;
                                    }

                                    if (string.IsNullOrEmpty(MinVelue) || string.IsNullOrEmpty(AverVelue) || string.IsNullOrEmpty(MaxVelue))
                                    {
                                        OnMessage("기준정보 설정이 되어있지 않습니다.");
                                        curLoadCell.DAILYCHKSTATUS = "부적합";
                                    }
                                    else
                                    {
                                        //2022.12.09 박희돈 레시피 디자이너의 기준값을 사용하여 적부 판단.
                                        if (Convert.ToDecimal(MinVelue) <= Convert.ToDecimal(curLoadCell.TAGVALUE) && Convert.ToDecimal(MaxVelue) >= Convert.ToDecimal(curLoadCell.TAGVALUE))
                                        {
                                            curLoadCell.DAILYCHKSTATUS = "적합";
                                        }
                                        else
                                        {
                                            curLoadCell.DAILYCHKSTATUS = "부적합";
                                        }
                                    }
                                    _mainWnd.gdDailyCheck.Refresh();
                                    popup.Close();
                                };

                                popup.btnCancel.Click += (s, e) =>
                                {
                                    _mainWnd.gdDailyCheck.Refresh();
                                    popup.Close();
                                };

                                popup.Show();
                            }

                            IsBusy = false;
                            ///

                            CommandResults["DailyCheckCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["DailyCheckCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["DailyCheckCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("DailyCheckCommand") ?
                        CommandCanExecutes["DailyCheckCommand"] : (CommandCanExecutes["DailyCheckCommand"] = true);
                });
            }
        }
        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            CommandResults["ComfirmCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "로드셀일일점검",
                                "로드셀일일점검",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("로드셀번호"));
                            //dt.Columns.Add(new DataColumn("청소상태"));
                            //dt.Columns.Add(new DataColumn("영점조정"));
                            dt.Columns.Add(new DataColumn("최소값"));
                            dt.Columns.Add(new DataColumn("TARGET"));
                            dt.Columns.Add(new DataColumn("최대값"));
                            dt.Columns.Add(new DataColumn("정확도측정값"));
                            dt.Columns.Add(new DataColumn("적합여부"));

                            foreach (var item in DailyCheckDatas)
                            {
                                var row = dt.NewRow();

                                row["로드셀번호"] = item.LOADCELLNO;
                                //row["청소상태"] = item.CLEANSTATUS ?? "";
                                //row["영점조정"] = item.ZEROSTATUS ?? "";
                                //2022.12.09 박희돈 상한,하한,평균값 추가
                                row["최소값"] = MinVelue ?? "";
                                row["TARGET"] = AverVelue ?? "";
                                row["최대값"] = MaxVelue ?? "";
                                row["정확도측정값"] = item.TAGVALUE ?? "";
                                row["적합여부"] = item.DAILYCHKSTATUS ?? "";

                                dt.Rows.Add(row);
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

                            ///

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        public class LoadCellTagDailyCheck : ViewModelBase
        {
            private int _LOADCELLNO;
            public int LOADCELLNO
            {
                get { return _LOADCELLNO; }
                set
                {
                    _LOADCELLNO = value;
                    OnPropertyChanged("LOADCELLNO");
                }
            }
            private string _CLEANSTATUS;
            public string CLEANSTATUS
            {
                get { return _CLEANSTATUS; }
                set
                {
                    _CLEANSTATUS = value;
                    OnPropertyChanged("CLEANSTATUS");
                }
            }
            private string _ZEROSTATUS;
            public string ZEROSTATUS
            {
                get { return _ZEROSTATUS; }
                set
                {
                    _ZEROSTATUS = value;
                    OnPropertyChanged("ZEROSTATUS");
                }
            }
            private string _TAGVALUE;
            public string TAGVALUE
            {
                get { return _TAGVALUE; }
                set
                {
                    _TAGVALUE = value;
                    OnPropertyChanged("TAGVALUE");
                }
            }
            private string _DAILYCHKSTATUS;
            public string DAILYCHKSTATUS
            {
                get { return _DAILYCHKSTATUS; }
                set
                {
                    _DAILYCHKSTATUS = value;
                    OnPropertyChanged("DAILYCHKSTATUS");
                }
            }
            private string _TAGID;
            public string TAGID
            {
                get { return _TAGID; }
                set
                {
                    _TAGID = value;
                    OnPropertyChanged("TAGID");
                }
            }


            private string _MinVelue;
            public string MinVelue
            {
                get { return _MinVelue; }
                set
                {
                    _MinVelue = value;
                    OnPropertyChanged("MinVelue");
                }
            }

            private string _MaxVelue;
            public string MaxVelue
            {
                get { return _MaxVelue; }
                set
                {
                    _MaxVelue = value;
                    OnPropertyChanged("MaxVelue");
                }
            }

            private string _AverVelue;
            public string AverVelue
            {
                get { return _AverVelue; }
                set
                {
                    _AverVelue = value;
                    OnPropertyChanged("AverVelue");
                }
            }
        }
        #endregion
    }
}