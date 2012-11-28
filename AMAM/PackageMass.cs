using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amam
{
	class PackageMass
	{
		private int internalCounter = 0;

		public int PackageMassValue
		{
			get;
			private set;
		}
		
		public string PackageMassName 
		{
			get;
			private set;
		}

		public PackageMass(string MassName)
		{
			PackageMassValue = internalCounter;
			PackageMassName = MassName;
			internalCounter++;
		}
	}
}
