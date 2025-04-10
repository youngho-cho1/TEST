using C1.Silverlight.Chart;
using C1.Silverlight.Chart.Extended;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Input;

namespace 보령
{
    [Description("실시간 장비 데이터(Analog value) 인터페이스, 챠트 + 표 생성")]
    public partial class 장비이력데이터V1 : ShopFloorCustomWindow
    {
        #region Properties

        double _selectedFromTime = double.NaN;
        public double SelectedFromTime
        {
            get { return double.IsNaN(_selectedFromTime) ? ChartMain.View.AxisX.Min : _selectedFromTime; }
            set
            {
                if (value == _selectedFromTime) return;

                _selectedFromTime = value;
                Update();
            }
        }

        double _selectedToTime = double.NaN;
        public double SelectedToTime
        {
            get { return double.IsNaN(_selectedToTime) ? ChartMain.View.AxisX.Max : _selectedToTime; }
            set
            {
                if (value == _selectedToTime) return;

                _selectedToTime = value;
                Update();
            }
        }

        /// <summary>
        /// 더 이상 Zoom 될 수 없는 최소 시간 간격 (1분)
        /// </summary>
        double _minRangeTime = 1.0 / (365 * 24 * 60);

        Storyboard _chartZoomStoryBoard;
        bool _isZoomAnimationCompleted = true;

        ChartPanel _chartPanel = new ChartPanel();

        장비이력데이터V1ViewModel _viewModel;

        #endregion

        public 장비이력데이터V1()
        {
            InitializeComponent();

            _viewModel = new 장비이력데이터V1ViewModel();


            this.DataContext = _viewModel;

            if (Foreground is SolidColorBrush)
            {
                Color clr = ((SolidColorBrush)Foreground).Color;
                ChartMainSilder.Box.Fill = new SolidColorBrush(Color.FromArgb(32, clr.R, clr.G, clr.B));
                ChartMainSilder.Box.Stroke = new SolidColorBrush(Color.FromArgb(200, clr.R, clr.G, clr.B));
            }

            Legend.Foreground = Foreground;

            ChartZoom.MouseLeftButtonDown += ChartZoom_MouseLeftButtonDown;

            ChartZoom.View.Layers.Add(_chartPanel);

            ChartZoom.ChartType = ChartType.LineSymbols;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region 마킹
        void ChartZoom_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource == _chartPanel)
                {
                    var point = _chartPanel.GetDataCoordinates(e);
                    if (point.X >= ChartZoom.View.AxisX.ActualMin && point.X <= ChartZoom.View.AxisX.ActualMax)
                    {
                        var popup = new 장비이력데이터마킹();

                        popup.Closed += (s2, e2) =>
                        {
                            if (popup.DialogResult == true)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine(string.Format("시간 : {0}", DateTime.FromOADate(point.X)));
                                sb.AppendLine(string.Format("값 : {0}", point.Y));
                                sb.Append("내용 : ");
                                sb.Append(popup.tbMark.Text);

                                AddMarker(point.X, sb.ToString());
                            }
                        };

                        popup.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                C1.Silverlight.C1MessageBox.Show(ex.ToString());
            }
        }

        void AddMarker(double x, string text)
        {
            var dataTemplate = (DataTemplate)Resources["marker"];
            var obj = (ChartPanelObject)dataTemplate.LoadContent();

            obj.DataPoint = new Point(x, double.NaN);
            obj.Action = ChartPanelAction.LeftMouseButtonDrag;
            obj.Attach = ChartPanelAttach.DataX;

            obj.Tag = text;

            _chartPanel.Children.Add(obj);
        }

        public void AddLegendMarker(double value, string name, Color color)
        {
            var obj = new ChartPanelObject()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
            };

            var bdr = new Border()
            {
                //BorderBrush = Background = new SolidColorBrush(Colors.LightGray),
                Padding = new Thickness(1),
            };
            var tb = new TextBlock() { Foreground = new SolidColorBrush(color) };
            var bind = new Binding();
            bind.Source = obj;

            bdr.BorderThickness = new Thickness(0, 0, 0, 0); // 회색선이 출력되서 안보이게 수정
            bdr.Margin = new Thickness(0, 0, 0, 0);
            obj.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            //bind.StringFormat = "y={0:#.#}";
            bind.Path = new PropertyPath("Tag");
            obj.DataPoint = new Point(double.NaN, value);

            //tb.SetBinding(TextBlock.TextProperty, bind);
            tb.Text = name;
            bdr.Child = tb;

            bdr.IsHitTestVisible = false;

            obj.Content = bdr;

            _chartPanel.Children.Add(obj);


            //            <c1:ChartPanelObject x:Name="obj" VerticalAlignment="Center" VerticalContentAlignment="Center" HorizontalAlignment="Stretch" HorizontalContentAlignment="Stretch">
            //    <Grid>
            //        <Border BorderBrush="DarkBlue"  BorderThickness="1" Background="LightBlue"  CornerRadius="3" HorizontalAlignment="Left">
            //            <Grid>
            //                <TextBlock Foreground="Black" x:Name="tbMin" Margin="2,2,18,2" Text="{Binding ElementName=obj,Path=Tag}" />
            //            </Grid>
            //        </Border>
            //        <Rectangle Height="2" Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Fill="Blue" VerticalAlignment="Center" HorizontalAlignment="Stretch" />
            //    </Grid>
            //</c1:ChartPanelObject>
        }

        public void AddMinMarker(double value)
        {
            var dataTemplate = (DataTemplate)Resources["markerMin"];
            var obj = (ChartPanelObject)dataTemplate.LoadContent();


            obj.DataPoint = new Point(double.NaN, value);
            obj.Action = ChartPanelAction.None;
            obj.Attach = ChartPanelAttach.DataY;

            obj.Tag = value.ToString();

            _chartPanel.Children.Add(obj);
        }

        public void AddMaxMarker(double value)
        {
            var dataTemplate = (DataTemplate)Resources["markerMax"];
            var obj = (ChartPanelObject)dataTemplate.LoadContent();

            obj.DataPoint = new Point(double.NaN, value);
            obj.Action = ChartPanelAction.None;
            obj.Attach = ChartPanelAttach.DataY;

            obj.Tag = value.ToString();

            _chartPanel.Children.Add(obj);
        }

        public void AddMinMarker(string tagName, double value)
        {
            var dataTemplate = (DataTemplate)Resources["markerMin"];
            var obj = (ChartPanelObject)dataTemplate.LoadContent();


            obj.DataPoint = new Point(double.NaN, value);
            obj.Action = ChartPanelAction.None;
            obj.Attach = ChartPanelAttach.DataY;

            obj.Tag = String.Format("{0} Min: {1}", tagName, value.ToString());

            _chartPanel.Children.Add(obj);
        }

        public void AddMaxMarker(string tagName, double value)
        {
            var dataTemplate = (DataTemplate)Resources["markerMax"];
            var obj = (ChartPanelObject)dataTemplate.LoadContent();

            obj.DataPoint = new Point(double.NaN, value);
            obj.Action = ChartPanelAction.None;
            obj.Attach = ChartPanelAttach.DataY;

            obj.Tag = String.Format("{0} Max: {1}", tagName, value.ToString());

            _chartPanel.Children.Add(obj);
        }

        public void AddMouseMarker()
        {
            var vmarker = CreateMouseMarker(false);
            _chartPanel.Children.Add(vmarker);
            vmarker.Action = ChartPanelAction.MouseMove;
            vmarker.Attach = ChartPanelAttach.DataY;

            var hmarker = CreateMouseMarker(true);
            _chartPanel.Children.Add(hmarker);
            hmarker.Action = ChartPanelAction.MouseMove;
            hmarker.Attach = ChartPanelAttach.DataX;
        }

        ChartPanelObject CreateMouseMarker(bool isHorizontal)
        {
            var obj = new ChartPanelObject();

            var bdr = new Border()
            {
                BorderBrush = Background = new SolidColorBrush(Colors.Transparent),
                Padding = new Thickness(2),
            };
            var tb = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black) };
            var bind = new Binding();
            bind.Source = obj;

            if (isHorizontal)
            {
                bdr.BorderThickness = new Thickness(0, 2, 0, 0);
                bdr.Margin = new Thickness(0, -1, 0, 0);
                obj.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                bind.StringFormat = "y={0:#.#}";
                bind.Path = new PropertyPath("DataPoint.Y");
                obj.DataPoint = new Point(double.NaN, 0.5);
            }
            else
            {
                bdr.BorderThickness = new Thickness(2, 0, 0, 0);
                bdr.Margin = new Thickness(-1, 0, 0, 0);
                obj.VerticalContentAlignment = VerticalAlignment.Stretch;
                bind.StringFormat = "x={0:MM-dd HH:mm:ss}";
                bind.Path = new PropertyPath("DataPoint.X");
                bind.Converter = new Double2DateTimeConverter();
                obj.DataPoint = new Point(0.5, double.NaN);
            }

            tb.SetBinding(TextBlock.TextProperty, bind);
            bdr.Child = tb;

            bdr.IsHitTestVisible = false;

            obj.Content = bdr;

            return obj;
        }

        public void ClearMarker()
        {
            _chartPanel.Children.Clear();
        }

        private void obj_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var curXvalue = DateTime.FromOADate((sender as ChartPanelObject).DataPoint.X);
            int idx = 0;

            List<double?> ListY = ((this.ChartZoom.LegendItems[0].Item as XYDataSeries).ValuesSource as double?[]).ToList();
            List<DateTime?> ListX = (this.ChartZoom.LegendItems[0].Item as XYDataSeries).XValuesSource as List<DateTime?>;

            foreach (var item in ListX)
            {
                if (item.HasValue && item.Value == curXvalue)
                    break;
                idx++;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("시간 : {0}", curXvalue));
            sb.AppendLine(string.Format("값 : {0}", ListY[idx].HasValue ? ListY[idx].Value : 0));
            sb.Append((sender as ChartPanelObject).Tag.ToString().Substring((sender as ChartPanelObject).Tag.ToString().LastIndexOf("내용 : ")));

            (sender as ChartPanelObject).Tag = sb.ToString();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var obj = (ChartPanelObject)btn.Tag;
            _chartPanel.Children.Remove(obj);
        }
        #endregion

        #region Chart animation manage

        void Update()
        {
            if (_chartZoomStoryBoard == null)
            {
                _chartZoomStoryBoard = new Storyboard();
                _chartZoomStoryBoard.Completed += (s, e) =>
                {
                    // Perform Update
                    ChartZoom.BeginUpdate();
                    ChartZoom.View.AxisX.Min = SelectedFromTime;
                    ChartZoom.View.AxisX.Max = SelectedToTime;
                    ChartZoom.EndUpdate();

                    _isZoomAnimationCompleted = true;
                };

                _chartZoomStoryBoard.Duration = new Duration(TimeSpan.FromSeconds(01));
            }
            if (_isZoomAnimationCompleted)
            {
                _chartZoomStoryBoard.Begin();
                _isZoomAnimationCompleted = false;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // adjust margin if necessary
            {
                Thickness mars = ChartMain.View.Margin;
                double w = ChartMainSilder.ThumbWidth;
                mars.Left -= w - ChartMain.Padding.Left;
                mars.Right -= w - ChartMain.Padding.Right;
                mars.Top += ChartMain.Padding.Top;
                mars.Bottom += ChartMain.Padding.Bottom;

                ChartMainSilder.Margin = mars;
                //slc.Margin = mars;
            }

            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region Silder event handlers
        void slider_LowerValueChanged(object sender, EventArgs e)
        {
            if (ChartMain != null && ChartZoom != null)
            {
                if (ChartMainSilder.UpperValue - ChartMainSilder.LowerValue < _minRangeTime)
                    ChartMainSilder.LowerValue = ChartMainSilder.UpperValue - _minRangeTime;

                Axis axisX = ChartMain.View.AxisX;
                if (axisX.Max != axisX.Min)
                {
                    double min = axisX.Min + ChartMainSilder.LowerValue * (axisX.Max - axisX.Min);
                    SelectedFromTime = min;

                    if (!SelectedFromTime.ToString().Equals("NaN"))
                        c1sliderFrom.Text = DateTime.FromOADate(SelectedFromTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }

        void slider_UpperValueChanged(object sender, EventArgs e)
        {
            if (ChartMain != null && ChartZoom != null)
            {
                if (ChartMainSilder.UpperValue - ChartMainSilder.LowerValue < _minRangeTime)
                    ChartMainSilder.UpperValue = ChartMainSilder.LowerValue + _minRangeTime;

                Axis ax = ChartMain.View.AxisX;
                if (ax.Max != ax.Min)
                {
                    double max = ax.Min + ChartMainSilder.UpperValue * (ax.Max - ax.Min);
                    SelectedToTime = max;

                    if (!SelectedToTime.ToString().Equals("NaN"))
                        c1sliderTo.Text = DateTime.FromOADate(SelectedToTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }

        void slider_GotFocus(object sender, RoutedEventArgs e)
        {
            // prevent drawing focus rectangle
            VisualStateManager.GoToState(ChartMainSilder, "Unfocused", true);
        }
        #endregion


        public class Double2DateTimeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is double && !((double)value).Equals(double.NaN)) return DateTime.FromOADate((double)value);

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }
        }

        private void c1pkrToTime_GotFocus(object sender, RoutedEventArgs e)
        {
            var popup = new 장비이력데이터Popup();

            popup.resTime = c1pkrToTime.DateTime.HasValue ? c1pkrToTime.DateTime.Value : DateTime.Now;
            popup.Closed += (s1, e1) =>
            {
                if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                    c1pkrToTime.DateTime = Convert.ToDateTime((this.DataContext as 장비이력데이터V1ViewModel).ToDt.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
            };

            popup.Show();
        }

        private void c1pkrFromTime_GotFocus(object sender, RoutedEventArgs e)
        {
            var popup = new 장비이력데이터Popup();

            popup.resTime = c1pkrFromTime.DateTime.HasValue ? c1pkrFromTime.DateTime.Value : DateTime.Now;
            popup.Closed += (s1, e1) =>
            {
                if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                    c1pkrFromTime.DateTime = Convert.ToDateTime((this.DataContext as 장비이력데이터V1ViewModel).FromDt.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
            };

            popup.Show();
        }
    }
}