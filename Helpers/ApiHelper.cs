using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BlindsRandomizerUWP.Entities;
using Newtonsoft.Json;

/// <summary>
/// Классы-помощники
/// </summary>
namespace BlindsRandomizerUWP.Helpers
{
    /// <summary>
    /// Класс-помощник для получения результатов последней игры по API
    /// </summary>
    public class ApiHelper
    {
        /// <summary>
        /// Получает результаты последней игры через API
        /// </summary>
        /// <returns>Результаты последней игры</returns>
        public async Task<GameResults> GetLastGameResults()
        {
            // URL для запроса
            string url = "http://poker.hamster56.ru/api/last-game";
            // Токен для авторизации
            string token = "3xTHto57cOJLY0A1NUpOQs6j14Z49X8X";
            // Результаты по-умолчанию
            GameResults results = new GameResults()
            {                
                type = "-",                
                places = new List<GamePlace>()
            };
            // Заводим клиент для запроса
            HttpClient client = new HttpClient();
            // Устанавливаем заголовок Accept
            client.DefaultRequestHeaders.Accept.Clear();           
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Устанавливаем заголовок авторизации
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Запрашиваем данные
            HttpResponseMessage response = await client.GetAsync(url);
            // Если данные получены
            if (response.IsSuccessStatusCode)
            {
                // Заполняем результаты последней игры
                string content = response.Content.ReadAsStringAsync().Result;
                results = JsonConvert.DeserializeObject<GameResults>(content);
            }            
            // Возвращаем результаты последней игры
            return results;
        }
    }
}
