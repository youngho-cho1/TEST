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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using LGCNS.iPharmMES.Common;

namespace 보령
{
    [Description("EM모니터링 요청")]
    public class EM모니터링 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            BR_BRS_REG_EM_EQPT_MONITOR _BR_BRS_REG_EM_EQPT_MONITOR = new BR_BRS_REG_EM_EQPT_MONITOR();

            _BR_BRS_REG_EM_EQPT_MONITOR.INDATAs.Clear();
            _BR_BRS_REG_EM_EQPT_MONITOR.OUTDATAs.Clear();

            // 전자서명
            var authHelper = new iPharmAuthCommandHelper();

            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                "EM모니터링요청",
                "EM모니터링요청",
                false,
                "OM_ProductionOrder_SUI",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }

            // 비즈룰 수행
            _BR_BRS_REG_EM_EQPT_MONITOR.INDATAs.Add(new BR_BRS_REG_EM_EQPT_MONITOR.INDATA
            {
                POID = this.CurrentOrder.ProductionOrderID,
                OPSGGUID = this.CurrentOrder.OrderProcessSegmentID,
                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
            });

            if(await _BR_BRS_REG_EM_EQPT_MONITOR.Execute() == true)
            {
                this.CurrentInstruction.Raw.ACTVAL = _BR_BRS_REG_EM_EQPT_MONITOR.OUTDATAs[0].RESULTMSG;
            }

            return outputValues;
        }
    }
}
