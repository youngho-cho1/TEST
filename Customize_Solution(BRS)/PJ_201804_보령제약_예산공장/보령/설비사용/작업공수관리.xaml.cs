using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ShopFloorUI;
using System.ComponentModel;
using Order;
using System.Threading.Tasks;

namespace 보령
{
    [Description("작업공수관리 화면")]
    public partial class 작업공수관리 : ShopFloorCustomWindow
    {
        public 작업공수관리()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,작업공수관리"; }
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            var jobPopup = new mgtJob(mgtJobViewModel.enumViewType.ShopFloor);
            jobPopup.Closed += new EventHandler(jobPopup_Closed);

            jobPopup.Show();
        }
        void jobPopup_Closed(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }

        private async void Main_Closing(object sender, CancelEventArgs e)
        {
            //try
            //{
            //    if (this.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && this.Phase.CurrentPhase.STATE.Equals("COMP"))
            //        e.Cancel = false;
            //    else
            //    {
            //        Exception exception = await ShowMsgBox();
            //        if (exception != null)
            //        {
            //            e.Cancel = true; // 창 종료 취소

            //            var jobPopup = new mgtJob(mgtJobViewModel.enumViewType.ShopFloor);
            //            jobPopup.Closed += new EventHandler(jobPopup_Closed);

            //            jobPopup.Show();
            //        }
            //        else
            //        {
            //            var temp = this.CurrentInstruction;
            //            temp.Raw.ACTVAL = "작업공수입력완료";
            //            var res = await this.Phase.RegistInstructionValue(temp, true);
            //            if (res != enumInstructionRegistErrorType.Ok)
            //                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", temp.Raw.IRTGUID, res));

            //            if (this.Dispatcher.CheckAccess()) this.DialogResult = true;
            //            else this.Dispatcher.BeginInvoke(() => this.DialogResult = true);

            //            this.Close();

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    e.Cancel = true;
            //    Console.WriteLine(string.Format("{0} : {1}", ex.Message, ex.StackTrace));
            //}
        }
        private async Task<Exception> ShowMsgBox()
        {
            var completion = new TaskCompletionSource<Exception>();

            var msgRes = MessageBox.Show("공수입력이 완료되었습니까?","",MessageBoxButton.OKCancel);
            if(msgRes == MessageBoxResult.OK)
                completion.TrySetResult(null);
            else
                completion.TrySetResult(new Exception("취소되었습니다."));

            var result = await completion.Task;
            return result;
        }
    }
}
