using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImIn
{
    public partial class LocationLogIn : Form
    {
        public LocationLogIn()
        {
            InitializeComponent();

            new LogInBuilder().InitLoad(this);
            new LogInBuilder().LoadScreen(this, true);
        }
    }
}
