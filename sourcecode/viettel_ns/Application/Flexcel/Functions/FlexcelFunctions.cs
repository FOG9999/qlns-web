using FlexCel.Core;
using System;

namespace VIETTEL.Application.Flexcel
{
    /// <summary>
    /// Implements a custom function that will sum the cells in a range that have the same
    /// color of the source cell. This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class SumCellsWithSameColor : TUserDefinedFunction
    {
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "SumCellsWithSameColor".
        /// </summary>
        public SumCellsWithSameColor() : base("SumCellsWithSameColor")
        {
        }

        /// <summary>
        /// Returns the sum of cells in a range that have the same color as a reference cell.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="parameters">In this case we expect 2 parameters, first the reference cell and then
        /// the range in which to sum. We will return an error otherwise.</param>
        /// <returns></returns>
        public override object Evaluate(TUdfEventArgs arguments, object[] parameters)
        {
            #region Get Parameters
            TFlxFormulaErrorValue Err;
            if (!CheckParameters(parameters, 2, out Err)) return Err;

            //The first parameter should be a range
            TXls3DRange SourceCell;
            if (!TryGetCellRange(parameters[0], out SourceCell, out Err)) return Err;

            //The second parameter should be a range too.
            TXls3DRange SumRange;
            if (!TryGetCellRange(parameters[1], out SumRange, out Err)) return Err;
            #endregion

            //Get the color in SourceCell. Note that if Source cell is a range with more than one cell,
            //we will use the first cell in the range. Also, as different colors can have the same rgb value, we will compare the actual RGB values, not the ExcelColors
            TFlxFormat fmt = arguments.Xls.GetCellVisibleFormatDef(SourceCell.Sheet1, SourceCell.Top, SourceCell.Left);
            int SourceColor = fmt.FillPattern.FgColor.ToColor(arguments.Xls).ToArgb();

            double Result = 0;
            //Loop in the sum range and sum the corresponding values.
            for (int s = SumRange.Sheet1; s <= SumRange.Sheet2; s++)
            {
                for (int r = SumRange.Top; r <= SumRange.Bottom; r++)
                {
                    for (int c = SumRange.Left; c <= SumRange.Right; c++)
                    {
                        int XF = -1;
                        object val = arguments.Xls.GetCellValue(s, r, c, ref XF);
                        if (val is double) //we will only sum numeric values.
                        {
                            TFlxFormat sumfmt = arguments.Xls.GetCellVisibleFormatDef(s, r, c);
                            if (sumfmt.FillPattern.FgColor.ToColor(arguments.Xls).ToArgb() == SourceColor)
                            {
                                Result += (double)val;
                            }
                        }
                    }
                }
            }
            return Result;
        }
    }


    /// <summary>
    /// Implements a custom function that will return true if a number is prime.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class IsPrime : TUserDefinedFunction
    {
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "IsPrime".
        /// </summary>
        public IsPrime() : base("IsPrime")
        {
        }

        /// <summary>
        /// Returns true if a number is prime.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="parameters">In this case we expect 1 parameter with the number. We will return an error otherwise.</param>
        /// <returns></returns>
        public override object Evaluate(TUdfEventArgs arguments, object[] parameters)
        {
            #region Get Parameters
            TFlxFormulaErrorValue Err;
            if (!CheckParameters(parameters, 1, out Err)) return Err;

            //The parameter should be a double or a range.
            double Number;
            if (!TryGetDouble(arguments.Xls, parameters[0], out Number, out Err)) return Err;
            #endregion

            //Return true if the number is prime.
            int n = Convert.ToInt32(Number);
            if (n == 2) return true;
            if (n < 2 || n % 2 == 0) return false;
            for (int i = 3; i <= Math.Sqrt(n); i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Implements a custom function that will choose between two different strings.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class BoolChoose : TUserDefinedFunction
    {
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "BoolChoose".
        /// </summary>
        public BoolChoose() : base("BoolChoose")
        {
        }

        /// <summary>
        /// Chooses between 2 different strings.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="parameters">In this case we expect 3 parameters: The first is a boolean, and the other 2 strings. We will return an error otherwise.</param>
        /// <returns></returns>
        public override object Evaluate(TUdfEventArgs arguments, object[] parameters)
        {
            #region Get Parameters
            TFlxFormulaErrorValue Err;
            if (!CheckParameters(parameters, 3, out Err)) return Err;

            //The first parameter should be a boolean.
            bool ChooseFirst;
            if (!TryGetBoolean(arguments.Xls, parameters[0], out ChooseFirst, out Err)) return Err;

            //The second parameter should be a string.
            string s1;
            if (!TryGetString(arguments.Xls, parameters[1], out s1, out Err)) return Err;

            //The third parameter should be a string.
            string s2;
            if (!TryGetString(arguments.Xls, parameters[2], out s2, out Err)) return Err;
            #endregion

            //Return s1 or s2 depending on ChooseFirst
            if (ChooseFirst) return s1; else return s2;
        }
    }

    /// <summary>
    /// Implements a custom function that will choose the lowest member in an array.
    /// This function mimics the VBA macro in the example, so when
    /// recalculating the sheet with FlexCel you will get the same results as with Excel.
    /// </summary>
    public class Lowest : TUserDefinedFunction
    {
        /// <summary>
        /// Creates a new instance and registers the class in the FlexCel recalculating engine as "Lowest".
        /// </summary>
        public Lowest() : base("Lowest")
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
            if (!CheckParameters(parameters, 1, out Err)) return Err;

            //The first parameter should be an array.
            object[,] SourceArray;
            if (!TryGetArray(arguments.Xls, parameters[0], out SourceArray, out Err)) return Err;
            #endregion

            double Result = 0;
            bool First = true;
            foreach (object o in SourceArray)
            {
                if (o is double)
                {
                    if (First)
                    {
                        First = false;
                        Result = (double)o;
                    }
                    else
                    {
                        if ((double)o < Result) Result = (double)o;
                    }
                }
                else return TFlxFormulaErrorValue.ErrValue;
            }

            return Result;
        }

    }

}
