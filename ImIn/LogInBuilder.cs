using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImIn
{
    using locHand = LocationLogInHandlers;
    using stfHand = StaffLogInHandlers;
    using colours = ColourScheme;
    using fonts = FontScheme;

    class LogInBuilder
    {
        private readonly static int logo_size = 250;
        private readonly static int input_width = 350;
        private readonly static int input_length = 15;
        private readonly static int but_width = input_width / 2;
        private readonly static int but_height = but_width / 3;
        private readonly static int exit_but_size = 70;

        private readonly static string placeholder_text = "Username";
        private readonly static Color placeholder_color = Color.LightGray;
        private readonly static Color text_color = Color.Black;

        private readonly static string web_link = "http://www.cammymcn.tech";
        private readonly static string web_link_caption = "Visit my website to learn more";
        private readonly static int web_link_width = 350;

        FontScheme fnts;

        /// <summary>
        /// Initial load of the form, sets to fullscreen and ensures clientsize matches
        /// </summary>
        /// <param name="window"> The form to apply the setup to </param>
        public void InitLoad(Form window)
        {
            window.WindowState = FormWindowState.Maximized;
            window.FormBorderStyle = FormBorderStyle.None;
            window.ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            window.BackColor = new colours().background;
        }


        /// <summary>
        /// Create and add all the controls to the form.  If location is false, the staff log in will be created.
        /// If location is set to true, the  location log in will be created.
        /// </summary>
        /// <param name="window"> The form to add the controls to </param>
        /// <param name="location"> Set to true if the location log in setup is required, false by default </param>
        public void LoadScreen(Form window, bool location = false)
        {
            fnts = new FontScheme(window.Width);

            // Logo in the middle of the screen
            PictureBox logo = new PictureBox
            {
                Bounds = new Rectangle((window.Width / 2) - (logo_size / 2),
                                        (window.Height / 2) - (logo_size + 25),
                                        logo_size,
                                        logo_size),
                Image = Image.FromFile(@"..\..\Images\LogoDesign02.png"),
                SizeMode = PictureBoxSizeMode.Zoom
            };


            // Label stating if the required info is for a location or a staff member
            Label HeaderLabel = new Label
            {
                Bounds = new Rectangle((window.Width / 2) - (input_width / 2),
                                        (window.Height / 2) - 10,
                                        input_width,
                                        30),
                Font = fnts.main_font,
                Text = (location) ? "Location Credentials" : "Employee Info",
                TextAlign = ContentAlignment.MiddleCenter
            };


            // Textbox for the ID of the location or the staff member
            TextBox LocationID = new TextBox
            {
                Bounds = new Rectangle((window.Width / 2) - (input_width / 2),
                                        (window.Height / 2) + HeaderLabel.Size.Height + 15,
                                        input_width,
                                        50)
            };
            GenInputFields(LocationID);
            // Add the event handlers
            LocationID.GotFocus += RemoveText;
            LocationID.LostFocus += AddText;


            // Textbox for the password for eitehr the location or the staff member
            TextBox LocationPassword = new TextBox
            {
                Bounds = new Rectangle((window.Width / 2) - (input_width / 2),
                                        LocationID.Location.Y + LocationID.Size.Height + 25,
                                        input_width,
                                        50),
                PasswordChar = "*".ToCharArray()[0]
            };
            GenInputFields(LocationPassword);
            // Add the event handlers 
            LocationPassword.GotFocus += RemoveText;
            LocationPassword.LostFocus += AddText;


            // Launch button for the location screen
            Button LaunchButton = new Button
            {
                Bounds = new Rectangle((window.Width / 2) - (but_width / 2),
                                        LocationPassword.Location.Y + LocationPassword.Size.Height + 25,
                                        but_width,
                                        but_height),
                Text = "Launch",
                Font = fnts.main_font,
                BackColor = new colours().button_background,
                FlatStyle = FlatStyle.Flat
            };
            // Add event handler using the LocationLogInHandlers file
            LaunchButton.Click += (sender, args) => { new locHand().Launch(window, LocationID.Text.ToString(), LocationPassword.Text.ToString()); };


            // Clock in button for the staff clock in screen
            Button ClockButton = new Button
            {
                Bounds = new Rectangle((window.Width / 2) - (but_width / 2),
                                        LocationPassword.Location.Y + LocationPassword.Size.Height + 25,
                                        but_width,
                                        but_height),
                Text = "Clock",
                Font = fnts.main_font,
                BackColor = new colours().button_background,
                FlatStyle = FlatStyle.Flat
            };
            // Add event handler using the StaffLogInHandlers file
            ClockButton.Click += (sender, args) => { new stfHand().SayHey(); };


            // Log in button for the staff clock in screen
            Button LogInButton = new Button
            {
                Bounds = new Rectangle((window.Width / 2) - (but_width / 4),
                                        ClockButton.Location.Y + ClockButton.Size.Height + 20,
                                        but_width / 2,
                                        but_height / 2),
                Text = "Log In",
                Font = fnts.sub_font,
                BackColor = new colours().button_background,
                FlatStyle = FlatStyle.Flat
            };
            // Add event handler using the StaffLogInHandlers file
            LogInButton.Click += (sender, args) => { new stfHand().LogIn(window); };


            // Link label to take the user to the website
            LinkLabel WebLink = new LinkLabel
            {
                Bounds = (location) ? new Rectangle((window.Width / 2) - (web_link_width / 2),
                                                    LaunchButton.Location.Y + LaunchButton.Size.Height + 25,
                                                    web_link_width,
                                                    25) 
                :
                                      new Rectangle((window.Width / 2) - (web_link_width / 2),
                                                    LogInButton.Location.Y + LogInButton.Size.Height + 25,
                                                    web_link_width,
                                                    25),
                Text = web_link_caption,
                TextAlign = ContentAlignment.MiddleCenter
            };
            // Event handler for when the link is clicked
            WebLink.LinkClicked += (sender, args) => { System.Diagnostics.Process.Start(web_link); };


            // Exit button at the top right of the screen, closes the application
            Button ExitButton = new Button
            {
                Bounds = new Rectangle(window.Width - (exit_but_size + 10), 10, exit_but_size, exit_but_size),
                Text = "Exit",
                Font = fnts.sub_font,
                BackColor = new colours().button_background,
                FlatStyle = FlatStyle.Flat
            };
            // Event handler
            ExitButton.Click += (sender, args) => { Application.Exit(); };


            // Add all the controls to the control group (the main window)
            window.Controls.Add(logo);
            window.Controls.Add(HeaderLabel);
            window.Controls.Add(LocationID);
            window.Controls.Add(LocationPassword);
            
            // Ensure that the correct buttons are added, depends on if location or staff log in
            if (location)
                window.Controls.Add(LaunchButton);
            else
            {
                window.Controls.Add(ClockButton);
                window.Controls.Add(LogInButton);
            }
    
            window.Controls.Add(WebLink);
            window.Controls.Add(ExitButton);
        }


        /// <summary>
        /// Sets up the input fields with the default values
        /// </summary>
        /// <param name="control"> The control to apply the values to </param>
        private void GenInputFields(TextBox control)
        {
            control.TextAlign = HorizontalAlignment.Center;
            control.Font = fnts.main_font;
            control.Text = placeholder_text;
            control.MaxLength = input_length;
            control.ForeColor = placeholder_color;
        }


        /// <summary>
        /// If the text is the defualt text, this will clear the text and set it to black.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveText(object sender, EventArgs e)
        {
            TextBox input = sender as TextBox;
            if (input.Text == placeholder_text)
            {
                input.Text = "";
                input.ForeColor = text_color;
            }
        }


        /// <summary>
        /// If the textbox has nothing in it, the placeholder text is applyed with the gray color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddText(object sender, EventArgs e)
        {
            TextBox input = sender as TextBox;
            if (string.IsNullOrWhiteSpace(input.Text))
            {
                input.Text = placeholder_text;
                input.ForeColor = placeholder_color;
            }
        }
        
    }
}
