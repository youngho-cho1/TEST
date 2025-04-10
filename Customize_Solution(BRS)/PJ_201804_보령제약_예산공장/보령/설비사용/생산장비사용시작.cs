﻿using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령
{
    [Description("생산장비 사용시작 액션 수행(해당 장비의 JOB도 같이 시작)")]
    public class 생산장비사용시작 : ShopFloorCustomClass
    {
        public override async Task<object> ExecuteAsync(object arg)
        {
            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.EQPTID == null).FirstOrDefault();
            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.ACTVAL));

            string roomId = AuthRepositoryViewModel.Instance.RoomID;
            DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

            // 전자서명 요청
            var authHelper = new iPharmAuthCommandHelper();
            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_BRS_EquipmentAction_PROCSTART");

            if (await authHelper.ClickAsync(
                Common.enumCertificationType.Function,
                Common.enumAccessType.Create,
                string.Format("{0} 하위 생산장비 시작 로그북 생성", roomId),
                string.Format("생산 시작 로그북 생성"),
                false,
                "EM_BRS_EquipmentAction_PROCSTART",
                "", null, null) == false)
            {
                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
            }

            foreach (var item in inputValues)
            {
                /* 2022.01.05 김호연 n/a 소문자도 로그북 기록 안되도록 변경
                   화면에 보이지 않는 장비들이 있어 Actval 값이 Null인 데이터가 있음. 
                   Actval 값이 Null이면 ToUpper() 함수 사용시 NullException 에러가 나기 때문에 
                   item.Raw.ACTVAL == null인경우엔 기존 로직을 수행하고 null이 아닌경우에 item.Raw.ACTVAL.ToUpper() 조건 수행하도록 로직 수정 */
                if (item.Raw.ACTVAL == null)
                {
                    if (item.Raw.ACTVAL != "N/A")
                    {
                        bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.INDATA()
                        {
                            ROOMNO = roomId ?? "",
                            USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                            LANGID = AuthRepositoryViewModel.Instance.LangID,
                            DTTM = null,
                            POID = this.CurrentOrder.ProductionOrderID,
                            OPSGGUID = this.CurrentOrder.OrderProcessSegmentID,
                            EQPTID = item.Raw.EQPTID ?? "",
                            ACTVAL = item.Raw.ACTVAL ?? ""
                        });

                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                            PAVAL = this.CurrentOrder.OrderID,
                            ROOMNO = roomId ?? ""
                        });
                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                            PAVAL = this.CurrentOrder.BatchNo,
                            ROOMNO = roomId ?? ""
                        });
                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                            PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                            ROOMNO = roomId ?? ""
                        });
                    }
                }
                else
                {
                    if (item.Raw.ACTVAL.ToUpper() != "N/A")
                    {
                        bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.INDATA()
                        {
                            ROOMNO = roomId ?? "",
                            USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCSTART"),
                            LANGID = AuthRepositoryViewModel.Instance.LangID,
                            DTTM = null,
                            POID = this.CurrentOrder.ProductionOrderID,
                            OPSGGUID = this.CurrentOrder.OrderProcessSegmentID,
                            EQPTID = item.Raw.EQPTID ?? "",
                            ACTVAL = item.Raw.ACTVAL ?? ""
                        });

                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                            PAVAL = this.CurrentOrder.OrderID,
                            ROOMNO = roomId ?? ""
                        });
                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                            PAVAL = this.CurrentOrder.BatchNo,
                            ROOMNO = roomId ?? ""
                        });
                        bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_PROCEQPT.PARAMDATA()
                        {
                            EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                            PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                            ROOMNO = roomId ?? ""
                        });
                    }
                }

            }

            if (await bizRule.Execute() == false) throw bizRule.Exception;

            this.CurrentInstruction.Raw.ACTVAL = string.Format("[{0}] 하위 생산장비 사용시작 로그북 자동생성", roomId);

            return outputValues;
        }
    }
}