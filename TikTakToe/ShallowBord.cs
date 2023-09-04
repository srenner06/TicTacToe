using TikTakToe.Enums;

namespace TikTakToe
{
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

		public static implicit operator ShallowBord(Bord b)
		{
			var sh = new ShallowBord(b);
			return sh;
		}

	}

}