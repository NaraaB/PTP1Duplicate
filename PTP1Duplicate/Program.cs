using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;


namespace PTP1Duplicate
{
    /// <summary>
    /// This is a duplicate project of INPTPZ1-20
    /// Has to be deleted later!!!
    /// </summary>
    class Program
    {
        private static int canvasSide;
        private static double xmin, ymin, xmax, ymax, xstep, ystep;
        private static Bitmap bitmapCanvas;
        private static Color[] colors;
        private static Polynome polynome, derivationOfPolynome;
        private static List<ComplexNumber> roots;
        private static int iteration, idOfRoot;
        private static ComplexNumber chosenComplex;

        private static void Initialize()
        {
            bitmapCanvas = new Bitmap(canvasSide, canvasSide);
            xstep = (xmax - xmin) / canvasSide;
            ystep = (ymax - ymin) / canvasSide;

            colors = new Color[] {Color.Red, Color.Green, Color.Blue };
            roots = new List<ComplexNumber>();
            polynome = new Polynome(new List<ComplexNumber>
            {
                new ComplexNumber { RealPart = 1},
                ComplexNumber.Zero,
                ComplexNumber.Zero,
                new ComplexNumber { RealPart = 1},
            });
            derivationOfPolynome = polynome.Derivate();
            Console.WriteLine("f(x) = " + polynome);
            Console.WriteLine("f'(x) = " + derivationOfPolynome);
        }

        static void Main(string[] args)
        {
            double[] doubleargs = new double[4];
            if (args.Length == 5)
            {            
                if (!Int32.TryParse(args[0], out canvasSide))
                {
                    Console.WriteLine("Error occured while parsing args[0] to canvasSide");
                    return;
                }                
                for (int i = 0; i < doubleargs.Length; i++)
                {
                    if (!Double.TryParse(args[i + 1], out doubleargs[i]))
                    {
                        Console.WriteLine("Error occured while parsing args[" + i + 1 + "] to doubleargs[" + i + "]");
                        return;
                    }
                }
                xmin = doubleargs[0]; xmax = doubleargs[1];
                ymin = doubleargs[2]; ymax = doubleargs[3];                  
            }
            else
            {
                Console.WriteLine("Please insert value for square shaped canvas side");
                Console.WriteLine("Length of canvas side =");
                if (!Int32.TryParse(Console.ReadLine(), out canvasSide))
                {
                    Console.WriteLine("Error occured while parsing input to canvasSide value");
                    return;
                }                
                Console.WriteLine("Please insert the Minimum and the Maximum values of x and y");
                Console.WriteLine("xmin = ");
                if (!Double.TryParse(Console.ReadLine(), out xmin))
                {
                    Console.WriteLine("Error occured while parsing input to xmin value");
                    return;
                }
                Console.WriteLine("xmax = ");
                if (!Double.TryParse(Console.ReadLine(), out xmax))
                {
                    Console.WriteLine("Error occured while parsing input to xmax value");
                    return;
                }
                Console.WriteLine("ymin = ");
                if (!Double.TryParse(Console.ReadLine(), out ymin))
                {
                    Console.WriteLine("Error occured while parsing input to ymin value");
                    return;
                }
                Console.WriteLine("ymax = ");
                if (!Double.TryParse(Console.ReadLine(), out ymax))
                {
                    Console.WriteLine("Error occured while parsing input to ymax value");
                    return;
                }
            }
            if (canvasSide > 0)
            {                
                Initialize();
                for (int xCoordinate = 0; xCoordinate < canvasSide; xCoordinate++)
                {
                    for (int yCoordinate = 0; yCoordinate < canvasSide; yCoordinate++)
                    {
                        chosenComplex = new ComplexNumber()
                        {
                            RealPart = xmin + yCoordinate * xstep,
                            ImaginaryPart = ymin + xCoordinate * ystep
                        };
                        if (chosenComplex.isComplexZero())
                        {
                            chosenComplex.RealPart = 0.0001;
                            chosenComplex.ImaginaryPart = 0.0001f;
                        }
                        FindSolutionByNewton();

                        FindIdOfRoot();

                        ColorizePixel(idOfRoot, iteration, yCoordinate, xCoordinate);
                    }
                }
                bitmapCanvas.Save("../../../INPTPZ1_output.png");
                Console.WriteLine("Output is saved as picture");
            }
            else
            {
                Console.WriteLine("Invalid input!");
                return;
            }
            Console.ReadKey();
            return;
        }

        private static void FindSolutionByNewton()
        {
            iteration = 0;
            for(int q = 0; q < 30; q++)
            {
                var diff = polynome.Evaluate(chosenComplex).Divide(derivationOfPolynome.Evaluate(chosenComplex));
                chosenComplex = chosenComplex.Subtract(diff);
                if (Math.Pow(diff.RealPart, 2) + Math.Pow(diff.ImaginaryPart, 2) >= 0.5)
                    q--;
                iteration++;
            }
        }

        private static void FindIdOfRoot()
        {
            idOfRoot = 0;
            bool isRootIdKnown = false;
            for(int w = 0; w < roots.Count; w++)
            {
                if(Math.Pow(chosenComplex.RealPart-roots[w].RealPart,2)+ Math.Pow(chosenComplex.ImaginaryPart - roots[w].ImaginaryPart, 2) <= 0.001)
                {
                    isRootIdKnown = true;
                    idOfRoot = w;
                }
            }
            if (!isRootIdKnown)
            {
                roots.Add(chosenComplex);
                idOfRoot = roots.Count;
            }
        }

        private static void ColorizePixel(int currentIdOfRoot, int currentIteration, int x, int y)
        {
            Color pxlColor = colors[currentIdOfRoot % colors.Length];
            int red = Math.Min(Math.Max(0, pxlColor.R - currentIteration * 2), 255);
            int green = Math.Min(Math.Max(0, pxlColor.G - currentIteration * 2), 255);
            int blue = Math.Min(Math.Max(0, pxlColor.B - currentIteration * 2), 255);
            pxlColor = Color.FromArgb(red, green, blue);
            bitmapCanvas.SetPixel(x, y, pxlColor);
        }
    }
}
