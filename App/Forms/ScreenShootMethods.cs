using ScreenShoot.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ScreenShoot
{
    public partial class ScreenShootForm
    {
        private void BrowseFolder()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                pathTextBox.Text = folderBrowserDialog.SelectedPath;
            else
                pathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        private void OpenFolder()
        {
            try
            {
                if (!string.IsNullOrEmpty(pathTextBox.Text) && Directory.Exists(pathTextBox.Text))
                    Process.Start(pathTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Can't open folder . .");
            }
        }

        private void OpenInEditor()
        {
            ListViewItem item;
            if ((item = listView.FirstSelectedItem()) != null && item.HasValidFileName())
            {
                Process paintProcess = Process.Start("mspaint", item.FilePath());
                paintProcess.WaitForExit();
                RefreshImages(item);
            }
        }

        private void RefreshImages(ListViewItem item)
        {
            if (item.HasValidFileName())
            {
                imagesList.Images[item.Index] = item.FilePath().PathToImage();
                listView.Refresh();
            }
        }

        private void DoClosingTaks()
        {
            win32.RemoveListener();
        }
        private void clearAll()
        {
            listView.Clear();
            imagesList.Images.Clear();
        }
    }
}
