using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
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
using System.Linq;

namespace 보령
{
    public class 소독액소분라벨발행 : ShopFloorCustomClass
    {
        public override Task<object> ExecuteAsync(object arg)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            var inputValues = InstructionModel.GetParameterSender(this.CurrentInstruction, this.Instructions);
            var outputValues = InstructionModel.GetResultReceiver(this.CurrentInstruction, this.Instructions);

            var viewModel = new PrinterSelectPopupViewModel() { RoomID = CurrentOrder.EquipmentID };
            var popup = new PrinterSelectPopup() { DataContext = viewModel };

            popup.Closed += async (s, e) =>
            {
                if (popup.DialogResult == true)
                {
                    // 전자서명 요청
                    var authHelper = new iPharmAuthCommandHelper();
                    authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_ReprintLabel");

                    if (await authHelper.ClickAsync(
                        Common.enumCertificationType.Function,
                        Common.enumAccessType.Create,
                        string.Format("소독액 소분 라벨 발행"),
                        "소독액 소분 라벨 발행",
                        false,
                        "OM_ProductionOrder_ReprintLabel",
                        "", null, null) == false)
                    {
                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                    }


                    var PrintBizRule = new BR_PHR_SEL_PRINT_LabelImage();
                    PrintBizRule.INDATAs.Clear();
                    PrintBizRule.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                    {
                        ReportPath = "/Reports/Label/LABEL_C0506_007_3",
                        PrintName = System.Windows.Browser.HttpUtility.UrlEncode(viewModel.SelectedPrinter.PRINTERNAME)
                    });

                    string[,] labelParam = new string[,] { { "POID", System.Windows.Browser.HttpUtility.UrlEncode(this.CurrentOrder.OrderID) }, { "USERID", System.Windows.Browser.HttpUtility.UrlEncode(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_ReprintLabel")) } };

                    for (int i = 0; i < labelParam.Rank; i++)
                    {
                        PrintBizRule.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                        {
                            ParamName = labelParam[i, 0],
                            ParamValue = labelParam[i, 1]
                        });
                    }

                    if (await PrintBizRule.Execute() == false) throw PrintBizRule.Exception;

                }

                this.CurrentInstruction.Raw.ACTVAL = string.Format("소독액 소분 라벨 발행");

                if (outputValues.Count > 0) outputValues[0].Raw.ACTVAL = string.Format("소독액 소분 라벨 발행");

                tcs.SetResult(outputValues);
            };
            popup.Show();

            return tcs.Task;
        }
    }
}
