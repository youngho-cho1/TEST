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
using System.Windows.Media.Effects;
using ImageTools;
using ImageTools.IO.Png;
using C1.Silverlight;
using System.Collections.ObjectModel;

namespace 보령
{
    public class 이미지등록ViewModel : ViewModelBase
    {
        #region [Property]

        이미지등록 _MainWnd;

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

        private ReadOnlyCollection<VideoCaptureDevice> _AvailableDevices;
        private int curCameraSeq = 0;
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

                            Image = null;
                            _MainWnd.imgUpload.Source = null;

                            if (null != _captureSource)
                            {
                                _captureSource.Stop();
                            }

                            _MainWnd.rectWebCamView.Visibility = Visibility.Collapsed;
                            _MainWnd.imgUpload.Visibility = Visibility.Visible;

                            _MainWnd.rectWebCamView.Fill = null;

                            if (_MainWnd != null)
                            {
                                Button btnConnetWebCam = _MainWnd.btnConnetWebCam;

                                if (btnConnetWebCam.Content.ToString() == "사진촬영")
                                {
                                    btnConnetWebCam.Content = "카메라연결";
                                }
                            }

                            //이미지 프리징 이벤트
                            //_captureSource.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(source_CaptureImageCompleted);
                            //_captureSource.CaptureImageAsync();

                            OpenFileDialog dlg = new OpenFileDialog();
                            if (dlg.ShowDialog() == true)
                            {
                                using (FileStream stream = dlg.File.OpenRead())
                                {
                                    BitmapSource source = new BitmapImage();
                                    source.SetSource(stream);
                                    _MainWnd.imgUpload.Source = source;

                                    Image = new WriteableBitmap(source);

                                    //ImageBrush ib = new ImageBrush();

                                    //ib.ImageSource = Image;
                                    //_MainWnd.rectWebCamView.Fill = ib;

                                    stream.Close();

                                    isEbConfirm = true;
                                }
                            }
                            else
                            {
                                isEbConfirm = false;
                            }

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

                            //이미지 캡쳐 디바이스 연결
                            //_captureSource = new CaptureSource();
                            //VideoCaptureDevice vcd = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                            //_captureSource.VideoCaptureDevice = vcd;

                            //VideoBrush = new VideoBrush();
                            //ib = new ImageBrush();

                            //VideoBrush.SetSource(_captureSource);

                            //_MainWnd.rectWebCamView.Fill = VideoBrush;

                            //if (CaptureDeviceConfiguration.RequestDeviceAccess())
                            //{
                            //    _captureSource.Start();
                            //}

                            ///////////////////////////////////////////////

                            Image = null;
                            _MainWnd.imgUpload.Source = null;

                            _MainWnd.rectWebCamView.Visibility = Visibility.Visible;
                            _MainWnd.imgUpload.Visibility = Visibility.Collapsed;

                            Button btnConnetWebCam = _MainWnd.btnConnetWebCam;

                            if (btnConnetWebCam.Content.ToString() == "카메라연결")
                            {
                                Image = null;
                                _MainWnd.imgUpload.Source = null;

                                if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
                                {
                                    _AvailableDevices = CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices();

                                    if (_AvailableDevices.Count > 0)
                                        curCameraSeq = _AvailableDevices.Count - 1;

                                    VideoCaptureDevice videoCaptureDevice = _AvailableDevices[curCameraSeq];

                                    if (videoCaptureDevice != null)
                                    {
                                        _captureSource.VideoCaptureDevice = videoCaptureDevice;
                                        _captureSource.Start();

                                        VideoBrush videoBrush = new VideoBrush();
                                        videoBrush.Stretch = Stretch.Uniform;
                                        videoBrush.SetSource(_captureSource);
                                        _MainWnd.rectWebCamView.Fill = videoBrush;

                                        btnConnetWebCam.Content = "사진촬영";
                                        isEbConfirm = false;

                                    }

                                }
                            }
                            else
                            {
                                //if (null != _capture)
                                //    _capture.Stop();

                                //rectVideo.Fill = this.Resources["imageBrush"] as ImageBrush;

                                if (null != _captureSource)
                                {
                                    if (_captureSource.State == CaptureState.Started)
                                    {
                                        //_capture.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(_capture_CaptureImageCompleted);
                                        _captureSource.CaptureImageAsync();
                                        //_capture.Stop();

                                        btnConnetWebCam.Content = "카메라연결";
                                        isEbConfirm = true;

                                    }
                                    else
                                    {
                                        //MessageBox.Show("aaa Start capture from Webcam and try again!");
                                    }
                                }
                                //else
                                //    MessageBox.Show("bbb Start capture from Webcam and try again!");
                            }

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

        public ICommand ChangeCameraCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ChangeCameraCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            CommandResults["ChangeCameraCommand"] = false;
                            CommandCanExecutes["ChangeCameraCommand"] = false;

                            ///
                            if(_AvailableDevices != null && _AvailableDevices.Count > 0 && _captureSource != null && _captureSource.State == CaptureState.Started)
                            {
                                if (_AvailableDevices.Count - 1 == curCameraSeq)
                                    curCameraSeq = 0;
                                else
                                    curCameraSeq++;

                                VideoCaptureDevice videoCaptureDevice = _AvailableDevices[curCameraSeq];
                                if (videoCaptureDevice != null)
                                {
                                    _captureSource.Stop();
                                    _captureSource.VideoCaptureDevice = videoCaptureDevice;
                                    _captureSource.Start();

                                    VideoBrush videoBrush = new VideoBrush();
                                    videoBrush.Stretch = Stretch.Uniform;
                                    videoBrush.SetSource(_captureSource);
                                    _MainWnd.rectWebCamView.Fill = videoBrush;
     
                                    isEbConfirm = false;

                                }
                            }
                            ///
                            
                            CommandResults["ChangeCameraCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["ChangeCameraCommand"] = true;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ChangeCameraCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangeCameraCommand") ?
                        CommandCanExecutes["ChangeCameraCommand"] : (CommandCanExecutes["ChangeCameraCommand"] = true);
                });
            }
        }

        void source_CaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
        {
            try
            {
                WriteableBitmap wb = e.Result;

                if (_captureSource != null)
                {
                    _captureSource.Stop();

                    //ImageBrush image = new ImageBrush();
                    //image.ImageSource = wb;
                    //_MainWnd.rectWebCamView.Fill = image;

                    _MainWnd.rectWebCamView.Visibility = Visibility.Collapsed;
                    _MainWnd.imgUpload.Visibility = Visibility.Visible;

                    _MainWnd.imgUpload.Source = wb;

                    Image = wb;
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        //encode 
        public static ImageTools.ExtendedImage ToImg(WriteableBitmap bitmap)
        {
            ImageTools.ExtendedImage image = new ImageTools.ExtendedImage(bitmap.PixelWidth, bitmap.PixelHeight);

            try
            {
                byte[] pixels = image.Pixels;
                int[] raster = bitmap.Pixels;
                if (raster != null)
                {
                    for (int y = 0; y < image.PixelHeight; y++)
                    {
                        for (int x = 0; x < image.PixelWidth; x++)
                        {
                            int pixelIndex = bitmap.PixelWidth * y + x;
                            int pixel = raster[pixelIndex];
                            byte a = (byte)(pixel >> 24 & 255);
                            float aFactor = (float)a / 255f;
                            if (aFactor > 0f)
                            {
                                byte r = (byte)((float)(pixel >> 16 & 255) / aFactor);
                                byte g = (byte)((float)(pixel >> 8 & 255) / aFactor);
                                byte b = (byte)((float)(pixel & 255) / aFactor);
                                pixels[pixelIndex * 4] = r;
                                pixels[pixelIndex * 4 + 1] = g;
                                pixels[pixelIndex * 4 + 2] = b;
                                pixels[pixelIndex * 4 + 3] = a;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Bitmap cannot accessed", e);
            }
            return image;
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
                            {
                                _MainWnd = arg as 이미지등록;
                                _MainWnd.Closed += _MainWnd_Closed;
                            }

                            if (_MainWnd.CurrentInstruction.Raw.NOTE != null && _MainWnd.CurrentInstruction.Raw.NOTE.Length > 0)
                            {
                                using (MemoryStream stream = new MemoryStream(_MainWnd.CurrentInstruction.Raw.NOTE))
                                {
                                    BitmapSource source = new BitmapImage();
                                    source.SetSource(stream);
                                    _MainWnd.imgUpload.Source = source;

                                    Image = new WriteableBitmap(source);

                                    stream.Close();

                                    //ImageBrush ib = new ImageBrush();

                                    //ib.ImageSource = Image;
                                    //_MainWnd.rectWebCamView.Fill = ib;
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

                            if (Image == null)
                            {
                                C1MessageBox.Show("이미지를 등록하세요", "경고", C1MessageBoxButton.OK);
                                return;
                            }

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

                            _MainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _MainWnd.CurrentInstruction.Raw.ACTVAL = "이미지등록";

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


        public ICommand NoRecordCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommand"] = false;
                            CommandCanExecutes["NoRecordConfirmCommand"] = false;

                            // 전자서명
                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            if (_MainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _MainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
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

                            Brush background = _MainWnd.PrintArea.Background;
                            _MainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _MainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _MainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _MainWnd.CurrentInstruction.Raw.NOTE = imageToByteArrayNoRecode();
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
                            CommandResults["NoRecordConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommand") ?
                        CommandCanExecutes["NoRecordConfirmCommand"] : (CommandCanExecutes["NoRecordConfirmCommand"] = true);
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

        public byte[] imageToByteArrayNoRecode()
        {
            try
            {

                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_MainWnd.PrintArea, null));
                System.IO.Stream stream = bitmap.GetStream(ImageFormat.Png, true);

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

        public 이미지등록ViewModel()
        {
            _captureSource = new CaptureSource();
            _captureSource.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(source_CaptureImageCompleted);
        }

        private void _MainWnd_Closed(object sender, EventArgs e)
        {
            if (_captureSource != null)
            {
                _captureSource.Stop();
            }
        }

        #endregion
    }
}
