using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicCacheManager
{
	class MusicObject
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public string Path { get; set; }
		public string Author { get; set; }
		public DateTime FileModifyTime { get; set; }

		public MusicObject(string name, string author, string id, string path, DateTime fileModifyTime)
		{
			Name = name;
			Id = id;
			Path = path;
			Author = author;
			FileModifyTime = fileModifyTime;
		}

		public override string ToString()
		{
			return Name + " - " + Author + "\t(" + FileModifyTime.ToString() + ")";
		}
	}
}
