﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashCode.Net;

namespace CashCodeTest
{
    class Program
    {
        static int Sum = 0;

        static CashCodeBillValidator c = new CashCodeBillValidator("COM1", 9600);


        static void Main(string[] args)
        {
            try
            {
                c.BillReceived += new BillReceivedHandler(c_BillReceived);
                c.BillStacking += new BillStackingHandler(c_BillStacking);
                c.BillCassetteStatusEvent += new BillCassetteHandler(c_BillCassetteStatusEvent);
                c.BillException += new BillExceptionHandler(c_BillException);
                c.ConnectBillValidator();

                if (c.IsConnected)
                {
                    c.PowerUpBillValidator();
                    c.StartListening();

                    c.EnableBillValidator();
                    Console.ReadKey();
                    c.AcceptBill();
                    c.DisableBillValidator();
                    Console.ReadKey();
                    c.EnableBillValidator();
                    Console.ReadKey();
                    c.RejectBill();
                    c.StopListening();
                }

                c.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void c_BillCassetteStatusEvent(object Sender, BillCassetteEventArgs e)
        {
            Console.WriteLine(e.Status.ToString());
        }

        static void c_BillStacking(object Sender, BillStackedEventArgs e)
        {
            Console.WriteLine("Купюра в стеке");

            e.Hold = true; 


            //if (Sum > 100)
            //{ 
            //    e.Cancel = true;
            //    Console.WriteLine("Превышен лимит единовременной оплаты");
            //}
        }

        static void c_BillReceived(object Sender, BillReceivedEventArgs e)
        {
            if (e.Status == BillRecievedStatus.Rejected)
            {
                Console.WriteLine(e.RejectedReason);
            }
            else if (e.Status == BillRecievedStatus.Accepted)
            {
                Sum += e.Value;
                Console.WriteLine("Bill accepted! " + e.Value + " руб. Общая сумму: " + Sum.ToString());
            }
        }

        static void c_BillException(object Sender, BillExceptionEventArgs e)
        {
            Console.WriteLine(e.Message);
            c.Dispose();
        }


    }
}
