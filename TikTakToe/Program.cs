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
				CustomMessageBox.Private.FormMessageBox starter = new CustomMessageBox.Private.FormMessageBox("Wollen Sie starten?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				DialogResult starterResult = starter.ShowDialog();
				if (starterResult == DialogResult.Yes)
				{
					_turn = Players.Player;
				}
				else if (starterResult == DialogResult.No)
				{
					_turn = Players.Computer;
				}
				else
				{
					return;
				}

				CustomMessageBox.Private.FormMessageBox frm = new CustomMessageBox.Private.FormMessageBox("Welchen Modus wollen sie spielen?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button4);
				frm.SetButtonsText("Einfach", "Mittel", "Unmöglich");

				DialogResult result = frm.ShowDialog();
				if (result == DialogResult.Abort)
				{
					_modus = Mode.einfach;
					break;
				}
				else if (result == DialogResult.Retry)
				{
					_modus = Mode.mittel;
					break;
				}
				else if (result == DialogResult.Ignore)
				{
					_modus = Mode.unmöglich;
					MessageBox.Show("Dieser Modus ist noch in Testphase");
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
				if (_bord.GetFreeFields().Count == 9)
				{
					_bord.feld1.Click(true);
				}
				else
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
							int id = FindBestTurn(this.Bord, Players.Computer, 0);
							_bord.GetFieldById(id).Click(true);
						}
					}
					_bord.stopVorhang = true;
					_bord.CloseVorhang();
				}
			}

			CheckVictory();
			_turn = Players.Player;
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
			if (_bord.GetFreeFields().Count == 0)
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
			else if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Player) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Player) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Player) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Player) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Player) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Player) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Player) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Player))
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
		}

		public bool CheckWin(Feld feld1, Feld feld2, Feld feld3, Players player)
		{
			if (feld1.GetStatus() == player && feld2.GetStatus() == player && feld3.GetStatus() == player)
			{
				return true;
			}
			return false;
		}

		//public Tuple<int, int> MiniMax(Bord bord, bool Computer, int depth = 0)
		//{
		//	int biggestScore = 0;
		//	int score = 0;
		//	if (Computer)
		//	{
		//		Bord clone = bord.Clone();
		//		foreach (Feld fld in clone.GetFreeFields())
		//		{
		//			fld.SetStatus(Players.Computer);
		//			Players result = MiniMaxWin(bord);
		//			if (result == Players.Computer)
		//			{
		//				return Tuple.Create(1, fld.id);
		//			}
		//			else if (result == Players.Player)
		//			{
		//				score = -1;
		//			}
		//			else if (result == Players.NoOne && clone.GetFreeFields().Count == 0) score = 0;
		//			if (score != -2) return Tuple.Create(score, fld.id);
		//			Tuple<int, int> newScore = MiniMax(clone, false, depth + 1);
		//			score = newScore.Item2;
		//			if (score > biggestScore) biggestScore = score;
		//			if (biggestScore == 1)
		//			{
		//				return newScore;
		//			}
		//		}
		//	}
		//	else
		//	{
		//		Bord clone = bord.Clone();
		//		foreach (Feld fld in clone.GetFreeFields())
		//		{

		//			fld.SetStatus(Players.Computer);
		//			Players result = MiniMaxWin(bord);
		//			if (result == Players.Computer)
		//			{
		//				score = 1;
		//			}
		//			else if (result == Players.Player)
		//			{
		//				score = -1;
		//			}
		//			else if (result == Players.NoOne && clone.GetFreeFields().Count == 0) score = 0;
		//			if (score != -2) return Tuple.Create(score, fld.id);
		//			MiniMax(clone, true);
		//		}
		//	}
		//	return null;
		//}

		public Players MiniMaxWin(Bord bord)
		{
			if (checkWinMiniMax(bord.feld1, bord.feld2, bord.feld3, Players.Player) || checkWinMiniMax(bord.feld4, bord.feld5, bord.feld6, Players.Player) || checkWinMiniMax(bord.feld7, bord.feld8, bord.feld9, Players.Player) || checkWinMiniMax(bord.feld1, bord.feld4, bord.feld7, Players.Player) || checkWinMiniMax(bord.feld2, bord.feld5, bord.feld8, Players.Player) || checkWinMiniMax(bord.feld3, bord.feld6, bord.feld9, Players.Player) || checkWinMiniMax(bord.feld1, bord.feld5, bord.feld9, Players.Player) || checkWinMiniMax(bord.feld3, bord.feld5, bord.feld7, Players.Player))
			{
				return Players.Player;
			}
			else if (checkWinMiniMax(bord.feld1, bord.feld2, bord.feld3, Players.Computer) || checkWinMiniMax(bord.feld4, bord.feld5, bord.feld6, Players.Computer) || checkWinMiniMax(bord.feld7, bord.feld8, bord.feld9, Players.Computer) || checkWinMiniMax(bord.feld1, bord.feld4, bord.feld7, Players.Computer) || checkWinMiniMax(bord.feld2, bord.feld5, bord.feld8, Players.Computer) || checkWinMiniMax(bord.feld3, bord.feld6, bord.feld9, Players.Computer) || checkWinMiniMax(bord.feld1, bord.feld5, bord.feld9, Players.Computer) || checkWinMiniMax(bord.feld3, bord.feld5, bord.feld7, Players.Computer))
			{
				return Players.Computer;
			}
			return Players.NoOne;
		}
		private bool checkWinMiniMax(Feld feld1, Feld feld2, Feld feld3, Players player)
		{
			return checkWinMiniMax(new Players[] { feld1.GetStatus(), feld2.GetStatus(), feld3.GetStatus() }, player);
		}

		private bool checkWinMiniMax(Players[] field, Players player)
		{
			if (field.All(x => x == player))
			{
				return true;
			}
			return false;
		}

		private int FindBestTurn(Bord bord, Players player, int count)
		{
			Bord clone = bord.Clone();
			bool max = player == Players.Computer;
			Dictionary<string, int> best = new Dictionary<string, int>();
			best.Add("id", -1);
			best.Add("sc", 2);
			if (max)
				best["sc"] = -2;

			foreach (Feld fld in clone.GetFreeFields())
			{
				fld.SetStatus(player, true);
				Dictionary<string, int> curr = new Dictionary<string, int>();
				curr.Add("id", fld.id);
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
				fld.SetStatus(Players.NoOne, true);

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

		private int score(Bord bord, Players player, int count)
		{
			Players winner = MiniMaxWin(bord);
			if (winner != Players.NoOne)
			{
				if (winner == player)
					return -1;
				else
					return 1;
			}
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

	public class TransparentLabel : Control
	{
		/// <summary>
		/// Creates a new <see cref="TransparentLabel"/> instance.
		/// </summary>
		public TransparentLabel()
		{
			TabStop = false;
		}

		/// <summary>
		/// Gets the creation parameters.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20;
				return cp;
			}
		}

		/// <summary>
		/// Paints the background.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// do nothing
		}

		/// <summary>
		/// Paints the control.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			DrawText();
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == 0x000F)
			{
				DrawText();
			}
		}

		private void DrawText()
		{
			using (Graphics graphics = CreateGraphics())
			using (SolidBrush brush = new SolidBrush(ForeColor))
			{
				SizeF size = graphics.MeasureString(Text, Font);

				// first figure out the top
				float top = 0;
				switch (textAlign)
				{
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleRight:
						top = (Height - size.Height) / 2;
						break;
					case ContentAlignment.BottomLeft:
					case ContentAlignment.BottomCenter:
					case ContentAlignment.BottomRight:
						top = Height - size.Height;
						break;
				}

				float left = -1;
				switch (textAlign)
				{
					case ContentAlignment.TopLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.BottomLeft:
						if (RightToLeft == RightToLeft.Yes)
							left = Width - size.Width;
						else
							left = -1;
						break;
					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						left = (Width - size.Width) / 2;
						break;
					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						if (RightToLeft == RightToLeft.Yes)
							left = -1;
						else
							left = Width - size.Width;
						break;
				}
				graphics.DrawString(Text, Font, brush, left, top);
			}
		}

		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <returns>
		/// The text associated with this control.
		/// </returns>
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				RecreateHandle();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// One of the <see cref="T:System.Windows.Forms.RightToLeft"/> values. The default is <see cref="F:System.Windows.Forms.RightToLeft.Inherit"/>.
		/// </returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
		/// The assigned value is not one of the <see cref="T:System.Windows.Forms.RightToLeft"/> values.
		/// </exception>
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
				RecreateHandle();
			}
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The <see cref="T:System.Drawing.Font"/> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont"/> property.
		/// </returns>
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				RecreateHandle();
			}
		}

		private ContentAlignment textAlign = ContentAlignment.TopLeft;
		/// <summary>
		/// Gets or sets the text alignment.
		/// </summary>
		public ContentAlignment TextAlign
		{
			get { return textAlign; }
			set
			{
				textAlign = value;
				RecreateHandle();
			}
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