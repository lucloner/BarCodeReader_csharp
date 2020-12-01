﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace 条码识别
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                var fileName = args[0];
                var type = ZBar.SymbolType.CODE128;
                if (args.Length > 1)
                {
                    try
                    {
                        type = (ZBar.SymbolType)Convert.ToInt32(args[1]);
                    }
                    catch (Exception) { }
                }
                DateTime now = DateTime.Now;
                Image primaryImage = Image.FromFile(fileName);

                Bitmap pImg = MainForm.MakeGrayscale3((Bitmap)primaryImage);
                using (ZBar.ImageScanner scanner = new ZBar.ImageScanner())
                {
                    scanner.SetConfiguration(type, ZBar.Config.Enable, 1);

                    List<ZBar.Symbol> symbols = new List<ZBar.Symbol>();
                    symbols = scanner.Scan((Image)pImg);

                    if (symbols != null && symbols.Count > 0)
                    {
                        string result = string.Empty;
                        symbols.ForEach(s => result += "条码内容:" + s.Data + " 条码质量:" + s.Quality + Environment.NewLine);
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine("null");
                    }
                }
            }
        }
    }
}
