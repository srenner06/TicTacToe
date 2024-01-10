using TicTacToe_WIn.Helpers;
using Utils.Extensions;
using Utils.Windows.Helpers;

namespace TikTakToe.Helpers;
public partial class SettingsViewer : Form
{
	public SettingsViewer()
	{
		InitializeComponent();
		SyncToSettings();
		CheckSaveButton();
	}

	private void SyncToSettings(Settings? settings = null)
	{
		settings ??= Settings.Load();

		pbP1Color.BackColor = settings.P1Color;
		pbP2Color.BackColor = settings.P2Color;
	}

	private void btnP1ChangeColor_Click(object sender, EventArgs e)
	{
		colorDialog1.Reset();
		colorDialog1.Color = pbP1Color.BackColor;
		colorDialog1.AnyColor = true;
		var result = colorDialog1.ShowDialog();
		if (result == DialogResult.OK)
		{
			var newColor = colorDialog1.Color;
			if (newColor.SameRGB(pbP2Color.BackColor))
				Msg.Warning("Spieler 1 und Spieler 2 können nicht die gleiche Farbe haben");
			else if (newColor.SameRGB(pbP1Color.BackColor) == false)
				pbP1Color.BackColor = colorDialog1.Color;
		}
		CheckSaveButton();
	}

	private void btnP2ChangeColor_Click(object sender, EventArgs e)
	{
		colorDialog1.Reset();
		colorDialog1.Color = pbP2Color.BackColor;
		colorDialog1.AnyColor = true;
		var result = colorDialog1.ShowDialog();
		if (result == DialogResult.OK)
		{
			var newColor = colorDialog1.Color;
			if (newColor.SameRGB(pbP1Color.BackColor))
				Msg.Warning("Spieler 1 und Spieler 2 können nicht die gleiche Farbe haben");
			else if (newColor.SameRGB(pbP2Color.BackColor) == false)
				pbP2Color.BackColor = colorDialog1.Color;
		}
		CheckSaveButton();
	}

	private void CheckSaveButton()
	{
		btnSave.Enabled = !IsSaved();
	}
	private bool IsSaved()
	{
		var settings = Settings.Load();
		return settings.P1Color.SameRGB(pbP1Color.BackColor) && settings.P2Color.SameRGB(pbP2Color.BackColor);
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		Save();
	}
	private void SettingsViewer_FormClosing(object sender, EventArgs e)
	{
		if (!IsSaved())
		{
			var response = Msg.Ask("Es gibt noch ungespeicherte Änderungen.\nWolle sie diese Speichern?");
			if (response == DialogResult.Yes)
				Save();
		}
	}

	private void Save()
	{
		var settings = Settings.Load();
		settings.P1Color = pbP1Color.BackColor;
		settings.P2Color = pbP2Color.BackColor;
		settings.Save();
		CheckSaveButton();
	}
}
