﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class ChatView : Window
	{
		private MatchModel _match;

		private string _dir;

		public ChatView()
		{
			InitializeComponent();
		}

		public ChatView(MatchModel match, string dir)
		{
			InitializeComponent();
			_match = match;
			_dir = dir;
			var viewModel = DataContext as ChatViewModel;
			viewModel.Match = match;
			viewModel.MyChatView = this;

			int scrollTo = Chat_ListView.Items.Count - 1;
			if (scrollTo > 0)
				Chat_ListView.ScrollIntoView(Chat_ListView.Items[scrollTo]);


			viewModel.NewChatMessageReceived += FlashWindow;
			Loaded += (sender, e) => SendMessage_TextBox.Focus();


			// Close window with ESC
			PreviewKeyDown += (object sender, KeyEventArgs e) =>
			{
				if (e.Key == Key.Escape)
					Close();
			};
		}
		
		/// <summary>
		/// Scrolls to bottom and updates items
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Update()
		{
			Chat_ListView.Items.Refresh();

			int scrollTo = Chat_ListView.Items.Count - 1;
			if (scrollTo > 0)
				Chat_ListView.ScrollIntoView(Chat_ListView.Items[scrollTo]);

		}

		/// <summary>
		/// Opens profile on header click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HeaderBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var matchProfileWindow = new MatchProfileView(_match, _dir);
			matchProfileWindow.Show();
		}

		/// <summary>
		/// Starts flashing window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FlashWindow(object sender, EventArgs e)
		{
			// If is active, flashes only once
			if (IsActive)
				WindowFlasher.Flash(this, 1);
			else
				WindowFlasher.Start(this);
		}
		
		/// <summary>
		/// Stops flashing window when it is activated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Activated(object sender, EventArgs e)
		{
			WindowFlasher.Stop(this);
		}

		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			if (_match == null || _match.Person == null || _match.Person.Photos == null
				|| _match.Person.Photos.Count == 0)
				return;

			BitmapImage b = new BitmapImage();
			Image img = sender as Image;
				
			var src = SerializationHelper.WorkingDir + _dir
				+ _match + "\\" + SerializationHelper.PHOTOS + _match.Person.Photos[0].Id + ".jpg";

			if (File.Exists(src))
			{
				try
				{
					b.BeginInit();
					b.CacheOption = BitmapCacheOption.OnLoad;
					b.UriSource = new Uri(src);
					b.EndInit();
					img.Source = b;
				}
				catch
				{
				}
			}
			
		}
	}
}
