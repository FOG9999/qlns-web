using FlexCel.Core;
using FlexCel.Report;
using System;
using System.Linq;
using Viettel.Extensions;

namespace VIETTEL.Application.Flexcel.Functions
{
    public class ToStringMoneyFunction : TUserDefinedFunction
    {

        /// <summary>
        /// Doc so tien
        /// </summary>
        public ToStringMoneyFunction() : base("ToStringMoney")
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
            //if (!CheckParameters(parameters, 1, out Err)) return Err;

            //The parameter should be a double or a range.
            double n1;
            if (!TryGetDouble(arguments.Xls, parameters[0], out n1, out Err)) return Err;

            string tienTo = null;
            if (parameters.Count() == 2)
            {
                if (!TryGetString(arguments.Xls, parameters[1], out tienTo, out Err)) return Err;
            }

            #endregion

            return string.IsNullOrWhiteSpace(tienTo) || n1 > 0 ? n1.ToStringMoney() : tienTo + n1.ToStringMoney().ToLower();
        }
    }



    /// <summary>
    /// Implements a custom function that will choose the lowest member in an array.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class TToStringMoneyImpl : TFlexCelUserFunction
    {
        private ExcelFile _xls;
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "Lowest".
        /// </summary>
        public TToStringMoneyImpl(ExcelFile xls) : base()
        {
            _xls = xls;
        }

        public override object Evaluate(object[] parameters)
        {
            //if (parameters == null || parameters.Length != 2)
            //{
            //    throw new ArgumentException("Kiem tra lai bien");
            //}

            //The parameter should be a double or a range.
            double n1 = Convert.ToDouble(parameters[0]);

            string tienTo = null;
            if (parameters.Count() == 2)
            {
                //if (!this.(arguments.Xls, parameters[1], out tienTo, out Err)) return Err;
                tienTo = parameters[1].ToString();
            }
            return string.IsNullOrWhiteSpace(tienTo) || n1 > 0 ? n1.ToStringMoney() : tienTo + n1.ToStringMoney().ToLower();
        }

    }
}
