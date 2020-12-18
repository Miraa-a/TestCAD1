﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCAD
{
    class Check_Mistakes
    {
        //public static string strDoublePointsError = "Некорректный контур: Две точки имеют одинаковые координаты";
        //public static string strIntersectionError = "Некорректный контур: Контур пересекает сам себя";
        //public static string strNoPointsError = "Некорректный контур: Нет точек области";
        //public string mistake_message;
        //public static IEnumerable<Vector2> repeatPoint;

        public static bool check = false;
        public static List<Vector2> strException(List<Vector2> p)
        {
            if (p[0] != p[p.Count - 1])//если не замкнут, то замыкаем
            {
                p.Add(p[0]);
               
            }
            var copy = p.Skip(1).Take(p.Count - 1);
            //bool check = false;
            IEnumerable<Vector2> repeatPoint = Repeat(copy);//проверяем на повторы вторую часть группы без последнего элемента
            if (check){
                foreach (var v in repeatPoint){
                    int tmp = copy.ToList().IndexOf(v);
                    p.RemoveAt(tmp + 1);
                }
            }
            check = false;
            copy = p.Take(p.Count - 1);
            //check = Repeat(copy);//проверяем на повторы первую часть группы без первого элемента
            if (check)
            {
                foreach (var v in repeatPoint)
                {
                    int tmp = copy.ToList().LastIndexOf(v);
                    p.RemoveAt(tmp);
                }
            }
            check = false;
            return p;
        } 
        private static IEnumerable<Vector2> Repeat(IEnumerable <Vector2> p)
        {
           // bool check;
            var repeatPoint = p.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();//группируем элементы на основе их значения, затем выбираем представителя группы, если в группе более одного элемента
            if (repeatPoint.Count != 0)//в группе есть повторы
            {
                check = true;
            }
            else//группа пустая, повторов нет
            {
                check = false;
            }
            return repeatPoint;
        }
    }
}
