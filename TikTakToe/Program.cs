using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using CustomMessageBox.Private;
using System.Linq;

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
	enum Schwierigkeit { einfach, mittel, unmöglich }

	class TikTakToe
	{
		private bool _ended = false;
		public bool Ended { get { return _ended; } }

		private Schwierigkeit _schwierigkeit;
		public Schwierigkeit Schwierigkeit { get { return _schwierigkeit; } }

		private Modus _modus;
		public Modus Modus { get { return _modus; } }


		private Bord _bord;

		private Players _PvCTurn;
		public Players PvCTurn { get { return _PvCTurn; } set { _PvCTurn = value; } }

		private Players _PvPTurn;
		public Players PvPTurn { get { return _PvPTurn; } set { _PvPTurn = value; } }

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

		private static Random rnd;
		private static IniFile ini;
		public const char colorSplitter = ',';

		public Color computerColor { get; private set; } = Color.Blue;
		public Color playerColor { get; private set; } = Color.Red;

		public Color player1Color { get; private set; } = Color.Blue;
		public Color player2Color { get; private set; } = Color.Red;


		public TikTakToe()
		{
			ini = new IniFile("settings.ini");
			rnd = new Random();
			Bord = new Bord(this);

			//PvC
			if (ini.KeyExists("computerColor", "PvC"))
			{
				try
				{
					int[] c = ini.Read("computerColor", "PvC").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					computerColor = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
			if (ini.KeyExists("playerColor", "PvC"))
			{
				try
				{
					int[] c = ini.Read("playerColor", "PvC").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					playerColor = Color.FromArgb(c[0], c[1], c[2], c[3]);
				} catch{ }
			}

			//PvP
			if (ini.KeyExists("player1Color", "PvP"))
			{
				try
				{
					int[] c = ini.Read("player1Color", "PvP").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					player1Color = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
			if (ini.KeyExists("player2Color", "PvP"))
			{
				try
				{
					int[] c = ini.Read("player2Color", "PvP").Split(colorSplitter).Select(i => int.Parse(i)).ToArray();
					player2Color = Color.FromArgb(c[0], c[1], c[2], c[3]);
				}
				catch { }
			}
		}

		public async void Start()
		{
			Task t = Helper.Activate(this.Bord.form);
			await t;

			this.Bord.ResetFields();
			while (true)
			{
				MsgForm frm = new MsgForm("Gegen wen wollen Sie spielen?", "", MessageBoxButtons.AbortRetryIgnore);
				frm.SetButtonsText("Spieler", "Computer", "");
				DialogResult answer = frm.ShowDialog();
				if (answer == DialogResult.Abort)
				{
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
					else if (a == DialogResult.Ignore)
					{
						MsgForm f = new MsgForm("Dieser Modus existiert nocht nicht");
						f.ShowDialog();
						return;
					}
					else
					{
						return;
					}
				}
				else if (answer == DialogResult.No)
				{

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
						return;
					}

					startfrage:
					MsgForm starter = new MsgForm("Wer soll starten?", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
					starter.SetButtonsText("Sie", "Computer", "Farben ändern");
					DialogResult starterResult = starter.ShowDialog(this.Bord.form);
					if (starterResult == DialogResult.Abort)
					{
						_PvCTurn = Players.Player1;
						break;
					}
					else if (starterResult == DialogResult.Retry)
					{
						_PvCTurn = Players.Computer_Player2;
						break;
					}
					else if (starterResult == DialogResult.Ignore)
					{
						SetColorsPvC();
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}
			}
			Bord.Show(_PvCTurn);
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
					ini.Write("computerColor", computerColor.A.ToString() + colorSplitter + computerColor.R.ToString() + colorSplitter + computerColor.G.ToString() + colorSplitter + computerColor.B.ToString(), "PvC");
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
					ini.Write("computerColor", playerColor.A.ToString() + colorSplitter + playerColor.R.ToString() + colorSplitter + playerColor.G.ToString() + colorSplitter + playerColor.B.ToString(), "PvC");
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
					ini.Write("player1Color", computerColor.A.ToString() + colorSplitter + computerColor.R.ToString() + colorSplitter + computerColor.G.ToString() + colorSplitter + computerColor.B.ToString(), "PvP");
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
					ini.Write("player2Color", playerColor.A.ToString() + colorSplitter + playerColor.R.ToString() + colorSplitter + playerColor.G.ToString() + colorSplitter + playerColor.B.ToString(), "PvP");
					f = new MsgForm($"Die Farbe für Spieler 2 wurde auf  \"{dialog.Color.Name}\" gesetzt");
					//f.Activate();
					f.ShowDialog();
					break;
				}
			} while (true);
		}

		public async void ComputerTurn()
		{
			CheckVictoryPvC();
			
			if (_PvCTurn != Players.Computer_Player2) return;
			//_bord.SetVorhangText("Computer_Player2 berechnet seinen Zug...");
			//_bord.SetVorhangColor(Color.Green);
			_bord.stopVorhang = false;
			_bord.startVorhang = false;
			//_bord.Vorhang();
			//_bord.Vorhang.BringToFront();
			//Form frm2 = new Form();
			//frm2.Controls.Add(_bord.Vorhang);
			//frm2.ShowDialog();

			if (_schwierigkeit == Schwierigkeit.einfach)
			{
				List<Feld> freeFields = _bord.GetFreeFields();
				Feld fld = freeFields[rnd.Next(freeFields.Count)];
				fld.Click(true);
			}
			else if (_schwierigkeit == Schwierigkeit.mittel)
			{
				Feld fieldToVictory = CheckIfPossibleWinn(Players.Computer_Player2);
				if (!(fieldToVictory is null))
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player1 can win
					Feld fieldToDefeat = CheckIfPossibleWinn(Players.Player1);
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
			else if (_schwierigkeit == Schwierigkeit.unmöglich)
			{
				var task1 = Task.Factory.StartNew(() => _bord.Vorhang());
				_bord.startVorhang = true;
				Feld fieldToVictory = CheckIfPossibleWinn(Players.Computer_Player2);
				if (!(fieldToVictory is null))
				{
					fieldToVictory.Click(true);
				}
				else
				{
					//Check if Player1 can win
					Feld fieldToDefeat = CheckIfPossibleWinn(Players.Player1);
					if (!(fieldToDefeat is null))
					{
						fieldToDefeat.Click(true);
					}
					else
					{
						int id = FindBestTurn(new ShallowBord(this.Bord), Players.Computer_Player2, 0);
						_bord.GetFieldById(id).Click(true);
					}
				}
			}
			_bord.stopVorhang = true;
			_bord.CloseVorhang();

			CheckVictoryPvC();
			_PvCTurn = Players.Player1;
			Task t = Helper.Activate(this.Bord.form);
			await t;
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

		public void CheckVictoryPvC()
		{
			string text = "";
			if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Player1) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Player1) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Player1) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Player1) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Player1) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Player1) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Player1) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Player1))
			{
				text = "Sie haben gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Computer_Player2) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Computer_Player2) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Computer_Player2) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Computer_Player2) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Computer_Player2))
			{
				text ="Sie haben verloren\nMöchten Sie noch einmal spielen?";
			}
			else if (_bord.GetFreeFields().Count == 0)
			{
				text = "Unentschieden\nMöchten Sie noch einmal spielen?";
			}

			if (text != "")
			{
				_ended = true;
				_PvCTurn = Players.NoOne;
				MsgForm frm = new MsgForm(text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (frm.ShowDialog(this.Bord.form) == DialogResult.Yes)
				{
					//this._bord.form.Dispose();
					Application.Restart();
					Environment.Exit(0);
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
		public void CheckVictoryPvP()
		{
			string text = "";

			if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Player1) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Player1) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Player1) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Player1) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Player1) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Player1) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Player1) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Player1))
			{
				text = "Spieler 1 hat gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (CheckWin(_bord.feld1, _bord.feld2, _bord.feld3, Players.Computer_Player2) || CheckWin(_bord.feld4, _bord.feld5, _bord.feld6, Players.Computer_Player2) || CheckWin(_bord.feld7, _bord.feld8, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld1, _bord.feld4, _bord.feld7, Players.Computer_Player2) || CheckWin(_bord.feld2, _bord.feld5, _bord.feld8, Players.Computer_Player2) || CheckWin(_bord.feld3, _bord.feld6, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld1, _bord.feld5, _bord.feld9, Players.Computer_Player2) || CheckWin(_bord.feld3, _bord.feld5, _bord.feld7, Players.Computer_Player2))
			{
				text = "Spieler 2 hat gewonnen\nMöchten Sie noch einmal spielen?";
			}
			else if (_bord.GetFreeFields().Count == 0)
			{
				text = "Unentschieden\nMöchten Sie noch einmal spielen?";
			}

			if (text != "")
			{
				_ended = true;
				_PvCTurn = Players.NoOne;
				MsgForm frm = new MsgForm(text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (frm.ShowDialog(this.Bord.form) == DialogResult.Yes)
				{
					//this._bord.form.Dispose();
					Application.Restart();
					Environment.Exit(0);
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
			this.vorhang = new MsgForm("Der Computer_Player2 berechnet seinen Zug...");
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
		
		public async void ResetFields()
		{
			await feld1.SetStatus(Players.NoOne, true);
			await feld2.SetStatus(Players.NoOne, true);
			await feld3.SetStatus(Players.NoOne, true);
			await feld4.SetStatus(Players.NoOne, true);
			await feld5.SetStatus(Players.NoOne, true);
			await feld6.SetStatus(Players.NoOne, true);
			await feld7.SetStatus(Players.NoOne, true);
			await feld8.SetStatus(Players.NoOne, true);
			await feld9.SetStatus(Players.NoOne, true);
		}
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
			Players opposite = Players.Computer_Player2;
			if (player == Players.Computer_Player2)
				opposite = Players.Player1;

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
		//private Panel panel;
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

			//Panel panel = new Panel();
			//panel.BackColor = Color.White;
			//panel.Size = new Size(size, size);
			//panel.Margin = new Padding(marginWidth);
			//panel.BorderStyle = BorderStyle.FixedSingle;
			//panel.Location = new Point(spalte * size + 2 * marginWidth, zeile * size + 2 * marginWidth);
			//panel.Click += Click_Event;
			//this._panel = panel;

			//frm.Controls.Add(panel);

			Label lbl = new Label();
			lbl.ForeColor = Color.Black;
			lbl.BackColor = Color.White;
			lbl.BorderStyle = BorderStyle.FixedSingle;
			lbl.AutoSize = false;
			lbl.TextAlign = ContentAlignment.MiddleCenter;
			lbl.Font = new Font(lbl.Font.FontFamily, size);
			lbl.Size = new Size(size, size);
			lbl.Location = new Point(spalte * size + 2 * marginWidth, zeile * size + 2 * marginWidth);
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

			if (fieldOwner == Players.Player1)
			{
				//this._label.Text = "X";
				Task t = Helper.ChangeColor(this._label, owner.playerColor);
				await t;
				//this._label.ForeColor = Color.Blue;
			}
			else if (fieldOwner == Players.Computer_Player2)
			{
				//this._label.Text = "O";
				Task t = Helper.ChangeColor(this._label, owner.computerColor);
				await t;
				//this._label.ForeColor = Color.Red;
			}
			else if (fieldOwner == Players.NoOne)
			{
				//this._label.Text = "";
				Task t = Helper.ChangeColor(this._label, Color.White);
				await t;
			}
			return true;
		}

		public async Task<bool> SetStatusPvP(Players newOwner, bool force = false)
		{
			if (!force)
			{
				if (fieldOwner != Players.NoOne || newOwner == Players.NoOne)
				{
					return false;
				}
			}
			fieldOwner = newOwner;

			if (fieldOwner == Players.Player1)
			{
				//this._label.Text = "X";
				Task t = Helper.ChangeColor(this._label, owner.playerColor);
				await t;
				//this._label.ForeColor = Color.Blue;
			}
			else if (fieldOwner == Players.Computer_Player2)
			{
				//this._label.Text = "O";
				Task t = Helper.ChangeColor(this._label, owner.computerColor);
				await t;
				//this._label.ForeColor = Color.Red;
			}
			else if (fieldOwner == Players.NoOne)
			{
				//this._label.Text = "";
				Task t = Helper.ChangeColor(this._label, Color.White);
				await t;
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
			if (this.owner.Modus == Modus.PvC)
			{
				if (owner.PvCTurn == Players.Player1 && computer == false)
				{
					if (SetStatus(Players.Player1).Result)
					{
						owner.PvCTurn = Players.Computer_Player2;
						owner.ComputerTurn();
					}
					else
					{
						return false;
					}
				}
				else if (owner.PvCTurn == Players.Computer_Player2 && computer == true)
				{
					if (SetStatus(Players.Computer_Player2).Result)
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
			else
			{
				if (this.owner.PvPTurn == Players.Player1)
				{
					if (SetStatusPvP(Players.Player1).Result)
					{
						this.owner.CheckVictoryPvP();
						this.owner.PvPTurn = Players.Computer_Player2;
						return true;
					}
				}
				else
				{
					if (SetStatusPvP(Players.Computer_Player2).Result)
					{
						this.owner.CheckVictoryPvP();
						this.owner.PvPTurn = Players.Player1;
						return true;
					}
				}
				return false;
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
			try
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
			catch { }
		}

		public async static Task ToFront(Control ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.BringToFront(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}

		public async static Task Activate(Form ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.Activate(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}

		 

	}


	class IniFile   // https://stackoverflow.com/a/14906422
	{
		string path;
		string EXE = Assembly.GetExecutingAssembly().GetName().Name;

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

		public IniFile(string IniPath = null)
		{
			path = Path.GetFullPath(IniPath ?? Path.ChangeExtension(Application.ExecutablePath, ".ini"));
		}

		public string Read(string Key, string Section = null)
		{
			var RetVal = new StringBuilder(255);
			GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, path);
			return RetVal.ToString();
		}

		public void Write(string Key, string Value, string Section = null)
		{
			WritePrivateProfileString(Section ?? EXE, Key, Value, path);
		}

		public void DeleteKey(string Key, string Section = null)
		{
			Write(Key, null, Section ?? EXE);
		}

		public void DeleteSection(string Section = null)
		{
			Write(null, null, Section ?? EXE);
		}

		public bool KeyExists(string Key, string Section = null)
		{
			return Read(Key, Section).Length > 0;
		}
	}

	enum Players{
		NoOne, Player1, Computer_Player2
	}
	enum Modus
	{
		PvP, PvC
	}

}