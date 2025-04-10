using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace 보령
{
    [Description("LIMS로 공정 중 시험의뢰를 한다")]
    public class 공정중시험의뢰 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_REG_LIMS_TEST_REQUEST_IP();
            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);
            
            string TST_REQ_NO = string.Empty;

            // 전자서명 요청
            //var authHelper = new iPharmAuthCommandHelper();
            //authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

            //if (await authHelper.ClickAsync(
            //    Common.enumCertificationType.Function,
            //    Common.enumAccessType.Create,
            //    string.Format("공정중 시험의뢰" ),
            //    string.Format("공정중 시험의뢰"),
            //    false,
            //    "OM_ProductionOrder_SUI",
            //    "", null, null) == false)
            //{
            //    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            //}

            bizRule.INDATAs.Add(new BR_BRS_REG_LIMS_TEST_REQUEST_IP.INDATA()
            {
                POID = this.CurrentOrder.ProductionOrderID,
                OPSGGUID = this.CurrentOrder.OrderProcessSegmentID,
                USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                OUTPUTID =  this.CurrentInstruction.Raw.BOMID// 
            });

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            if (bizRule.OUTDATAs.Count > 0)
            {
                TST_REQ_NO = bizRule.OUTDATAs[0].TST_REQ_NO;
            }

            this.CurrentInstruction.Raw.ACTVAL = TST_REQ_NO;

            return outputValues;
        }
    }
}
