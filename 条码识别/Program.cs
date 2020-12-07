using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZXing;

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
                return;
            }
            var fileName = args[0];
            var type = ZBar.SymbolType.CODE128;
            var module = "zbar";
            if (args.Length > 1)
            {
                try
                {
                    type = (ZBar.SymbolType)Convert.ToInt32(args[1]);
                }
                catch (Exception)
                {
                    module = args[1];
                }
            }
            DateTime now = DateTime.Now;
            string result = "null";
            switch (module)
            {
                case "zbar":
                    Image primaryImage = Image.FromFile(fileName);
                    Bitmap pImg = MainForm.MakeGrayscale3((Bitmap)primaryImage);
                    using (ZBar.ImageScanner scanner = new ZBar.ImageScanner())
                    {
                        scanner.SetConfiguration(type, ZBar.Config.Enable, 1);

                        List<ZBar.Symbol> symbols = new List<ZBar.Symbol>();
                        symbols = scanner.Scan((Image)pImg);

                        if (symbols != null && symbols.Count > 0)
                        {
                            result = string.Empty;
                            symbols.ForEach(s => result += $"条码内容:{s.Data} 条码质量:{s.Quality}{Environment.NewLine}");
                        }
                    }
                    break;
                case "zxing":
                    var zx = zxing(fileName, 0);
                    result = string.Empty;
                    result += "条码内容:" + zx;
                    break;
                case "softech":
                    var softech = checkBarcode(fileName, 0);
                    result = string.Empty;
                    foreach(var s in softech)
                    {
                        result += $"条码内容:{s}\n";
                    }
                    break;
                case "softech2":
                    var softech2 = checkBarcode(fileName, 2);
                    result = string.Empty;
                    foreach (var s in softech2)
                    {
                        result += $"条码内容:{s}\n";
                    }
                    break;
            }
            Console.WriteLine(result);
#if DEBUG
            Console.ReadKey();
#endif
        }

        public static string zxing(string img, int barcode_format)
        {
            BarcodeReader reader = new BarcodeReader();
            reader.Options.CharacterSet = "UTF-8";
            //reader.Options.PossibleFormats.Add((BarcodeFormat)Enum.ToObject(typeof(BarcodeFormat),barcode_format));
            Bitmap map = new Bitmap(img);
            Result result = reader.Decode(map);
            return result.Text;
        }

        static readonly checkBarcode eng = new checkBarcode();
        public static string[] checkBarcode(string img, int mode)
        {
            Bitmap map = new Bitmap(img);
            var list = mode == 0 ? eng.ReadBarCode(map) : eng.Read(map);
            var str = new string[list.Count];
            Array.Copy(list.ToArray(), str, list.Count);
            return str;
        }
    }
}
