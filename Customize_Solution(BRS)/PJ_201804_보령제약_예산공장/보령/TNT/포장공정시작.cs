using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace 보령
{
    public class 포장공정시작 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_UDB_REG_ProductionOrder_Packing_Start();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.ISVISIBLE == "Y").FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.IRTGUID));

            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Output");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("포장 공정 확인"),
                "포장 공정 확인(포장 오더 Track&Trace 정합성 체크",
                false,
                "OM_ProductionOrder_Output",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }


            bizRule.INDATAs.Add(new BR_UDB_REG_ProductionOrder_Packing_Start.INDATA()
            {
                POID = CurrentOrder.ProductionOrderID,
                USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Output"),
                EQPTID = CurrentOrder.EquipmentID,
                OPSGGUID = CurrentOrder.OrderProcessSegmentID
            });


            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("{0}", "포장 공정 시작");

            return outputValues;
        }
    }
}
