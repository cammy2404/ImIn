using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ImIn
{
    using colours = ColourScheme;
    using fonts = FontScheme;

    class MainMenuBuilder
    {
        Form input_window = null;

        FontScheme fnts;

        private readonly static int header_height = 75;

        private int menu_width = 0;
        private int menu_logo_size = 0;

        private int menu_but_width = 0;
        private int menu_but_height = 0;
        private int menu_but_spacer = 30;
        private float menu_but_size_multiplier = 0.75f;

        private readonly static int exit_but_width = 150;

        private int but_count = 0;


        public void LoadScreen(Form window)
        {
            input_window = window;

            menu_width = window.Width / 7;
            menu_logo_size = (int)(menu_width / 1.75);

            menu_but_width = menu_width - 16;
            menu_but_height = ((window.Height - (menu_logo_size + 10) - header_height) - (menu_but_spacer * 6)) / 5;

            fnts = new FontScheme(window.Width);


            window.Controls.Add(CreateHeader());
            window.Controls.Add(CreateMenu());
        }

        private Panel CreateHeader()
        {
            Panel HeaderPanel = new Panel
            {
                Bounds = new Rectangle(0, 0, input_window.Width, header_height),
                BackColor = new colours().header
            };

            Label ULabel = new Label
            {
                Bounds = new Rectangle(0, 0, 150, HeaderPanel.Height),
                Text = "User:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = new colours().header_text,
                Font = fnts.header_font
            };

            Label Username = new Label
            {
                Bounds = new Rectangle(ULabel.Location.X + ULabel.Width, 0, 500, HeaderPanel.Height),
                Text = "Cammy",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = new colours().header_text,
                Font = fnts.header_font
            };

            Button ExitButton = new Button
            {
                Bounds = new Rectangle(HeaderPanel.Width - (exit_but_width + 10),
                                       10,
                                       exit_but_width,
                                       HeaderPanel.Height - 20),
                Text = "Exit",
                Font = fnts.sub_font,
                BackColor = new colours().button_background,
                FlatStyle = FlatStyle.Flat
            };
            ExitButton.Click += (sender, args) => {
                input_window.Controls.Clear();
                new LogInBuilder().LoadScreen(input_window);
            };

            HeaderPanel.Controls.Add(ULabel);
            HeaderPanel.Controls.Add(Username);
            HeaderPanel.Controls.Add(ExitButton);
            return HeaderPanel;
        }

        private Panel CreateMenu()
        {
            Panel MenuPanel = new Panel
            {
                Bounds = new Rectangle(0, header_height, menu_width, (input_window.Height - header_height)),
                BackColor = new colours().menu_background
            };

            PictureBox logo = new PictureBox
            {
                Bounds = new Rectangle((menu_width / 2) - (menu_logo_size / 2), (MenuPanel.Height) - (menu_logo_size + 10), menu_logo_size, menu_logo_size),
                Image = Image.FromFile(@"..\..\Images\LogoDesign02-inverted.png"),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            but_count = 0;

            Button MenuTimecards = new Button();
            InitMenuButtons(MenuTimecards);
            MenuTimecards.Text = "Timecards";


            Button MenuUpcomingTimecards = new Button();
            InitMenuButtons(MenuUpcomingTimecards);
            MenuUpcomingTimecards.Text = "Upcoming Timecards";


            Button MenuNotifications = new Button();
            InitMenuButtons(MenuNotifications);
            MenuNotifications.Text = "Notifications";


            Button MenuMessages = new Button();
            InitMenuButtons(MenuMessages);
            MenuMessages.Text = "Messages";


            Button MenuBasicInfo = new Button();
            InitMenuButtons(MenuBasicInfo);
            MenuBasicInfo.Text = "Basic Information";



            MenuPanel.Controls.Add(logo);
            MenuPanel.Controls.Add(MenuTimecards);
            MenuPanel.Controls.Add(MenuUpcomingTimecards);
            MenuPanel.Controls.Add(MenuNotifications);
            MenuPanel.Controls.Add(MenuMessages);
            MenuPanel.Controls.Add(MenuBasicInfo);
            return MenuPanel;
        }

        private void InitMenuButtons(Button control)
        {
            control.Bounds = new Rectangle(8, 
                                           (menu_but_spacer * (but_count + 1)) + (menu_but_height * but_count), 
                                           menu_but_width, 
                                           (int)(menu_but_height * menu_but_size_multiplier));
            control.BackColor = new colours().button_background;
            control.Font = fnts.main_font;
            control.FlatStyle = FlatStyle.Flat;
            but_count++;
        }
    }
}
