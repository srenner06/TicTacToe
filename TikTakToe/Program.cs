using System.Runtime.Serialization.Formatters.Binary;

namespace TikTakToe
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			//Application.Run(new Form1());
			TikTakToe ttt = new TikTakToe();
			ttt.Start();
		}
	}
	enum Mode { einfach, mittel, unmöglich }

	class TikTakToe
	{
		private bool _ended = false;
		public bool Ended { get { return _ended; } }
		private Mode _modus;
		public Mode Modus { get { return _modus; } }
		private Bord _bord;
		private Players _turn;

		 public Players Turn { get { return _turn; } set { _turn = value; } }

		public Bord Bord
		{
			get
			{
				return _bord;
			}
			set
			{
				_bord = value;
			}
		}

		private static Random rnd = new Random();

		public void Start()
		{
			this.Bord = new Bord(this);
			while (true)
			{
				CustomMessageBox.Private.FormMessageBox frm = new CustomMessageBox.Private.FormMessageBox("Welchen Modus wollen sie spielen?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button4);
				frm.SetButtonsText("Einfach", "Mittel", "Unmöglich");

				DialogResult result = frm.ShowDialog();
				if (result == DialogResult.Abort)
				{
					_modus = Mode.einfach;
				}
				else if (result == DialogResult.Retry)
				{
					_modus = Mode.mittel;
				}
				else if (result == DialogResult.Ignore)
				{
					_modus = Mode.unmöglich;
				}
				else
				{
					return;
				}

				CustomMessageBox.Private.FormMessageBox starter = new CustomMessageBox.Private.FormMessageBox("Wollen Sie starten?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				DialogResult starterResult = starter.ShowDialog();
				if (starterResult == DialogResult.Yes)
				{
					_turn = Players.Player;
					break;
				}
				else if (starterResult == DialogResult.No)
				{
					_turn = Players.Computer;
					break;
				}
				else
				{
					return;
				}
			}
			Bord.Show(_turn);
		}

		public async void ComputerTurn()
		{
			CheckVictory();
			
			if (_turn != Players.Computer) return;
			//_bord.SetVorhangText("Computer berechnet seinen Zug...");
			//_bord.SetVorhangColor(Color.Green);
			_bord.stopVorhang = false;
			_bord.startVorhang = false;
			//_bord.Vorhang();
			//_bord.Vorhang.BringToFront();
			//Form frm2 = new Form();
			//frm2.Controls.Add(_bord.Vorhang);
			//frm2.ShowDialog();

			if (_modus == Mode.einfach)
			{
				List<Feld> freeFields = _bord.GetFreeFields();
				Feld fld = freeFields[rnd.Next(freeFields.Count)];
				fld.Click(true);
			}
			else if (_modus == Mode.mittel)
			{
				Feld fieldToVictory = CheckIfPossibleWinn(Players.Computer);
				if (!(fieldToVictory is null))
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player can win
					Feld fieldToDefeat = CheckIfPossibleWinn(Players.Player);
					if (!(fieldToDefeat is null))
					{
						fieldToDefeat.Click(true);
					}
					else
					{
						List<int> freeCorners = new List<int>();
						if (_bord.GetFieldById(5).GetStatus() == Players.NoOne)
						{
							_bord.GetFieldById(5).Click(true);
						}
						else
						{
							if (_bord.GetFieldById(1).GetStatus() == Players.NoOne)
							{
								freeCorners.Add(1);
							}
							if (_bord.GetFieldById(3).GetStatus() == Players.NoOne)
							{
								freeCorners.Add(3);
							}
							if (_bord.GetFieldById(7).GetStatus() == Players.NoOne)
							{
								freeCorners.Add(7);
							}
							if (_bord.GetFieldById(9).GetStatus() == Players.NoOne)
							{
								freeCorners.Add(9);
							}

							if (freeCorners.Count != 0)
							{
								int id = freeCorners[rnd.Next(freeCorners.Count)];
								_bord.GetFieldById(id).Click(true);
							}
							else
							{
								List<Feld> freeFields = _bord.GetFreeFields();
								Feld fld = freeFields[rnd.Next(freeFields.Count)];
								fld.Click(true);
							}
						}
					}
				}
			}
			else if (_modus == Mode.unmöglich)
			{
				var task1 = Task.Factory.StartNew(() => _bord.Vorhang());
				_bord.startVorhang = true;
				Feld fieldToVictory = CheckIfPossibleWinn(Players.Computer);
				if (!(fieldToVictory is null))
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player can win
					Feld fieldToDefeat = CheckIfPossibleWinn(Players.Player);
					if (!(fieldToDefeat is null))
					{
						fieldToDefeat.Click(true);
					}
					else
					{
						int id = FindBestTurn(new ShallowBord(this.Bord), Players.Computer, 0);
						_bord.GetFieldById(id).Click(true);
					}
				}
			}
			_bord.stopVorhang = true;
			_bord.CloseVorhang();

			CheckVictory();
			_turn = Players.Player;
			_bord.form.Activate();
		}

		public Feld CheckIfPossibleWinn(Players checkedPlayer)
		{
			Feld feldToVictory = null;
			feldToVictory = CheckPossibleWinn(_bord.feld1, _bord.feld2, _bord.feld3, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld4, _bord.feld5, _bord.feld6, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld7, _bord.feld8, _bord.feld9, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld1, _bord.feld4, _bord.feld7, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld2, _bord.feld5, _bord.feld8, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld3, _bord.feld6, _bord.feld9, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld1, _bord.feld5, _bord.feld9, checkedPlayer);
			if (!(feldToVictory is null)) return feldToVictory;
			feldToVictory = CheckPossibleWinn(_bord.feld3, _bord.feld5, _bord.feld7, checkedPlayer);
			return feldToVictory;
		}

		public Feld CheckPossibleWinn(Feld feld1, Feld feld2, Feld feld3, Players checkedPlayer)
		{
			Players f1 = feld1.GetStatus();
			Players f2 = feld2.GetStatus();
			Players f3 = feld3.GetStatus();
			if (f1 == checkedPlayer && f2 == checkedPlayer && f3 == Players.NoOne)
			{
				return feld3;
			}
			else if (f1 == checkedPlayer && f2 == Players.NoOne && f3 == checkedPlayer)
			{
				return feld2;
			}
			else if (f1 == Players.NoOne && f2 == checkedPlayer && f3 == checkedPlayer)
			{
				return feld1;
			}
			else
			{
				return null;
			}
		}

		public void CheckVictory()
		{
			if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Player) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Player) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Player) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Player) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Player) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Player) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Player) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Player))
			{
				_ended = true;
				_turn = Players.NoOne;
				CustomMessageBox.Private.FormMessageBox frm = new CustomMessageBox.Private.FormMessageBox("Sie haben gewonnen\nMöchten Sie noch einmal starten?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (frm.ShowDialog() == DialogResult.Yes)
				{
					this._bord.form.Dispose();
					this._bord.form.Close();
					Start();
				}
				if (System.Windows.Forms.Application.MessageLoop)
				{
					// WinForms app
					System.Windows.Forms.Application.Exit();
				}
				else
				{
					// Console app
					System.Environment.Exit(1);
				}
			}
			else if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Computer) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Computer) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Computer) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Computer) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Computer) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Computer) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Computer) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Computer))
			{
				_ended = true;
				_turn = Players.NoOne;
				CustomMessageBox.Private.FormMessageBox frm = new CustomMessageBox.Private.FormMessageBox("Sie haben verloren\nMöchten Sie noch einmal starten?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (frm.ShowDialog() == DialogResult.Yes)
				{
					this._bord.form.Dispose();
					this._bord.form.Close();
					this.Start();
				}
				if (System.Windows.Forms.Application.MessageLoop)
				{
					// WinForms app
					System.Windows.Forms.Application.Exit();
				}
				else
				{
					// Console app
					System.Environment.Exit(1);
				}
			}
			else if (_bord.GetFreeFields().Count == 0)
			{
				_ended = true;
				_turn = Players.NoOne;

				CustomMessageBox.Private.FormMessageBox frm = new CustomMessageBox.Private.FormMessageBox("Unentschieden\nMöchten Sie noch einmal starten?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (frm.ShowDialog() == DialogResult.Yes)
				{
					this._bord.form.Dispose();
					this._bord.form.Close();
					Start();
				}
				if (System.Windows.Forms.Application.MessageLoop)
				{
					// WinForms app
					System.Windows.Forms.Application.Exit();
				}
				else
				{
					// Console app
					System.Environment.Exit(1);
				}
			}
		}

		public bool CheckWin(Feld feld1, Feld feld2, Feld feld3, Players player)
		{
			if (feld1.GetStatus() == player && feld2.GetStatus() == player && feld3.GetStatus() == player)
			{
				return true;
			}
			return false;
		}

	
		private int FindBestTurn(ShallowBord bord, Players player, int count)
		{
			ShallowBord clone = new ShallowBord();
			bord.CopyTo(ref clone);

			bool max = player == Players.Computer;
			Dictionary<string, int> best = new Dictionary<string, int>();
			best.Add("id", -1);
			best.Add("sc", 2);
			if (max)
				best["sc"] = -2;

			foreach (int fld in clone.GetFreeFields())
			{
				clone.SetByNum(fld, player, true);
				Dictionary<string, int> curr = new Dictionary<string, int>();
				curr.Add("id", fld);
				curr.Add("sc", score(clone, player, count));
				if (max)
				{
					if (curr["sc"] > best["sc"])
					{
						best = curr;
					}
				}
				else
				{
					if (curr["sc"] < best["sc"])
					{
						best = curr;
					}
				}
				clone.SetByNum(fld, Players.NoOne, true);

				//if (count == 0 && best["sc"] == 1)
				//	return best["id"];

				//if (max)
				//{
				//	if (best["sc"] == 1)
				//		return 1;
				//}
				//else
				//{
				//	if (best["sc"] == -1)
				//		return -1;
				//}
			}
			if (count == 0)
			{
				return best["id"];
			}
			else
			{
				return best["sc"];
			}
		}

		private int score(ShallowBord bord, Players player, int count)
		{
			Players winner = bord.CheckWin();
			if (winner != Players.NoOne)
			{
				if (winner == Players.Computer)
					return 1;
				else
					return -1;
			}
			if (bord.GetFreeFields().Count == 0)
				return 0;

			if (player == Players.Computer)
			{
				return FindBestTurn(bord, Players.Player, count + 1);
			}
			else
			{
				return FindBestTurn(bord, Players.Computer, count + 1);
			}
		}
	}



	class Bord
	{
		private TikTakToe owner;
		public readonly List<Feld> Felder = new List<Feld>();

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

		private Form vorhang;
		public bool stopVorhang = false;
		public bool startVorhang = false;

		public Bord(TikTakToe newOwner)
		{
			owner = newOwner;

			Form frm = new Form();
			frm.AllowTransparency = true;
			frm.BackColor = Color.Black;

			frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			feld1 = new Feld(ref frm, owner, 0, 0, 1);
			feld2 = new Feld(ref frm, owner, 0, 1, 2);
			feld3 = new Feld(ref frm, owner, 0, 2, 3);
			feld4 = new Feld(ref frm, owner, 1, 0, 4);
			feld5 = new Feld(ref frm, owner, 1, 1, 5);
			feld6 = new Feld(ref frm, owner, 1, 2, 6);
			feld7 = new Feld(ref frm, owner, 2, 0, 7);
			feld8 = new Feld(ref frm, owner, 2, 1, 8);
			feld9 = new Feld(ref frm, owner, 2, 2, 9);

			Felder.AddRange(new Feld[] { feld1, feld2, feld3, feld4, feld5, feld6, feld7, feld8, feld9 });

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

		public Bord(TikTakToe newOwner, List<Feld> fields)
		{
			if (fields.Count != 9) return;
			owner = newOwner;

			Form frm = new Form();
			frm.AllowTransparency = true;
			frm.BackColor = Color.Black;

			feld1 = new Feld(ref frm, owner, fields[0].Spalte, fields[0].Zeile, fields[0].id, fields[0].GetStatus());
			feld2 = new Feld(ref frm, owner, fields[1].Spalte, fields[1].Zeile, fields[1].id, fields[1].GetStatus());
			feld3 = new Feld(ref frm, owner, fields[2].Spalte, fields[2].Zeile, fields[2].id, fields[2].GetStatus());
			feld4 = new Feld(ref frm, owner, fields[3].Spalte, fields[3].Zeile, fields[3].id, fields[3].GetStatus());
			feld5 = new Feld(ref frm, owner, fields[4].Spalte, fields[4].Zeile, fields[4].id, fields[4].GetStatus());
			feld6 = new Feld(ref frm, owner, fields[5].Spalte, fields[5].Zeile, fields[5].id, fields[5].GetStatus());
			feld7 = new Feld(ref frm, owner, fields[6].Spalte, fields[6].Zeile, fields[6].id, fields[6].GetStatus());
			feld8 = new Feld(ref frm, owner, fields[7].Spalte, fields[7].Zeile, fields[7].id, fields[7].GetStatus());
			feld9 = new Feld(ref frm, owner, fields[8].Spalte, fields[8].Zeile, fields[8].id, fields[8].GetStatus());

			Felder.AddRange(new Feld[] { feld1, feld2, feld3, feld4, feld5, feld6, feld7, feld8, feld9 });

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

		public void Show(Players starter = Players.Player)
		{
			if (starter == Players.Computer)
			{
				owner.Turn = starter;
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
			List<Feld> results = new List<Feld>();
			if (feld1.GetStatus() == Players.NoOne)
				results.Add(feld1);
			if (feld4.GetStatus() == Players.NoOne)
				results.Add(feld4);
			if (feld7.GetStatus() == Players.NoOne)
				results.Add(feld7);
			if (feld2.GetStatus() == Players.NoOne)
				results.Add(feld2);
			if (feld5.GetStatus() == Players.NoOne)
				results.Add(feld5);
			if (feld8.GetStatus() == Players.NoOne)
				results.Add(feld8);
			if (feld3.GetStatus() == Players.NoOne)
				results.Add(feld3);
			if (feld6.GetStatus() == Players.NoOne)
				results.Add(feld6);
			if (feld9.GetStatus() == Players.NoOne)
				results.Add(feld9);
			return results;
		}

		public Feld GetFieldById(int id)
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
			Bord newBord = new Bord(owner, this.Felder);
			return newBord;
		}

		public async Task Vorhang()
		{
			this.vorhang = new CustomMessageBox.Private.FormMessageBox("Der Computer berechnet seinen Zug...");
			while (true)
			{
				if (stopVorhang)
					break;
				if (startVorhang)
				this.vorhang.ShowDialog();
			}
		}

		public bool CloseVorhang()
		{
			try
			{
				if (!(this.vorhang is null))
				this.vorhang.Invoke(delegate() { vorhang.Close(); });
				return true;
			}
			catch
			{
				return false;
			}
			return false;
		}
		//private void AllControls(Control c, bool hide)
		//{
		//	if (hide)
		//	{
		//		c.SendToBack();
		//		foreach (Control child in c.Controls)
		//		{
		//			AllControls(child, true);
		//		}
		//	}
		//	else
		//	{
		//		c.BringToFront();
		//		foreach (Control child in c.Controls)
		//		{
		//			AllControls(child, false);
		//		}
		//	}
		//}
		//public void SetVorhangText(string text)
		//{
		//	this.vorhang.Text = text;
		//}
		//public void SetVorhangColor(Color color)
		//{
		//	this.vorhang.BackColor = color;
		//}
	}

	class ShallowBord
	{
		#region fields
		public Players feld1 = Players.NoOne;
		public Players feld2 = Players.NoOne;
		public Players feld3 = Players.NoOne;
		public Players feld4 = Players.NoOne;
		public Players feld5 = Players.NoOne;
		public Players feld6 = Players.NoOne;
		public Players feld7 = Players.NoOne;
		public Players feld8 = Players.NoOne;
		public Players feld9 = Players.NoOne;
		#endregion
		public ShallowBord() { }

		public ShallowBord(Bord import)
		{
			feld1 = import.feld1.GetStatus();
			feld2 = import.feld2.GetStatus();
			feld3 = import.feld3.GetStatus();
			feld4 = import.feld4.GetStatus();
			feld5 = import.feld5.GetStatus();
			feld6 = import.feld6.GetStatus();
			feld7 = import.feld7.GetStatus();
			feld8 = import.feld8.GetStatus();
			feld9 = import.feld9.GetStatus();
		}

		public void ImportBord(Bord import)
		{
			feld1 = import.feld1.GetStatus();
			feld2 = import.feld2.GetStatus();
			feld3 = import.feld3.GetStatus();
			feld4 = import.feld4.GetStatus();
			feld5 = import.feld5.GetStatus();
			feld6 = import.feld6.GetStatus();
			feld7 = import.feld7.GetStatus();
			feld8 = import.feld8.GetStatus();
			feld9 = import.feld9.GetStatus();
		}

		public void CopyTo(ref ShallowBord newBord)
		{
			newBord.feld1 = feld1;
			newBord.feld2 = feld2;
			newBord.feld3 = feld3;
			newBord.feld4 = feld4;
			newBord.feld5 = feld5;
			newBord.feld6 = feld6;
			newBord.feld7 = feld7;
			newBord.feld8 = feld8;
			newBord.feld9 = feld9;
		}

		public bool CheckPossibleWin(Players player)
		{
			if (CheckPossibleWin(feld1, feld2, feld3, player)) return true;
			if (CheckPossibleWin(feld4, feld5, feld6, player)) return true;
			if (CheckPossibleWin(feld7, feld8, feld9, player)) return true;

			if (CheckPossibleWin(feld1, feld4, feld7, player)) return true;
			if (CheckPossibleWin(feld2, feld5, feld8, player)) return true;
			if (CheckPossibleWin(feld3, feld6, feld9, player)) return true;

			if (CheckPossibleWin(feld1, feld5, feld9, player)) return true;
			if (CheckPossibleWin(feld3, feld5, feld7, player)) return true;

			return false;
		}

		public bool CheckPossibleWin(Players f1, Players f2, Players f3, Players player)
		{
			Players opposite = Players.Computer;
			if (player == Players.Computer)
				opposite = Players.Player;

			if (f1 == opposite || f2 == opposite || f3 == opposite)
				return false;

			int count = 0;
			if (f1 == player) count++;
			if (f2 == player) count++;
			if (f3 == player) count++;

			if (count >= 2) return true;

			return false;
		}

		public Players CheckWin()
		{
			if (feld1 == feld2 && feld1 == feld3) return feld1;
			if (feld4 == feld5 && feld4 == feld6) return feld4;
			if (feld7 == feld8 && feld7 == feld9) return feld7;

			if (feld1 == feld4 && feld1 == feld7) return feld1;
			if (feld2 == feld5 && feld2 == feld8) return feld2;
			if (feld3 == feld6 && feld3 == feld9) return feld3;

			if (feld1 == feld5 && feld1 == feld9) return feld1;
			if (feld3 == feld5 && feld3 == feld7) return feld3;

			return Players.NoOne;
		}

		public List<int> GetFreeFields()
		{
			List<int> list = new List<int>();
			if (feld1 == Players.NoOne) list.Add(1);
			if (feld2 == Players.NoOne) list.Add(2);
			if (feld3 == Players.NoOne) list.Add(3);
			if (feld4 == Players.NoOne) list.Add(4);
			if (feld5 == Players.NoOne) list.Add(5);
			if (feld6 == Players.NoOne) list.Add(6);
			if (feld7 == Players.NoOne) list.Add(7);
			if (feld8 == Players.NoOne) list.Add(8);
			if (feld9 == Players.NoOne) list.Add(9);

			return list;
		}
		public void SetByNum(int i, Players player, bool force = false)
		{
			if (force)
			{
				if (i == 1) feld1 = player;
				if (i == 2) feld2 = player;
				if (i == 3) feld3 = player;
				if (i == 4) feld4 = player;
				if (i == 5) feld5 = player;
				if (i == 6) feld6 = player;
				if (i == 7) feld7 = player;
				if (i == 8) feld8 = player;
				if (i == 9) feld9 = player;
				return;
			}

			if (i == 1 || feld1 == Players.NoOne) feld1 = player;
			if (i == 2 || feld2 == Players.NoOne) feld2 = player;
			if (i == 3 || feld3 == Players.NoOne) feld3 = player;
			if (i == 4 || feld4 == Players.NoOne) feld4 = player;
			if (i == 5 || feld5 == Players.NoOne) feld5 = player;
			if (i == 6 || feld6 == Players.NoOne) feld6 = player;
			if (i == 7 || feld7 == Players.NoOne) feld7 = player;
			if (i == 8 || feld8 == Players.NoOne) feld8 = player;
			if (i == 9 || feld9 == Players.NoOne) feld9 = player;
		}

	}

	class Feld
	{
		private Players fieldOwner = Players.NoOne;

		public readonly int Spalte;
		public readonly int Zeile;
		public readonly int id;
		private TikTakToe owner;
		private Label _label;
		private Panel _panel;
		private Point _location;
		public Point Location { get { return _location; } }
		private Size _size;
		public Size Size { get { return _size; } }
		private int _marginWidth;
		public int MarginWidth { get { return _marginWidth; } }

		public Feld(ref Form frm, TikTakToe owner, int zeile, int spalte, int id, Players player = Players.NoOne)
		{
			this.owner = owner;
			this.id = id;
			this.Spalte = spalte;
			this.Zeile = zeile;

			int size = 81;
			int marginWidth = 1;

			Panel panel = new Panel();
			panel.BackColor = Color.White;
			panel.Size = new Size(size, size);
			panel.Margin = new Padding(marginWidth);
			panel.BorderStyle = BorderStyle.FixedSingle;
			panel.Location = new Point(spalte * size + 2 * marginWidth, zeile * size + 2 * marginWidth);
			panel.Click += Click_Event;
			this._panel = panel;

			frm.Controls.Add(panel);

			Label lbl = new Label();
			lbl.ForeColor = Color.Black;
			lbl.BackColor = Color.White;
			lbl.BorderStyle = BorderStyle.FixedSingle;
			lbl.AutoSize = false;
			lbl.TextAlign = ContentAlignment.MiddleCenter;
			lbl.Font = new Font(lbl.Font.FontFamily, size);
			lbl.Size = panel.Size;
			lbl.Location = panel.Location;
			lbl.Click += Click_Event;
			this._label = lbl;
			SetStatus(player, true);
			frm.Controls.Add(lbl);

			_location = lbl.Location;
			_size = lbl.Size;
			_marginWidth = marginWidth;

			lbl.BringToFront();
		}

		public async Task<bool> SetStatus(Players newOwner, bool force = false)
		{
			if (!force)
			{
				if (fieldOwner != Players.NoOne || newOwner == Players.NoOne)
				{
					return false;
				}
			}
			fieldOwner = newOwner;

			if (fieldOwner == Players.Player)
			{
				this._label.Text = "X";
				Task t = Helper.ChangeColor(this._label, Color.Red);
				await t;
				this._label.ForeColor = Color.Blue;
			}
			else if (fieldOwner == Players.Computer)
			{
				this._label.Text = "O";
				Task t = Helper.ChangeColor(this._label, Color.Blue);
				await t;
				this._label.ForeColor = Color.Red;
			}
			return true;
		}

		public Players GetStatus()
		{
			return this.fieldOwner;
		}

		private void Click_Event(object sender, EventArgs e)
		{
			Click(false);
		}
		public bool Click(bool computer)
		{
			if (owner.Turn == Players.Player && computer == false)
			{
				if (SetStatus(Players.Player).Result)
				{
					owner.Turn = Players.Computer;
					owner.ComputerTurn();
				}
				else
				{
					return false;
				}
			}
			else if (owner.Turn == Players.Computer && computer == true)
			{
				if (SetStatus(Players.Computer).Result)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}
	}

	public class Helper
	{
		internal static async Task ChangeColor(Control ctrl, Color color)
		{
			SafeInvoke(ctrl, delegate () { ctrl.BackColor = color; }, false);
			ctrl.Refresh();
			ctrl.Update();
		}
		private static void SafeInvoke(Control uiElement, Action updater, bool forceSynchronous)
		{
			if (uiElement == null)
			{
				throw new ArgumentNullException("uiElement");
			}

			if (uiElement.InvokeRequired)
			{
				if (forceSynchronous)
				{
					uiElement.Invoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
				}
				else
				{
					uiElement.BeginInvoke((Action)delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
				}
			}
			else
			{
				if (uiElement.IsDisposed)
				{
					throw new ObjectDisposedException("Control is already disposed.");
				}

				updater();
			}
		}

		public async static Task ToFront(Control ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.BringToFront(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}
	}

	enum Players{
		NoOne, Computer, Player
	}

}