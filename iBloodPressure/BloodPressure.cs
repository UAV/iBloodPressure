
using System;
using SQLite;
namespace iBloodPressure
{


	public class BloodPressure
	{
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
		public int Systolic { get; set;}
		public int Diastolic { get; set;}
		public int PulsePerMin { get; set;}
		public DateTime TimeStamp {get; set;}

		public BloodPressure ()
		{
			TimeStamp = DateTime.Now;
		}
	}
}
