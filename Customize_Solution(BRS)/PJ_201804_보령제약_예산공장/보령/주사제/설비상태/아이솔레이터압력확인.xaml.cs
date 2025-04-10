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
using ShopFloorUI;
using System.ComponentModel;
using System.Collections.ObjectModel;
using static 보령.아이솔레이터압력확인ViewModel;

namespace 보령
{
    [Description("isolator 압력 확인")]
    public partial class 아이솔레이터압력확인 : ShopFloorCustomWindow
    {
        public 아이솔레이터압력확인()
        {
            InitializeComponent();

            int module_cnt;
            int grove_cnt;
            for (module_cnt = 1; module_cnt <= 5; module_cnt++)
            {
                if (module_cnt == 1)
                {
                    for (grove_cnt = 1; grove_cnt <= 12; grove_cnt++)
                    {
                        ModuleGrovePressureList.Add(new ModuleGrovePressure
                        {
                            ModuleNo = "Module " + module_cnt,
                            GloveNo = "Glove " + grove_cnt,
                            Pressure = ""
                        });
                    }
                }
                else if (module_cnt == 2 || module_cnt == 3 || module_cnt == 5)
                {
                    for (grove_cnt = 1; grove_cnt <= 4; grove_cnt++)
                    {
                        ModuleGrovePressureList.Add(new ModuleGrovePressure
                        {
                            ModuleNo = "Module " + module_cnt,
                            GloveNo = "Glove " + grove_cnt,
                            Pressure = ""
                        });
                    }
                }
                else if (module_cnt == 4)
                {
                    for (grove_cnt = 1; grove_cnt <= 2; grove_cnt++)
                    {
                        ModuleGrovePressureList.Add(new ModuleGrovePressure
                        {
                            ModuleNo = "Module " + module_cnt,
                            GloveNo = "Glove " + grove_cnt,
                            Pressure = ""
                        });
                    }
                }
            }

            ModuleGrovePressureList.Add(new ModuleGrovePressure
            {
                ModuleNo = "Module 1-5",
                GloveNo = "VHP Pressure hold test",
                Pressure = ""
            });
        }
        public ObservableCollection<ModuleGrovePressure> ModuleGrovePressureList = new ObservableCollection<ModuleGrovePressure>();
        public override string TableTypeName
        {
            get { return "TABLE,아이솔레이터압력확인"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Module_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = (sender as TextBox).Text;
                decimal chk;

                if(!string.IsNullOrWhiteSpace(text) && decimal.TryParse(text, out chk))
                {
                    if(chk >= 40)
                    {
                        (sender as TextBox).Text = "";
                        MessageBox.Show("범위를 벗어났습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void AllModule_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = (sender as TextBox).Text;
                decimal chk;

                if (!string.IsNullOrWhiteSpace(text) && decimal.TryParse(text, out chk))
                {
                    if (chk >= 25)
                    {
                        (sender as TextBox).Text = "";
                        MessageBox.Show("범위를 벗어났습니다.");
                    } 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
