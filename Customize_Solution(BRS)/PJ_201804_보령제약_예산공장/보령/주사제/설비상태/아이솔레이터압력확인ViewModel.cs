using C1.Silverlight.Imaging;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace 보령
{
    public class 아이솔레이터압력확인ViewModel : ViewModelBase
    {
        #region [Property]
        public 아이솔레이터압력확인ViewModel()
        {
            _ModuleGrovePressureList = new ObservableCollection<ModuleGrovePressure>();
        }

        아이솔레이터압력확인 _mainWnd;

        private ObservableCollection<ModuleGrovePressure> _ModuleGrovePressureList;
        public ObservableCollection<ModuleGrovePressure> ModuleGrovePressureList
        {
            get { return _ModuleGrovePressureList; }
            set
            {
                _ModuleGrovePressureList = value;
                OnPropertyChanged("ModuleGrovePressureList");
            }
        }
        #endregion
        #region [BizRule]
        #endregion
        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///
                            if (arg != null && arg is 아이솔레이터압력확인)
                            {
                                _mainWnd = arg as 아이솔레이터압력확인;
                                IsBusy = true;

                                _ModuleGrovePressureList = _mainWnd.ModuleGrovePressureList;
                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            if (await CheckAllData() == false)
                                throw new Exception("부적합한 테스트 결과가 있습니다.");

                            // 전자서명
                            var authHelper = new iPharmAuthCommandHelper();
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    true,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "아이솔레이터압력확인",
                                "아이솔레이터압력확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // 이미지 변환
                            C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.MainSection, null));
                            System.IO.Stream stream = bitmap.GetStream(ImageFormat.Png, true);

                            int len = (int)stream.Seek(0, SeekOrigin.End);

                            byte[] datas = new byte[len];

                            stream.Seek(0, SeekOrigin.Begin);
                            stream.Read(datas, 0, datas.Length);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "아이솔레이터압력확인";
                            _mainWnd.CurrentInstruction.Raw.NOTE = datas;

                            // 결과 기록
                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        public class ModuleGrovePressure : ViewModelBase
        {
            private string _ModuleNo;
            public string ModuleNo
            {
                get { return _ModuleNo; }
                set
                {
                    _ModuleNo = value;
                    OnPropertyChanged("ModuleNo");
                }
            }
            private string _GloveNo;
            public string GloveNo
            {
                get { return _GloveNo; }
                set
                {
                    _GloveNo = value;
                    OnPropertyChanged("GloveNo");
                }
            }
            private string _Pressure;
            public string Pressure
            {
                get { return _Pressure; }
                set
                {
                    _Pressure = value;
                    OnPropertyChanged("Pressure");
                }
            }
        }
        //private Task<bool> setInitialData()
        //{
        //    try
        //    {
        //        var tcs = new TaskCompletionSource<bool>();
        //        tcs.TrySetResult(true);

        //        if (_ModuleGrovePressureList == null)
        //            _ModuleGrovePressureList = new ObservableCollection<ModuleGrovePressure>();

        //        _ModuleGrovePressureList.Clear();
        //        // Module 1 - 3
        //        int module_cnt;
        //        int grove_cnt;
        //        for (module_cnt = 1; module_cnt <= 5; module_cnt++)
        //        {
        //            if (module_cnt == 1)
        //            {
        //                for (grove_cnt = 1; grove_cnt <= 12; grove_cnt++)
        //                {
        //                    ModuleGrovePressureList.Add(new ModuleGrovePressure
        //                    {
        //                        ModuleNo = "Module " + module_cnt,
        //                        GloveNo = "Glove " + grove_cnt,
        //                        Pressure = ""
        //                    });
        //                }
        //            }
        //            else if (module_cnt == 2 || module_cnt == 3 || module_cnt == 5)
        //            {
        //                for (grove_cnt = 1; grove_cnt <= 4; grove_cnt++)
        //                {
        //                    ModuleGrovePressureList.Add(new ModuleGrovePressure
        //                    {
        //                        ModuleNo = "Module " + module_cnt,
        //                        GloveNo = "Glove " + grove_cnt,
        //                        Pressure = ""
        //                    });
        //                }
        //            }
        //            else if (module_cnt == 4)
        //            {
        //                for (grove_cnt = 1; grove_cnt <= 2; grove_cnt++)
        //                {
        //                    ModuleGrovePressureList.Add(new ModuleGrovePressure
        //                    {
        //                        ModuleNo = "Module " + module_cnt,
        //                        GloveNo = "Glove " + grove_cnt,
        //                        Pressure = ""
        //                    });
        //                }
        //            }
        //        }

        //        _ModuleGrovePressureList.Add(new ModuleGrovePressure
        //        {
        //            ModuleNo = "Module 1-5",
        //            GloveNo = "VHP Pressure hold test",
        //            Pressure = ""
        //        });

        //        Console.WriteLine(DateTime.Now);

        //        return tcs.Task;
        //    }
        //    catch (Exception ex)
        //    {
        //        var tcs = new TaskCompletionSource<bool>();
        //        tcs.SetResult(false);
        //        OnException(ex.Message, ex);
        //        return tcs.Task;
        //    }
        //}
        private Task<bool> CheckAllData()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();
            tcs.TrySetResult(true);
            decimal chk;
            foreach (var item in _ModuleGrovePressureList)
            {
                if (!string.IsNullOrWhiteSpace(item.Pressure) && decimal.TryParse(item.Pressure, out chk))
                {
                    if (item.ModuleNo == "Module 1-5")
                    {
                        if (chk >= 25)
                            tcs.TrySetResult(false);
                    }
                    else
                    {
                        if (chk >= 40)
                            tcs.TrySetResult(false);
                    }
                }
                else
                    tcs.TrySetResult(false);
            }
            return tcs.Task;
        }
        #endregion
    }
}
