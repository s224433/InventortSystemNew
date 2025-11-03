using System;
using System.Collections.Generic;
using Avalonia;

namespace ItemSorterRobot
{
    public class Item
    {
        public string Name { get; set; }
        public double PricePerUnit { get; set; }
        public uint InventoryLocation { get; set; }

        public Item(string name, double price, uint location)
        {
            Name = name;
            PricePerUnit = price;
            InventoryLocation = location;
        }
    }

    public class OrderLine
    {
        public Item Item { get; set; }
        public uint Quantity { get; set; }

        public OrderLine(Item item, uint quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    public class Order
    {
        public List<OrderLine> OrderLines { get; set; } = new();
        public DateTime Time { get; set; } = DateTime.Now;
        public double TotalPrice { get; set; }

        public void CalculateTotalPrice()
        {
            double total = 0;
            foreach (var line in OrderLines)
                total += line.Item.PricePerUnit * line.Quantity;
            TotalPrice = total;
        }
    }

    public class ProcessNextOrder
    {
        private readonly Queue<Order> _orderQueue = new();

        public void AddOrder(Order order) => _orderQueue.Enqueue(order);

        public Order ProcessNextOrderMethod()
        {
            if (_orderQueue.Count == 0)
            {
                Console.WriteLine("Ingen ordrer i køen.");
                return null!;
            }

            var nextOrder = _orderQueue.Dequeue();
            nextOrder.CalculateTotalPrice();
            Console.WriteLine($"Behandler ordre fra {nextOrder.Time}, totalpris: {nextOrder.TotalPrice:F2}");
            return nextOrder;
        }
    }

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace();

        public static void RunDemo()
        {
            var item1 = new Item("Hårspænde", 3.5, 11);
            var item2 = new Item("Hårelastik", 2.0, 12);
            var item3 = new Item("Hårbørste", 7.9, 14);

            var line1 = new OrderLine(item1, 10);
            var line2 = new OrderLine(item2, 5);
            var line3 = new OrderLine(item3, 20);

            var order = new Order();
            order.OrderLines.AddRange(new[] { line1, line2, line3 });

            var processor = new ProcessNextOrder();
            processor.AddOrder(order);
            processor.ProcessNextOrderMethod();

            var sorter = new ItemSorterRobot("127.0.0.1", 30002, "movej(p[{id}])");
            foreach (var line in order.OrderLines)
                sorter.PickUp(line.Item.InventoryLocation);
        }
    }
}
