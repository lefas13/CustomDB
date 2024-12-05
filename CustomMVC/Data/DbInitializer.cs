using System;
using System.Linq;
using CustomMVC.Models;

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
            for (int i = 0; i < 500; i++)
            {
                db.Agents.Add(new Agent
                {
                    FullName = "FullName " + i,
                    IdNumber = "ID " + i
                });
            }
            db.SaveChanges();

            // Заполнение типов товаров
            for (int i = 0; i < 500; i++)
            {
                db.GoodTypes.Add(new GoodType
                {
                    Name = "Type " + i,
                    Measurement = "Measurement " + i,
                    AmountOfFee = i
                });
            }
            db.SaveChanges();

            // Заполнение складов
            for (int i = 0; i < 500; i++)
            {
                db.Warehouses.Add(new Warehouse
                {
                    WarehouseNumber = "Num" + i,
                });
            }
            db.SaveChanges();

            // Заполнение товаров
            var goodTypes = db.GoodTypes.ToList();
            for (int i = 0; i < 20000; i++)
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
            for (int i = 0; i < 500; i++)
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
