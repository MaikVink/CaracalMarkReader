using System;
using System.Collections.Generic;
using System.IO;

namespace MeanMedianSD
{
    class Program
    {
        //for the mean
        static double number;
        static int amount;

        static List<double> allnumbers = new List<double>(); //for the median
        static List<double> forMedian = new List<double>(); //for Q1
        static List<double> afterMedian = new List<double>(); //for Q2
        static readonly string textFile = @"..."; //textfile where the data gets stored, for example: @"C:\Downloads\data.txt"
        
        static void Main(string[] args)
        {
            //readfile and gather the information for the data (mostly mean and median).
            string[] lines = File.ReadAllLines(textFile);
            foreach(string line in lines)
            {
                string[] verhouding = line.Split('\t');
                if (verhouding.Length <= 1)
                    continue;
                if (verhouding[0] == "NVD" || verhouding[0] == "VR" || verhouding[0] == "V") //this data gets looked at seperately, should be ignored while calculating the rest of the data.
                    continue;
                double value = double.Parse(verhouding[0]);
                int people = Int32.Parse(verhouding[2].Trim('(', ')')); //when copying from caracal the numbers has (), this needs to be removed before turning it in a number
                if (value < 5.5)
                    continue;
                number += value * people;
                amount += people;
                for (int j = 0; j < people; j++)
                {
                    allnumbers.Add(value); //each value gets added so we can easily determine the mean.
                }

            }
            //calculating the mean
            double mean = number / amount;



            //calculating the median
            double median;
            if (allnumbers.Count % 2 == 1) //if oneven, pick the middle value
            {
                median = allnumbers[allnumbers.Count / 2];

                //ready the data for Q1 and Q3.
                for(int m = 0; m < allnumbers.Count / 2; m ++)
                {
                    forMedian.Add(allnumbers[m]);
                    afterMedian.Add(allnumbers[allnumbers.Count - 1 - m]);
                }
            }
            else //if even, pick the middle two values, put them togheter and divide them by 2
            {
                double midvalue1 = allnumbers[allnumbers.Count / 2];
                double midvalue2 = allnumbers[(allnumbers.Count / 2) - 1]; //because a list starts at index 0, you need to do -1 to get the left end of the middle
                median = (midvalue1 + midvalue2) / 2;

                //ready the data for Q1 and Q3
                for (int m = 0; m < allnumbers.Count / 2; m++)
                {
                    forMedian.Add(allnumbers[m]);
                    afterMedian.Add(allnumbers[allnumbers.Count / 2 + m]);
                }
            }



            //calculate Q1, Q3, IQR, upperWisker and Lowerwisker
            double Q1;
            double Q3;
            // the Moore & McCabe method, exclusive median
            if (forMedian.Count % 2 == 0) //if the amount of values left and right from the median are an even total, do the same thing as you did for an even median.
            {
                double midQ3value1 = forMedian[forMedian.Count / 2];
                double midQ3value2 = forMedian[(forMedian.Count / 2) - 1];
                double midQ1value1 = afterMedian[afterMedian.Count / 2];
                double midQ1value2 = afterMedian[(afterMedian.Count / 2) - 1];
                Q1 = (midQ1value1 + midQ1value2) / 2;
                Q3 = (midQ3value1 + midQ3value2) / 2;
            }
            else //if the amount of values left and right from the midan are an oneven total, do the same thing as you did for an oneven median.
            {
                Q1 = afterMedian[afterMedian.Count / 2];
                Q3 = forMedian[forMedian.Count / 2];
            }
            double IQR = Q3 - Q1;
            double lowerWisker = Q1 - 1.5 * IQR;
            double upperWisker = Q3 + 1.5 * IQR;



            //calculate the amount of outliners and the standard deviation
            double som = 0;
            int upOutliner = 0;                                             //the amount of marks that are higher than the upperWisker
            int lowOutliner = 0;                                            //the amount of marks that are lower than the lowerWisker
            int outliners;
            foreach(double mark in allnumbers)
            {
                if (mark > upperWisker)
                    upOutliner++;
                else if (mark < lowerWisker)
                    lowOutliner++;
                som += Math.Pow(mark - mean, 2);                            //sommation of the difference from the mean of every value
            }
            outliners = upOutliner + lowOutliner;
            double variance = som / (amount - 1);                           //dividing by sample size (n) - 1
            double SD = Math.Sqrt(variance);                                //taking the sqaure root




            //write down all the data
            Console.WriteLine("mean: " + mean);
            Console.WriteLine("SD: " + SD);
            Console.WriteLine("median: " + median);
            Console.WriteLine("Q1: " + Q1);
            Console.WriteLine("Q3: " + Q3);
            Console.WriteLine("IQR: " + IQR);
            Console.WriteLine("upperWisker: " + upperWisker);
            Console.WriteLine("lowerWisker " + lowerWisker);
            Console.WriteLine("amount of outliners above the upperWisker: " + upOutliner);
            Console.WriteLine("amount of outliner below the lowerWisker: " + lowOutliner);
            Console.WriteLine("total amount of outliners: " + outliners);
            Console.ReadLine();
        }
    }
}
