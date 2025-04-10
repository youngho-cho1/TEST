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
using C1.Silverlight.Data;

namespace 보령
{
    [Description("작업상 사용시작 액션 수행(로그인한 유저의 JOB도 같이 시작)")]
    public class 작업장사용시작 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            string roomId = AuthRepositoryViewModel.Instance.RoomID ?? "";
            DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

            if (inputValues.Count > 0 && !string.IsNullOrWhiteSpace(inputValues[0].Raw.ACTVAL))
            {
                roomId = inputValues[0].Raw.ACTVAL ?? "";

                bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.INDATA()
                {
                    ROOMNO = roomId,
                    USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    DTTM = null
                });

                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                    PAVAL = this.CurrentOrder.OrderID,
                    ROOMNO = roomId
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                    PAVAL = this.CurrentOrder.BatchNo,
                    ROOMNO = roomId
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                    PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                    ROOMNO = roomId
                });
            }
            else
            {
                bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.INDATA()
                {
                    ROOMNO = roomId,
                    USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    DTTM = null
                });

                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                    PAVAL = this.CurrentOrder.OrderID,
                    ROOMNO = roomId
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                    PAVAL = this.CurrentOrder.BatchNo,
                    ROOMNO = roomId
                });
                bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_ROOM.PARAMDATA()
                {
                    EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                    PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                    ROOMNO = roomId
                });
            }

            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_BRS_EquipmentAction_PROCSTART");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("[{0}] 생산 시작 로그북 생성", roomId),
                string.Format("생산 시작 로그북 생성"),
                false,
                "EM_BRS_EquipmentAction_PROCSTART",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("[{0}] 작업장 사용시작 로그북 자동생성", roomId);

            return outputValues;
        }
    }
}