using System.Collections.Generic;
using System.Linq;

namespace MarkupConverter
{
	public class HtmlFromXamlTableInfo
	{
		public IDictionary<HtmlThickness, int> Borders
		{
			get { return borders = borders ?? new Dictionary<HtmlThickness, int>(); }
		}
		private Dictionary<HtmlThickness, int> borders;

		public void AddBorder(HtmlThickness border)
		{
            int found;
            if (Borders.TryGetValue(border, out found))
			{
                Borders[border] = found + 1;
			}
			else
			{
                Borders[border] = 1;
			}
		}

		public HtmlThickness CommonBorder
		{
			get { return Borders.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault(); }
		} 
	}
}