using TikTakToe.Enums;

namespace TikTakToe
{
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

		public Feld(ref Form frm, TikTakToe owner, int zeile, int spalte, Players player = Players.NoOne)
		{
			this.owner = owner;
			this.Spalte = spalte;
			this.Zeile = zeile;
			this.id = zeile * 3 + spalte + 1;

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
			_label = lbl;
			_ = SetStatus(player, true);
			frm.Controls.Add(lbl);

			_location = lbl.Location;
			_size = lbl.Size;
			_marginWidth = marginWidth;

			lbl.BringToFront();
		}

		public bool SetStatus(Players newOwner, bool force = false)
		{
			if (!force)
			{
				if (fieldOwner != Players.NoOne || newOwner == Players.NoOne)
				{
					return false;
				}
			}
			fieldOwner = newOwner;
			var newColor = fieldOwner switch
										{
											Players.Player1 => owner.playerColor,
											Players.Computer_Player2 => owner.computerColor,
											Players.NoOne => Color.White,
											_ => throw new NotImplementedException()
										};

			Helper.ChangeColor(_label, newColor);
			return true;
		}

		public bool SetStatusPvP(Players newOwner, bool force = false)
		{
			if (!force)
			{
				if (fieldOwner != Players.NoOne || newOwner == Players.NoOne)
				{
					return false;
				}
			}
			fieldOwner = newOwner;
			var newColor = fieldOwner switch
			{
											Players.Player1 => owner.playerColor,
											Players.Computer_Player2 => owner.computerColor,
											Players.NoOne => Color.White,
											_ => throw new NotImplementedException()
										};
			Helper.ChangeColor(_label, newColor);
			return true;
		}

		public Players GetStatus()
		{
			return this.fieldOwner;
		}

		private void Click_Event(object? sender, EventArgs e)
		{
			Click(false);
		}
		public bool Click(bool computer)
		{
			if (this.owner.Modus == Modus.PvC)
			{
				if (owner.PvCTurn == Players.Player1 && computer == false)
				{
					if (SetStatus(Players.Player1))
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
					return SetStatus(Players.Computer_Player2);
				}
				return false;
			}
			else
			{
				if (this.owner.PvPTurn == Players.Player1)
				{
					if (SetStatusPvP(Players.Player1))
					{
						this.owner.CheckVictoryPvP();
						this.owner.PvPTurn = Players.Computer_Player2;
						return true;
					}
				}
				else
				{
					if (SetStatusPvP(Players.Computer_Player2))
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

}