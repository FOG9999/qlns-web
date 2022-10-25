using FlexCel.Core;
using FlexCel.Report;
using System;

namespace VIETTEL.Application.Flexcel
{

    /// <summary>
    /// Implements a custom function that will choose the lowest member in an array.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class ToPercent : TUserDefinedFunction
    {
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "Lowest".
        /// </summary>
        public ToPercent() : base("ToPercent")
        {
        }

        /// <summary>
        /// Chooses the lowest element in an array.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="parameters">In this case we expect 1 parameter that should be an array. We will return an error otherwise.</param>
        /// <returns></returns>
        public override object Evaluate(TUdfEventArgs arguments, object[] parameters)
        {
            #region Get Parameters
            TFlxFormulaErrorValue Err;
            if (!CheckParameters(parameters, 2, out Err)) return Err;

            //The parameter should be a double or a range.
            double n1;
            if (!TryGetDouble(arguments.Xls, parameters[0], out n1, out Err)) return Err;

            double n2;
            if (!TryGetDouble(arguments.Xls, parameters[1], out n2, out Err)) return Err;

            #endregion

            if (n1 == 0)
                return string.Empty;

            if (n2 == 0)
                return string.Empty;

            if (n1 % n2 == 0)
            {
                return n1 * 100 / n2;
            }
            else
            {
                var result = System.Math.Round(n1 * 100 / n2, 2).ToString();
                return result;
                //return (n1 * 100 / n2).ToString("##,##");
            }
        }

    }


    /// <summary>
    /// Implements a custom function that will choose the lowest member in an array.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class TToPercentImpl : TFlexCelUserFunction
    {
        private ExcelFile _xls;
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "Lowest".
        /// </summary>
        public TToPercentImpl(ExcelFile xls) : base()
        {
            _xls = xls;
        }

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length != 2)
            {
                throw new ArgumentException("Kiem tra lai bien");
            }

            //The parameter should be a double or a range.
            double n1 = Convert.ToInt32(parameters[0]);

            double n2 = Convert.ToInt32(parameters[1]);

            if (n1 == 0)
                return string.Empty;

            if (n2 == 0)
                return string.Empty;

            if (n1 % n2 == 0)
            {
                return n1 * 100 / n2;
            }
            else
            {
                var result = System.Math.Round(n1 * 100 / n2, 2).ToString();
                return result;
            }
        }

    }
}
