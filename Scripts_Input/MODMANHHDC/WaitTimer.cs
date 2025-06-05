using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODMANHHDC
{
    public class WaitTimer
    {
		public IActionListener timeListener;

		public int idAction;

		public long timeExecute;

		public bool isON;

		private object p;
		public void setTimer(IActionListener actionListener, int action, long timeEllapse, object p = null)
		{
			timeListener = actionListener;
			idAction = action;
			timeExecute = mSystem.currentTimeMillis() + timeEllapse;
			isON = true;
			this.p = p;
		}

		public void update()
		{
			long num = mSystem.currentTimeMillis();
			if (!isON || num <= timeExecute)
			{
				return;
			}
			isON = false;
			try
			{
				if (idAction > 0)
				{
					timeListener.perform(idAction, p);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
