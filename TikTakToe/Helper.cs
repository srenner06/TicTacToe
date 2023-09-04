namespace TikTakToe
{
	public class Helper
	{
		internal static void ChangeColor(Control ctrl, Color color)
		{
			SafeInvoke(ctrl, delegate () { ctrl.BackColor = color; }, false);
			ctrl.Refresh();
			ctrl.Update();
		}
		private static void SafeInvoke(Control uiElement, Action updater, bool forceSynchronous)
		{
			try
			{
				if (uiElement is null)
				{
					throw new ArgumentNullException(nameof(uiElement));
				}

				if (uiElement.InvokeRequired)
				{
					if (forceSynchronous)
					{
						uiElement.Invoke(delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
					}
					else
					{
						uiElement.BeginInvoke(delegate { SafeInvoke(uiElement, updater, forceSynchronous); });
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

		public static void ToFront(Control ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.BringToFront(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}

		public static void Hide(Form ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.Hide(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}

		public static void Activate(Form ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.Activate(); }, false);
			ctrl.Refresh();
			ctrl.Update();
		}

		public static void Close(Form ctrl)
		{
			SafeInvoke(ctrl, delegate () { ctrl.Close(); }, false);
		}	 

	}

}