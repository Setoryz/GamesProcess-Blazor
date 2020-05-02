using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTests
{
    class TurningSearchParameter
    {
        public int NoOfWeeksToDisplay { get; set; }
        public int GameSelection { get; set; }
        public int GroupSelection { get; set; }
        private int initNum;

        public int InitialNumber
        {
            get { return initNum; }
            set { initNum = value; TurningNumber = ((value % 10) * 10) + (value - (value % 10)); }
        }

        public int InitialNumberLocation { get; set; }
        public int InitialNumberPosition { get; set; }
        public int TurningNumber { get; set; }
        public int TurningNumberLocation { get; set; }
        public int TurningNumberPosition { get; set; }
        public int TurningNumberWeek { get; set; }
        public int TurningNumberWeekSelect { get; set; }
    }
}
