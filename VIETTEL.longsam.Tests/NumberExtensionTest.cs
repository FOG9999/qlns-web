// <copyright file="NumberExtensionTest.cs">Copyright ©  2017</copyright>
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Viettel.Extensions.Tests
{
    /// <summary>This class contains parameterized unit tests for NumberExtension</summary>
    [PexClass(typeof(NumberExtension))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestFixture]
    public partial class NumberExtensionTest
    {
        /// <summary>Test stub for ToStringMoney(Int64)</summary>
        //[PexMethod]
        ////[PexMethod(MaxConstraintSolverTime = 20)]
        //public string ToStringMoneyTest(long Tien)
        //{
        //    string result = NumberExtension.ToStringMoney(Tien);
        //    // TODO: add assertions to method NumberExtensionTest.ToStringMoneyTest(Int64)

        //    var numbers = new List<long>{
        //        1000,
        //        1000000,
        //        1000000000,
        //        10000000000000,
        //    };

        //    //var rd = new Random();
        //    //for (int i = 0; i < 10; i++)
        //    //{
        //    //    var n = rd.Next(100000000);
        //    //    numbers.Add((long)n);
        //    //}

        //    numbers.ForEach(n =>
        //    {
        //        var r = NumberExtension.ToStringMoney(n);
        //        Console.WriteLine(r);
        //    });

        //    return result;

        //}


        //[PexMethod]
        //public void ToStringMoneyTest()
        //{
        //    var numbers = new List<long>{
        //        1000,
        //        1000000,
        //        1000000000,
        //        10000000000000,
        //    };

        //    var rd = new Random();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        var n = rd.Next(100000000);
        //        numbers.Add((long)n);
        //    }

        //    numbers.ForEach(n =>
        //    {
        //        var r = NumberExtension.ToStringMoney(n);
        //        Console.WriteLine(r);
        //    });
        //}


        [PexMethod]
        public void ToStringMoneyTest2()
        {
            var numbers = new List<long>{
                1000,
                1000000,
                1000000000,
                10000000000000,
                104,
                14000,
                124004
            };

            //var rd = new Random();
            //for (int i = 0; i < 10; i++)
            //{
            //    var n = rd.Next(100000000);
            //    numbers.Add((long)n);
            //}

            numbers.ForEach(n =>
            {
                var r = NumberExtension.ToStringMoney(n);
                Console.WriteLine(r);
            });
        }

    }
}
