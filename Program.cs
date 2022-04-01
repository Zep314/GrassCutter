// Программа ==============ГАЗОНОКОСИЛКА=====================
// Есть прямоугольное поле, обнесенное забором, на котором растет трава и валяются камни. По нему ездит газонокосилка.
// Газонокосилке нужно скосить всю траву на поле не врезавшись в забор и не наехав на камень, иначе - она сломается.
// В газонокосилке, так же, есть бензин, который кончается во время работы. Одна единица бензина тратится на одно переещение.
// На поле есть клетка, на которой можно заправить полный бак газонокосилке (это поле, на котором она появляется вначале).
// Нужно скосить все поле, не сломав газонокосилку, посчитать количество перемещений для этого.

using System;

int w_width = (Console.WindowWidth-1)/2; // размеры  поля
int w_height = Console.WindowHeight-2; // оставляем место для информационной строки
int [,] pole = new int[w_height,w_width];
// значение элементов массива:
// 0 - клетка с травой, которую еще не скосили  - [!]
// 1 - скошенная клетка                         - [ ]
// 2 - камень                                   - [@]
// 3 - забор                                    - [X]
// 4 - газонокосилка                            - [Q]
// 5 - клетка - заправка                        - [F]

const int max_fuel = 200;  // такой у нас топливный бак
int fuel=max_fuel;  // остаток топлива в баке
int move = 0;  //  счетчик шагов
int fuel_x=0;  // объявляем координаты заправки
int fuel_y=0;
int gc_x = 0;  // координаты самой газонокосилки
int gc_y = 0;
int target_x;  // координаты какой либо точки, в которую нам надо придти.
int target_y;
int nm;  // тут храним, куда движемся (NextMove)
int mode = 0; // состояние газонокосили (чего делаем)
// mode = 0 - движемся в точку (target_x,target_y) на карте
// mode = 1 - движемся на заправку (fuel_x,fuel_y) на карте
// mode = 2 - мы все скосили, стоп игра!
// mode = 3 - косим траву
// mode = 4 - косилка сломалась (врезалась в стену или наехала на камень)
// mode = 5 - кончился бензин

void PrintPole(int [,] local_pole)
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
    string what_can_i_do = "";
    switch (mode)
    {
        case 0: what_can_i_do = String.Format("Двигаюсь в точку [{0:d2},{1:d2}]",target_x,target_y);break;
        case 1: what_can_i_do = String.Format("Двигаюсь на заправку в точку [{0:d2},{1:d2}]",target_x,target_y);break;
        case 2: what_can_i_do = "Работа завершена!";break;
        case 3: what_can_i_do = "Кошу траву";break;
        
        default: what_can_i_do = "??? Чтото пошло не так...";break;
    }
    Console.WriteLine($"Q - выход; Топливо: {fuel:d3}; Шаги: {move:d4};[{nm}]Режим: "+what_can_i_do+$" gc_x={gc_x}, gc_y={gc_y}");
    //w_height={w_height}, w_width={w_width}
    Console.SetCursorPosition(fuel_x,fuel_y);
    Console.Write("F");
    Console.SetCursorPosition(gc_x,gc_y);
    Console.Write("Q");
    Console.SetCursorPosition(0,w_height);
} // end print_pole

int NextMove(int lmode, int t_x, int t_y, int g_x, int g_y, int[,] local_pole) 
{   // определяем, куда шагаем на следующий шаг, возвращаем
    // 2 - вниз     (как кнопки на доп. цифровой клавиатуре)
    // 4 - влево
    // 6 - вправо
    // 8 - вверх
    int ret =4;

    return ret;
} // end NextMove

void ModifyPole(int dx, int dy)
{   // модифицируем матрицу поля, в зависимости от движения газонокосилки
    pole[gc_y,gc_x] = 1;
    gc_x += dx;
    gc_y += dy;
    if (pole[gc_y,gc_x] == 2 || pole[gc_y,gc_x] == 3)
    {
        mode = 4;  // режим поломки
    }
    pole[gc_y,gc_x] = 4; // ставим на клетку саму газонокосилку
}  // end ModifyPole

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
mode =0;
target_x = 2;
target_y = 3;


nm = NextMove(mode,target_x,target_y,gc_x,gc_y,pole); // тут храним куда движемся
PrintPole(pole);
while (Console.ReadKey().Key!=ConsoleKey.Q)
{
    PrintPole(pole);
    switch (nm)
    {
        case 2: ModifyPole(0,1);break;
        case 4: ModifyPole(-1,0);break;
        case 6: ModifyPole(1,0);break;
        case 8: ModifyPole(0,-1);break;
        default: break; // чтото пошло не так 
    }
    
    fuel--;
    move++;
    nm = NextMove(mode,target_x,target_y,gc_x,gc_y,pole);
}
Console.WriteLine();
