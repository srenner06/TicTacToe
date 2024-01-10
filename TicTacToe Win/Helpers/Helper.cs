namespace TicTacToe.Win.Helpers;

public static class Helper
{
	public static void ChangeColor(Control ctrl, Color color)
	{
		ctrl.SafeInvoke(() => ctrl.BackColor = color);
		ctrl.Refresh();
		ctrl.Update();
	}
	public static void SafeInvoke<T>(this T uiElement, Action func) where T : Control
	{
		try
		{
			if (uiElement.InvokeRequired)
				uiElement.Invoke(func);
			else
				func();
		}
		catch //(Exception e)
		{
		}
	}
	public static void SafeInvokeAsync<T>(this T uiElement, Action<T> func, AsyncCallback callBack) where T : Control
	{
		try
		{
			if (uiElement.InvokeRequired)
				func.BeginInvoke(uiElement, callBack, null);
			else
				func(uiElement);
		}
		catch //(Exception e)
		{
		}
	}

	public static void ToFront(Control ctrl)
	{
		ctrl.SafeInvoke(ctrl.BringToFront);
		ctrl.Refresh();
		ctrl.Update();
	}

	public static void Hide(Form ctrl)
	{
		ctrl.SafeInvoke(ctrl.Hide);
		ctrl.Refresh();
		ctrl.Update();
	}

	public static void Activate(Form ctrl)
	{
		ctrl.SafeInvoke(ctrl.Activate);
		ctrl.Refresh();
		ctrl.Update();
	}

	public static void Close(Form ctrl)
	{
		ctrl.SafeInvoke(ctrl.Close);
	}

}