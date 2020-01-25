using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImIn
{
    class LocationLogInHandlers
    {
        public void Launch(Form window, string username, string password)
        {
            string loc_id = "-1";

            foreach (Control c in window.Controls)
                if (c is Button || c is TextBox)
                    c.Enabled = false;

            Cursor.Current = Cursors.WaitCursor;

            Thread accessDB = new Thread(() => {
                loc_id = CheckCredentials(username, password);
            });
            accessDB.Start();
            accessDB.Join();
            Cursor.Current = Cursors.Default;
            if (loc_id == "-1")
            {
                foreach (Control c in window.Controls)
                    if (c is Button || c is TextBox)
                        c.Enabled = true;
            } else {
                window.Controls.Clear();
                new LogInBuilder().LoadScreen(window);
            }
        }

        private string CheckCredentials(string username, string password)
        {
            DBConnection db = new DBConnection();

            string com_password = new PasswordHash().ComputeSha256Hash(password);

            List<string>[] results = db.Select("select ID, Username, Password from Location");

            for (int i = 0; i < results[1].Count; i++)
                if (results[1][i].Trim() == username)
                    if (results[2][i].Trim() == com_password)
                        return results[0][i];

            return "-1";
        }

    }
}
