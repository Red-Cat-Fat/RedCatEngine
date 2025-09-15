using System;
using UnityEngine;

namespace RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes
{
    /// <summary>
    /// Обёртка над строкой, представляющая тег объекта в Unity. 
    /// Позволяет использовать теги более безопасно и удобно, например, для сравнения с GameObject.
    /// </summary>
	[Serializable]
    public struct UnityTag
    {
        /// <summary>
        /// Содержит имя тега в виде строки.
        /// </summary>
        public string Tag;

        /// <summary>
        /// Неявное преобразование из <see cref="UnityTag"/> в <see cref="string"/>.
        /// </summary>
        /// <param name="tag">Структура <see cref="UnityTag"/>.</param>
        /// <returns>Строковое представление тега.</returns>
        public static implicit operator string(UnityTag tag)
        {
            return tag.Tag;
        }

        /// <summary>
        /// Неявное преобразование из строки в <see cref="UnityTag"/>.
        /// </summary>
        /// <param name="tag">Строковое значение тега.</param>
        /// <returns>Созданный объект <see cref="UnityTag"/>.</returns>
        public static implicit operator UnityTag(string tag)
        {
            UnityTag result = default;
            result.Tag = tag;
            return result;
        }

        /// <summary>
        /// Сравнивает тег этого объекта с тегом указанного GameObject.
        /// </summary>
        /// <param name="obj">Объект <see cref="GameObject"/>, у которого проверяется тег.</param>
        /// <returns><see langword="true"/>, если теги совпадают; в противном случае — <see langword="false"/>.</returns>
        public bool CompareTag(GameObject obj)
        {
            return obj.CompareTag(this);
        }
    }
}