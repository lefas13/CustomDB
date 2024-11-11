using System;
using System.Linq;
using CustomMVC.Models;
using System.Runtime.ConstrainedExecution;

namespace CustomMVC.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CustomContext db)
        {
            db.Database.EnsureCreated();

            if (db.Agents.Any())
            {
                return; // Если есть данные, инициализация не требуется
            }

            Random randObj = new(1);

            // Заполнение таблицы агентов
            string[] agentNames = [ "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" ];
            foreach (var agent in agentNames)
            {
                db.Agents.Add(new Agent
                {
                    FullName = "FullName " + agent,
                    IdNumber = "ID " + agent
                });
            }
            db.SaveChanges();

            // Заполнение типов товаров
            string[] nameTypes = [ "Мебель", "Электроника", "Одежда", "Медиакаменты", "Продукты питания", "Химия", "Транспорт" ];
            int j = 1;
            foreach (var goodType in nameTypes)
            {
                db.GoodTypes.Add(new GoodType
                {
                    Name = goodType,
                    Measurement = "Measurement " + j,
                    AmountOfFee = j
                });
                j++;
            }
            db.SaveChanges();

            // Заполнение складов
            string[] warehouseNumbers = [ "1111", "2222", "3333", "4444", "5555", "6666", "7777" ];
            foreach (var warehouse in warehouseNumbers)
            {
                db.Warehouses.Add(new Warehouse
                {
                    WarehouseNumber = warehouse,
                });
            }
            db.SaveChanges();

            // Заполнение товаров
            var goodTypes = db.GoodTypes.ToList();
            for (int i = 0; i < 50; i++)
            {
                db.Goods.Add(new Good
                {
                    GoodTypeId = goodTypes[randObj.Next(goodTypes.Count)].GoodTypeId,
                    Name = "Good " + i
                });
            }
            db.SaveChanges();

            // Заполнение пошлин
            var goods = db.Goods.ToList();
            var agents = db.Agents.ToList();
            for (int i = 0; i < 100; i++)
            {
                var good = goods[randObj.Next(goods.Count)];
                var agent = agents[randObj.Next(agents.Count)];
                db.Fees.Add(new Fee
                {
                    AgentId = agent.AgentId,
                    GoodId = good.GoodType.GoodTypeId,
                    WarehouseId = good.GoodType.GoodTypeId,
                    DocumentNumber = "Document" + i,
                    ReceiptDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(30))),
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(randObj.Next(1, 30))),
                    ExportDate = DateOnly.FromDateTime(DateTime.Now.AddDays(randObj.Next(1, 30))),
                    FeeAmount = randObj.Next(1000, 10000),
                    Amount = randObj.Next(10, 100)
                });
            }
            db.SaveChanges();
        }
    }
}
