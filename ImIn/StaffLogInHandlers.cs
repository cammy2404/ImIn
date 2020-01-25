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


            if (staff_id != "-1")
            {
                Thread updateTimeLog = new Thread(() => {
                    LogClockIn(staff_id);
                });
                updateTimeLog.Start();
                updateTimeLog.Join();
            }


            Cursor.Current = Cursors.Default;

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

            List<string>[] results = db.Select("select ID, Name, PIN from Employee"); // where LocationID = '00000001'");

            for (int i = 0; i < results[1].Count; i++)
                if (results[2][i].Trim() == com_password)
                    return results[0][i];

            return "-1";
        }


        public void LogClockIn(string staff_id)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string date = DateTime.Now.ToString("yyyy/MM/dd");
            DBConnection db = new DBConnection();

            List<string>[] results = db.Select("select ClockIn from TimeLog where EmployeeID = " + staff_id + " order by ClockTime desc, ClockDate desc");

            bool in_out = !(bool.Parse(results[0][0]));

            db.Insert("insert into TimeLog(EmployeeID, ClockIn, ClockTime, ClockDate) values (" + staff_id + ", " + in_out + ", '" + time + "', '" + date + "')");

        }

    }
}
