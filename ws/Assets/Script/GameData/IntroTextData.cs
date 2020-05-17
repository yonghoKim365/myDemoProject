using System;
using System.Collections.Generic;

sealed public class IntroTextData
{
	public enum Type
	{
		Date, Day, Time, Normal
	}
	
	public Type type = Type.Normal;

	public string text;


	public int startHour, startMin, endHour, endMin;
	public int[] day;
	public int startDay, startMonth, endDay, endMonth;

	public int weight = 0;

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		text = (l[k["TEXT"]]).ToString();

		Util.parseObject(l[k["WEIGHT"]], out weight, 0);

		string d = (l[k["DATE"]]).ToString().Trim();
		
		if(string.IsNullOrEmpty(d) == false)
		{
			string[] date = d.Split(',');
			if(date.Length == 2 )
			{
				int.TryParse(date[0].Substring(0,2), out startMonth);
				int.TryParse(date[0].Substring(2,2), out startDay);

				int.TryParse(date[1].Substring(0,2), out endMonth);
				int.TryParse(date[1].Substring(2,2), out endDay);

				type = Type.Date;
				return;
			}

		}


		d = (l[k["DAY"]]).ToString().Trim();

		if(string.IsNullOrEmpty(d) == false)
		{
			day = Util.stringToIntArray(d,',');

			if(day.Length > 0)
			{
				type = Type.Day;
				return;
			}
		}

		string t = (l[k["TIME"]]).ToString().Trim();

		if(string.IsNullOrEmpty(t) == false)
		{
			string[] tm = t.Split(',');
			if(tm.Length == 2 )

			int.TryParse(tm[0].Substring(0,2), out startHour);
			int.TryParse(tm[0].Substring(2,2), out startMin);
			
			int.TryParse(tm[1].Substring(0,2), out endHour);
			int.TryParse(tm[1].Substring(2,2), out endMin);

			type = Type.Time;
			return;
		}
	}



	public static int sort(IntroTextData x, IntroTextData y)
	{
		return y.weight.CompareTo(x.weight);
	}


}
