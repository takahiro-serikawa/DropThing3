using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropThing3
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;

            Application.Run(new DropForm());
        }

        private static void Application_ThreadException(object sender,
           System.Threading.ThreadExceptionEventArgs e)
        {
            DropForm.AppStatusText(System.Drawing.Color.Fuchsia, e.Exception.Message);
        }
    }
}
