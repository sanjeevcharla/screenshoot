using System;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using ScreenShoot.Util;
using static ScreenShoot.Util.Constants;

namespace ScreenShoot
{
    public partial class ScreenShootForm : Form
    {
        Win32 win32;
        string picturesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        SoundPlayer clickPlayer = new SoundPlayer(Properties.Resources.click);
        ImageList imagesList = new ImageList();


        public ScreenShootForm()
        {
            InitializeComponent();
            pathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            listView.LargeImageList = imagesList;
            listView.LargeImageList.ImageSize = new Size(IMAGE_SIZE, IMAGE_SIZE);
            listView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

            win32 = new Win32();
            win32.KeyDown += new KeyEventHandler(PrntScreenDown);
            win32.AddListener();

            SystemEvents.SessionEnding += new SessionEndingEventHandler((sender, e) => Application.Exit());

            this.Height = SystemInformation.VirtualScreen.Height - 40;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
        }

        private void CaptureScreen()
        {
            string name = String.IsNullOrEmpty(namesTextBox.Text)
                ? DateTime.Now.NameFormat()
                : namesTextBox.Text + "_" + (listView.Items.Count + 1);
            string fileName = Path.Combine(pathTextBox.Text, name + "." + "jpg");

            Image currentImage = Win32.ScreenShot();
            currentImage.Save(fileName);

            imagesList.Images.Add(fileName.PathToImage());
            ListViewItem imageItem = new ListViewItem(name);
            imageItem.Tag = fileName;
            imageItem.ImageIndex = imagesList.Images.Count - 1;
            listView.Items.Add(imageItem);
            clickPlayer.Play();
        }

        #region Events
        private void browseButton_Click(object sender, EventArgs e)
        {
            BrowseFolder();
        }

        void PrntScreenDown(object sender, KeyEventArgs e)
        {
            CaptureScreen();
        }

        private void ScreenShootForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Clearbutton_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenInEditor();
        }

        private void ScreenShootForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoClosingTaks();
        }
        #endregion

    }
}
