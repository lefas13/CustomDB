using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections;
using Azure;
using System.Threading.Tasks;
using CustomApp.Models;
using Microsoft.Extensions.Logging;

namespace CustomApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<CustomContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;

            using (CustomContext db = new CustomContext(options))
            {
                //Выполняем разные методы, содержащие операции выборки и изменения данных
                Console.WriteLine("====== Будет выполнена выборка данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Select(db);
                Console.WriteLine("====== Будет выполнена вставка данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Insert(db);
                Console.WriteLine("====== Выборка после вставки ========");
                Select(db);
                Console.WriteLine("====== Будет выполнено обновление данных (нажмите любую клави-шу) ========");
                Console.ReadKey();
                Update(db);
                Console.WriteLine("====== Выборка после обновления ========");
                Select(db);
                Console.WriteLine("====== Будет выполнено удаление данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Delete(db);
                Console.WriteLine("====== Выборка после удаления ========");
                Select(db);
                Return(db);
            }
            Console.Read();

            static void Print(string sqltext, IEnumerable items)
            {
                Console.WriteLine(sqltext);
                Console.WriteLine("Записи: ");
                foreach (var item in items)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine();
                Console.ReadKey();
            }

            static void Select(CustomContext db)
            {
                // Определение LINQ запроса 1
                var queryLINQ1 = from a in db.Agents
                                select new
                                {
                                    Код_агента = a.AgentId,
                                    Фамилия_Имя_Отчество = a.FullName,
                                    Номер_Удостоверения = a.IdNumber,
                                };

                string comment = "1. Результат выполнения запроса на выборку всех данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один»: \r\n";
                //для наглядности выводим не более 20 записей
                Print(comment, queryLINQ1.Take(20).ToList());

                // Определение LINQ запроса 2
                var queryLINQ2 = from w in db.Warehouses
                                where w.WarehouseNumber.EndsWith("1")
                                select new
                                {
                                    Код_склада = w.WarehouseId,
                                    Номер_склада = w.WarehouseNumber,
                                };

                comment = "2. Результат выполнения запроса на выборку данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один», отфильтрованные по определенному условию, налагающему ограничения на одно или несколько полей: \r\n";
                //для наглядности выводим не более 20 записей
                Print(comment, queryLINQ2.Take(20).ToList());

                // Определение LINQ запроса 3
                var queryLINQ3 = from f in db.Fees
                                group f.Amount by f.WarehouseId into gr
                                select new
                                {
                                    Код_пошлина = gr.Key,
                                    Количество_товаров = gr.Sum()
                                };
                comment = "3. Результат выполнения запроса на выборку данных, сгруппированных по любому из полей данных с выводом какого-либо итогового результата (min, max, avg, сount или др.) по выбранному полю из таблицы, стоящей в схеме базы данных нас стороне отношения «многие»: \r\n";
                //для наглядности выводим не более 20 записей
                Print(comment, queryLINQ3.Take(10).ToList());

                // Определение LINQ запроса 4
                var queryLINQ4 = from g in db.Goods
                                join gt in db.GoodTypes
                                on g.GoodTypeId equals gt.GoodTypeId
                                select new
                                {
                                    Код_товара = g.GoodId,
                                    Название_товара = g.Name,
                                    Тип_товара = gt.Name
                                };
                comment = "4. Результат выполнения запроса на выборку данных из двух полей двух таблиц, связанных между собой отношением «один-ко-многим»: \r\n";
                //для наглядности выводим не более 20 записей
                Print(comment, queryLINQ4.Take(20).ToList());

                // Определение LINQ запроса 5
                var queryLINQ5 = from g in db.Goods
                                join gt in db.GoodTypes
                                on g.GoodTypeId equals gt.GoodTypeId
                                where gt.GoodTypeId == 2
                                select new
                                {
                                    Код_товара = g.GoodId,
                                    Название_товара = g.Name,
                                    Тип_товара = gt.Name
                                };
                comment = "5. Результат выполнения запроса на выборку данных из двух таблиц, связанных между собой отношением «один-ко-многим» и отфильтрованным по некоторому условию, налагающему ограничения на значения одного или нескольких полей: \r\n";
                //для наглядности выводим не более 20 записей
                Print(comment, queryLINQ5.Take(20).ToList());
            }

            static void Insert(CustomContext db)
            {
                // Создать нового склада
                Warehouse warehouse = new Warehouse
                {
                    WarehouseNumber = "100001",
                };
                // Создать новую пошлина
                Fee fee = new Fee
                {
                    WarehouseId = 1,
                    GoodId = 1,
                    ReceiptDate = DateOnly.MinValue,
                    Amount = 2,
                    DocumentNumber = "f",
                    AgentId = 1,
                    FeeAmount = 20,
                    PaymentDate = DateOnly.MinValue,
                    ExportDate = DateOnly.MinValue,
                };

                // Добавить в DbSet
                db.Warehouses.Add(warehouse);
                db.Fees.Add(fee);
                // Сохранить изменения в базе данных
                db.SaveChanges();
            }

            static void Delete(CustomContext db)
            {
                //подлежащие удалению записи в таблице Warehouses
                string number = "100001";
                var warehouse = db.Warehouses.Where(c => c.WarehouseNumber == number);

                //подлежащие удалению записи в таблице Fees
                string documentNumber = "f";
                var fee = db.Fees
                    .Where(c => c.DocumentNumber == documentNumber);

                //Удаление записи в таблице Fees    
                db.Fees.RemoveRange(fee);
                // сохранить изменения в базе данных
                db.SaveChanges();

                //Удаление нескольких записи в таблице Warehouses
                db.Warehouses.RemoveRange(warehouse);

                // сохранить изменения в базе данных
                db.SaveChanges();
            }

            static void Update(CustomContext db)
            {
                //подлежащие обновлению записи в таблице Agents
                string FIO = "FullName 1";
                var agent = db.Agents.Where(c => c.FullName == FIO).FirstOrDefault();
                //обновление
                if (agent != null)
                {
                    agent.FullName = "FullName";
                    agent.IdNumber = "0";
                };

                // сохранить изменения в базе данных
                db.SaveChanges();

            }

            static void Return(CustomContext db)
            {
                //подлежащие обновлению записи в таблице Agents
                string FIO = "FullName";
                var agent = db.Agents.Where(c => c.FullName == FIO).FirstOrDefault();
                //обновление
                if (agent != null)
                {
                    agent.FullName = "FullName 1";
                    agent.IdNumber = "1";
                };

                // сохранить изменения в базе данных
                db.SaveChanges();

            }

        }
    }
}

