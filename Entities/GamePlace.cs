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
    /// Класс, представляющий место человека в игре
    /// </summary>
    public class GamePlace
    {
        /// <summary>
        /// ID в БД
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Место
        /// </summary>
        public int place { get; set; }
        /// <summary>
        /// Имя игрока
        /// </summary>
        public string player { get; set; }
    }
}
