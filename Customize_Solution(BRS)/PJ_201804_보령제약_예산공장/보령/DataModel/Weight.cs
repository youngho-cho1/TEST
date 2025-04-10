using LGCNS.iPharmMES.Common;
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

namespace 보령
{
    public class Weight : ViewModelBase
    {
        #region Property
        private decimal _Value = 0;
        public decimal Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("WeightString");
                    OnPropertyChanged("WeightUOMString");
                    OnPropertyChanged("WeightStringWithSeperator");
                    OnPropertyChanged("WeightUOMStringWithSeperator");
                }
            }
        }
        private string _Uom;
        public string Uom
        {
            get { return _Uom; }
            set
            {
                if (_Uom != value)
                {                    
                    _Uom = value;
                    OnPropertyChanged("Uom");
                    OnPropertyChanged("WeightString");
                    OnPropertyChanged("WeightUOMString");
                    OnPropertyChanged("WeightStringWithSeperator");
                    OnPropertyChanged("WeightUOMStringWithSeperator");
                }
            }
        }
        private int _Precision = 9;
        public int Precision
        {
            get { return _Precision; }
            set
            {
                if (_Precision != value)
                {
                    _Precision = value;
                    OnPropertyChanged("Precision");
                    OnPropertyChanged("WeightString");
                    OnPropertyChanged("WeightUOMString");
                    OnPropertyChanged("WeightStringWithSeperator");
                    OnPropertyChanged("WeightUOMStringWithSeperator");
                }

            }
        }
        #endregion
        /// <summary>
        /// 단위 미포함
        /// </summary>
        public string WeightString
        {
            get { return Value.ToString("F" + Precision); }
        }
        /// <summary>
        /// 단위 포함
        /// </summary>
        public string WeightUOMString
        {
            get { return WeightString + " " + Uom; }
        }
        /// <summary>
        /// 천단위 구분자 포함
        /// </summary>
        public string WeightStringWithSeperator
        {
            get
            {
                if (_Precision > 0)
                {
                    string val = WeightString;
                    int idx = val.IndexOf(".");

                    string intval = Convert.ToDecimal(val.Substring(0, idx)).ToString("#,0");
                    string decimalval = val.Substring(idx);

                    return intval + decimalval;
                }
                else
                    return _Value.ToString("#,0");
            }
        }
        /// <summary>
        /// 천단위 구분자, 단위 포함
        /// </summary>
        public string WeightUOMStringWithSeperator
        {
            get { return WeightStringWithSeperator + " " + Uom; }
        }
        /// <summary>
        /// 전달받은 저울값과 단위로 ScaleWeight 세팅
        /// True : 변환 성공, False : 변환 실패
        /// </summary>
        /// <param name="weightuom"></param>
        public bool SetWeight(string weightuom)
        {
            string weight;
            string uom;
            decimal result;

            if (weightuom.Contains("G") || weightuom.Contains("g"))
            {
                weight = weightuom.ToUpper().Replace("G", "");
                uom = weightuom.Replace(weight, "");
            }
            else if (weightuom.ToUpper().Contains("KG"))
            {
                weight = weightuom.ToUpper().Replace("KG", "");
                uom = weightuom.Replace(weight, "");
            }
            else
                return false;

            if (decimal.TryParse(weight, out result))
            {
                Value = result;

                int decimalPoint = weight.IndexOf(".");
                if (decimalPoint == -1)
                {
                    this.Precision = 0;
                }
                else
                {
                    this.Precision = (weight.Length - decimalPoint - 1);
                }

                Uom = uom;

                return true;
            }

            return false;
        }
        /// <summary>
        /// 전달받은 저울값과 단위로 ScaleWeight 세팅
        /// True : 변환 성공, False : 변환 실패
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="uom"></param>
        public bool SetWeight(string weight, string uom)
        {
            decimal result;
            if (decimal.TryParse(weight, out result))
            {
                Value = result;

                int decimalPoint = weight.IndexOf(".");
                if (decimalPoint == -1)
                {
                    this.Precision = 0;
                }
                else
                {
                    this.Precision = (weight.Length - decimalPoint - 1);
                }

                Uom = uom;

                return true;
            }

            return false;
        }
        /// <summary>
        /// 전달받은 저울값과 단위로 ScaleWeight 세팅
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="uom"></param>
        /// <param name="precision"></param>
        public void SetWeight(decimal weight, string uom, int precision)
        {
            Value = weight;
            Uom = uom;
            Precision = precision;
        }

        /// <summary>
        /// 무게 합산
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Weight Add(Weight weight)
        {
            return new Weight()
            {
                Value = Add(this.Value, this.Uom, weight.Value, weight.Uom),
                Uom = this.Uom,
                Precision = this.Precision
            };
        }
        /// < summary>
        /// uomA를 기준으로 더하는 함수. 
        /// 단위변환용으로 사용할 경우, weightA를 0으로 하고 원하는 단위를 uomA에 지정하면됨
        /// 예, Add(0m,"g",100m,"kg") => 100000g
        /// < /summary>
        /// < param name="weightA">< /param>
        /// < param name="uomA"> Base UOM < /param>
        /// < param name="weightB">< /param>
        /// < param name="uomB">< /param>
        /// < returns>< /returns>
        public static decimal Add(decimal weightA, string uomA, decimal weightB, string uomB)
        {
            decimal uA = GetConvertIndex(uomA);
            decimal uB = GetConvertIndex(uomB);

            if (uA == 0 || uB == 0) return weightA + weightB;

            return weightA + (weightB * (uA / uB));
        }

        /// <summary>
        /// 무게 차감
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Weight Subtract(Weight weight)
        {
            return new Weight()
            {
                Value = Subtract(this.Value, this.Uom, weight.Value, weight.Uom),
                Uom = this.Uom,
                Precision = this.Precision
            };
        }
        /// < summary>
        /// uomA를 기준으로 빼는 함수
        /// < /summary>
        /// < param name="weightA">< /param>
        /// < param name="uomA">< /param>
        /// < param name="weightB">< /param>
        /// < param name="uomB">< /param>
        /// < returns></returns>
        public static decimal Subtract(decimal weightA, string uomA, decimal weightB, string uomB)
        {
            decimal uA = GetConvertIndex(uomA);
            decimal uB = GetConvertIndex(uomB);

            if (uA == 0 || uB == 0) return weightA - weightB;

            return weightA - (weightB * (uA / uB));
        }

        static decimal GetConvertIndex(string uom)
        {
            try
            {
                switch (uom.ToUpper())
                {
                    case "KG":
                        return 1;
                    case "G":
                        return 1000;
                    case "MG":
                        return 1000000;
                }
            }
            catch { }
            return 0;
        }

        public Weight Copy()
        {
            return (Weight)this.MemberwiseClone();
        }

    }
}
