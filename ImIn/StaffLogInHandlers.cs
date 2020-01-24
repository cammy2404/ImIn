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

        public void ClockUser()
        {
            Console.WriteLine("User has clocked in");

            DBConnection db = new DBConnection();

            string com_password = ComputeSha256Hash(password);

            List<string>[] results = db.Select("select ID, Username, Password from Location");


        }

        public void LogIn(Form window)
        {
            new MainMenuBuilder().LoadScreen(window);
        }

    }
}
