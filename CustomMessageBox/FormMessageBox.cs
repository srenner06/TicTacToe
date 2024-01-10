using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomMessageBox.Private
{
	public partial class MsgForm : Form
	{
		#region Properties & Fields
		//Fields
		private Color primaryColor = Color.CornflowerBlue;
		private int borderSize = 2;

		//Properties
		public Color PrimaryColor
		{
			get { return primaryColor; }
			set
			{
				primaryColor = value;
				this.BackColor = primaryColor;//Form Border Color
				this.panelTitleBar.BackColor = PrimaryColor;//Title Bar Back Color
			}
		}

		#endregion

		#region Contructors
		public MsgForm(string text) : this(text, "")
		{ }
		public MsgForm(string text, string caption) : this(text, caption, MessageBoxButtons.OK)
		{ }
		public MsgForm(string text, string caption, MessageBoxIcon? icon) : this(text, caption, MessageBoxButtons.OK, icon)
		{ }
		public MsgForm(string text, string caption, MessageBoxButtons buttons) : this(text, caption, buttons, null)
		{ }
		public MsgForm(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon? icon) : this(text, caption, buttons, icon, MessageBoxDefaultButton.Button1)
		{ }

		public MsgForm(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon? icon, MessageBoxDefaultButton defaultButton)
		{
			InitializeComponent();
			InitializeItems();
			this.PrimaryColor = primaryColor;
			this.labelMessage.Text = text;
			this.labelCaption.Text = caption;
			SetFormSize();
			SetButtons(buttons, defaultButton);
			if (icon != null)
				SetIcon((MessageBoxIcon)icon);
		}
		#endregion

		//-> Private Methods
		private void InitializeItems()
		{
			this.FormBorderStyle = FormBorderStyle.None;
			this.Padding = new Padding(borderSize);//Set border size
			this.labelMessage.MaximumSize = new Size(550, 0);
			this.btnClose.DialogResult = DialogResult.Cancel;
			this.button1.DialogResult = DialogResult.OK;
			this.button1.Visible = false;
			this.button2.Visible = false;
			this.button3.Visible = false;
		}
		private void SetFormSize()
		{
			var widht = this.labelMessage.Width + this.pictureBoxIcon.Width + this.panelBody.Padding.Left;
			var height = this.panelTitleBar.Height + this.labelMessage.Height + this.panelButtons.Height + this.panelBody.Padding.Top;
			this.Size = new Size(widht, height);
		}
		private void SetButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
		{
			var xCenter = (this.panelButtons.Width - button1.Width) / 2;
			var yCenter = (this.panelButtons.Height - button1.Height) / 2;

			switch (buttons)
			{
				case MessageBoxButtons.OK:
					//OK Button
					button1.Visible = true;
					button1.Location = new Point(xCenter, yCenter);
					button1.Text = "Ok";
					button1.DialogResult = DialogResult.OK;//Set DialogResult

					//Set Default Button
					SetDefaultButton(defaultButton);
					break;
				case MessageBoxButtons.OKCancel:
					//OK Button
					button1.Visible = true;
					button1.Location = new Point(xCenter - (button1.Width / 2) - 5, yCenter);
					button1.Text = "Ok";
					button1.DialogResult = DialogResult.OK;//Set DialogResult

					//Cancel Button
					button2.Visible = true;
					button2.Location = new Point(xCenter + (button2.Width / 2) + 5, yCenter);
					button2.Text = "Cancel";
					button2.DialogResult = DialogResult.Cancel;//Set DialogResult
					button2.BackColor = Color.DimGray;

					//Set Default Button
					if (defaultButton != MessageBoxDefaultButton.Button3)//There are only 2 buttons, so the Default Button cannot be Button3
						SetDefaultButton(defaultButton);
					else
						SetDefaultButton(MessageBoxDefaultButton.Button1);
					break;

				case MessageBoxButtons.RetryCancel:
					//Retry Button
					button1.Visible = true;
					button1.Location = new Point(xCenter - (button1.Width / 2) - 5, yCenter);
					button1.Text = "Retry";
					button1.DialogResult = DialogResult.Retry;//Set DialogResult

					//Cancel Button
					button2.Visible = true;
					button2.Location = new Point(xCenter + (button2.Width / 2) + 5, yCenter);
					button2.Text = "Cancel";
					button2.DialogResult = DialogResult.Cancel;//Set DialogResult
					button2.BackColor = Color.DimGray;

					//Set Default Button
					if (defaultButton != MessageBoxDefaultButton.Button3)//There are only 2 buttons, so the Default Button cannot be Button3
						SetDefaultButton(defaultButton);
					else
						SetDefaultButton(MessageBoxDefaultButton.Button1);
					break;

				case MessageBoxButtons.YesNo:
					//Yes Button
					button1.Visible = true;
					button1.Location = new Point(xCenter - (button1.Width / 2) - 5, yCenter);
					button1.Text = "Yes";
					button1.DialogResult = DialogResult.Yes;//Set DialogResult

					//No Button
					button2.Visible = true;
					button2.Location = new Point(xCenter + (button2.Width / 2) + 5, yCenter);
					button2.Text = "No";
					button2.DialogResult = DialogResult.No;//Set DialogResult
					button2.BackColor = Color.IndianRed;

					//Set Default Button
					if (defaultButton != MessageBoxDefaultButton.Button3)//There are only 2 buttons, so the Default Button cannot be Button3
						SetDefaultButton(defaultButton);
					else
						SetDefaultButton(MessageBoxDefaultButton.Button1);
					break;
				case MessageBoxButtons.YesNoCancel:
					//Yes Button
					button1.Visible = true;
					button1.Location = new Point(xCenter - button1.Width - 5, yCenter);
					button1.Text = "Yes";
					button1.DialogResult = DialogResult.Yes;//Set DialogResult

					//No Button
					button2.Visible = true;
					button2.Location = new Point(xCenter, yCenter);
					button2.Text = "No";
					button2.DialogResult = DialogResult.No;//Set DialogResult
					button2.BackColor = Color.IndianRed;

					//Cancel Button
					button3.Visible = true;
					button3.Location = new Point(xCenter + button2.Width + 5, yCenter);
					button3.Text = "Cancel";
					button3.DialogResult = DialogResult.Cancel;//Set DialogResult
					button3.BackColor = Color.DimGray;

					//Set Default Button
					SetDefaultButton(defaultButton);
					break;

				case MessageBoxButtons.AbortRetryIgnore:
					//Abort Button
					button1.Visible = true;
					button1.Location = new Point(xCenter - button1.Width - 5, yCenter);
					button1.Text = "Abort";
					button1.DialogResult = DialogResult.Abort;//Set DialogResult
					button1.BackColor = Color.Goldenrod;

					//Retry Button
					button2.Visible = true;
					button2.Location = new Point(xCenter, yCenter);
					button2.Text = "Retry";
					button2.DialogResult = DialogResult.Retry;//Set DialogResult                    

					//Ignore Button
					button3.Visible = true;
					button3.Location = new Point(xCenter + button2.Width + 5, yCenter);
					button3.Text = "Ignore";
					button3.DialogResult = DialogResult.Ignore;//Set DialogResult
					button3.BackColor = Color.IndianRed;

					//Set Default Button
					SetDefaultButton(defaultButton);
					break;
			}
		}
		private void SetDefaultButton(MessageBoxDefaultButton defaultButton)
		{
			switch (defaultButton)
			{
				case MessageBoxDefaultButton.Button1://Focus button 1
					button1.Select();
					button1.ForeColor = Color.White;
					button1.Font = new Font(button1.Font, FontStyle.Underline);
					break;
				case MessageBoxDefaultButton.Button2://Focus button 2
					button2.Select();
					button2.ForeColor = Color.White;
					button2.Font = new Font(button2.Font, FontStyle.Underline);
					break;
				case MessageBoxDefaultButton.Button3://Focus button 3
					button3.Select();
					button3.ForeColor = Color.White;
					button3.Font = new Font(button3.Font, FontStyle.Underline);
					break;
			}
		}
		public void SetButtonsText(string text1 = "", string text2 = "", string text3 = "")
		{
			button1.Text = text1;
			button2.Text = text2;
			button3.Text = text3;
		}
		private void SetIcon(MessageBoxIcon icon)
		{
			switch (icon)
			{
				case MessageBoxIcon.Error: //Error
					this.pictureBoxIcon.Image = Properties.Resources.error;
					PrimaryColor = Color.FromArgb(224, 79, 95);
					this.btnClose.FlatAppearance.MouseOverBackColor = Color.Crimson;
					break;
				case MessageBoxIcon.Information: //Information
					this.pictureBoxIcon.Image = Properties.Resources.information;
					PrimaryColor = Color.FromArgb(38, 191, 166);
					break;
				case MessageBoxIcon.Question://Question
					this.pictureBoxIcon.Image = Properties.Resources.question;
					PrimaryColor = Color.FromArgb(10, 119, 232);
					break;
				case MessageBoxIcon.Exclamation://Exclamation
					this.pictureBoxIcon.Image = Properties.Resources.exclamation;
					PrimaryColor = Color.FromArgb(255, 140, 0);
					break;
				case MessageBoxIcon.None: //None
					this.pictureBoxIcon.Image = Properties.Resources.chat;
					PrimaryColor = Color.CornflowerBlue;
					break;
			}
		}

		//-> Events Methods
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#region -> Drag Form
		[DllImport("user32.DLL", EntryPoint = "SendMessage")]
		private static extern void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
		[DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
		private static extern void ReleaseCapture();
		private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(this.Handle, 0x112, 0xf012, 0);
		}
		#endregion
	}
}
