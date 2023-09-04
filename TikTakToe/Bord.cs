using CustomMessageBox.Private;
using TikTakToe.Enums;

namespace TikTakToe
{
	class Bord : IDisposable
	{
		private TikTakToe owner;
		public Feld[] Felder =>  new[] { feld1, feld2, feld3, feld4, feld5, feld6, feld7, feld8, feld9 };

		public readonly Form form;

		public readonly Feld feld1;
		public readonly Feld feld2;
		public readonly Feld feld3;
		public readonly Feld feld4;
		public readonly Feld feld5;
		public readonly Feld feld6;
		public readonly Feld feld7;
		public readonly Feld feld8;
		public readonly Feld feld9;

		private Form? vorhang;
		private bool showVorhang = false;

		public Bord(TikTakToe newOwner)
		{
			owner = newOwner;

			Form frm = new Form();
			frm.AllowTransparency = true;
			frm.BackColor = Color.Black;

			frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			feld1 = new Feld(ref frm, owner, 0, 0);
			feld2 = new Feld(ref frm, owner, 0, 1);
			feld3 = new Feld(ref frm, owner, 0, 2);
			feld4 = new Feld(ref frm, owner, 1, 0);
			feld5 = new Feld(ref frm, owner, 1, 1);
			feld6 = new Feld(ref frm, owner, 1, 2);
			feld7 = new Feld(ref frm, owner, 2, 0);
			feld8 = new Feld(ref frm, owner, 2, 1);
			feld9 = new Feld(ref frm, owner, 2, 2);


			//frm.Size = new Size(feld9.Location.X + feld9.Size.Width + 2 * feld9.MarginWidth, feld9.Location.Y + feld9.Size.Height + 2 * feld9.MarginWidth);

			//Label vorhang = new Label();
			//vorhang.AutoSize = false;
			//vorhang.TextAlign = ContentAlignment.MiddleCenter;
			//vorhang.ForeColor = Color.Black;
			//vorhang.Location = feld1.Location;
			//vorhang.Size = new Size(feld9.Location.X + feld9.Size.Width - feld1.Location.X, feld9.Location.Y + feld9.Size.Height - feld1.Location.Y);
			//vorhang.SendToBack();
			//this.vorhang = vorhang;
			//frm.Controls.Add(vorhang);

			frm.AutoSize = true;
			frm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			frm.MinimumSize = frm.Size;
			frm.MaximumSize = frm.Size;
			form = frm;
		}

		public Bord(TikTakToe newOwner, Feld[] fields)
		{
			if (fields.Length != 9)
				throw new ArgumentException("Felder müssen 9 sein", nameof(fields));

			owner = newOwner;

			Form frm = new Form();
			frm.AllowTransparency = true;
			frm.BackColor = Color.Black;

			feld1 = new Feld(ref frm, owner, fields[0].Spalte, fields[0].Zeile, fields[0].GetStatus());
			feld2 = new Feld(ref frm, owner, fields[1].Spalte, fields[1].Zeile, fields[1].GetStatus());
			feld3 = new Feld(ref frm, owner, fields[2].Spalte, fields[2].Zeile, fields[2].GetStatus());
			feld4 = new Feld(ref frm, owner, fields[3].Spalte, fields[3].Zeile, fields[3].GetStatus());
			feld5 = new Feld(ref frm, owner, fields[4].Spalte, fields[4].Zeile, fields[4].GetStatus());
			feld6 = new Feld(ref frm, owner, fields[5].Spalte, fields[5].Zeile, fields[5].GetStatus());
			feld7 = new Feld(ref frm, owner, fields[6].Spalte, fields[6].Zeile, fields[6].GetStatus());
			feld8 = new Feld(ref frm, owner, fields[7].Spalte, fields[7].Zeile, fields[7].GetStatus());
			feld9 = new Feld(ref frm, owner, fields[8].Spalte, fields[8].Zeile, fields[8].GetStatus());


			//frm.Size = new Size(feld9.Location.X + feld9.Size.Width + 2 * feld9.MarginWidth, feld9.Location.Y + feld9.Size.Height + 2 * feld9.MarginWidth);
			frm.Size = new Size(feld9.Location.X + feld9.Size.Width + 1, feld9.Location.Y + feld9.Size.Height + 1);

			//Label vorhang = new Label();
			//vorhang.AutoSize = false;
			//vorhang.TextAlign = ContentAlignment.MiddleCenter;
			////vorhang.BackColor = Color.Transparent;// Color.FromArgb(150, 0, 0, 0);
			//vorhang.ForeColor = Color.Black;
			//vorhang.Location = feld1.Location;
			//vorhang.Size = new Size(feld9.Location.X + feld9.Size.Width - feld1.Location.X, feld9.Location.Y + feld9.Size.Height - feld1.Location.Y);
			//vorhang.SendToBack();
			//this.vorhang = vorhang;
			//frm.Controls.Add(vorhang);
			frm.AutoSize = true;
			frm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			frm.MinimumSize = frm.Size;
			frm.MaximumSize = frm.Size;
			form = frm;
		}

		public void Show(Players starter = Players.Player1)
		{
			if (starter == Players.Computer_Player2)
			{
				owner.ComputerTurn();
			}
			try 
			{ 
				form.ShowDialog();
			}
			catch
			{}
		}

		public List<Feld> GetFreeFields()
		{
			return Felder.Where(f => f.GetStatus() == Players.NoOne).ToList();
		}

		public Feld? GetFieldById(int id)
		{
			foreach (Feld fld in Felder)
			{
				if (fld.id == id)
				{
					return fld;
				}
			}
			return null;
		}

		public Bord Clone()
		{
			Bord newBord = new Bord(owner, Felder);
			return newBord;
		}

		public void StartVorhang(string text)
		{
			showVorhang = true;
			vorhang ??= new MsgForm(text);
			vorhang.FormClosing += Vorhang_FormClosing;
		}


		private void Vorhang_FormClosing(object? sender, FormClosingEventArgs e)
		{
			e.Cancel = !showVorhang;
		}

		public void CloseVorhang()
		{
			showVorhang = false;
			try
			{
				if (vorhang is not null)
					Helper.Close(vorhang);
			}
			catch
			{
			}
		}
		
		public void ResetFields()
		{
			feld1.SetStatus(Players.NoOne, true);
			feld2.SetStatus(Players.NoOne, true);
			feld3.SetStatus(Players.NoOne, true);
			feld4.SetStatus(Players.NoOne, true);
			feld5.SetStatus(Players.NoOne, true);
			feld6.SetStatus(Players.NoOne, true);
			feld7.SetStatus(Players.NoOne, true);
			feld8.SetStatus(Players.NoOne, true);
			feld9.SetStatus(Players.NoOne, true);
		}

		public void Dispose()
		{
			form.Close();
			form.Dispose();
		}
	}

}