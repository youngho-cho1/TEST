using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace 보령
{
    [Description("소독액 사용량 계산")]
    public class 소독원액사용량계산 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_MaterialSubLot_UsedQty();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.ISVISIBLE == "Y").FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.IRTGUID));

            if (inputValues.Count != 2) throw new Exception(string.Format("입력 파라미터 갯수가 일치하지 않습니다"));


            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Output");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("소독원액 사용량 계산", inputValues[0].Raw.ACTVAL),
                "소독원액 사용량 계산",
                false,
                "OM_ProductionOrder_Output",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }


            bizRule.INDATAs.Add(new BR_BRS_UPD_MaterialSubLot_UsedQty.INDATA()
            {
                MSUBLOTBCD = inputValues[0].Raw.ACTVAL != null ? inputValues[0].Raw.ACTVAL : "",
                USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Output"),
                USEDQTY = Convert.ToSingle(inputValues[1].Raw.ACTVAL),
                POID = this.CurrentOrder.ProductionOrderID != null ? this.CurrentOrder.ProductionOrderID : ""
            });


            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("[{0}] 소독원액 사용량 계산", Convert.ToSingle(inputValues[1].Raw.ACTVAL));

            return outputValues;
        }
    }
}
