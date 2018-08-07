using System;
using CommonModel.Kernel;
using CommonModel.RandomStreamProducing;
using System.Collections.Generic;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {
        //Условие завершения прогона модели True - завершить прогон. По умолчанию false. </summary>
        public override bool MustStopRun(int variantCount, int runCount)
        {
            return (Time >= TP);
        }

        //установка метода перебора вариантов модели
        public override bool MustPerformNextVariant(int variantCount)
        {
            //используем один вариант модели
            return variantCount < 1;
        }

        //true - продолжить выполнение ПРОГОНОВ модели;
        //false - прекратить выполнение ПРОГОНОВ модели. по умолчению false.
        public override bool MustPerformNextRun(int variantCount, int runCount)
        {
            return runCount < 1; //выполняем 1 прогон модели
        }

        //Задание начального состояния модели для нового варианта модели
        public override void SetNextVariant(int variantCount)
        {
            #region Параметры модели
            // Математическое ожидание дневного спроса на товар
            MO = 10;
            // СКО входного потока
            SKO = 2;
            // Значение точки восстановления
            ZTV = 20;
            // Значение объёма восстановления 
            ZOV = 80;
            // Объём товара в магазине в текущий момент времени
            VT = 100;
            // Суммарный объём неудовлетворённого
            Q_LOST = 0;
            // Заявка на пополнение подана или нет
            ZNP = false;
            // Количество поданных заявок на пополнение товара
            Q_POP = 0;
            // Суммарный объём пролеживающего товара
            VPRT = 0;
            // Время прогона
            TP = 100;
            // Значения приведённых дней
            DAY = new int[] { 6, 7, 8, 9, 10};
            // Вероятности пополнения запаса за соответствующее количество приведённых дней
            P = new double[] { 0.05, 0.25, 0.3, 0.22, 0.18 };
            // Стоимость подачи заявки на пополнение
            C_POP = 10;
            // Стоимость потерь возможных продаж из-за исчерпания запаса от единицы неудовлетворённого спроса
            C_POT = 1;
            // Стоимость хранения одной единицы товара
            C_HR = 1;
            #endregion

            #region Установка параметров законов распределения

            (GenPurchase.BPN as GeneratedBaseRandomStream).Seed = 1;
            GenPurchase.Mx = 10;
            GenPurchase.Sigma = 2;

            #endregion
        }

        public override void StartModelling(int variantCount, int runCount)
        {
            #region Задание начальных значений модельных переменных и объектов
            #endregion

            #region Cброс сборщиков статистики

            #endregion

            //Печать заголовка строки состояния модели
            TraceModelHeader();

            #region Планирование начальных событий
            
            var ev1 = new K1();                                 // создание объекта события
            Purchase Z1 = new Purchase();
            Z1.VSP = (int)GenPurchase.GenerateValue();
            ev1.Z = Z1;                                        // передача библиотекаря в событие
            PlanEvent(ev1, 0.0);                          // планирование события 3
			Tracer.PlanEventTrace(ev1);

            #endregion
        }

        //Действия по окончанию прогона
        public override void FinishModelling(int variantCount, int runCount)
        {
            Tracer.AnyTrace("");
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("============Статистические результаты моделирования===========");
            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");
            Tracer.TraceOut("Время моделирования: " + string.Format("{0:0.00}", Time));
            Tracer.TraceOut("Среднедневные затраты на подачу заявки на пополнение: " + string.Format("{0:0.00}", Q_POP * C_POP/TP));
            Tracer.TraceOut("Среднедневные потери от неудовлетворенного спроса: " + string.Format("{0:0.00}", Q_LOST * C_POT / TP));
            Tracer.TraceOut("Среднедневные потери от пролеживания товара: " + string.Format("{0:0.00}", VPRT * C_HR / TP));
            Tracer.TraceOut("Суммарные среднедневные потери: " + string.Format("{0:0.00}", Q_POP * C_POP / TP+ Q_LOST * C_POT / TP + VPRT * C_HR / TP));
        }

        //Печать заголовка
        void TraceModelHeader()
        {
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("======================= Запущена модель ======================");
            Tracer.TraceOut("==============================================================");
            //вывод заголовка трассировки
            Tracer.AnyTrace("");
            Tracer.AnyTrace("Параметры модели:");
            Tracer.AnyTrace("Математическое ожидание: " + MO);
            Tracer.AnyTrace("СКО: " + SKO);
            Tracer.AnyTrace("Значение точки восстановления: " + ZTV);
            Tracer.AnyTrace("Значение объёма восстановления запаса: " + ZOV);
            Tracer.AnyTrace("Значение приведённых дней: " + DAY[0] + " " + DAY[1] + " " + DAY[2] + " " + DAY[3] + " " + DAY[4]);
            Tracer.AnyTrace("Вероятности пополнения запаса: " + P[0] + " " + P[1] + " " + P[2] + " " + P[3] + " " + P[4]);
            Tracer.AnyTrace("Стоимость подачи заявки на пополнение: " + C_POP);
            Tracer.AnyTrace("Стоимость потерь возможных: " + C_POT);
            Tracer.AnyTrace("Стоимость хранения одной единицы товара: " + C_HR);
            Tracer.AnyTrace("Время прогона модели: " + TP);
            Tracer.AnyTrace("");
            Tracer.AnyTrace("Начальное состояние модели:");
            TraceModel();
            Tracer.AnyTrace("");

            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");
        }

        //Печать строки состояния
        void TraceModel()
        {
			Tracer.AnyTrace("T = " + T + " VT = " + VT + " Q_LOST = " + Q_LOST + " VPRT = " + VPRT + " ZNP = " + ZNP + " Q_POP = " + Q_POP);
        }      
    }
}

