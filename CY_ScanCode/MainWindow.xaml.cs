using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CY_ScanCode
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{


		private int passedCount = 0;

		private int failedCount = 0;

		private StringBuilder firstCode = new StringBuilder();

		private StringBuilder secondCode = new StringBuilder();

		private SoundPlayer soundPlayer;

		public ObservableCollection<Record> Records { get; set; }

		public class Record
		{
			public string 扫码结果 { get; set; }

			public Record(string code)
			{
				this.扫码结果 = code;
			}
		}

		public MainWindow()
		{
			this.InitializeComponent();
			this.Records = new ObservableCollection<Record>();
			this.DataGrid1.ItemsSource = this.Records;
			this.InitializeSound();
			this.Start();
		}

		private void InitializeSound()
		{
			var path1 = Directory.GetCurrentDirectory() + @"\error.wav";
			var path2 = Environment.CurrentDirectory + @"\error.wav";
			this.soundPlayer = new SoundPlayer(path2);
		}

		private void Start()
		{
			this.TextBox_BarcodeScan.Focusable = true;
			this.TextBox_QrcodeScan.Focusable = true;
			this.TextBox_BarcodeScan.Focus();
			var resetColor = (SolidColorBrush)this.FindResource("TextForeground");
			this.Path_Result.Fill = resetColor;
			this.Path_Result.Data = (Geometry)this.FindResource("Reday");
			this.TextBlock_Result.Foreground = resetColor;
			this.TextBlock_Result.Text = "准备";

			this.TextBox_Barcode.Text = string.Empty;
			this.TextBox_BarcodeScan.Text = string.Empty;
			this.TextBox_Qrcode.Text = string.Empty;
			this.TextBox_QrcodeScan.Text = string.Empty;

			this.soundPlayer.Stop();
		}

		public void WriteTextAsCsv(string fileName, ObservableCollection<Record> records)
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("扫码信息");

			foreach (var record in records)
			{
				stringBuilder.AppendLine(record.扫码结果);
			}

			File.WriteAllText(fileName, stringBuilder.ToString());
		}

		public void WriteTextAsTxt(string fileName, ObservableCollection<Record> records)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"扫码成功数:{this.passedCount}");
			stringBuilder.AppendLine($"扫码失败数:{this.failedCount}");
			stringBuilder.AppendLine("条码信息");
			foreach (var record in records)
			{
				stringBuilder.AppendLine(record.扫码结果);
			}
			File.WriteAllText(fileName, stringBuilder.ToString());
		}

		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			this.Start();
		}

		private void Button_Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void TextBox_BarcodeScan_KeyDown(object sender, KeyEventArgs e)
		{
			var code = Enum.GetName(e.Key.GetType(), e.Key);

			if (code == "Return") { this.firstCode.Remove(25, 1); this.TextBox_QrcodeScan.Focus(); this.TextBox_BarcodeScan.Text = this.firstCode.ToString(); }

			else firstCode.Append(code.Substring(1) + '-');
		}

		private void TextBox_QrcodeScan_KeyDown(object sender, KeyEventArgs e)
		{
			var code = Enum.GetName(e.Key.GetType(), e.Key);

			if (code == "Return")
			{
				this.secondCode.Remove(25, 1);
				this.TextBox_QrcodeScan.Text = this.secondCode.ToString();
				if (this.firstCode.ToString() == this.secondCode.ToString())
				{
					var passForeground = (SolidColorBrush)this.FindResource("PassForeground");
					this.TextBlock_Result.Text = "通过";
					this.passedCount++;
					this.TextBlock_Passed.Text = this.passedCount.ToString();
					this.Path_Result.Data = (Geometry)this.FindResource("Pass");
					this.Path_Result.Fill = passForeground;
					this.TextBlock_Result.Foreground = passForeground;
					this.TextBox_Barcode.Text = this.firstCode.ToString();
					this.TextBox_Qrcode.Text = this.secondCode.ToString();
					this.TextBox_BarcodeScan.Clear();
					this.TextBox_QrcodeScan.Clear();
					this.TextBox_BarcodeScan.Focus();
					this.Records.Add(new Record(this.firstCode.Replace("-", string.Empty).ToString()));
				}
				else
				{
					var failColor = new SolidColorBrush(Colors.Red);
					this.TextBlock_Result.Text = "失败";
					this.failedCount++;
					this.TextBlock_Failed.Text = this.failedCount.ToString();
					this.Path_Result.Data = (Geometry)this.FindResource("Fail");
					this.Path_Result.Fill = failColor;
					this.TextBlock_Result.Foreground = failColor;
					this.TextBox_BarcodeScan.Focusable = false;
					this.TextBox_QrcodeScan.Focusable = false;
					soundPlayer.PlayLooping();
				}

				this.firstCode.Clear();
				this.secondCode.Clear();
			}
			else this.secondCode.Append(code.Substring(1) + '-');

		}

		private void Button_Save_Click(object sender, RoutedEventArgs e)
		{
			var saveFile = new System.Windows.Forms.SaveFileDialog();
			saveFile.DefaultExt = string.Format("*.{0}", "txt");
			saveFile.AddExtension = true;
			saveFile.Filter = string.Format("{0} files|*.{0}", "txt");
			saveFile.OverwritePrompt = true;
			saveFile.CheckPathExists = true;
			saveFile.FileName = DateTime.Now.ToString("yyyyMMddhhmmss");
			if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				try
				{
					this.WriteTextAsTxt(saveFile.FileName, this.Records);
					MessageBox.Show("扫码信息导出成功。");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void Button_Clear_Click(object sender, RoutedEventArgs e)
		{
			this.failedCount = 0;
			this.passedCount = 0;
			this.TextBlock_Failed.Text = this.failedCount.ToString();
			this.TextBlock_Passed.Text = this.passedCount.ToString();
		}

		private void Button_Min_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void Button_Max_Click(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Normal)
			{
				this.WindowState = WindowState.Maximized;
				this.Button_Max.Content = this.FindResource("Path_Nor");
			}
			else if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;

				this.Button_Max.Content = this.FindResource("Path_Max");
			}
		}

		private void DockPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}
	}
}
