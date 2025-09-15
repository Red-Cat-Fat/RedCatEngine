using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Services.RandomServices
{
    public static class RandomServiceExtensions
    {
        public static T GetRandomElement<T>(this IRandomService randomService, IEnumerable<T> items)
        {
            var enumerable = items as T[] ?? items.ToArray();
            var index = randomService.GetRange(0, enumerable.Length);
            return enumerable[index];
        }

        /// <summary>
        /// Возвращает новую последовательность с элементами из inputSequence в случайном порядке (по алгориму Фишера-Йетса).
        /// </summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="randomService">Генератор случайных чисел</param>
        /// <param name="inputSequence">Исходная последовательность</param>
        /// <returns>Перемешанная последовательность</returns>
        public static IEnumerable<T> GetShuffle<T>(this IRandomService randomService, IEnumerable<T> inputSequence)
            => inputSequence.OrderBy(_ => randomService.GetRange(0, int.MaxValue));
    }
}