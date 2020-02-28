using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ABridge.Photos;
using Microsoft.WindowsAPICodePack.Dialogs;
using ABridge.Core.Services;

namespace ABridge.Views
{
    public partial class MainPage : Page
    {
        private FileService _fileService = new FileService();
        private List<String> _tagList;
        public PhotoCollection Photos;
        public MainPage()
        {
            InitializeComponent();
            Photos = (PhotoCollection)(Application.Current.Resources["Photos"] as ObjectDataProvider)?.Data;
        }

        private void _Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_OpenText.Text.Length == 0) return;
            Photos.Clear();
            Photos.UpdatePhotosWithTag(_OpenText.Text, _EditText.Text);
        }
        private void _Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _OpenText.Text = dialog.FileName;
                Photos.Clear();
                Photos.UpdatePhotosWithTag(_OpenText.Text, "");
            }


        }
        private void _TagAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            String text = _EditText.Text;
            if (text.Length == 0) return;
            if (AddTag(text) == false) return;
            _tagList.Add(text);
            _fileService.Save<List<String>>(AppDomain.CurrentDomain.BaseDirectory, "Config.json", _tagList);
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)_TreeView.SelectedItem;
            if (DeleteTag((String)item.Tag) == true)
            {
                _tagList.Remove((String)item.Tag);
                _fileService.Save<List<String>>(AppDomain.CurrentDomain.BaseDirectory, "Config.json", _tagList);
            }
        }


        private void GetSubDirectories(TreeViewItem itemParent)
        {
            if (itemParent == null) return;
            if (itemParent.Items.Count != 0) return;

            try

            {
                string strPath = itemParent.Tag as string;
                DirectoryInfo dInfoParent = new DirectoryInfo(strPath);
                foreach (DirectoryInfo dInfo in dInfoParent.GetDirectories())
                {
                    TreeViewItem item = new TreeViewItem();

                    item.Header = dInfo.Name;
                    item.Tag = dInfo.FullName;
                    item.Expanded += new RoutedEventHandler(item_Expanded);
                    itemParent.Items.Add(item);
                }

            }

            catch (Exception ex)
            {


            }

        }
        void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem itemParent = sender as TreeViewItem;
            if (itemParent == null) return;

            if (itemParent.Items.Count == 0) return;

            foreach (TreeViewItem item in itemParent.Items)
            {
                GetSubDirectories(item);
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _tagList = _fileService.Read<List<String>>(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
            if (_tagList == null) _tagList = new List<string>();
            foreach(String _tag in _tagList)
            {
                AddTag(_tag);
            }
        }

        private void AddDrive()
        {
            foreach (string str in Directory.GetLogicalDrives())
            {
                try
                {
                    if (str == "C:\\") continue;
                    TreeViewItem item = new TreeViewItem();
                    item.Header = str;
                    item.Tag = str;
                    item.Expanded += new RoutedEventHandler(item_Expanded);
                    _TreeView.Items.Add(item);
                    GetSubDirectories(item);
                }
                catch (Exception ex)
                {

                }

            }
        }

        private bool AddTag(String tag)
        {
            foreach(TreeViewItem tv in _TreeView.Items)
            {
                if ((String)tv.Tag == tag) return false;
            }
            TreeViewItem item = new TreeViewItem();
            item.Header = tag;
            item.Tag = tag;
            item.Expanded += new RoutedEventHandler(item_Expanded);
            _TreeView.Items.Add(item);
            return true;
        }

        private bool DeleteTag(String tag)
        {
            foreach (TreeViewItem tv in _TreeView.Items)
            {
                if ((String)tv.Tag == tag)
                {
                    _TreeView.Items.Remove(tv.Tag);
                    return true;
                }
            }
            return false;
        }

        private void OnPhotoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Photo photo = (Photo)PhotosListBox.SelectedItem;
            System.Diagnostics.Process.Start("explorer.exe", photo.Source);
        }

        private void EditPhoto(object sender, RoutedEventArgs e)
        {
            var pvWindow = new PhotoViewer { SelectedPhoto = (Photo)PhotosListBox.SelectedItem };
            var location = PhotosListBox.PointToScreen(new Point(0, 0));
            pvWindow.Left = location.X;
            pvWindow.Top = location.Y;
            pvWindow.Show();
        }

        private void _TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Photos.Clear();
            TreeViewItem item = (TreeViewItem)_TreeView.SelectedItem;
            Photos.UpdatePhotosWithTag(_OpenText.Text, (String)item.Tag);
        }

        private void _TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Photos.Clear();
            TreeViewItem item = (TreeViewItem)_TreeView.SelectedItem;
            Photos.UpdatePhotosWithTag(_OpenText.Text, (String)item.Tag);
        }
    }
}
