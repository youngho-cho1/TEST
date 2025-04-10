using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace 보령
{
    [Description("액조제탱크 투입")]
    public class 액조제탱크투입 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_Charge_Multi();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.EQPTID == null).FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.ACTVAL));

            string eqptid = string.Empty;

            foreach (var item in inputValues)
            {
                if (string.IsNullOrWhiteSpace(item.Raw.ACTVAL) && string.IsNullOrWhiteSpace(item.Raw.EQPTID))
                {
                    throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", item.Raw.IRTGUID));
                }

                eqptid = eqptid + (!string.IsNullOrWhiteSpace(item.Raw.ACTVAL) ? item.Raw.ACTVAL : item.Raw.EQPTID) + ",";
            }

            eqptid = eqptid.Substring(0, eqptid.LastIndexOf(','));

            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_BRS_EquipmentAction_PROCSTART");
            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("[{0}] 액조제탱크 투입 로그북 생성", eqptid),
                string.Format("액조제탱크 투입 로그북 생성"),
                false,
                "EM_BRS_EquipmentAction_PROCSTART",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }

            foreach (var item in inputValues)
            {
                bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_Charge_Multi.INDATA()
                {
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : "",
                    USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    DTTM = null
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_Charge_Multi.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                    PAVAL = this.CurrentOrder.OrderID,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : ""
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_Charge_Multi.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                    PAVAL = this.CurrentOrder.BatchNo,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : ""
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_Charge_Multi.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                    PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : ""
                });
            }

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("[{0}] 액조제탱크 투입 로그북 자동생성", eqptid);

            return outputValues;
        }
    }
}
