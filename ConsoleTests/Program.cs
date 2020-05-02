using ConsoleTests.ComponentClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {

            #region old
            //SelectListNum selectListNum = new SelectListNum();


            //Console.WriteLine("Enter the maximum value");
            //selectListNum.MaxNum = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter the minimum value");
            //selectListNum.MinNum = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter the Skip value");
            //selectListNum.Skip = int.Parse(Console.ReadLine());

            //Console.WriteLine("The array generated is: ");
            //foreach (var item in selectListNum.NumArray)
            //{
            //    Console.Write($"{item}-");
            //}

            //int num = 90;
            //int num1 = num % 10;
            //int num2 = (num - num1) / 10;
            //int newNum = (num1 * 10) + num2;

            //var myList = new List<int[]>
            //{
            //    new int[] { 1,2,3,4,5 },
            //    new int[] { 1,2,6,5,5 },
            //    new int[] { 5,5,9,5,6 }
            //};

            //foreach (int[] numArr in myList)
            //{
            //    if(numArr.myMethod(11))
            //    {
            //        foreach (var item in numArr)
            //        {
            //            Console.Write($"{item} ");
            //        }
            //        Console.WriteLine();
            //    }
            //}

            //Console.WriteLine(); 
            #endregion

            var listNum = new List<int>();

            for (int i = 0; i < 1000; i++)
            {
                listNum.Add(i);
            }

            foreach (var item in listNum)
            {
                Console.Write($"{item} ");
            }
            Console.ReadLine();
        }


    }
}
