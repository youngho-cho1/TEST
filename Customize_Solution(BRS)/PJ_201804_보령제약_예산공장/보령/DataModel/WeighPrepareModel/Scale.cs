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

namespace 보령.DataModel.WeighPrepareModel
{
    public class Scale
    {
        public string ID { get; set; }
        public string Precision { get; set; }
        public string ScaleRange { get; set; }
        public string CalibrationStatus { get; set; }
        public string DailyCheckStatus { get; set; }
        public string MonthlyCheckStatus { get; set; }
        public DateTime LastCalibrationDate { get; set; }
        public string Model { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public double ScaleMin { get; set; }
        public double ScaleMax { get; set; }
        public string UnitID { get; set; }
        public string Interface { get; set; }
        public string ScaleType { get; set; }
        public string ScaleTypeName { get; set; }

        public BR_PHR_UPD_EquipmentAction_ShopFloor PrepareScaleUse(string batchNo, string materialName, 
            string quantity, string lotNo, string remark, string functionCode)
        {
            var bizRuleUseScale = new BR_PHR_UPD_EquipmentAction_ShopFloor();
            bizRuleUseScale.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.INDATA()
            {
                EQPTID = this.ID,
                EQACID = AuthRepositoryViewModel.GetSystemOptionValue("SCALE_USE_ONACTION"),
                USER = AuthRepositoryViewModel.GetUserIDByFunctionCode(functionCode),
                LANGID = AuthRepositoryViewModel.Instance.LangID,
                DTTM = null,
            });

            bizRuleUseScale.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.PARAMDATA()
            {
                EQPTID = this.ID,
                EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_STATUS"),
                EQPAID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_BATCHNO"),
                PAVAL = batchNo
            });
            bizRuleUseScale.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.PARAMDATA()
            {
                EQPTID = this.ID,
                EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_STATUS"),
                EQPAID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_MATERIAL"),
                PAVAL = materialName
            });
            bizRuleUseScale.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.PARAMDATA()
            {
                EQPTID = this.ID,
                EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_STATUS"),
                EQPAID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_QUANTITY"),
                PAVAL = quantity
            });
            bizRuleUseScale.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.PARAMDATA()
            {
                EQPTID = this.ID,
                EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_STATUS"),
                EQPAID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_LOTNO"),
                PAVAL = lotNo
            });
            bizRuleUseScale.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.PARAMDATA()
            {
                EQPTID = this.ID,
                EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALEUSE_STATUS"),
                EQPAID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_SCALECHECK_REMARK"),
                PAVAL = remark
            });

            return bizRuleUseScale;
        }

        public BR_PHR_UPD_EquipmentAction_ShopFloor CompleteScaleUse(string code = "WM_Weighing_Dispensing")
        {
            var bizRuleUseScale = new BR_PHR_UPD_EquipmentAction_ShopFloor();
            bizRuleUseScale.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_ShopFloor.INDATA()
            {
                EQPTID = this.ID,
                EQACID = AuthRepositoryViewModel.GetSystemOptionValue("SCALE_USE_OFFACTION"),
                USER = AuthRepositoryViewModel.GetUserIDByFunctionCode(code),
                LANGID = AuthRepositoryViewModel.Instance.LangID,
                DTTM = null,
            });

            return bizRuleUseScale;
        }
    }
}
