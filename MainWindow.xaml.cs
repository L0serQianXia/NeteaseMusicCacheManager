﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace NeteaseMusicCacheManager
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private string cachePath;

		public MainWindow()
		{
			InitializeComponent();
			if (!Directory.Exists(".\\NeteaseMusicCacheManager"))
			{
				Directory.CreateDirectory(".\\NeteaseMusicCacheManager");
			}
			labelDecryptPath.Content = Path.GetFullPath(".\\NeteaseMusicCacheManager");
			string cachePathFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NetEase\\CloudMusic\\cache_path";
			if(!File.Exists(cachePathFile))
			{
				MessageBox.Show("找不到缓存路径。", "错误：", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(-1);
			}
			cachePath = File.ReadAllText(cachePathFile, Encoding.Unicode);
			if (!Directory.Exists(cachePath))
			{
				MessageBox.Show("找不到缓存目录。", "错误：", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(-1);
			}
			labelCachePath.Content = cachePath;
		}

		// TODO: 添加缓存文件总数量
		private void BtnGetCaches_Click(object sender, RoutedEventArgs e)
		{
			string[] files = Directory.GetFiles(cachePath, "*.uc");
			for(int i = 0; i < files.Length; i++)
			{
				string filePath = files[i];
				lstCache.Items.Add(filePath);
				//TODO: 这里改成API获取歌曲名称: https://music.163.com/api/song/detail/?id={ID}&ids=%5B{ID}%5D
				//filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar));
			}
		}

		private void BtnPlay_Click(object sender, RoutedEventArgs e)
		{
			string selectedMusicPath = lstCache.SelectedItem.ToString();
			string musicLocation = DecryptCache(selectedMusicPath);
			// 调用系统默认播放器播放音乐
			Process process = new Process();
			process.StartInfo.FileName = musicLocation;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
		{
			string selectedMusicPath = lstCache.SelectedItem.ToString();
			string musicPath = DecryptCache(selectedMusicPath, labelDecryptPath.Content + "\\" + GetMusicIdFromPath(selectedMusicPath) + ".mp3");
			MessageBox.Show("提取完毕！文件存储为" + musicPath, "提示：", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		// TODO: 添加提取进度
		private void BtnDecryptAll_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.ItemCollection caches = lstCache.Items;
			for(int i = 0; i < caches.Count; i++)
			{
				string path = caches.GetItemAt(i).ToString();
				DecryptCache(path, labelDecryptPath.Content + "\\" + GetMusicIdFromPath(path) + ".mp3");
			}
			MessageBox.Show("提取完毕！文件存储于" + labelDecryptPath.Content, "提示：", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void BtnLocateDecryptFolder_Click(object sender, RoutedEventArgs e)
		{
			// 资源管理器定位到输出目录
			Process process = new Process();
			process.StartInfo.FileName = "explorer.exe";
			process.StartInfo.Arguments = (string)labelDecryptPath.Content;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private string DecryptCache(string path, string targetPath = "")
		{
			// 未提供目标路径，则创建临时文件
			if (string.IsNullOrWhiteSpace(targetPath))
			{
				// 创建临时文件，并修改后缀为MP3
				targetPath = Path.GetTempFileName();
				string newName = Path.ChangeExtension(targetPath, ".mp3");
				File.Move(targetPath, newName);
				targetPath = newName;
			}

			try
			{
				// 异或A3进行解密
				FileStream fileIn = File.OpenRead(path);
				FileStream fileOut = File.OpenWrite(targetPath);
				byte[] b = new byte[1024];
				while (fileIn.Read(b, 0, b.Length) > 0)
				{
					for (int i = 0; i < b.Length; i++)
					{
						b[i] ^= 0xA3;
					}
					fileOut.Write(b, 0, b.Length);
				}
				fileOut.Close();
				fileIn.Close();
			}
			catch(IOException e)
			{
				MessageBox.Show("引发异常：\n" + e.Message, "错误：", MessageBoxButton.OK, MessageBoxImage.Warning);
				return "";
			}
			return targetPath;
		}

		private string GetMusicIdFromPath(string fullPathStr)
		{
			string s = fullPathStr.Substring(fullPathStr.LastIndexOf("\\"));
			s = s.Replace(".uc", "");
			s = s.Substring(0, s.IndexOf("-"));
			return s;
		}
	}
}
