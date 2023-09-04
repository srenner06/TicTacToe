using CustomMessageBox.Private;
using TikTakToe.Enums;

namespace TikTakToe
{
    class TikTakToe : IDisposable
	{

		private Schwierigkeit _schwierigkeit;
		public Schwierigkeit Schwierigkeit => _schwierigkeit;

		private Modus _modus;
		public Modus Modus => _modus;


		public Bord Bord { get; private set; }

		public Players PvCTurn { get; set; }

		public Players PvPTurn { get; set; }

		private static Random rnd = new(Guid.NewGuid().GetHashCode());

		private static readonly Lazy<IniFile> _ini = new(() => new IniFile("settings.ini"));
		private static IniFile Ini => _ini.Value;


		public const char colorSplitter = ',';

		public Color computerColor { get; private set; } = Color.Blue;
		public Color playerColor { get; private set; } = Color.Red;

		public Color player1Color { get; private set; } = Color.Blue;
		public Color player2Color { get; private set; } = Color.Red;


		public TikTakToe()
		{
			Bord = new Bord(this);

			//PvC
			if (Ini.KeyExists("computerColor", "PvC"))
			{
				try
				{
					int[] c = Ini.Read("computerColor", "PvC").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					computerColor = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
			if (Ini.KeyExists("playerColor", "PvC"))
			{
				try
				{
					int[] c = Ini.Read("playerColor", "PvC").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					playerColor = Color.FromArgb(c[0], c[1], c[2], c[3]);
				} catch{ }
			}

			//PvP
			if (Ini.KeyExists("player1Color", "PvP"))
			{
				try
				{
					int[] c = Ini.Read("player1Color", "PvP").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					player1Color = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
			if (Ini.KeyExists("player2Color", "PvP"))
			{
				try
				{
					int[] c = Ini.Read("player2Color", "PvP").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					player2Color = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
		}

		public Task Start()
		{
			Helper.Activate(Bord.form);

			this.Bord.ResetFields();
			while (true)
			{
				MsgForm frm = new MsgForm("Gegen wen wollen Sie spielen?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
				frm.SetButtonsText("Spieler", "Computer", "Online");
				DialogResult answer = frm.ShowDialog();
				if (answer == DialogResult.Abort)
				{
					_modus = Modus.PvP;
					if (player1Color == player2Color)
						SetColorsPvP();
					frm = new MsgForm("", "", MessageBoxButtons.YesNo);
					frm.SetButtonsText("Starten", "Farben ändern");
					DialogResult a = frm.ShowDialog(this.Bord.form);
					if (a == DialogResult.Yes)
					{
						PvCTurn = Players.NoOne;
						PvPTurn = Players.Player1;
						break;
					}
					else if (a == DialogResult.Retry)
					{
						SetColorsPvP();
					}
					else
					{
						return Task.CompletedTask;
					}
				}
				else if (answer == DialogResult.Retry)
				{
					_modus = Modus.PvC;

					if (computerColor == playerColor)
						SetColorsPvC();

					frm = new MsgForm("Welchen Schwierigkeit wollen sie spielen?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button4);
					frm.SetButtonsText("Einfach", "Mittel", "Unmöglich");

					DialogResult result = frm.ShowDialog(this.Bord.form);
					if (result == DialogResult.Abort)
					{
						_schwierigkeit = Schwierigkeit.einfach;
					}
					else if (result == DialogResult.Retry)
					{
						_schwierigkeit = Schwierigkeit.mittel;
					}
					else if (result == DialogResult.Ignore)
					{
						_schwierigkeit = Schwierigkeit.unmöglich;
					}
					else
					{
						return Task.CompletedTask;
					}

					MsgForm starter = new MsgForm("Wer soll starten?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
					starter.SetButtonsText("Sie", "Computer", "Farben ändern");
					DialogResult starterResult = starter.ShowDialog(this.Bord.form);
					if (starterResult == DialogResult.Abort)
					{
						PvCTurn = Players.Player1;
						break;
					}
					else if (starterResult == DialogResult.Retry)
					{
						PvCTurn = Players.Computer_Player2;
						break;
					}
					else if (starterResult == DialogResult.Ignore)
					{
						SetColorsPvC();
					}
					else
					{
						return Task.CompletedTask;
					}
				}
				else if (answer == DialogResult.Ignore)
				{
					MsgForm f = new MsgForm("Dieser Modus existiert nocht nicht");
					f.ShowDialog();
				}
				else
				{
					return Task.CompletedTask;
				}
			}
			Bord.Show(PvCTurn);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			Bord.Dispose();
		}

		private void SetColorsPvC()
		{
			MsgForm f;
			ColorDialog dialog = new ColorDialog();
			do
			{
				f = new MsgForm("Bitte die Farbe für den Computer auswählen");
				//f.Activate();
				f.ShowDialog();
				dialog.ShowDialog();
				computerColor = dialog.Color;

				if (computerColor != Color.White)
				{
					Ini.Write("computerColor", computerColor.A.ToString() + colorSplitter + computerColor.R.ToString() + colorSplitter + computerColor.G.ToString() + colorSplitter + computerColor.B.ToString(), "PvC");
					f = new MsgForm($"Die Farbe für den Computer wurde auf \"{dialog.Color.Name}\" gesetzt");
					//f.Activate();
					f.ShowDialog();
					break;
				}
				else
				{
					f = new MsgForm("Die Farbe darf nicht Weiss sein", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}

			} while (true);

			do
			{
				f = new MsgForm("Bitte die Farbe für den Spieler wählen");
				//f.Activate();
				f.ShowDialog();
				dialog.ShowDialog();
				playerColor = dialog.Color;
				if (playerColor == computerColor)
				{
					f = new MsgForm("Der Computer und der Spieler dürfen nicht die gleiche Farbe haben", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}
				else if (playerColor == Color.White)
				{
					f = new MsgForm("Die Farbe darf nicht Weiss sein", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}
				else
				{
					Ini.Write("computerColor", playerColor.A.ToString() + colorSplitter + playerColor.R.ToString() + colorSplitter + playerColor.G.ToString() + colorSplitter + playerColor.B.ToString(), "PvC");
					f = new MsgForm($"Die Farbe für den Spieler wurde auf  \"{dialog.Color.Name}\" gesetzt");
					//f.Activate();
					f.ShowDialog();
					break;
				}
			} while (true);
		}
		private void SetColorsPvP()
		{
			MsgForm f;
			ColorDialog dialog = new ColorDialog();
			do
			{
				f = new MsgForm("Bitte die Farbe für Spieler 1 auswählen");
				//f.Activate();
				f.ShowDialog();
				dialog.ShowDialog();
				computerColor = dialog.Color;

				if (computerColor != Color.White)
				{
					Ini.Write("player1Color", computerColor.A.ToString() + colorSplitter + computerColor.R.ToString() + colorSplitter + computerColor.G.ToString() + colorSplitter + computerColor.B.ToString(), "PvP");
					f = new MsgForm($"Die Farbe für Spieler 1 wurde auf \"{dialog.Color.Name}\" gesetzt");
					//f.Activate();
					f.ShowDialog();
					break;
				}
				else
				{
					f = new MsgForm("Die Farbe darf nicht Weiss sein", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}

			} while (true);

			do
			{
				f = new MsgForm("Bitte die Farbe für den Spieler 2 wählen");
				//f.Activate();
				f.ShowDialog();
				dialog.ShowDialog();
				playerColor = dialog.Color;
				if (playerColor == computerColor)
				{
					f = new MsgForm("Spieler 1 und Spieler 2 dürfen nicht die gleiche Farbe haben", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}
				else if (playerColor == Color.White)
				{
					f = new MsgForm("Die Farbe darf nicht Weiss sein", "", MessageBoxIcon.Warning);
					//f.Activate();
					f.ShowDialog();
				}
				else
				{
					Ini.Write("player2Color", playerColor.A.ToString() + colorSplitter + playerColor.R.ToString() + colorSplitter + playerColor.G.ToString() + colorSplitter + playerColor.B.ToString(), "PvP");
					f = new MsgForm($"Die Farbe für Spieler 2 wurde auf  \"{dialog.Color.Name}\" gesetzt");
					//f.Activate();
					f.ShowDialog();
					break;
				}
			} while (true);
		}

		public void ComputerTurn()
		{
			CheckVictoryPvC();
			
			if (PvCTurn != Players.Computer_Player2) return;
			//_bord.SetVorhangText("Computer_Player2 berechnet seinen Zug...");
			//_bord.SetVorhangColor(Color.Green);
			Bord.StartVorhang("Der Computer berechnet seinen Zug...");
			//_bord.Vorhang();
			//_bord.Vorhang.BringToFront();
			//Form frm2 = new Form();
			//frm2.Controls.Add(_bord.Vorhang);
			//frm2.ShowDialog();

			if (_schwierigkeit == Schwierigkeit.einfach)
			{
				List<Feld> freeFields = Bord.GetFreeFields();
				Feld fld = freeFields[rnd.Next(freeFields.Count)];
				fld.Click(true);
			}
			else if (_schwierigkeit == Schwierigkeit.mittel)
			{
				Feld? fieldToVictory = CheckIfPossibleWin(Players.Computer_Player2);
				if (fieldToVictory is not null)
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player1 can win
					Feld? fieldToDefeat = CheckIfPossibleWin(Players.Player1);
					if (fieldToDefeat is not null)
					{
						fieldToDefeat.Click(true);
					}
					else
					{
						var freeCorners = new List<int>();
						if (Bord.GetFieldById(5)!.GetStatus() == Players.NoOne)
						{
							Bord.GetFieldById(5)!.Click(true);
						}
						else
						{
							if (Bord.GetFieldById(1)!.GetStatus() == Players.NoOne)
							{
								freeCorners.Add(1);
							}
							if (Bord.GetFieldById(3)!.GetStatus() == Players.NoOne)
							{
								freeCorners.Add(3);
							}
							if (Bord.GetFieldById(7)!.GetStatus() == Players.NoOne)
							{
								freeCorners.Add(7);
							}
							if (Bord.GetFieldById(9)!.GetStatus() == Players.NoOne)
							{
								freeCorners.Add(9);
							}

							if (freeCorners.Count != 0)
							{
								int id = freeCorners[rnd.Next(freeCorners.Count)];
								Bord.GetFieldById(id)!.Click(true);
							}
							else
							{
								var freeFields = Bord.GetFreeFields();
								Feld fld = freeFields[rnd.Next(freeFields.Count)];
								fld.Click(true);
							}
						}
					}
				}
			}
			else if (_schwierigkeit == Schwierigkeit.unmöglich)
			{
				Feld? fieldToVictory = CheckIfPossibleWin(Players.Computer_Player2);
				if (fieldToVictory is not null)
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player1 can win
					Feld? fieldToDefeat = CheckIfPossibleWin(Players.Player1);
					if (fieldToDefeat is not null)
					{
						fieldToDefeat.Click(true);
					}
					else
					{
						int id = FindBestTurn(this.Bord, Players.Computer_Player2, 0);
						Bord.GetFieldById(id)?.Click(true);
					}
				}
			}

			Bord.CloseVorhang();

			CheckVictoryPvC();
			PvCTurn = Players.Player1;
			Helper.Activate(Bord.form);
		}

		public Feld? CheckIfPossibleWin(Players checkedPlayer)
		{
			Feld? feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld1, Bord.feld2, Bord.feld3, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld4, Bord.feld5, Bord.feld6, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld7, Bord.feld8, Bord.feld9, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld1, Bord.feld4, Bord.feld7, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld2, Bord.feld5, Bord.feld8, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld3, Bord.feld6, Bord.feld9, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld1, Bord.feld5, Bord.feld9, checkedPlayer);
			if (feldToVictory is not null) return feldToVictory;
			feldToVictory = CheckPossibleWin(Bord.feld3, Bord.feld5, Bord.feld7, checkedPlayer);
			return feldToVictory;
		}

		public static Feld? CheckPossibleWin(Feld feld1, Feld feld2, Feld feld3, Players checkedPlayer)
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

		public void CheckVictoryPvC()
		{
			string text = "";
			if (CheckWin(Bord.feld1, Bord.feld2, Bord.feld3, Players.Player1) || CheckWin(Bord.feld4, Bord.feld5, Bord.feld6, Players.Player1) || CheckWin(Bord.feld7, Bord.feld8, Bord.feld9, Players.Player1) || CheckWin(Bord.feld1, Bord.feld4, Bord.feld7, Players.Player1) || CheckWin(Bord.feld2, Bord.feld5, Bord.feld8, Players.Player1) || CheckWin(Bord.feld3, Bord.feld6, Bord.feld9, Players.Player1) || CheckWin(Bord.feld1, Bord.feld5, Bord.feld9, Players.Player1) || CheckWin(Bord.feld3, Bord.feld5, Bord.feld7, Players.Player1))
			{
				text = "Sie haben gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (CheckWin(Bord.feld1, Bord.feld2, Bord.feld3, Players.Computer_Player2) || CheckWin(Bord.feld4, Bord.feld5, Bord.feld6, Players.Computer_Player2) || CheckWin(Bord.feld7, Bord.feld8, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld1, Bord.feld4, Bord.feld7, Players.Computer_Player2) || CheckWin(Bord.feld2, Bord.feld5, Bord.feld8, Players.Computer_Player2) || CheckWin(Bord.feld3, Bord.feld6, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld1, Bord.feld5, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld3, Bord.feld5, Bord.feld7, Players.Computer_Player2))
			{
				text ="Sie haben verloren\nMöchten Sie noch einmal spielen?";
			}
			else if (Bord.GetFreeFields().Count == 0)
			{
				text = "Unentschieden\nMöchten Sie noch einmal spielen?";
			}

			if (text != "")
			{
				End(text);
			}
		}
		public void CheckVictoryPvP()
		{
			string text = "";

			if (CheckWin(Bord.feld1, Bord.feld2, Bord.feld3, Players.Player1) || CheckWin(Bord.feld4, Bord.feld5, Bord.feld6, Players.Player1) || CheckWin(Bord.feld7, Bord.feld8, Bord.feld9, Players.Player1) || CheckWin(Bord.feld1, Bord.feld4, Bord.feld7, Players.Player1) || CheckWin(Bord.feld2, Bord.feld5, Bord.feld8, Players.Player1) || CheckWin(Bord.feld3, Bord.feld6, Bord.feld9, Players.Player1) || CheckWin(Bord.feld1, Bord.feld5, Bord.feld9, Players.Player1) || CheckWin(Bord.feld3, Bord.feld5, Bord.feld7, Players.Player1))
			{
				text = "Spieler 1 hat gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (CheckWin(Bord.feld1, Bord.feld2, Bord.feld3, Players.Computer_Player2) || CheckWin(Bord.feld4, Bord.feld5, Bord.feld6, Players.Computer_Player2) || CheckWin(Bord.feld7, Bord.feld8, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld1, Bord.feld4, Bord.feld7, Players.Computer_Player2) || CheckWin(Bord.feld2, Bord.feld5, Bord.feld8, Players.Computer_Player2) || CheckWin(Bord.feld3, Bord.feld6, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld1, Bord.feld5, Bord.feld9, Players.Computer_Player2) || CheckWin(Bord.feld3, Bord.feld5, Bord.feld7, Players.Computer_Player2))
			{
				text = "Spieler 2 hat gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (Bord.GetFreeFields().Count == 0)
			{
				text = "Unentschieden\nMöchten Sie noch einmal spielen?";
			}

			if (text != "")
			{
				End(text);
			}
		}
		private void End(string text)
		{
			PvCTurn = Players.NoOne;
			var frm = new MsgForm(text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
			if (frm.ShowDialog(this.Bord.form) == DialogResult.Yes)
			{
				Helper.Hide(Bord.form);
				Start();
			}
			Bord.form.Close();
		}
		public static bool CheckWin(Feld feld1, Feld feld2, Feld feld3, Players player)
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

			bool max = player == Players.Computer_Player2;
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
				if (winner == Players.Computer_Player2)
					return 1;
				else
					return -1;
			}
			if (bord.GetFreeFields().Count == 0)
				return 0;

			if (player == Players.Computer_Player2)
			{
				return FindBestTurn(bord, Players.Player1, count + 1);
			}
			else
			{
				return FindBestTurn(bord, Players.Computer_Player2, count + 1);
			}
		}
	}

}