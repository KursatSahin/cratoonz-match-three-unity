using System;
using Core.Services;

namespace Game
{
    public interface IRandomGenerator : IService
    {
        public int GetRandomNumber(int max = Int32.MaxValue, int min = 0);
    }
    
    public class RandomGenerator : IRandomGenerator
    {
        private readonly System.Random _rand;

        public RandomGenerator()
        {
            _rand = new Random();
        }

        public RandomGenerator(int seed)
        {
            _rand = new Random(seed);
        }

        public int GetRandomNumber(int max = Int32.MaxValue, int min = 0)
        {
            return _rand.Next(min, max);
        }
    }
}