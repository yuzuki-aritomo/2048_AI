using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class Program
{
    public static void Main(string[] args)
    {
        int rand_seed = args.Length > 0 ? int.Parse(args[0]) : 0;
        string log_path = args.Length > 1 ? args[1] : "";

        Tuple<int, int> fieldsize = new Tuple<int, int>(4, 4);

        GameController cont = new GameController(fieldsize, rand_seed);

        cont.Run(typeof(MainSolver), log_path: log_path);
    }
    public static int Tst(int n)
    {
        for(int i = 0; i < 10; i++)
        {
            n += i;
        }
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(n);
        }
        return n;
    }
}