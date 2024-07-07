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

		public MusicObject(string name, string id, string path)
		{
			Name = name;
			Id = id;
			Path = path;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
