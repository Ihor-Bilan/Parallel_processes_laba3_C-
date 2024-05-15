using System;
using System.Collections.Generic;
using System.Threading;

namespace LAb3_paralel_process
{
    class Program
    {
        static void Main(string[] args)
        {
            int storageSize = 30; // Максимальна місткість сховища
            int totalItems = 80; // Загальна кількість продукції

            Program program = new Program();
            program.Start(storageSize, totalItems);

            Console.ReadKey();
        }

        private void Start(int storageSize, int totalItems)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Semaphore access = new Semaphore(1, 1);
            Semaphore full = new Semaphore(0, storageSize);
            Semaphore empty = new Semaphore(storageSize, storageSize);

            int itemsPerProducer = totalItems / 5; // Кількість елементів для кожного виробника
            int itemsPerConsumer = totalItems / 10; // Кількість елементів для кожного споживача

            List<Thread> consumers = new List<Thread>();
            List<Thread> producers = new List<Thread>();

            // Створення потоків виробників
            for (int i = 0; i < 3; i++)
            {
                Thread producerThread = new Thread(() => Producer(itemsPerProducer, access, full, empty));
                producers.Add(producerThread);
            }

            // Створення потоків споживачів
            for (int i = 0; i < 5; i++)
            {
                Thread consumerThread = new Thread(() => Consumer(itemsPerConsumer, access, full, empty));
                consumers.Add(consumerThread);
            }

            // Запуск потоків виробників
            foreach (Thread producerThread in producers)
            {
                producerThread.Start();
            }

            // Запуск потоків споживачів
            foreach (Thread consumerThread in consumers)
            {
                consumerThread.Start();
            }
        }


        private readonly List<string> storage = new List<string>();

        private void Producer(int itemsToProduce, Semaphore access, Semaphore full, Semaphore empty)
        {
            for (int i = 0; i < itemsToProduce; i++)
            {
                empty.WaitOne();
                access.WaitOne();

                storage.Add( i.ToString());
                Console.WriteLine("Виробник поклав у сховище продукт " + i);


                access.Release();
                full.Release();
            }
        }

        private void Consumer(int itemsToConsume, Semaphore access, Semaphore full, Semaphore empty)
        {
            for (int i = 0; i < itemsToConsume; i++)
            {
                full.WaitOne();
                access.WaitOne();

                string item = storage[0];
                storage.RemoveAt(0);
                Console.WriteLine("Споживач взяв із сховища продукт " + item);

                access.Release();
                empty.Release();
            }
        }
    }
}
