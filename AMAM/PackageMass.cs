namespace Amam
{
	class PackageMass
	{
		private readonly int _internalCounter;

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

		public PackageMass(string massName)
		{
			PackageMassValue = _internalCounter;
			PackageMassName = massName;
			_internalCounter++;
		}
	}
}
