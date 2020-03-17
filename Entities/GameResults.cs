using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Сущности приложения
/// </summary>
namespace BlindsRandomizerUWP.Entities
{
    /// <summary>
    /// Класс, представляющий результаты игры
    /// </summary>
    public class GameResults
    {
        /// <summary>
        /// ID в БД
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Тип игры
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Дата игры
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// Список мест
        /// </summary>
        public IList<GamePlace> places { get; set; }
    }
}
