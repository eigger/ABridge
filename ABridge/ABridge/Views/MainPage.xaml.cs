using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PhotoViewerDemo;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace ABridge.Views
{
    public partial class MainPage : Page
    {
        private static int MAX_LIST_COUNT = 200;
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
            DirFileSearch(_OpenText.Text, _SearchText.Text);
        }
        private void _Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            // 처음 보여줄 폴더 설정(안해도 됨) 
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _OpenText.Text = dialog.FileName;
                Photos.Clear();
                DirFileSearch(_OpenText.Text, "");
            }


        }
        private FileInfo[] GetFilesFromFolder(string path)
        {
            string[] extensions = new[] { ".jpg", ".gif", ".png" };
            DirectoryInfo dInfo = new DirectoryInfo(path);

            FileInfo[] files = dInfo.GetFiles()
              .Where(f => extensions.Contains(f.Extension.ToLower()))
              .ToArray();
            return files;
        }

        void DirFileSearch(string path, string tag = "")
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                FileInfo[] files = GetFilesFromFolder(path);

                foreach (FileInfo file in files)
                {
                    Photo photo = new Photo(file.FullName);
                    try
                    {
                        if (photo.Metadata.tags == null)
                        {
                            if (tag.Length == 0)
                            {
                                if (Photos.Count > MAX_LIST_COUNT) break;
                                Photos.Add(photo);
                            }
                        }
                        else
                        {
                            foreach (string t in photo.Metadata.tags)
                            {
                                AddTag(t);
                            }
                            foreach (string t in photo.Metadata.tags)
                            {
                                if (tag.Length > 0 && tag == t)
                                {
                                    if (Photos.Count > MAX_LIST_COUNT) break;
                                    Photos.Add(photo);
                                    break;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }

                                                           
                }
                if (Photos.Count > MAX_LIST_COUNT) return;
                if (dirs.Length > 0)                 
                {                     
                    foreach(string dir in dirs)                     
                    {                         
                        DirFileSearch(dir, tag);                     
                    }                 
                }                             
            }             
            catch(Exception ex)             
            {                 
                Console.WriteLine(ex);                 
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

            catch (Exception except)
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
                catch (Exception except)
                {

                }

            }
        }

        private void AddTag(String tag)
        {
            foreach(TreeViewItem tv in _TreeView.Items)
            {
                if ((String)tv.Tag == tag) return;
            }
            TreeViewItem item = new TreeViewItem();
            item.Header = tag;
            item.Tag = tag;
            item.Expanded += new RoutedEventHandler(item_Expanded);
            _TreeView.Items.Add(item);
        }

        private void OnPhotoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pvWindow = new PhotoViewer { SelectedPhoto = (Photo)PhotosListBox.SelectedItem };
            pvWindow.Show();
        }

        private void EditPhoto(object sender, RoutedEventArgs e)
        {
            var pvWindow = new PhotoViewer { SelectedPhoto = (Photo)PhotosListBox.SelectedItem };
            pvWindow.Show();
        }

        private void _TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Photos.Clear();
            TreeViewItem item = (TreeViewItem)_TreeView.SelectedItem;
            DirFileSearch(_OpenText.Text, (String)item.Tag);
        }

        private void _TreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Photos.Clear();
            TreeViewItem item = (TreeViewItem)_TreeView.SelectedItem;
            DirFileSearch(_OpenText.Text, (String)item.Tag);
        }
    }
}
