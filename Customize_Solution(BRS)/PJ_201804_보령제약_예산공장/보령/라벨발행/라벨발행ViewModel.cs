using C1.Silverlight.Chart.Extended;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LGCNS.EZMES.Common;
using common = LGCNS.iPharmMES.Common.Common;

namespace 보령
{
    public class 라벨발행ViewModel : ViewModelBase
    {
        BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        public BR_PHR_SEL_System_Printer BR_PHR_SEL_System_Printer
        {
            get { return _BR_PHR_SEL_System_Printer; }
            set
            {
                _BR_PHR_SEL_System_Printer = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_SEL_LabelImage _BR_PHR_SEL_LabelImage;
        public BR_PHR_SEL_LabelImage BR_PHR_SEL_LabelImage
        {
            get { return _BR_PHR_SEL_LabelImage; }
            set
            {
                _BR_PHR_SEL_LabelImage = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_SEL_CommonCode_Label _BR_PHR_SEL_CommonCode_Label;
        public BR_PHR_SEL_CommonCode_Label BR_PHR_SEL_CommonCode_Label
        {
            get { return _BR_PHR_SEL_CommonCode_Label; }
            set
            {
                _BR_PHR_SEL_CommonCode_Label = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_PRINT_LabelImage _BR_PHR_PRINT_LabelImage;
        public BR_PHR_PRINT_LabelImage BR_PHR_PRINT_LabelImage
        {
            get { return _BR_PHR_PRINT_LabelImage; }
            set
            {
                _BR_PHR_PRINT_LabelImage = value;
                NotifyPropertyChanged();
            }
        }


        BR_PHR_SEL_System_Printer.OUTDATA _SelectedPrinter;
        public BR_PHR_SEL_System_Printer.OUTDATA SelectedPrinter
        {
            get { return _SelectedPrinter; }
            set
            {
                _SelectedPrinter = value;

                Is_Enable_OKBtn = _SelectedPrinter != null ? true : false;

                NotifyPropertyChanged();
            }
        }

        private bool _is_Enable_OKBtn;
        public bool Is_Enable_OKBtn
        {
            get { return _is_Enable_OKBtn; }
            set
            {
                _is_Enable_OKBtn = value;
                NotifyPropertyChanged();
            }
        }

        string _roomID;
        public string RoomID
        {
            get { return _roomID; }
            set
            {
                _roomID = value;
                NotifyPropertyChanged();
            }
        }

        string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set
            {
                _roomName = value;
                NotifyPropertyChanged();
            }
        }

        
        int _copies;
        public int Copies
        {
            get { return _copies; }
            set
            {
                _copies = value;
                NotifyPropertyChanged();
            }
        }

        byte[] _label;
        public byte[] Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyPropertyChanged();
            }
        }

        bool _protrait;
        public bool Protrait
        {
            get { return _protrait; }
            set
            {
                _protrait = value;
                NotifyPropertyChanged();
            }
        }

        bool _landScape;
        public bool LandScape
        {
            get { return _landScape; }
            set
            {
                _landScape = value;
                NotifyPropertyChanged();
            }
        }

        
        
        Visibility _cboVisibility = Visibility.Collapsed;
        public Visibility cboVisibility
        {
            get { return _cboVisibility; }
            set
            {
                _cboVisibility = value;
                NotifyPropertyChanged();
            }
        }

        string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                NotifyPropertyChanged();
            }
        }

        string _selectedLabel;
        public string SelectedLabel
        {
            get { return _selectedLabel; }
            set
            {
                _selectedLabel = value;
                NotifyPropertyChanged();
            }
        }
        
        라벨발행 _mainWnd;

        string[,] labelParam;

        public ICommand LoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadCommand"] = false;
                            CommandCanExecutes["LoadCommand"] = false;

                            ///
                            if (arg != null) _mainWnd = arg as 라벨발행;

                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;
                            RoomID = AuthRepositoryViewModel.Instance.RoomID;
                            RoomName = AuthRepositoryViewModel.Instance.RoomName;

                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);


                            string labelType = string.Empty;

                            if (inputValues.Count > 0)
                               labelType = inputValues[0].Raw.ACTVAL;

                            //comboBox 조회
                            _BR_PHR_SEL_CommonCode_Label.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_CommonCode_Label.INDATA()
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                LABELTYPE = labelType
                            });

                            if (await _BR_PHR_SEL_CommonCode_Label.Execute() == false) return;

                            if (_BR_PHR_SEL_CommonCode_Label.OUTDATAs.Count > 0)
                            {
                                if (_BR_PHR_SEL_CommonCode_Label.OUTDATAs[0].IFCOMBODISPLAY == "Y")
                                {
                                    cboVisibility = Visibility.Visible;
                                }
                                else
                                {
                                    cboVisibility = Visibility.Collapsed;
                                    Path = _BR_PHR_SEL_CommonCode_Label.OUTDATAs[0].ATTRIBUTE1;
                                }
                            }


                            if (RoomID == null || RoomID.Length == 0)
                            {
                                OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "SFU1202"));
                                return;
                            }

                            //이미지조회
                            await selLabelImage(Path);
                            

                            //프린터조회
                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_System_Printer.INDATA()
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = RoomID
                            });

                            if (await _BR_PHR_SEL_System_Printer.Execute() == false) return;

                            var defaultPrinter = _BR_PHR_SEL_System_Printer.OUTDATAs.Where(o => o.ISDEFAULT == "Y").FirstOrDefault();

                            if (defaultPrinter != null)
                            {
                                SelectedPrinter = defaultPrinter;
                            }
                            ///

                            CommandResults["LoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommand") ?
                        CommandCanExecutes["LoadCommand"] : (CommandCanExecutes["LoadCommand"] = true);
                });
            }
        }

        
        public ICommand cboLabelTypeCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["cboLabelTypeCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["cboLabelTypeCommand"] = false;
                            CommandCanExecutes["cboLabelTypeCommand"] = false;

                            ///
                            if (arg != null)
                            {
                                var item = _BR_PHR_SEL_CommonCode_Label.OUTDATAs.Where(o => o.CMCODE == arg.ToString()).FirstOrDefault();

                                Path = item.ATTRIBUTE1.ToString();


                                if (RoomID == null || RoomID.Length == 0)
                                {
                                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "SFU1202"));
                                    return;
                                }

                                await selLabelImage(Path);
                            }
                            ///

                            CommandResults["cboLabelTypeCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["cboLabelTypeCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["cboLabelTypeCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("cboLabelTypeCommand") ?
                        CommandCanExecutes["cboLabelTypeCommand"] : (CommandCanExecutes["cboLabelTypeCommand"] = true);
                });
            }
        }
        

        private async Task<bool> selLabelImage(string reportPath)
        {
            if (reportPath != null && reportPath != string.Empty)
            {
                //레포트 조회
                _BR_PHR_SEL_LabelImage.INDATAs.Clear();
                _BR_PHR_SEL_LabelImage.OUTDATAs.Clear();

                _BR_PHR_SEL_LabelImage.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_LabelImage.INDATA()
                {
                    ReportPath = reportPath
                });

                if (RoomID == null || RoomID.Length == 0)
                {
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LogInInfo.LangID, "SFU1202"));
                    return false;
                }

                labelParam = new string[,] { { "EQPTID", RoomID }, { "POID", _mainWnd.CurrentOrder.OrderID } };

                for (int i = 0; i < labelParam.Rank; i++)
                {

                    _BR_PHR_SEL_LabelImage.Parameterss.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_LabelImage.Parameters()
                    {
                        ParamName = labelParam[i,0],
                        ParamValue = labelParam[i,1]
                    });
                }

                if (await _BR_PHR_SEL_LabelImage.Execute() == false) return false;

                if (_BR_PHR_SEL_LabelImage.OUTDATAs.Count > 0)
                {

                    Is_Enable_OKBtn = true;

                    Label = _BR_PHR_SEL_LabelImage.OUTDATAs[0].Label;

                    if (_BR_PHR_SEL_LabelImage.OUTDATAs[0].Landscape == true)
                    {
                        LandScape = true;
                        Protrait = false;
                    }
                    else
                    {
                        LandScape = false;
                        Protrait = true;
                    }
                }
                else
                {
                    Is_Enable_OKBtn = false;
                }
            }
        

            return true;
        }

        public ICommand SelectionCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelectionCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SelectionCommandAsync"] = false;
                            CommandCanExecutes["SelectionCommandAsync"] = false;

                            ///
                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_System_Printer.INDATA()
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = RoomID,
                            });

                            if (await _BR_PHR_SEL_System_Printer.Execute() == false) return;

                            var defaultPrinter = _BR_PHR_SEL_System_Printer.OUTDATAs.Where(o => o.ISDEFAULT == "Y").FirstOrDefault();

                            if (defaultPrinter != null)
                            {
                                SelectedPrinter = defaultPrinter;
                            }
                            ///

                            CommandResults["SelectionCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SelectionCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SelectionCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelectionCommandAsync") ?
                        CommandCanExecutes["SelectionCommandAsync"] : (CommandCanExecutes["SelectionCommandAsync"] = true);
                });
            }
        }


        
        public ICommand PrintCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PrintCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PrintCommand"] = false;
                           // CommandCanExecutes["PrintCommand"] = false;

                            ///

                            if (arg != null)
                            {

                                _mainWnd = arg as 라벨발행;

                                _BR_PHR_PRINT_LabelImage.INDATAs.Clear();

                                if (_BR_PHR_SEL_System_Printer.OUTDATAs.Where(o => o.ISDEFAULT == "Y").ToList().Count == 0)
                                    throw new Exception(string.Format(" [{0}-{1}] 룸에 기본 프린트가 설정되어 있지 않습니다. 선택해주세요.", RoomID, RoomName));


                                foreach (var item in _BR_PHR_SEL_System_Printer.OUTDATAs.Where(o => o.ISDEFAULT == "Y").ToList())
                                {

                                    for (int i = 0; i < Copies; i++)
                                    {
                                        _BR_PHR_PRINT_LabelImage.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_PRINT_LabelImage.INDATA()
                                        {
                                            Width = _BR_PHR_SEL_LabelImage.OUTDATAs[0].Width,
                                            Height = _BR_PHR_SEL_LabelImage.OUTDATAs[0].Height,
                                            UOM = _BR_PHR_SEL_LabelImage.OUTDATAs[0].UOM,
                                            Landscape = LandScape ? true : false,
                                            Label = _BR_PHR_SEL_LabelImage.OUTDATAs[0].Label,
                                            PrinterName = item.PRINTERNAME
                                        });

                                        if (await _BR_PHR_PRINT_LabelImage.Execute() == false) return;
                                    }


                                }

                                //if (await _BR_PHR_PRINT_LabelImage.Execute() == false) return;


                                Brush background = _mainWnd.Label.Background;
                                _mainWnd.Label.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                                _mainWnd.Label.BorderThickness = new System.Windows.Thickness(1);
                                _mainWnd.Label.Background = new SolidColorBrush(Colors.White);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = "Image attached";

                                if (_BR_PHR_SEL_LabelImage.OUTDATAs[0].Label is byte[])
                                {
                                    using (var ms = new System.IO.MemoryStream(_BR_PHR_SEL_LabelImage.OUTDATAs[0].Label as byte[]))
                                    {
                                        byte[] datas = _BR_PHR_SEL_LabelImage.OUTDATAs[0].Label.ToArray();

                                        _mainWnd.CurrentInstruction.Raw.NOTE = datas;
                                    }
                                }

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true, false);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            ///

                            CommandResults["PrintCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PrintCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PrintCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PrintCommand") ?
                        CommandCanExecutes["PrintCommand"] : (CommandCanExecutes["PrintCommand"] = true);
                });
            }
        }
        
     
        

        public 라벨발행ViewModel()
        {
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_PHR_SEL_LabelImage = new BR_PHR_SEL_LabelImage();
            _BR_PHR_SEL_CommonCode_Label = new BR_PHR_SEL_CommonCode_Label();
            _BR_PHR_PRINT_LabelImage = new BR_PHR_PRINT_LabelImage();

            Is_Enable_OKBtn = false;

            LandScape = false;
            Protrait = true;

            Copies = 1;
        }
    }
}
