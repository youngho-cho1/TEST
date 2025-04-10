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
using System.ComponentModel;
using 보령.UserControls;
using 보령;

namespace 보령.UserControls
{
    public partial class ScaleControlDW : UserControl, INotifyPropertyChanged
    {
        public enum State { UNDER, UNDERCLB, MATCH, OVERCLB, OVER };
        public static double CURRENT_GAGE_SIZE = 0;

        public ScaleControlDW()
        {
            _color0 = new SolidColorBrush(Colors.Red);
            _color1 = new SolidColorBrush(Colors.Orange);
            _color2 = new SolidColorBrush(Colors.Green);
            _color3 = new SolidColorBrush(Colors.Yellow);
            _color4 = new SolidColorBrush(Colors.Red);

            _calibrationType = 보령.UserControls.CalibrationType.RATE;
            _targetAcceptanceRate = 0.1;
            _targetValue = 100;
            Unit = "";

            InitializeComponent();

            changeTarget();
        }

        #region Firalbe Properties

        private State _state;
        public State ScaleState
        {
            get
            {
                return _state;
            }
        }
        public event StateChangedDWHandler StateChanged;

        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value",
        typeof(double),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).Value = (double)e.NewValue;
        })));
        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                double oldValue = _value;
                _value = value;

                if (oldValue != _value)
                {
                    if (ValueChanged != null)
                        ValueChanged(this, oldValue, value);

                    changeState();
                }

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
        public event ValueChangedDWHandler ValueChanged;

        #endregion

        #region Calibration Properties

        public static DependencyProperty CalibrationTypeProperty = DependencyProperty.Register("CalibrationType",
        typeof(CalibrationType),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).CalibrationType = (CalibrationType)e.NewValue;
        })));
        private CalibrationType _calibrationType;
        public CalibrationType CalibrationType
        {
            get
            {
                return _calibrationType;
            }
            set
            {
                _calibrationType = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CalibrationType"));
            }
        }


        #region UnitProperty
        public static DependencyProperty UnitProperty = DependencyProperty.Register("Unit",
        typeof(string),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).Unit = (string)e.NewValue;
        })));
        private string _Unit;
        public string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                string oldValue = _Unit;
                _Unit = value;

                changeTarget();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Unit"));
                    //PropertyChanged(this, new PropertyChangedEventArgs("TotalValue"));
                }
            }
        }
        public static DependencyProperty BomUnitProperty = DependencyProperty.Register("BomUnit",
        typeof(string),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).BomUnit = (string)e.NewValue;
        })));
        private string _BomUnit;
        public string BomUnit
        {
            get
            {
                return _BomUnit;
            }
            set
            {
                string oldValue = _BomUnit;
                _BomUnit = value;

                changeTarget();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BomUnit"));
                    //PropertyChanged(this, new PropertyChangedEventArgs("TotalValue"));
                }
            }
        }
        #endregion


        public static DependencyProperty TargetValueProperty = DependencyProperty.Register("TargetValue",
        typeof(double),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).TargetValue = (double)e.NewValue;
        })));
        private double _targetValue;
        public double TargetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                _targetValue = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TargetValue"));
            }
        }

        public static DependencyProperty ScaleNameProperty = DependencyProperty.Register("ScaleName",
        typeof(string),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).ScaleName = (string)e.NewValue;
        })));
        private string _ScaleName;
        public string ScaleName
        {
            get
            {
                return _ScaleName;
            }
            set
            {
                _ScaleName = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ScaleName"));
            }
        }

        public static DependencyProperty PrinterNameProperty = DependencyProperty.Register("PrinterName",
      typeof(string),
      typeof(ScaleControlDW),
      new PropertyMetadata(new PropertyChangedCallback((s, e) =>
      {
          ((ScaleControlDW)s).PrinterName = (string)e.NewValue;
      })));
        private string _PrinterName;
        public string PrinterName
        {
            get
            {
                return _PrinterName;
            }
            set
            {
                _PrinterName = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PrinterName"));
            }
        }

        private double _targetAcceptanceRate;
        public double TargetAcceptanceRate
        {
            get
            {
                return _targetAcceptanceRate;
            }
            set
            {
                _targetAcceptanceRate = value;
                changeTarget();
            }
        }

        public static DependencyProperty UnderClbValueProperty = DependencyProperty.Register("UnderClbValue",
        typeof(double),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).UnderClbValue = (double)e.NewValue;
        })));
        private double _underClbValue;
        public double UnderClbValue
        {
            get
            {
                return _underClbValue;
            }
            set
            {
                _underClbValue = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UnderClbValue"));
            }
        }

        public static DependencyProperty OverClbValueProperty = DependencyProperty.Register("OverClbValue",
        typeof(double),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).OverClbValue = (double)e.NewValue;
        })));
        private double _overClbValue;
        public double OverClbValue
        {
            get
            {
                return _overClbValue;
            }
            set
            {
                _overClbValue = value;
                changeTarget();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OverClbValue"));
            }
        }

        // 게이지 중량
        public static DependencyProperty TotalValueProperty = DependencyProperty.Register("TotalValue",
        typeof(double),
        typeof(ScaleControlDW),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((ScaleControlDW)s).TotalValue = (double)e.NewValue;
        })));
        private double _TotalValue;
        public double TotalValue
        {
            get
            {
                double returnVal = 0;
                returnVal = _TotalValue;


                return returnVal;
            }
            set
            {
                _TotalValue = value;

                string strRemainQty = string.Empty;

                if (TotalValue <= 0)
                    strRemainQty = string.Format("{0,10:N3}", TargetValue).Trim();
                else
                    strRemainQty = string.Format("{0,10:N3}", (TargetValue - TotalValue)).Trim();

                this.TotalValueText.Text = string.Concat(string.Format("{0,10:N3}", _TotalValue), "/", strRemainQty, " ", Unit);

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalValue"));
            }
        }

        #endregion

        #region Shape Properties
        private Brush _color0;
        public Brush Color0
        {
            get
            {
                return _color0;
            }
            set
            {
                _color0 = value;
                if (ScaleState == State.UNDER)
                    ScaleBar.Fill = Color0;
            }
        }

        private Brush _color1;
        public Brush Color1
        {
            get
            {
                return _color1;
            }
            set
            {
                _color1 = value;
                if (ScaleState == State.UNDERCLB)
                    ScaleBar.Fill = _color1;
            }
        }

        private Brush _color2;
        public Brush Color2
        {
            get
            {
                return _color2;
            }
            set
            {
                _color2 = value;
                if (ScaleState == State.MATCH)
                    ScaleBar.Fill = _color2;
            }
        }

        private Brush _color3;
        public Brush Color3
        {
            get
            {
                return _color3;
            }
            set
            {
                _color3 = value;
                if (ScaleState == State.OVERCLB)
                    ScaleBar.Fill = _color3;
            }
        }

        private Brush _color4;
        public Brush Color4
        {
            get
            {
                return _color4;
            }
            set
            {
                _color4 = value;
                if (ScaleState == State.OVER)
                    ScaleBar.Fill = _color4;
            }
        }

        public Brush BackgroundColor
        {
            get
            {
                return BackgroundRect.Fill;
            }
            set
            {
                BackgroundRect.Fill = value;
            }
        }

        //public void AddDispenseLine()
        //{
        //    dBar.Visibility = Visibility.Visible;
        //    dBar.Margin = new Thickness(CURRENT_GAGE_SIZE, 0, 0, 0);
        //}

        //public void RemoveDispenseLine()
        //{
        //    dBar.Visibility = Visibility.Collapsed;
        //    dBar.Margin = new Thickness(0, 0, 0, 0);
        //}
        #endregion

        #region change Target
        private void changeTarget()
        {
            if (TargetValueText == null || CalibrationMinValueText == null || CalibrationMaxValueText == null)
                return;

            TargetValueText.Text = string.Concat("지시량 ", string.Format("{0,10:N3}", TargetValue).Trim(), (string.IsNullOrEmpty(BomUnit) ? "" : BomUnit));
            //this.UnitText.Text = Unit;

            if (TargetValue == 0) return;

            double uClbValue;
            double oClbValue;

            if (CalibrationType == 보령.UserControls.CalibrationType.RATE)
            {
                uClbValue = TargetValue * UnderClbValue;
                oClbValue = TargetValue * OverClbValue;
            }
            else
            {
                uClbValue = UnderClbValue;
                oClbValue = OverClbValue;
            }

            CalibrationMinValueText.Text = Math.Round((TargetValue - uClbValue), 3).ToString() + (string.IsNullOrEmpty(BomUnit) ? "" : BomUnit);
            CalibrationMaxValueText.Text = Math.Round((TargetValue + oClbValue), 3).ToString() + (string.IsNullOrEmpty(BomUnit) ? "" : BomUnit);

            ScaleNameText.Text = " " + ScaleName;
            PrnterNameText.Text = " " + PrinterName;

            string strRemainQty = string.Empty;

            if (TotalValue <= 0)
                strRemainQty = string.Format("{0,10:N3}", TargetValue).Trim();
            else
                strRemainQty = string.Format("{0,10:N3}", (TargetValue - TotalValue)).Trim();

            this.TotalValueText.Text = string.Concat(string.Format("{0,10:N3}", _TotalValue), "/", strRemainQty, " ", Unit);

            changeState();
        }
        #endregion

        #region change State
        private void changeState()
        {
            // Calibration 영역을 구함
            double uClbValue;
            double oClbValue;
            if (CalibrationType == 보령.UserControls.CalibrationType.RATE)
            {
                uClbValue = TargetValue * UnderClbValue;
                oClbValue = TargetValue * OverClbValue;
            }
            else
            {
                uClbValue = UnderClbValue;
                oClbValue = OverClbValue;
            }

            // Accept 영역을 구함
            double uAcptValue;
            double oAcptValue;
            if (TargetAcceptanceRate > 1 || TargetAcceptanceRate < 0)
            {
                uAcptValue = uClbValue * 0.1;
                oAcptValue = oClbValue * 0.1;
            }
            else
            {
                uAcptValue = uClbValue * TargetAcceptanceRate;
                oAcptValue = oClbValue * TargetAcceptanceRate;
            }

            // 실제 게이지가 차지하는 영역 계산
            if (!double.IsNaN(CalibrationGridRect.ActualWidth))
            {
                double calibrationSize = CalibrationGridRect.ActualWidth;
                double underCalibrationSize = UnderCalibrationRect.ActualWidth;
                double overCalibrationSize = OverCalibrationRect.ActualWidth;
                double calcValue = Value > 0 ? Value : 0;

                double size = 0;

                // 어느 영역에 해당하는지 계산
                State newState;
                if (Value < TargetValue - uClbValue) // Value가 Calibration 영역 아래에 있는 경우
                {
                    newState = State.UNDER;
                    double rate = Value / (TargetValue - uClbValue);
                    size = underCalibrationSize * rate;
                    ScaleBar.Fill = Color0;
                }
                else if (Value >= TargetValue - uClbValue && Value < TargetValue - uAcptValue) // Value가 Calibration 영역의 Acceptance 아래에 있는 경우
                {
                    newState = State.UNDERCLB;
                    double rate = (Value - (TargetValue - uClbValue)) / (2 * uClbValue);
                    size = underCalibrationSize + (calibrationSize * rate);
                    ScaleBar.Fill = Color1;
                }
                else if (Value >= TargetValue - uAcptValue && Value <= TargetValue) // Value가 Calibration 영역의 아래 Acceptance 안에 있는 경우
                {
                    newState = State.MATCH;
                    if (uClbValue != 0)
                    {
                        double rate = (Value - (TargetValue - uClbValue)) / (2 * uClbValue);
                        size = underCalibrationSize + (calibrationSize * rate);
                    }
                    else
                        size = underCalibrationSize + calibrationSize / 2;

                    ScaleBar.Fill = Color2;
                }
                else if (Value >= TargetValue && Value <= TargetValue + oAcptValue) // Value가 Calibration 영역의 아래 Acceptance 안에 있는 경우
                {
                    newState = State.MATCH;
                    if (uClbValue != 0)
                    {
                        double rate = (Value - (TargetValue - oClbValue)) / (2 * oClbValue);
                        size = underCalibrationSize + (calibrationSize * rate);
                    }
                    else
                        size = underCalibrationSize + calibrationSize / 2;

                    ScaleBar.Fill = Color2;
                }
                else if (Value > TargetValue + oAcptValue && Value < TargetValue + oClbValue) // Value가 Calibration 영역의 Acceptance 위에 있는 경우
                {
                    newState = State.OVERCLB;
                    double rate = (Value - (TargetValue - oClbValue)) / (2 * oClbValue);
                    size = underCalibrationSize + (calibrationSize * rate);
                    ScaleBar.Fill = Color3;
                }
                else // Value가 Calibration 영역 위에 있는 경우
                {
                    newState = State.OVER;
                    double rate = (Value - (TargetValue + oClbValue)) / ((TargetValue - oClbValue) / 3);
                    if (rate > 1)
                        rate = 1;
                    size = underCalibrationSize + calibrationSize + (overCalibrationSize * rate);
                    ScaleBar.Fill = Color4;
                }

                // ScaleBar 크기 변경
                //if (!double.IsNaN(this.ActualWidth) && this.ActualWidth != 0)
                ScaleBar.Margin = new Thickness(0, 0, this.ActualWidth - size, 0);
                CURRENT_GAGE_SIZE = size;


                // state가 변경되면 상태 변경 이벤트 발생
                if (!ScaleState.Equals(newState))
                {
                    try
                    {
                        State oldState = ScaleState;
                        _state = newState;
                        if (StateChanged != null)
                        {
                            StateChanged(this, new StateChangedDWEventArg(oldState, newState));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            changeState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            changeState();
        }
    }

    //public enum CalibrationType { RATE, CONSTANT };

    public class StateChangedDWEventArg : EventArgs
    {
        private ScaleControlDW.State _oldState;
        private ScaleControlDW.State _newState;

        public ScaleControlDW.State OldState { get { return _oldState; } }
        public ScaleControlDW.State NewState { get { return _newState; } }

        public StateChangedDWEventArg(ScaleControlDW.State oldState, ScaleControlDW.State newState)
        {
            _oldState = oldState;
            _newState = newState;
        }
    }

    public delegate void StateChangedDWHandler(ScaleControlDW sender, StateChangedDWEventArg arg);
    public delegate void ValueChangedDWHandler(ScaleControlDW sender, double oldValue, double newValue);
}