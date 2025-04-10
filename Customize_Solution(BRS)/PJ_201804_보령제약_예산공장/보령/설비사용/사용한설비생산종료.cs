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
using LGCNS.iPharmMES.Common;
using System.Threading.Tasks;

namespace 보령
{
    [Description("해당 작업장에서 해당 오더, 공정에 사용한 장비에 대해 일괄 사용 종료시킨다.")]
    public class 사용한설비생산종료 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCEND_ALL();

            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_BRS_EquipmentAction_PROCEND");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("생산 종료 로그북 생성"),
                string.Format("생산 종료 로그북 생성"),
                false,
                "EM_BRS_EquipmentAction_PROCEND",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }

            bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCEND_ALL.INDATA
            {
                LANGID = AuthRepositoryViewModel.Instance.LangID,
                ROOMID = AuthRepositoryViewModel.Instance.RoomID,
                POID = CurrentOrder.ProductionOrderID,
                OPSGGUID = CurrentOrder.OrderProcessSegmentID,
                USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCEND")
            });

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("작업장, 설비 생산종료");

            return outputValues;
        }
    }
}
