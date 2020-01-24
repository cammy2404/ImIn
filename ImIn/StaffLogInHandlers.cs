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
        }

        public void LogIn(Form window)
        {
            new MainMenuBuilder().LoadScreen(window);
        }

    }
}
