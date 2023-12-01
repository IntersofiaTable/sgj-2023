using System;

namespace Tests.Editor
{
    public class RandomMock
    {
        private readonly int[] randoms;
        private int current;
        
        public RandomMock(params int[] randoms)
        {
            this.randoms = randoms;
            this.current = -1;
        }
        
        public int Next(int min, int max)
        {
            current++;
            if (current >= randoms.Length) throw new Exception("end of random list plz fix setup");//current = 0;
            return randoms[current];
        }
        
    }
}