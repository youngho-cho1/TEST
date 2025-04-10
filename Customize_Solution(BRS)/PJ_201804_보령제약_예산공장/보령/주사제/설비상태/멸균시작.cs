using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace 보령
{
    [Description("멸균 시작 액션 수행")]
    public class 멸균시작 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_STERILE();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.EQPTID == null).FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.IRTGUID));

            //if (inputValues.Count != 1) throw new Exception(string.Format("입력 파라미터 갯수가 일치하지 않습니다"));


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
                string.Format("[{0}] 멸균 시작 로그북 생성", eqptid),
                string.Format("멸균 시작 로그북 생성"),
                false,
                "EM_BRS_EquipmentAction_PROCSTART",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }


            foreach (var item in inputValues)
            {

                bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_STERILE.INDATA()
                {
                    //EQACNAME = "생산시작",
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : item.Raw.EQPTID,
                    USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    DTTM = null
                });

                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_STERILE.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                    PAVAL = this.CurrentOrder.OrderID,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : item.Raw.EQPTID,
                    // EQSTID = "PD_EQSTPROC",
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_STERILE.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                    PAVAL = this.CurrentOrder.BatchNo,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : item.Raw.EQPTID,
                    // EQSTID = "PD_EQSTPROC",
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_STERILE.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                    PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                    EQPTID = item.Raw.ACTVAL != null ? item.Raw.ACTVAL : item.Raw.EQPTID,
                    //EQSTID = "PD_EQSTPROC",
                });
            }

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("[{0}] 멸균 시작 로그북 자동생성", eqptid);

            return outputValues;
        }
    }
}
