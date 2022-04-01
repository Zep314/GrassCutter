// Программа ==============ГАЗОНОКОСИЛКА=====================
// Есть прямоугольное поле, обнесенное забором, на котором растет трава и валяются камни. По нему ездит газонокосилка.
// Газонокосилке нужно скосить всю траву на поле не врезавшись в забор и не наехав на камень, иначе - она сломается.
// В газонокосилке, так же, есть бензин, который кончается во время работы. Одна единица бензина тратится на одно переещение.
// На поле есть клетка, на которой можно заправить полный бак газонокосилке (это поле, на котором она появляется вначале).
// Нужно скосить все поле, не сломав газонокосилку, посчитать количество перемещений для этого.

using System;

int w_width = (Console.WindowWidth-1)/2; // размеры  поля
int w_height = Console.WindowHeight-2; // оставляем место для информационной строки
const int max_fuel = 200;  // такой у нас топливный бак
int fuel=max_fuel;  // остаток топлива в баке
int move = 0;  //  счетчик шагов
int fuel_x=0;  // объявляем координаты заправки
int fuel_y=0;
int gc_x = 0;  // координаты самой газонокосилки
int gc_y = 0;


void print_pole(int [,] local_pole)
{
    Console.SetCursorPosition(0,0);
    for (int i=0;i<w_height;i++)
    {
        for(int j=0;j<w_width;j++)
        {
            switch (local_pole[i,j])
            {
                case 0: Console.Write("!");break;
                case 1: Console.Write(" ");break;
                case 2: Console.Write("@");break;
                case 3: Console.Write("X");break;
                case 4: Console.Write("Q");break;
                case 5: Console.Write("F");break;
                default: Console.Write("?");break; //что-то пошло не так...
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine($"Q - выход; Топливо: \t{fuel}; Шаги: \t{move} \t\t w_height={w_height}, w_width={w_width}");
    Console.SetCursorPosition(fuel_x,fuel_y);
    Console.Write("F");
    Console.SetCursorPosition(gc_x,gc_y);
    Console.Write("Q");
    Console.SetCursorPosition(0,w_height);
}


int [,] pole = new int[w_height,w_width];
// значение элементов массива:
// 0 - клетка с травой, которую еще не скосили  - [!]
// 1 - скошенная клетка                         - [ ]
// 2 - камень                                   - [@]
// 3 - забор                                    - [X]
// 4 - газонокосилка                            - [Q]
// 5 - клетка - заправка                        - [F]


for (int i=0;i<w_height;i++)
    for(int j=0;j<w_width;j++)
        pole[i,j] = 0;  // кругом трава

// обнесем все забором
for (int i=0;i<w_height;i++)
{
    pole[i,0] = 3;          // забор слева
    pole[i,w_width-1] = 3; // забор снизу
}
for(int j=0;j<w_width;j++)
{
    pole[0,j] = 3;         // забор слева
    pole[w_height-1,j] = 3; // забор справа
}
Random rnd = new Random(5);  // ----НУЖНО УБРАТЬ ЧИСЛО ИЗ РАНДОМА!!!!!!!!

for(int i=0;i<5;i++)  // разбрасываем камни
{
    int x = rnd.Next(1,w_width-2);
    int y = rnd.Next(1,w_height-2);
    pole[y,x]=2;
}

fuel_x = rnd.Next(1,w_width-2);  // задаем место появления газонокосилки, и по совместительству - точку заправки
fuel_y = rnd.Next(1,w_height-2);
while (pole[fuel_y,fuel_x]==3)  // перезададим точку появления (вдруг в камень попали)
{
    fuel_x = rnd.Next(1,w_width-2);
    fuel_y = rnd.Next(1,w_height-2);
}

gc_x = fuel_x;
gc_y = fuel_y;

Console.Clear();

print_pole(pole);
while (Console.ReadKey().Key!=ConsoleKey.Q)
{
    fuel--;
    move++;
    print_pole(pole);
}

