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

namespace WMS
{
    public enum FromToCalendarPart
    {
        FromDatePart,
        ToDatePart
    }

    public partial class PartWashing : UserControl
    {
        public string Barcode="";

        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged_From;
        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged_To;
        
        private const string NULL_SHORT_DATE_STRING = "<yyyy-MM-dd>";
        private const string NULL_LONG_DATE_STRING = "<yyyy'년' M'월' d'일' dddd>";

        public PartWashing()
        {
            InitializeComponent();
            Main.UpdateLayout();
            Main.Focus();
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Barcode.Length > 0)
            {
                var Ds = LayoutRoot.DataContext as PartWashingViewModel;
                Ds.KeyDownCommandAsync.Execute(Barcode);
                Barcode = "";
                Main.Focus();
            }
            else
            {
                Barcode = Barcode + e.Key;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var Ds = LayoutRoot.DataContext as PartWashingViewModel;
            Ds.RemoveCommandAsync.Execute((sender as Button).DataContext);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Plugin.Focus();
            Main.Focus();

            FromPartCalendar.SelectedDatesChanged += FromPartCalendar_SelectedDatesChanged;
            ToPartCalendar.SelectedDatesChanged += ToPartCalendar_SelectedDatesChanged;
            OpenCalendarPart.Click += new RoutedEventHandler(_OpenCalendarPart_Click);
            FromPartText.GotFocus += new RoutedEventHandler(_FromPartText_GotFocus);
            ToPartText.GotFocus += new RoutedEventHandler(_ToPartText_GotFocus);
            PopupPart.Opened += new EventHandler(_PopupPart_Opened);
            PopupPart.Closed += new EventHandler(_PopupPart_Closed);

            SetDateText(FromToCalendarPart.FromDatePart, DateTime.Now.AddDays(-3), DatePickerFormat.Short);
            SetDateText(FromToCalendarPart.ToDatePart, DateTime.Now, DatePickerFormat.Short);
        }

        void ToPartCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDateText(FromToCalendarPart.ToDatePart, (DateTime?)(sender as Calendar).SelectedDate, DatePickerFormat.Short);
        }

        void FromPartCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDateText(FromToCalendarPart.FromDatePart, (DateTime?)(sender as Calendar).SelectedDate, DatePickerFormat.Short);
        }

        private void Main_Unloaded(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).EQPTTIMEROff();
        }

        private void btnDry_Click(object sender, RoutedEventArgs e)
        {
            tabWashing.IsSelected = false;
            tbDry.IsSelected = true;

            (LayoutRoot.DataContext as PartWashingViewModel).isWashEble = true;
            (LayoutRoot.DataContext as PartWashingViewModel).isDryEble = false;

            if ((LayoutRoot.DataContext as PartWashingViewModel).MPop.isLoaded)
            {
                (LayoutRoot.DataContext as PartWashingViewModel).MPop.Hide();
            }
        }

        private void btnWashing_Click(object sender, RoutedEventArgs e)
        {
            tabWashing.IsSelected = true;
            tbDry.IsSelected = false;

            (LayoutRoot.DataContext as PartWashingViewModel).isWashEble = false;
            (LayoutRoot.DataContext as PartWashingViewModel).isDryEble = true;

            if ((LayoutRoot.DataContext as PartWashingViewModel).MPop.isLoaded)
            {
                (LayoutRoot.DataContext as PartWashingViewModel).MPop.Show();
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if ((LayoutRoot.DataContext as PartWashingViewModel).SelectDryHistory.WSGUID != null)
            {
                if ((LayoutRoot.DataContext as PartWashingViewModel).SelectDryHistory.STATUS != "READY")
                {
                    DryGrid.RowDefinitions[0].Height = new GridLength(0);
                    DryGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                }
            }

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            DryGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            DryGrid.RowDefinitions[1].Height = new GridLength(0);
        }

        private void chkDry_Checked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).CheckDuplte((sender as CheckBox).DataContext, true, "DryingComplete");
        }

        private void chkDry_Unchecked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).CheckDuplte((sender as CheckBox).DataContext, false, "DryingComplete");
        }

        private void chkRew_Checked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).CheckDuplte((sender as CheckBox).DataContext, true, "Rewashing");
        }

        private void chkRew_Unchecked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).CheckDuplte((sender as CheckBox).DataContext, false, "Rewashing");
        }

        private void iPharmMESCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as PartWashingViewModel).CleanDateFrom = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            (LayoutRoot.DataContext as PartWashingViewModel).CleanDateTo = DateTime.Now.ToString("yyyy-MM-dd");
            FromPartText.IsEnabled = false;
            ToPartText.IsEnabled = false;
            OpenCalendarPart.IsEnabled = false;
        }

        private void iPharmMESCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            FromPartText.IsEnabled = true;
            ToPartText.IsEnabled = true;
            OpenCalendarPart.IsEnabled = true;
        }

        private void txtMain_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void _ToPartText_GotFocus(object sender, RoutedEventArgs e)
        {
            ToPartText.SelectAll();
        }

        private void _FromPartText_GotFocus(object sender, RoutedEventArgs e)
        {
            FromPartText.SelectAll();
        }

        private void SetDateText(FromToCalendarPart part, DateTime? date, DatePickerFormat format)
        {
            if (part == FromToCalendarPart.FromDatePart && FromPartText != null)
            {
                if (format == DatePickerFormat.Short)
                {
                    FromPartText.Text = date != null ? date.Value.ToShortDateString() : NULL_SHORT_DATE_STRING;
                }
                else if (format == DatePickerFormat.Long)
                {
                    FromPartText.Text = date != null ? date.Value.ToLongDateString() : NULL_LONG_DATE_STRING;
                }
            }
            else if (part == FromToCalendarPart.ToDatePart && ToPartText != null)
            {
                if (format == DatePickerFormat.Short)
                {
                    ToPartText.Text = date != null ? date.Value.ToShortDateString() : NULL_SHORT_DATE_STRING;
                }
                else if (format == DatePickerFormat.Long)
                {
                    ToPartText.Text = date != null ? date.Value.ToLongDateString() : NULL_LONG_DATE_STRING;
                }
            }
        }

        private void _OpenCalendarPart_Click(object sender, RoutedEventArgs e)
        {
            PopupPart.IsOpen = !PopupPart.IsOpen;
        }

        private void _PopupPart_Opened(object sender, EventArgs e)
        {
            this.Focus();

            Application.Current.RootVisual.MouseLeftButtonDown += OnRootMouseLeftButtonDown;
        }

        private void OnRootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PopupPart.IsOpen)
            {
                PopupPart.IsOpen = false;
            }
        }

        private void _PopupPart_Closed(object sender, EventArgs e)
        {
            Application.Current.RootVisual.MouseLeftButtonDown -= OnRootMouseLeftButtonDown;
        }

    }
}
