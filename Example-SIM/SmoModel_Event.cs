using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel.Kernel;

namespace Model_Lab
{
    public partial class SmoModel : Model
    {
        // класс для события 1 - приход пассажира на остановку
        public class K1 : TimeModelEvent<SmoModel>
        {
            #region Атрибуты события
            public Purchase Z;
            #endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.Tracer.EventTrace(this, "VSP = " + Z.VSP);
                Model.T++;
                if (Z.VSP < Model.VT)
                {
                    Model.VT -= Z.VSP;
                    Model.VPRT += Model.VT;
                }
                else
                {
                    Model.Q_LOST += Z.VSP - Model.VT;
                    Model.VT = 0;
                }

                if (Model.VT < Model.ZTV)
                {
                    if (!Model.ZNP)
                    {
                        Model.ZNP = true;
                        Model.Q_POP += 1;

                        Random rnd = new Random();
                        double tempP = rnd.Next(0, 100)/100.0;
                        double dt2 = 0;

                        if (tempP <= Model.P[0])
                        {
                            dt2 = Model.DAY[0];
                        }
                        else if (tempP > Model.P[0] && tempP <= Model.P[0] + Model.P[1])
                        {
                            dt2 = Model.DAY[1];
                        }
                        else if (tempP > Model.P[0] + Model.P[1] && tempP <= Model.P[0] + Model.P[1] + Model.P[2])
                        {
                            dt2 = Model.DAY[2];
                        }
                        else if (tempP > Model.P[0] + Model.P[1] + Model.P[2] && tempP <= Model.P[0] + Model.P[1] + Model.P[2] + Model.P[3])
                        {
                            dt2 = Model.DAY[3];
                        }
                        else
                        {
                            dt2 = Model.DAY[4];
                        }

                        Model.Tracer.AnyTrace("Пополнение через " + dt2 + " дней");

                        var ev2 = new K2();                                 // создание объекта события
                        Model.PlanEvent(ev2, dt2);                          // планирование события 3
                        Model.Tracer.PlanEventTrace(ev2);
                        Model.Tracer.AnyTrace("");
                        Model.TraceModel();
                        Model.Tracer.AnyTrace("");
                    }
                }

                var ev1 = new K1();                                 // создание объекта события
                Z.VSP = (int)Model.GenPurchase.GenerateValue();
                ev1.Z = Z;                                        // передача библиотекаря в событие
                double dt1 = 1.0;
                Model.PlanEvent(ev1, dt1);                          // планирование события 3
                Model.Tracer.PlanEventTrace(ev1);
                Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }

        // класс для события 2 - приход автобуса на остановку
        public class K2 : TimeModelEvent<SmoModel>
        {
            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                Model.Tracer.EventTrace(this);
                Model.VT += Model.ZOV;
                Model.ZNP = false;
                Model.Tracer.AnyTrace("");
                Model.TraceModel();
                Model.Tracer.AnyTrace("");
            }
        }
    }
}
