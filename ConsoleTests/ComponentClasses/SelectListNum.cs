namespace ConsoleTests.ComponentClasses
{
    class SelectListNum
    {
        public int MaxNum { get; set; } = 1;
        public int MinNum { get; set; } = 0;
        public int Skip { get; set; } = 1;
        public int ArraySize
        {
            get => ((MaxNum - MinNum) / Skip) + 1;
        }
        public int[] NumArray
        {
            get
            {
                int[] numArr = new int[ArraySize];
                if (ArraySize > 1)
                {
                    for (int i = 0; i < numArr.Length; i++)
                    {
                        numArr[i] = (i == 0) ? MinNum : (numArr[i - 1] + Skip);
                    }
                }
                return numArr;
            }
        }
        
    }
}
