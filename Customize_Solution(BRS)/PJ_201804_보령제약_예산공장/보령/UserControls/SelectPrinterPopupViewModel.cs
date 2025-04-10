using LGCNS.iPharmMES.Common;
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

namespace 보령.UserControls
{
    public class SelectPrinterPopupViewModel : ViewModelBase
    {
        #region property
        private string _RoomID;
        public string RoomID
        {
            get { return _RoomID; }
            set
            {
                _RoomID = value;
                OnPropertyChanged("RoomID");
            }
        }
        private string _RoomName;
        public string RoomName
        {
            get { return _RoomName; }
            set
            {
                _RoomName = value;
                GetPrinterInfo(_RoomID);
                OnPropertyChanged("RoomName");
            }
        }
        private BR_PHR_SEL_System_Printer.OUTDATA _selectedPrint;
        public BR_PHR_SEL_System_Printer.OUTDATA selectedPrint
        {
            get { return _selectedPrint; }
            set
            {
                _selectedPrint = value;
                OnPropertyChanged("selectedPrint");
            }
        }
        #endregion
        #region bizrule
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
        public BR_PHR_SEL_System_Printer BR_PHR_SEL_System_Printer
        {
            get { return _BR_PHR_SEL_System_Printer; }
            set
            {
                _BR_PHR_SEL_System_Printer = value;
                OnPropertyChanged("BR_PHR_SEL_System_Printer");
            }
        }
        #endregion
        #region command
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            RoomID = AuthRepositoryViewModel.Instance.RoomID;
                            RoomName = AuthRepositoryViewModel.Instance.RoomName;
                            OnPropertyChanged("RoomID");
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                       CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
               });
            }
        }
        #endregion
        private async void GetPrinterInfo(string roomid)
        {
            try
            {
                _selectedPrint = null;
                _BR_PHR_SEL_System_Printer.INDATAs.Clear();
                _BR_PHR_SEL_System_Printer.OUTDATAs.Clear();
                _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                {
                    ROOMID = string.IsNullOrWhiteSpace(roomid) ? AuthRepositoryViewModel.Instance.RoomID : roomid,
                    //IPADDRESS = Common.ClientIP
                });

                if(await BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                {
                    if (_BR_PHR_SEL_System_Printer.OUTDATAs.Count == 1)
                        selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                    else
                    {
                        foreach (var item in _BR_PHR_SEL_System_Printer.OUTDATAs)
                        {
                            if (item.ISDEFAULT == "Y")
                                selectedPrint = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
    }
}
