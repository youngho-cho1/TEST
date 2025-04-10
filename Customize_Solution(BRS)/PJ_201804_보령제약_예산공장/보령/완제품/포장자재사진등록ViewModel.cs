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
using LGCNS.iPharmMES.Common;
using C1.Silverlight.Data;
using ShopFloorUI;
using System.IO;
using System.Windows.Media.Imaging;
using C1.Silverlight.Imaging;

namespace 보령
{
    public class 포장자재사진등록ViewModel : ViewModelBase
    {
        #region [Property]

        포장자재사진등록 _MainWnd;

        private bool _isEbConfirm;
        public bool isEbConfirm
        {
            get { return _isEbConfirm; }
            set
            {
                _isEbConfirm = value;
                NotifyPropertyChanged();
            }
        }

        private WriteableBitmap _Image;
        public WriteableBitmap Image
        {
            get { return _Image; }
            set { _Image = value; NotifyPropertyChanged(); }
        }

        private VideoBrush _VideoBrush;
        public VideoBrush VideoBrush
        {
            get { return _VideoBrush; }
            set { _VideoBrush = value; NotifyPropertyChanged(); }
        }

        CaptureSource _captureSource;

        #endregion



        #region[Bizlue]

        #endregion

        #region [Command]

        public ICommand OpenFileCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["OpenFileCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            CommandResults["OpenFileCommand"] = false;
                            CommandCanExecutes["OpenFileCommand"] = false;

                            if (arg != null)
                                _MainWnd = arg as 포장자재사진등록;

                            OpenFileDialog dlg = new OpenFileDialog();
                            if (dlg.ShowDialog() == true)
                            {
                                using (FileStream stream = dlg.File.OpenRead())
                                {                                                                        
                                    BitmapSource source = new BitmapImage();
                                    source.SetSource(stream);
                                    Image = new WriteableBitmap(source);

                                    stream.Close();                                   
                                }
                            }

                            isEbConfirm = true;

                            CommandResults["OpenFileCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["OpenFileCommand"] = true;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["OpenFileCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("OpenFileCommand") ?
                        CommandCanExecutes["OpenFileCommand"] : (CommandCanExecutes["OpenFileCommand"] = true);
                });
            }
        }

        public ICommand TakePictureCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["TakePicutreCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            CommandResults["TakePicutreCommand"] = false;
                            CommandCanExecutes["TakePicutreCommand"] = false;

                            if (arg != null)
                                _MainWnd = arg as 포장자재사진등록;

                            //if (CaptureDeviceConfiguration.RequestDeviceAccess())
                            //{
                            //    _captureSource.Start();
                            //}

                            //isEbConfirm = true;

                            CommandResults["TakePicutreCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["TakePicutreCommand"] = true;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["TakePicutreCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("TakePicutreCommand") ?
                        CommandCanExecutes["TakePicutreCommand"] : (CommandCanExecutes["TakePicutreCommand"] = true);
                });
            }
        }
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
                                IsBusy = true;
                                CommandResults["LoadedCommand"] = false;
                                CommandCanExecutes["LoadedCommand"] = false;

                                if (arg != null)
                                    _MainWnd = arg as 포장자재사진등록;

                                if (_MainWnd.CurrentInstruction.Raw.NOTE != null && _MainWnd.CurrentInstruction.Raw.NOTE.Length > 0)
                                {
                                    using (MemoryStream stream = new MemoryStream(_MainWnd.CurrentInstruction.Raw.NOTE))
                                    {
                                        BitmapSource source = new BitmapImage();
                                        source.SetSource(stream);
                                        Image = new WriteableBitmap(source);

                                        stream.Close();
                                    }
                                }

                                isEbConfirm = false;

                                CommandResults["LoadedCommand"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["LoadedCommand"] = true;
                                OnException(ex.Message, ex);
                            }
                            finally
                            {
                                CommandCanExecutes["LoadedCommand"] = true;
                                IsBusy = false;
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

                                if (arg != null)
                                    _MainWnd = arg as 포장자재사진등록;

                                var authHelper = new iPharmAuthCommandHelper();

                                if (_MainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _MainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                                {
                                    // 전자서명 요청
                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("기록값을 변경합니다."),
                                        string.Format("기록값 변경"),
                                        true,
                                        "OM_ProductionOrder_SUI",
                                        "", _MainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }                                

                                _MainWnd.CurrentInstruction.Raw.NOTE   = imageToByteArray();
                                _MainWnd.CurrentInstruction.Raw.ACTVAL = "Image Attached";

                                var result = await _MainWnd.Phase.RegistInstructionValue(_MainWnd.CurrentInstruction);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _MainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }                                                                

                                if (_MainWnd.Dispatcher.CheckAccess()) _MainWnd.DialogResult = true;
                                else _MainWnd.Dispatcher.BeginInvoke(() => _MainWnd.DialogResult = true);
                                
                                CommandResults["ConfirmCommandAsync"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["ConfirmCommandAsync"] = false;
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

        private byte[] imageToByteArray()
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(Image);
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);

                stream.Read(datas, 0, datas.Length);

                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region[Construct]

        public 포장자재사진등록ViewModel()
        {
            //_captureSource = new CaptureSource();
            //VideoBrush = new System.Windows.Media.VideoBrush();

            //VideoCaptureDevice vcd = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
            //_captureSource.VideoCaptureDevice = vcd;

            //VideoBrush.SetSource(_captureSource);
        }

        #endregion
    }
}
