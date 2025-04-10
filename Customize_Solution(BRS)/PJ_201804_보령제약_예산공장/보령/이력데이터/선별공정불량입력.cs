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
using ShopFloorUI;
using System.Threading.Tasks;
using System.ComponentModel;
using LGCNS.iPharmMES.Common;

namespace 보령
{
    [Description("선별공정에서 발생한 불량품 수량을 기록한다.")]
    public class 선별공정불량입력 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizrule = new BR_BRS_REG_INSTPCTION_FAILQTY();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            bizrule.INDATAs.Clear();

            bizrule.INDATAs.Add(new BR_BRS_REG_INSTPCTION_FAILQTY.INDATA
            {
                POID = this.CurrentOrder.ProductionOrderID,
                OPSGGUID = this.CurrentOrder.OrderProcessSegmentID,
                FAILQTY = inputValues[0].Raw.ACTVAL,
                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
            });

            if (await bizrule.Execute() == false) throw bizrule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("불량 : {0}개 입력",inputValues[0].Raw.ACTVAL);

            return outputValues;
        }
    }
}
