﻿using System;
using Treehopper;
using System.Threading;
namespace BlinkEventDriven
{
    class Program
    {
        static TreehopperUSB Board;
        static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            /// You may be tempted to sit in a while() loop and use Thread.Sleep() for delays,
            /// but this will block the thread that's listening for board changes. Instead,
            /// use a timer running on the application's main thread to blink the LED.
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += timer_Elapsed;

            Console.Write("Waiting for board to be connected...");

            TreehopperUSB.BoardAdded += TreehopperUSB_BoardAdded;
            TreehopperUSB.BoardRemoved += TreehopperUSB_BoardRemoved;
            Thread.Sleep(-1); // Wait here forever.
        }

        // This is called when a board is plugged into the computer.
        static void TreehopperUSB_BoardAdded(TreehopperManager sender, TreehopperUSB board)
        {
            Board = board;
            Console.WriteLine("Board found:");
            Console.WriteLine(board.Description);
            Board.Open();
            timer.Start();
        }


        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Board.Pin1.ToggleOutput();
        }


        static void TreehopperUSB_BoardRemoved(TreehopperManager sender, TreehopperUSB board)
        {
            Board.Close();
            timer.Stop();
        }

    }
}
