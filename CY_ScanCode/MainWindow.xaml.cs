using System;
using System.Collections.ObjectModel;
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
		public MainWindow()
		{
			InitializeComponent();
			this.DataGrid1.ItemsSource = this.Records;
			this.Start();
		}

		public ObservableCollection<Record> Records { get; set; } = new ObservableCollection<Record>();

		public class Record
		{
			public string 扫码结果 { get; set; }

			public Record(string code)
			{
				this.扫码结果 = code;
			}
		}

		StringBuilder test = new StringBuilder();

		StringBuilder firstCode = new StringBuilder();

		StringBuilder secondCode = new StringBuilder();
		
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

			if (code == "Return") this.TextBox_QrcodeScan.Focus();
			
			else firstCode.Append(code.Substring(1));
		}

		private void TextBox_QrcodeScan_KeyDown(object sender, KeyEventArgs e)
		{
			var code = Enum.GetName(e.Key.GetType(), e.Key);

			if (code == "Return")
			{
				if (this.firstCode.ToString() == this.secondCode.ToString())
				{
					var passForeground = (SolidColorBrush)this.FindResource("PassForeground");
					this.TextBlock_Result.Text = "通过";
					this.Path_Result.Data = (Geometry)this.FindResource("Pass");
					this.Path_Result.Fill = passForeground;
					this.TextBlock_Result.Foreground = passForeground;
					this.TextBox_Barcode.Text = this.firstCode.ToString();
					this.TextBox_Qrcode.Text = this.secondCode.ToString();
					this.TextBox_BarcodeScan.Clear();
					this.TextBox_QrcodeScan.Clear();
					this.TextBox_BarcodeScan.Focus();
					this.Records.Add(new Record(this.firstCode.ToString()));
				}
				else
				{
					var failColor= new SolidColorBrush(Colors.Red);
					this.TextBlock_Result.Text = "失败";
					this.Path_Result.Data = (Geometry)this.FindResource("Fail");
					this.Path_Result.Fill = failColor;
					this.TextBlock_Result.Foreground = failColor;
					this.TextBox_BarcodeScan.Focusable = false;
					this.TextBox_QrcodeScan.Focusable = false;
				}

				this.firstCode.Clear();
				this.secondCode.Clear();
			}
			else this.secondCode.Append(code.Substring(1));
			
		}
	}
}
