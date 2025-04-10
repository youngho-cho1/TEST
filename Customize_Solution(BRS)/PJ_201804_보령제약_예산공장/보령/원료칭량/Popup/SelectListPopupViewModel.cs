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
using System.Collections.Generic;
using System.Windows.Interactivity;

using LGCNS.EZMES.ControlsLib;

namespace 보령
{
    public class SelectListPopupViewModel : ViewModelBase
    {
        private string _PopupTitle;
        public string PopupTitle
        {
            get
            {
                return _PopupTitle;
            }
            set
            {
                _PopupTitle = value;
                OnPropertyChanged("PopupTitle");
            }
        }

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        private List<ExtraInformation> _ExtraInfos;
        public List<ExtraInformation> ExtraInfos
        {
            get
            {
                return _ExtraInfos;
            }
            set
            {
                _ExtraInfos = value;
                OnPropertyChanged("ExtraInfos");
                OnPropertyChanged("ExtraText");
            }
        }

        public string ExtraText
        {
            get
            {
                string text = "";
                if (ExtraInfos != null)
                {
                    foreach (ExtraInformation info in ExtraInfos)
                    {
                        text += info.Header + " : " + info.Value + "  ";
                    }
                }
                return text;
            }
        }

        private List<LGCNS.EZMES.ControlsLib.DataColumn> _Headers;
        public List<LGCNS.EZMES.ControlsLib.DataColumn> Headers
        {
            get
            {
                return _Headers;
            }
            set
            {
                _Headers = value;
                OnPropertyChanged("Headers");
            }
        }

        private List<object> _Datas;
        public List<object> Datas
        {
            get
            {
                return _Datas;
            }
            set
            {
                _Datas = value;
                OnPropertyChanged("Datas");
                OnPropertyChanged("GirdData");
            }
        }

        public List<object> GirdData
        {
            get
            {
                return new List<object>(Datas);
            }
        }

        private bool _IsAlwaysEnableOKBtn;
        public bool IsAlwaysEnableOKBtn
        {
            get
            {
                return _IsAlwaysEnableOKBtn;
            }
            set
            {
                _IsAlwaysEnableOKBtn = value;
                OnPropertyChanged("IsAlwaysEnableOKBtn");
                OnPropertyChanged("IsEnableOKBtn");
            }
        }

        private bool _IsEnableOKBtn = false;
        public bool IsEnableOKBtn
        {
            get
            {
                if (IsAlwaysEnableOKBtn)
                    return true;
                else
                    return _IsEnableOKBtn;
            }
            set
            {
                _IsEnableOKBtn = value;
                OnPropertyChanged("IsEnableOKBtn");
            }
        }

        public object SelectedData { get; set; }

        public void Refresh()
        {
            OnPropertyChanged("GirdData");
        }

        public SelectListPopupViewModel()
        {
        }
    }

    public class ExtraInformation
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }

    public class SelectionChangedBehavior : TriggerAction<CNSDataGrid>
    {
        #region ContextProperty
        public static DependencyProperty ContextProperty = DependencyProperty.Register("Context",
        typeof(SelectListPopupViewModel),
        typeof(SelectionChangedBehavior),
        new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            try
            {
                ((SelectionChangedBehavior)s).Context = (SelectListPopupViewModel)e.NewValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        })));
        private SelectListPopupViewModel _Context;
        public SelectListPopupViewModel Context
        {
            get
            {
                return _Context;
            }
            set
            {
                _Context = value;
            }
        }
        #endregion

        protected override void Invoke(object parameter)
        {
            try
            {
                object SelectedData = AssociatedObject.SelectedItem;
                Context.SelectedData = SelectedData;
                if (SelectedData != null)
                    Context.IsEnableOKBtn = true;
                else
                    Context.IsEnableOKBtn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }

}
