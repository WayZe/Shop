using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonModel.StatisticsCollecting;
using CommonModel.RandomStreamProducing;
using CommonModel.Collections;
using CommonModel.Kernel;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {

        #region Параметры модели
        // Номер дня
        int T = 0;

        // Математическое ожидание дневного спроса на товар
        double MO;
        // СКО входного потока
        double SKO;
        // Значение точки восстановления
        double ZTV;
        // Значение объёма восстановления 
        int ZOV;
        // Значения приведённых дней
        int[] DAY = new int[5];
        // Вероятности пополнения запаса за соответствующее количество приведённых дней
        double[] P = new double[5];
        // Стоимость подачи заявки на пополнение
        double C_POP;
        // Стоимость потерь возможных продаж из-за исчерпания запаса от единицы неудовлетворённого спроса
        double C_POT;
        // Стоимость хранения одной единицы товара
        double C_HR;
        // Время прогона
        double TP;
        #endregion

        #region Переменные состояния модели
        // Объём товара в магазине в текущий момент времени
        int VT;
        // Суммарный объём неудовлетворённого
        int Q_LOST;
        // Заявка на пополнение подана или нет
        bool ZNP;
        // Количество поданных заявок на пополнение товара
        int Q_POP;
        // Суммарный объём пролеживающего товара
        double VPRT;
        #endregion

        #region Дополнительные структуры

        // Заявки в узлах ВС
        public class Pass
        {
            // Сквозной номер пассажира
            public int NZ;
        }

        // Покупка товара
        public class Purchase
        {
            // Величина дневного спроса на товар
            public int VSP;
        }

        // Элемент очереди заявки в узлах ВС 
        class PassRec : QueueRecord
        {
            public Pass Z;
        }

        // группа списков для определения состояния входных очередей заявок в узлах ВС
        SimpleModelList<PassRec> VQ;

        // группа списков для очередей на выдачу заказов преподавателям
        // SimpleModelList<ReaderRec>[] QVZP;

        // группа списков для очередей на выдачу заказов студентам
        // SimpleModelList<ReaderRec>[] QVZS;

        // очередь свободных библиотекарей
        //  SimpleModelList<LibrarianRec> QBL;

        #endregion

        #region Cборщики статистики
        
        // 	Интенсивность числа полных циклов  ?????????????????????? 
        Variance<int> Variance_LQ;

        // МО и дисперсия количества читателей, обслуживаемых библиотекарем за один заход
        Variance<double> Variance_TQ;

        #endregion

        #region Генераторы ПСЧ

        // Генератор объема заказа
        NormalStream GenPurchase;

        // Генератор числа выходящих пассажиров
        // DiscreteStream<int> GenKolPassOut;

        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name)
            : base(parent, name)
        {
            GenPurchase = InitModelObject<NormalStream>("Генератор времени появления пассажиров");
        }

        #endregion
    }
}
