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
using System.Collections.ObjectModel;
using System.Linq;
using C1.Silverlight.Data;
using ShopFloorUI;

namespace 보령
{
    public class 중량선별기범위ViewModel : ViewModelBase
    {
        #region [Property]
        private 중량선별기범위 _mainWnd;

        private string _curWeight;
        public string curWeight
        {
            get { return _curWeight; }
            set
            {
                _curWeight = value;
                OnPropertyChanged("curWeight");
            }
        }
        private decimal _minWeight;
        public decimal minWeight
        {
            get { return _minWeight; }
            set
            {
                _minWeight = value;
                OnPropertyChanged("minWeight");
            }
        }
        private decimal _avgWeight;
        public decimal avgWeight
        {
            get { return _avgWeight; }
            set
            {
                _avgWeight = value;
                OnPropertyChanged("avgWeight");
            }
        }
        private decimal _maxWeight;
        public decimal maxWeight
        {
            get { return _maxWeight; }
            set
            {
                _maxWeight = value;
                OnPropertyChanged("maxWeight");
            }
        }

        private ObservableCollection<CartonWeight> _CartonWeightList;
        public ObservableCollection<CartonWeight> CartonWeightList
        {
            get { return _CartonWeightList; }
            set
            {
                _CartonWeightList = value;
                OnPropertyChanged("CartonWeightList");
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
                return new CommandBase(arg =>
                {
                        try
                        {
                            CommandCanExecutes["LoadedCommand"] = false;
                            CommandResults["LoadedCommand"] = false;

                            ///

                            if (arg != null && arg is 중량선별기범위)
                            {
                                _mainWnd = arg as 중량선별기범위;
                            }

                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommand"] = true;
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
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            CommandResults["ConfirmCommandAsync"] = false;

                            ///
                            if (_CartonWeightList != null && _CartonWeightList.Count > 0)
                            {
                                var authHelper = new iPharmAuthCommandHelper();

                                if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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
                                    "중량선별기범위",
                                    "중량선별기범위",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("SEQ"));
                                dt.Columns.Add(new DataColumn("WEIGHT"));
                                dt.Columns.Add(new DataColumn("MINWEIGHT"));
                                dt.Columns.Add(new DataColumn("AVGWEIGHT"));
                                dt.Columns.Add(new DataColumn("MAXWEIGHT"));

                                foreach (var item in _CartonWeightList)
                                {
                                    var row = dt.NewRow();
                                    row["SEQ"] = item.SEQ.ToString();
                                    row["WEIGHT"] = item.WEIGHT.ToString("0.##0g");
                                    row["MINWEIGHT"] = minWeight.ToString("0.##0g");
                                    row["AVGWEIGHT"] = avgWeight.ToString("0.##0g");
                                    row["MAXWEIGHT"] = maxWeight.ToString("0.##0g");

                                    dt.Rows.Add(row);
                                }

                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);


                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
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
        #region [Constructor]
        public 중량선별기범위ViewModel()
        {
            _CartonWeightList = new ObservableCollection<CartonWeight>();
        }
        #endregion
        #region [User Define]
        public class CartonWeight : ViewModelBase
        {
            private bool _CHK;
            public bool CHK
            {
                get { return _CHK; }
                set
                {
                    _CHK = value;
                    OnPropertyChanged("CHK");
                }
            }

            private int _SEQ;
            public int SEQ
            {
                get { return _SEQ; }
                set
                {
                    _SEQ = value;
                    OnPropertyChanged("SEQ");
                }
            }

            private decimal _WEIGHT;
            public decimal WEIGHT
            {
                get { return _WEIGHT; }
                set
                {
                    _WEIGHT = value;
                    OnPropertyChanged("WEIGHT");
                }
            }
        }

        public void AddCartonWeight(decimal target)
        {
            int inx = _CartonWeightList.Count;

            _CartonWeightList.Add(new CartonWeight
            {
                CHK = false,
                SEQ = inx+1,
                WEIGHT = target
            });

            CalSummaryData();
        }
        public void DelCartonWeight()
        {
            int inx = 1;
            var temp = _CartonWeightList.Where(x => x.CHK == false);
            var tempList = new ObservableCollection<CartonWeight>();

            foreach (var item in temp)
            {
                tempList.Add(new CartonWeight
                {
                    CHK = false,
                    SEQ = inx++,
                    WEIGHT = item.WEIGHT
                });
            }

            _CartonWeightList = null;
            CartonWeightList = tempList;

            CalSummaryData();
        }
        public void CalSummaryData()
        {
            if (_CartonWeightList.Count > 0)
            {
                minWeight = _CartonWeightList.Min(x => x.WEIGHT);
                avgWeight = _CartonWeightList.Average(x => x.WEIGHT);
                maxWeight = _CartonWeightList.Max(x => x.WEIGHT);
            }
            else
            {
                minWeight = 0m;
                avgWeight = 0m;
                maxWeight = 0m;
            }
        }
        public void RowCheck(object obj)
        {
            if (obj != null)
                (obj as CartonWeight).CHK = true;
        }
        public void AllCheck(bool res)
        {
            foreach (var item in CartonWeightList)
            {
                item.CHK = res;
            }
        }
        #endregion
    }
}
