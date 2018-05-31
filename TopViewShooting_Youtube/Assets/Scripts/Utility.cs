using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] _array, int _seed)
    {
        System.Random prng = new System.Random(_seed);          // prng 수도 랜덤 숫자 생성기

        for (int i = 0; i < _array.Length - 1; i++)         // -1 한 이유는 마지막 루프는 생략해도 되기 때문
        {
            int randomIndex = prng.Next(i, _array.Length);
            T tempItem = _array[randomIndex];
            _array[randomIndex] = _array[i];
            _array[i] = tempItem;
        }

        return _array;
    }
}
