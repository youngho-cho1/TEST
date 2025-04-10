using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace 보령
{
    [Description("소독액 원액 재고 조회")]
    public class 소독액원액재고조회 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            //var bizRule = new BR_DWS_REG_ProductionOrder_MaterialSublot_Solution();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.ISVISIBLE == "Y").FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.IRTGUID));

            if (inputValues.Count != 1) throw new Exception(string.Format("입력 파라미터 갯수가 일치하지 않습니다"));

            var bizRuleComponentList = new BR_PHR_SEL_MaterialSubLot_MLOTID_Active();
            bizRuleComponentList.INDATAs.Add(new BR_PHR_SEL_MaterialSubLot_MLOTID_Active.INDATA()
            {
                MLOTID = inputValues[0].Raw.ACTVAL != null ? inputValues[0].Raw.ACTVAL : "",
                MSUBLOTTYPE = "INV"
            });
            if (await bizRuleComponentList.Execute() == false) throw bizRuleComponentList.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("소독원액 재고 : {0}", Convert.ToDecimal(bizRuleComponentList.OUTDATAs[0].MSUBLOTQTY));
            //if (outputValues.Count > 0) outputValues[0].Raw.ACTVAL = string.Format("소독원액 재고 : {0}",Convert.ToDecimal(bizRuleComponentList.OUTDATAs[0].MSUBLOTQTY));

            return outputValues;
        }
    }
}
