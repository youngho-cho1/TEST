using C1.Silverlight;
using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;

namespace 보령
{
    [Description("BMS에서 온도, 습도, 차압순으로 정보를 가져온다.")]
    public partial class BMS실시간데이터 : ShopFloorCustomWindow
    {
        public bool isDirty = true;

        private class TimerHandler
        {
            BMS실시간데이터 popup;

            public TimerHandler(BMS실시간데이터 popup, int timeTickSecond)
            {
                this.popup = popup;

            }
        }

        private DateTime currTime;
        BR_BRS_GET_ROOM_BMS_INFO _BR_BRS_GET_ROOM_BMS_INFO = new BR_BRS_GET_ROOM_BMS_INFO();

        public BMS실시간데이터()
        {
            InitializeComponent();

            this.Loaded += TestRealtimeDataPopup_Loaded;
            
        }

        async Task GetValues()
        {
            try
            {
                this.busyIndicator.IsBusy = true;

                var refInst = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);

                _BR_BRS_GET_ROOM_BMS_INFO = new BR_BRS_GET_ROOM_BMS_INFO();
                _BR_BRS_GET_ROOM_BMS_INFO.INDATAs.Add(new BR_BRS_GET_ROOM_BMS_INFO.INDATA()
                {
                    ROOMID = AuthRepositoryViewModel.Instance.RoomID
                });
                if (refInst.Count > 0)
                {
                    foreach (var item in refInst)
                    {
                        _BR_BRS_GET_ROOM_BMS_INFO.INDATA_REFs.Add(new BR_BRS_GET_ROOM_BMS_INFO.INDATA_REF()
                        {
                            EQPTID = item.Raw.ACTVAL ?? ""
                        });
                    }
                }                

                if (await _BR_BRS_GET_ROOM_BMS_INFO.Execute() == false) return;

                dgMon.ItemsSource = _BR_BRS_GET_ROOM_BMS_INFO.OUTDATAs;
                
                if (_selectedIndex >= 0)
                {
                    if (dgMon.Dispatcher.CheckAccess()) dgMon.SelectedIndex = _selectedIndex;
                    else dgMon.Dispatcher.BeginInvoke(() => dgMon.SelectedIndex = _selectedIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.busyIndicator.IsBusy = false;
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async void TestRealtimeDataPopup_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                await GetSysTime();
                await GetValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataClear()
        {
            dgMon.ItemsSource = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private async void iPharmAuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.busyIndicator.IsBusy = true;


                if (isDirty)
                    isDirty = false;
                else
                    return;

                var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

                var ds = new DataSet();
                var dt = new DataTable("DATA");
                ds.Tables.Add(dt);

                dt.Columns.Add(new DataColumn("작업장"));
                dt.Columns.Add(new DataColumn("항목명"));
                dt.Columns.Add(new DataColumn("값"));
                dt.Columns.Add(new DataColumn("시간"));

                var sb = new StringBuilder();



                int idx = 0;

                foreach (var item in (dgMon.ItemsSource as BR_BRS_GET_ROOM_BMS_INFO.OUTDATACollection))
                {
                    var row = dt.NewRow();

                    row["작업장"] = !string.IsNullOrEmpty(item.ROOMID) ? item.ROOMID : "N/A";
                    row["항목명"] = !string.IsNullOrEmpty(item.EQATNAME) ? item.EQATNAME : "N/A";
                    row["값"] = !string.IsNullOrEmpty(item.ACTVAL) ? item.ACTVAL : "N/A";
                    row["시간"] = !string.IsNullOrEmpty(item.CURDATETIME) ? item.CURDATETIME : "N/A";

                    dt.Rows.Add(row);

                    if (outputValues != null && outputValues.Count > idx)
                    {
                        outputValues[idx].Raw.ACTVAL = item.ACTVAL;

                        idx++;
                    }
                }

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

                this.CurrentInstruction.Raw.ACTVAL = "TABLE,BMS실시간데이터";
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

                this.busyIndicator.IsBusy = false;

                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DialogResult = true;
                isDirty = true;
                this.busyIndicator.IsBusy = false;
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

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await GetSysTime();
                await GetValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

     
    }
}
