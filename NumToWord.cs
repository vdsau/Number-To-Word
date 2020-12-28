using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Test
{
    public class NumToWord
    {

        #region Fields

        private int _numberRank;
        private int _integralPart;
        private string _textNumber;
        private decimal _number;
        private string[] _unitsMap;
        private string[] _tensMap;
        private string[] _higherRankMap;
        #endregion

        #region Properties

        public string TextNumber
        {
            get
            {
                //if (!String.IsNullOrEmpty(_textNumber) && _textNumber.Length > 1)
                //    return char.ToUpper(_textNumber.First()) + _textNumber.Substring(1).ToLower();
                //else
                return _textNumber;
            }
            set { _textNumber = value; }
        }

        public decimal Number
        {
            get => _number;
            set
            {
                _number = value;
                IsLessThenNull = (value > 0) ? false : true;
                CalcNumberRank();
                ConvertNumberToText();
            }
        }

        // after the point
        public int FractionPart
        {
            get
            {
                if (Number == 0)
                    return 0;
                else
                    return Int32.Parse(Number.ToString().Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)[1]);
            }
        }

        // before the point
        public int IntegralPart
        {
            get => Math.Abs(Convert.ToInt32(Math.Truncate(Number)));
            private set => _integralPart = value;
        }

        public int NumberRank
        {
            get => _numberRank;
            private set => _numberRank = value;
        }

        public bool IsLessThenNull
        { get; private set; }

        #endregion

        #region Constructors

        public NumToWord(decimal number)
        {
            this.InitializeProperties();
            Number = number;
        }

        #endregion

        #region Methods

        private void InitializeProperties()
        {
            _unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            _tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            _higherRankMap = new[] { "hundred", "thousand", "milliion", "billion", "trillion", "quadrillion" };
        }

        public override string ToString() => TextNumber;

        private void CalcNumberRank()
        {
            var number = IntegralPart;

            while (number > 0)
            {
                number /= 10;
                NumberRank++;
            }
        }

        private void ConvertNumberToText()
        {
            if (IntegralPart == 0)
            {
                TextNumber = "zero";
                return;
            }

            try
            {
                string temp = (IsLessThenNull) ? "minus " : "";

                var ranks = IntegralPart.ToString("#,#", CultureInfo.InvariantCulture).Split(',');

                for (int rank = 0, highRank = ranks.Length - 1; rank < ranks.Length; rank++, highRank--)
                {
                    string hgRankTemp = (highRank != 0) ? _higherRankMap[highRank] : "";

                    temp += $"{DefineRankText(ranks[rank])} {hgRankTemp} ";
                }

                TextNumber = temp.TrimEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex?.Message}");
            }

        }

        private string DefineRankText(string number)
        {
            string result = "";

            if (!String.IsNullOrEmpty(number))
            {
                if (number.Length == 3)
                {
                    int hundred = int.Parse(number[0].ToString());
                    if (hundred != 0)
                    {
                        result = $"{_unitsMap[hundred]} {_higherRankMap[0]} ";
                    }
                    result += DefineTens($"{number.Substring(1, 2)}");
                }
                if (number.Length == 2)
                {
                    result = DefineTens(number);
                }
                if (number.Length == 1)
                {
                    int index = int.Parse(number[0].ToString());
                    result = _unitsMap[index];
                }
            }

            return result;
        }

        private string DefineTens(string number)
        {
            string result = "";
            if (!String.IsNullOrEmpty(number))
            {
                int tens = int.Parse(number),
                    secondNum = int.Parse(number[1].ToString());
                if (tens == 0)
                {
                    // if number == 00 or == 0
                    ;
                }
                else
                {
                    if (tens < 20)
                    {
                        // all numbers from 0 to 19
                        result = _unitsMap[tens];
                    }
                    else
                    {
                        int firstNum = int.Parse(number[0].ToString());
                        if (secondNum == 0 && firstNum != 0)
                        {
                            // for example, 30
                            result = _tensMap[firstNum];
                        }
                        else
                        {
                            // single number such as 5
                            result = $"{_tensMap[firstNum]}-{_unitsMap[secondNum]}";
                        }
                    }
                }

            }
            return result;
        }

        #endregion

    }
}
