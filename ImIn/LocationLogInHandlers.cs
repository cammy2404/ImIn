using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

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

            Console.WriteLine("Thread Starting");

            Cursor.Current = Cursors.WaitCursor;

            Thread accessDB = new Thread(() => {
                loc_id = CheckCredentials(username, password);
            });
            accessDB.Start();
            accessDB.Join();
            Cursor.Current = Cursors.Default;

            Console.WriteLine("Thread Complete");
            Console.WriteLine(loc_id);
            if (loc_id == "-1")
            {
                foreach (Control c in window.Controls)
                    if (c is Button || c is TextBox)
                        c.Enabled = true;
                Console.WriteLine("Failed to log in");
            } else {
                window.Controls.Clear();
                new LogInBuilder().LoadScreen(window);
                Console.WriteLine("Successfully logged in");
            }
        }

        private string CheckCredentials(string username, string password)
        {
            DBConnection db = new DBConnection();

            string com_password = ComputeSha256Hash(password);

            List<string>[] results = db.Select("select ID, Username, Password from Location");

            for (int i = 0; i < results[1].Count; i++)
                if (results[1][i].Trim() == username)
                    if (results[2][i].Trim() == com_password)
                        return results[0][i];

            return "-1";
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
