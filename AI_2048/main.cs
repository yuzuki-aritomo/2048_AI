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
        GameController cont1 = new GameController(fieldsize, rand_seed);
        GameController cont2 = new GameController(fieldsize, rand_seed);
        GameController cont3 = new GameController(fieldsize, rand_seed);
        GameController cont4 = new GameController(fieldsize, rand_seed);
        GameController cont5 = new GameController(fieldsize, rand_seed);
        GameController cont6 = new GameController(fieldsize, rand_seed);

        Thread thread = new Thread(new ThreadStart(() =>
        {
            cont.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread1 = new Thread(new ThreadStart(() =>
        {
            cont1.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread2 = new Thread(new ThreadStart(() =>
        {
            cont2.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread3 = new Thread(new ThreadStart(() =>
        {
            cont3.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread4 = new Thread(new ThreadStart(() =>
        {
            cont4.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread5 = new Thread(new ThreadStart(() =>
        {
            cont5.Run(typeof(MainSolver), log_path: log_path);
        }));
        Thread thread6 = new Thread(new ThreadStart(() =>
        {
            cont6.Run(typeof(MainSolver), log_path: log_path);
        }));
        thread.Start();
        //thread1.Start();
        //thread2.Start();
        //thread3.Start();
        //thread4.Start();
        //thread5.Start();
        //thread6.Start();
    }
}