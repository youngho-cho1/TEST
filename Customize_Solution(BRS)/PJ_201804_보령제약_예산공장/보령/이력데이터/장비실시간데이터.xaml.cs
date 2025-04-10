using C1.Silverlight;
using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;


namespace 보령
{
    [Description("실시간 장비 데이터(Analog value, Digital value(On/Off)String, Datetime) 인터페이스")]
    public partial class 장비실시간데이터 : ShopFloorCustomWindow
    {
        private class TimerHandler
        {
            장비실시간데이터 popup;
            //DispatcherTimer tmr;

            public TimerHandler(장비실시간데이터 popup, int timeTickSecond)
            {
                this.popup = popup;

                //tmr = new DispatcherTimer();
                //tmr.Interval = new System.TimeSpan(0, 0, timeTickSecond);
                //tmr.Tick += Handler;
            }

            //public void Start()
            //{
            //    tmr.Start();
            //}

            //public void Handler(object s, EventArgs e)
            //{
            //    tmr.Stop();

            //    if (!popup.DialogResult.HasValue)
            //    {
            //        popup.GetValues();

            //        tmr.Start();
            //    }
            //    else
            //    {
            //        tmr = null;
            //        popup = null;
            //    }
            //}
        }

        private TimerHandler tmr;
        private DateTime currTime;
        private bool isbusy = false;
        //bool isDeviation = false;

        BR_PHR_SEL_Element_ELMNAME _BR_PHR_SEL_Element_ELMNAME;
        BR_PHR_SEL_Element_Variable _BR_PHR_SEL_Element_Variable;
        BR_PHR_SEL_RealtimeData _BR_PHR_SEL_RealtimeData;

        public 장비실시간데이터()
        {
            InitializeComponent();

            this.Loaded += TestRealtimeDataPopup_Loaded;

            //tmr = new TimerHandler(this, 10);

            _BR_PHR_SEL_Element_ELMNAME = new BR_PHR_SEL_Element_ELMNAME();
            _BR_PHR_SEL_Element_Variable = new BR_PHR_SEL_Element_Variable();
            _BR_PHR_SEL_RealtimeData = new BR_PHR_SEL_RealtimeData();

            dgMon.ItemsSource = _BR_PHR_SEL_RealtimeData.OUTDATAs;
        }

        async Task GetValues()
        {
            try
            {
                var paramInsts = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);

                var bizRule = _BR_PHR_SEL_RealtimeData;

                foreach (var inst in paramInsts)
                {
                    if (inst.Raw.CANCELYN == "N")
                    {
                        bizRule.INDATAs.Add(new BR_PHR_SEL_RealtimeData.INDATA()
                        {
                            EQPTID = inst.Raw.EQPTID,
                            TAGID = inst.Raw.TAGID,
                            ACTVAL = inst.Raw.ACTVAL
                        });
                    }
                }
                
                if(!string.IsNullOrEmpty(this.CurrentInstruction.Raw.EQPTID) && !string.IsNullOrEmpty(this.CurrentInstruction.Raw.TAGID))
                {
                    bizRule.INDATAs.Add(new BR_PHR_SEL_RealtimeData.INDATA()
                    {
                        EQPTID = this.CurrentInstruction.Raw.EQPTID,
                        TAGID = this.CurrentInstruction.Raw.TAGID,
                        ACTVAL = this.CurrentInstruction.Raw.ACTVAL
                    });
                }                

                if (await bizRule.Execute() == false) return;

                bizRule.OUTDATAs.ToList().ForEach(o =>
                {
                    var matched = _BR_PHR_SEL_Element_Variable.OUTDATAs.Where(o2 => o2.VARNAME == o.TAGID).FirstOrDefault();
                    if (matched != null)
                    {
                        o.TAGNAME = matched.VARDESC;
                    }
                });

                bizRule.OUTDATAs.ToList().ForEach(o =>
                {
                    if (!string.IsNullOrWhiteSpace(o.BACTVAL))
                    {
                        o.ACTVAL = o.BACTVAL;
                    }
                });
                grdEqpt.DataContext = bizRule.OUTDATAs[0];

                if (_selectedIndex >= 0)
                {
                    if (dgMon.Dispatcher.CheckAccess()) dgMon.SelectedIndex = _selectedIndex;
                    else dgMon.Dispatcher.BeginInvoke(() => dgMon.SelectedIndex = _selectedIndex);
                }
                //dgMon.ItemsSource = bizRule.OUTDATAs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async Task GetNewValues()
        {
            try
            {
                var paramInsts = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);

                var bizRule = _BR_PHR_SEL_RealtimeData;

                foreach (var inst in paramInsts)
                {
                    if (inst.Raw.CANCELYN == "N")
                    {
                        bizRule.INDATAs.Add(new BR_PHR_SEL_RealtimeData.INDATA()
                        {
                            EQPTID = inst.Raw.EQPTID,
                            TAGID = inst.Raw.TAGID,
                            ACTVAL = EqptId.Text
                        });
                    }
                }

                if (!string.IsNullOrEmpty(this.CurrentInstruction.Raw.EQPTID) && !string.IsNullOrEmpty(this.CurrentInstruction.Raw.TAGID))
                {
                    bizRule.INDATAs.Add(new BR_PHR_SEL_RealtimeData.INDATA()
                    {
                        EQPTID = EqptId.Text,
                        TAGID = this.CurrentInstruction.Raw.TAGID,
                        ACTVAL = this.CurrentInstruction.Raw.ACTVAL
                    });
                }

                if (await bizRule.Execute() == false) return;

                bizRule.OUTDATAs.ToList().ForEach(o =>
                {
                    var matched = _BR_PHR_SEL_Element_Variable.OUTDATAs.Where(o2 => o2.VARNAME == o.TAGID).FirstOrDefault();
                    if (matched != null)
                    {
                        o.TAGNAME = matched.VARDESC;
                    }
                });

                bizRule.OUTDATAs.ToList().ForEach(o =>
                {
                    if (!string.IsNullOrWhiteSpace(o.BACTVAL))
                    {
                        o.ACTVAL = o.BACTVAL;
                    }
                });
                grdEqpt.DataContext = bizRule.OUTDATAs[0];

                if (_selectedIndex >= 0)
                {
                    if (dgMon.Dispatcher.CheckAccess()) dgMon.SelectedIndex = _selectedIndex;
                    else dgMon.Dispatcher.BeginInvoke(() => dgMon.SelectedIndex = _selectedIndex);
                }
                //dgMon.ItemsSource = bizRule.OUTDATAs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async Task GetEqptInfo()
        {
            try
            {
                var paramInsts = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);

                if (paramInsts.Count > 0)
                {
                    var bizRule = new BR_PHR_SEL_Equipment_GetInfo();
                    bizRule.INDATAs.Add(new BR_PHR_SEL_Equipment_GetInfo.INDATA()
                    {
                        EQPTID = paramInsts[0].Raw.EQPTID,
                        LANGID = AuthRepositoryViewModel.Instance.LangID
                    });

                    if (await bizRule.Execute() == false || bizRule.OUTDATAs.Count <= 0) return;

                    //grdEqpt.DataContext = bizRule.OUTDATAs[0];

                    _BR_PHR_SEL_Element_ELMNAME.INDATAs.Add(new BR_PHR_SEL_Element_ELMNAME.INDATA()
                    {
                        EQPTID = paramInsts[0].Raw.EQPTID,
                    });

                    if (await _BR_PHR_SEL_Element_ELMNAME.Execute() == false || _BR_PHR_SEL_Element_ELMNAME.OUTDATAs.Count <= 0) return;

                    _BR_PHR_SEL_Element_Variable.INDATAs.Add(new BR_PHR_SEL_Element_Variable.INDATA()
                    {
                        ELMNO = _BR_PHR_SEL_Element_ELMNAME.OUTDATAs[0].ELMNO
                    });

                    if (await _BR_PHR_SEL_Element_Variable.Execute() == false || _BR_PHR_SEL_Element_Variable.OUTDATAs.Count <= 0) return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async Task GetSysTime()
        {
            try
            {
                var bizRule = new BR_CUS_GET_SYSTIME();

                if (await bizRule.Execute())
                {
                    currTime = bizRule.OUTDATAs[0].SYSTIME.Value;
                    grdTime.DataContext = bizRule.OUTDATAs[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async void TestRealtimeDataPopup_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await GetEqptInfo();
            await GetSysTime();
            await GetValues();

            //tmr.Start();
        }

        async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await GetValues();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private async void iPharmAuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isbusy == false)
                {
                    isbusy = true;


                    string oldTagName = string.Empty;
                    foreach (var item in (dgMon.ItemsSource as BR_PHR_SEL_RealtimeData.OUTDATACollection))
                    {
                        if (item.UPDDTTM <= currTime)
                        {
                            oldTagName = item.TAGNAME;
                            break;
                        }
                    }

                    //if (MessageBox.Show(string.Format("{0} 의 업데이트된 시간이 현재 시간 이전입니다. 기록을 진행하겠습니까?", oldTagName),
                    //    "iPharmMES", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    //{
                    //    return;
                    //}

                    var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

                    if (outputValues != null)
                    {
                        var ds = new DataSet();
                        var dt = new DataTable("DATA");
                        ds.Tables.Add(dt);

                        dt.Columns.Add(new DataColumn("설비"));
                        dt.Columns.Add(new DataColumn("항목명"));
                        dt.Columns.Add(new DataColumn("값"));
                        dt.Columns.Add(new DataColumn("시간"));

                        var sb = new StringBuilder();

                        foreach (var item in (dgMon.ItemsSource as BR_PHR_SEL_RealtimeData.OUTDATACollection))
                        {
                            var row = dt.NewRow();

                            row["설비"] = !string.IsNullOrEmpty(item.DIPLAY_EQPTID) ? item.DIPLAY_EQPTID : "N/A";
                            row["항목명"] = !string.IsNullOrEmpty(item.DISPALY_TAGNAME) ? item.DISPALY_TAGNAME : "N/A";
                            row["값"] = !string.IsNullOrEmpty(item.ACTVAL) ? item.ACTVAL : "N/A";
                            row["시간"] = (item.UPDDTTM.HasValue) ? item.UPDDTTM : DateTime.MinValue;

                            dt.Rows.Add(row);
                            
                            outputValues.ForEach(o =>
                            {
                                if (o.Raw.TAGID == item.TAGID) o.Raw.ACTVAL = item.ACTVAL;
                                //if (o.Raw.TAGID == item.TAGID)
                                //    if (o.Raw.TARGETVAL != item.ACTVAL)
                                //        isDeviation = true;
                            });

                            //if (outputValues.Count == 0)
                            //{
                            //    isDeviation = true;
                            //}
                        }

                        //bool isDeviationconfirm = false;

                        //if (isDeviation == true &&
                        //    this.CurrentInstruction.Raw.DVTPASSYN != "Y" &&
                        //    this.CurrentInstruction.Raw.VLTTYPE != enumValidationTypeCode.QMVLTNONE.ToString())
                        //{
                        //     if (MessageBox.Show("입력값이 기준값을 벗어났습니다. 기록을 진행하시겟습니까?", "기록이탈",MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        //        return;

                        //    var authHelper = new iPharmAuthCommandHelper();

                        //    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                        //    enumRoleType inspectorRole = enumRoleType.ROLE001;
                        //    if (this.Phase.CurrentPhase.INSPECTOR_ROLE != null && Enum.TryParse<enumRoleType>(this.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                        //    {
                        //    }

                        //    if (await authHelper.ClickAsync(
                        //        Common.enumCertificationType.Role,
                        //        Common.enumAccessType.Create,
                        //        "기록값 일탈에 대해 서명후 기록을 진행합니다 ",
                        //        "Deviation Sign",
                        //        true,
                        //        "OM_ProductionOrder_Deviation",
                        //        "",
                        //        this.CurrentInstruction.Raw.RECIPEISTGUID,
                        //        this.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                        //    {
                        //        return;
                        //    }
                        //    this.CurrentInstruction.Raw.DVTFCYN = "Y";
                        //    this.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");

                        //    isDeviationconfirm = true;
                        //}

                        if (this.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && this.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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
                                "", this.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }
                        }

                        var xml = BizActorRuleBase.CreateXMLStream(ds);
                        var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                        this.CurrentInstruction.Raw.ACTVAL = "TABLE,장비실시간데이터";
                        this.CurrentInstruction.Raw.NOTE = bytesArray;

                        var result = await this.Phase.RegistInstructionValue(this.CurrentInstruction);
                        if (result != enumInstructionRegistErrorType.Ok)
                        {
                            throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", CurrentInstruction.Raw.IRTGUID, result));
                        }

                        foreach (var item in outputValues)
                        {
                            result = await this.Phase.RegistInstructionValue(item);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", item.Raw.IRTGUID, result));
                            }
                        }

                    }
                    DialogResult = true;
                }               
                //this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        int _selectedIndex = 0;
        private void dgMon_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgMon.SelectedIndex >= 0)
            {
                _selectedIndex = dgMon.SelectedIndex;
            }
        }

        private async void TxtEqptId_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await GetNewValues();
                dgMon.ItemsSource = _BR_PHR_SEL_RealtimeData.OUTDATAs;
            }
        }
    }
}
