using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImIn
{
    class StaffLogInHandlers
    {

        public void ClockUser(string password, Form window)
        {
            string staff_id = "-1";

            Console.WriteLine("User is clocking in");

            foreach (Control c in window.Controls)
                if (c is Button || c is TextBox)
                    c.Enabled = false;

            Cursor.Current = Cursors.WaitCursor;

            Thread accessDB = new Thread(() => {
                staff_id = VerifyUser(password);
            });
            accessDB.Start();
            accessDB.Join();

            Cursor.Current = Cursors.Default;

            if (staff_id != "-1")
                Console.WriteLine("User clocked in");


            foreach (Control c in window.Controls)
            {
                if (c is Button || c is TextBox)
                    c.Enabled = true;
                if (c is TextBox)
                {
                    c.ForeColor = new ColourScheme().placeholder_color;
                    c.Text = "PIN";
                }
            }
        }

        public void LogIn(string password, Form window)
        {
            if (VerifyUser(password) != "-1")
                new MainMenuBuilder().LoadScreen(window);
        }



        public string VerifyUser(string password)
        {
            DBConnection db = new DBConnection();

            string com_password = new PasswordHash().ComputeSha256Hash(password);

            List<string>[] results = db.Select("select ID, Name, PIN from Employee where LocationID = '00000001'");

            for (int i = 0; i < results[1].Count; i++)
                if (results[2][i].Trim() == com_password)
                    return results[0][i];

            return "-1";
        }

    }
}
