using CustomMVC.Models;

namespace Tests
{
    internal class TestDataHelper
    {
        public static List<Agent> GetFakeAgentsList()
        {
            return new List<Agent>
            {
                new Agent {
                    AgentId = 1,
                    FullName = "А.А. Американ",
                    IdNumber = "ID1"
                },
                new Agent {
                    AgentId = 2,
                    FullName = "М.М. Мирный",
                    IdNumber = "ID2"
                },
                new Agent {
                    AgentId = 3,
                    FullName = "Д.Д. Домовов",
                    IdNumber = "ID3"
                }
            };
        }

        public static List<Warehouse> GetFakeWarehousesList()
        {
            return new List<Warehouse>
            {
                new Warehouse {
                    WarehouseId = 1,
                    WarehouseNumber = "1111"
                },
                new Warehouse {
                    WarehouseId = 2,
                    WarehouseNumber = "2222"
                },
                new Warehouse {
                    WarehouseId = 3,
                    WarehouseNumber = "3333"
                }
            };
        }

        public static List<GoodType> GetFakeGoodTypesList()
        {
            return new List<GoodType>
            {
                new GoodType
                {
                    GoodTypeId = 1,
                    Name = "Электроника",
                    Measurement = "шт.",
                    AmountOfFee = 150.00M,
                },
                new GoodType
                {
                    GoodTypeId = 2,
                    Name = "Мебель",
                    Measurement = "шт.",
                    AmountOfFee = 50.00M,
                },
                new GoodType
                {
                    GoodTypeId = 3,
                    Name = "Еда",
                    Measurement = "кг",
                    AmountOfFee = 75.00M,
                }
            };
        }

        public static List<Good> GetFakeGoodsList()
        {
            int goods_number = 10; // Количество товаров
            int goodTypes_number = GetFakeGoodTypesList().Count; // Получение списка типов товара

            Random randObj = new Random(1); // Создание генератора случайных чисел
            List<Good> goods = new List<Good>(); // Список для хранения товаров

            // Заполнение списка товаров
            for (int goodID = 1; goodID <= goods_number; goodID++)
            {
                int goodTypeID = randObj.Next(1, goodTypes_number + 1); // Случайный выбор типа товара
                //DateTime today = DateTime.Now.Date; // Текущая дата
                //DateOnly serviceDate = DateOnly.FromDateTime(today.AddDays(-serviceID)); // Дата услуги (от сегодняшней даты)

                // Создание нового товара и добавление в список
                goods.Add(new Good
                {
                    GoodId = goodID,
                    GoodTypeId = goodTypeID,
                    Name = "Товар" + goodID,
                    GoodType = GetFakeGoodTypesList().SingleOrDefault(m => m.GoodTypeId == goodTypeID), // Привязка типа товара
                });
            }
            var goodtypes = GetFakeGoodTypesList();

            return goods;
        }

        public static List<Fee> GetFakeFeesList()
        {
            int fees_number = 20; // Количество пошлин
            int agents_number = GetFakeAgentsList().Count; // Получение списка агентов
            int goods_number = GetFakeGoodsList().Count; // Получение списка товаров
            int warehouses_number = GetFakeWarehousesList().Count; // Получение списка складов

            Random randObj = new Random(1); // Создание генератора случайных чисел
            List<Fee> fees = new List<Fee>(); // Список для хранения пошлин

            // Заполнение списка пошлин
            for (int feeID = 1; feeID <= fees_number; feeID++)
            {
                int agentID = randObj.Next(1, agents_number + 1); // Случайный выбор агента
                int goodID = randObj.Next(1, goods_number + 1); // Случайный выбор товара
                DateTime today = DateTime.Now.Date; // Текущая дата
                DateOnly feeDate = DateOnly.FromDateTime(today.AddDays(-feeID)); // Дата пошлины (от сегодняшней даты)

                // Создание новой пошлины и добавление в список
                fees.Add(new Fee
                {
                    FeeId = feeID,
                    AgentId = agentID,
                    GoodId = goodID,
                    WarehouseId = GetFakeGoodsList().SingleOrDefault(m => m.GoodId == goodID).GoodTypeId,
                    DocumentNumber = "Dc" + feeID,
                    Amount = feeID,
                    FeeAmount = 100 + 10 * feeID,
                    ReceiptDate = feeDate,
                    PaymentDate = feeDate,
                    ExportDate = feeDate,
                    Agent = GetFakeAgentsList().SingleOrDefault(m => m.AgentId == agentID), // Привязка агента
                    Good = GetFakeGoodsList().SingleOrDefault(m => m.GoodId == goodID), // Привязка товара
                    Warehouse = GetFakeWarehousesList().SingleOrDefault(m => m.WarehouseId == GetFakeGoodsList().SingleOrDefault(m => m.GoodId == goodID).GoodTypeId), // Привязка склада
                });
            }
            var agents = GetFakeAgentsList();
            var goods = GetFakeGoodsList();
            var warehouses = GetFakeWarehousesList();

            return fees;
        }
    }
}
