// Программа ==============ГАЗОНОКОСИЛКА=====================
// Есть прямоугольное поле, обнесенное забором, на котором растет трава и валяются камни. По нему ездит газонокосилка.
// Газонокосилке нужно скосить всю траву на поле не врезавшись в забор и не наехав на камень, иначе - она сломается.
// В газонокосилке, так же, есть бензин, который кончается во время работы. Одна единица бензина тратится на одно переещение.
// На поле есть клетка, на которой можно заправить полный бак газонокосилке (это поле, на котором она появляется вначале).
// Нужно скосить все поле, не сломав газонокосилку, посчитать количество перемещений для этого.

using System;

////////////////// Переменные и константы всякие ////////////////////
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
int save_target_x=0;  // Сохраняем координаты цели, если нам надо прерваться
int save_target_y=0;
int nm;  // тут храним, куда движемся (NextMove)
int mode = 0; // состояние газонокосили (чего делаем)
// mode = 0 - движемся в точку (target_x,target_y) на карте
// mode = 1 - движемся на заправку (fuel_x,fuel_y) на карте
// mode = 2 - мы все скосили, стоп игра!
// mode = 3 - косим траву
// mode = 4 - косилка сломалась (врезалась в стену или наехала на камень)
// mode = 5 - кончился бензин

///////////////// Нужные функции и методы ///////////////////////////////////////////
void PrintPole(int [,] local_pole)  // отрисовка графики в 4К ;)
{
    Console.SetCursorPosition(0,0);  //курсор станавливаем в левый верхний угол терминала
    for (int i=0;i<w_height;i++)
    {
        for(int j=0;j<w_width;j++)
        {
            switch (local_pole[i,j])  // печатаем нужный символ в клеточке поля
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
    string what_can_i_do = "";  // формируем информационную  строку внизу
    switch (mode)
    {
        case 0: what_can_i_do = String.Format("Двигаюсь в точку [{0:d2},{1:d2}]            ",target_x,target_y);break;
        case 1: what_can_i_do = String.Format("Двигаюсь на заправку в точку [{0:d2},{1:d2}]",target_x,target_y);break;
        case 2: what_can_i_do = "Работа завершена!                  ";break;
        case 3: what_can_i_do = "Кошу траву";break;
        case 4: what_can_i_do = "Газонокосилка сломалась            ";break;
        case 5: what_can_i_do = "Кончился бензин                    ";break;
        default: what_can_i_do = "??? Чтото пошло не так...";break;
    }
    Console.WriteLine($"Q - выход; Топливо: {fuel:d3}; Шаги: {move:d4};[{nm}]Режим: "+what_can_i_do);
    //+$" gc_x={gc_x:d2}, gc_y={gc_y:d2}"
    //w_height={w_height}, w_width={w_width}
    Console.SetCursorPosition(fuel_x,fuel_y);  // рисуем место заправки
    Console.Write("F");
    Console.SetCursorPosition(gc_x,gc_y);  // рисуем саму газонокосилку
    Console.Write("Q");
    Console.SetCursorPosition(0,w_height);  // ставим курсор слева внизу - чтобы не мешался
} // end print_pole
/////////////////////////////////////////////////////////////////////////
bool CanMoveTo(int direction)   // определяем, можем ли мы двигаться в нужном направлении
{                               // вроде сканера на камни
    bool ret = true;
    int dx =0;
    int dy =0;
    switch (direction)  // определяем какую координату меняем и в какую сторону
    {
        case 2: dy=1;break;
        case 4: dx=-1;break;
        case 6: dx=1;break;
        case 8: dy=-1;break;
    }
    if ((pole[gc_y+dy,gc_x+dx]==2)||(pole[gc_y+dy,gc_x+dx]==3)) {ret=false;} // ага! камень или забор!
    return ret;
}
/////////////////////////////////////////////////////////////////////////
int NextMove(int t_x, int t_y, int g_x, int g_y, int[,] local_pole) 
{   // определяем, куда шагаем на следующий шаг, возвращаем
    // 2 - вниз     (как кнопки на доп. цифровой клавиатуре)
    // 4 - влево
    // 6 - вправо
    // 8 - вверх
    int ret;
    if (Math.Abs(g_x-t_x)>Math.Abs(g_y-t_y))    // вычисляем какая координата будет изменяться
        {if (g_x-t_x>0) ret = 4; else ret = 6;} // т.е. будем двигаться вертикально или горизонтально 
    else
        {if (g_y-t_y>0) ret = 8; else ret = 2;}
    if (!CanMoveTo(ret))  // проверяем возможность двигаться туда, куда хотим
        if (ret==4||ret==6) // впереди препятствие - двигаемся перпендикулярно тому, куда хотели двигаться
            {if (CanMoveTo(2)) {ret=2;} else {ret=8;}}
        else     
            {if (CanMoveTo(6)) {ret=6;} else {ret=4;}}
    return ret;
} // end NextMove
///////////////////////////////////////////////////////////////////////////
void ModifyPole(int dx, int dy)
{   // модифицируем матрицу поля, в зависимости от движения газонокосилки
    pole[gc_y,gc_x] = 1;
    if (((gc_x+dx)>0)&&((gc_x+dx)<w_width-1)) {gc_x += dx;}
    if (((gc_y+dy)>0)&&((gc_y+dy)<w_height-1)) {gc_y += dy;}
    if (pole[gc_y,gc_x] == 2 || pole[gc_y,gc_x] == 3)
    {
        mode = 4;  // режим поломки
    }
    pole[gc_y,gc_x] = 4; // ставим на клетку саму газонокосилку
}  // end ModifyPole
///////////////////////////////////////////////////////////////////////////
void GetNewTarget() //Создаем новую цель для газонокосилки
{                   //Просто ищем первое нескошенное поле
    bool found = false;
    for(int i=0;i<w_height;i++)
    {
        for(int j=0;j<w_width;j++)
        {
            if (pole[i,j]==0) {found = true;target_x = j; target_y = i;break;}
        }
        if (found) {break;}
    }
}


///////////////////////////////////////////////////////////////////////////
//////////////////// Сама программа начинается ////////////////////////////
///////////////////////////////////////////////////////////////////////////

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
Random rnd = new Random();  // ----НУЖНО УБРАТЬ ЧИСЛО ИЗ РАНДОМА!!!!!!!!

for(int i=0;i<Convert.ToInt32(w_height*w_width/100*1.0);i++)  // разбрасываем камни - 1% от всего поля
{
    int x = rnd.Next(1,w_width-2);
    int y = rnd.Next(1,w_height-2);
    pole[y,x]=2;
}

fuel_x = rnd.Next(1,w_width-2);  // задаем место появления газонокосилки, и по совместительству - точку заправки
fuel_y = rnd.Next(1,w_height-2);
while (pole[fuel_y,fuel_x]==3)  // перезададим точку появления (если вдруг в камень попали)
{
    fuel_x = rnd.Next(1,w_width-2);
    fuel_y = rnd.Next(1,w_height-2);
}

gc_x = fuel_x; // стартуем с заправки
gc_y = fuel_y;

Console.Clear();
mode = 0;
target_x = 1; //почему то без присваивания значения не хотело собираться...
target_y = 1;
GetNewTarget();  // Ищем первую клетку с травой
bool is_clean_pole = false;
nm = NextMove(target_x,target_y,gc_x,gc_y,pole); // определяем, куда двигаться будем
PrintPole(pole);
Console.SetCursorPosition(w_width/2-19,w_height/2);  // начальная информация
Console.WriteLine("Нажимайете -=пробел=-, чтобы двигаться");
while (Console.ReadKey().Key!=ConsoleKey.Q) // работаем, пока кнопку Q не нажмут (или цикл break не прервет)
{
    PrintPole(pole);  // красиво отрисовываем поле со всеми объектами
    if ((mode==0)||(mode==1)||(mode==3))  // работаем, пока рабочие режимы
    {
        switch (nm) // шагаем в ту сторону, в которую предсказали
        {
            case 2: ModifyPole(0,1);break;
            case 4: ModifyPole(-1,0);break;
            case 6: ModifyPole(1,0);break;
            case 8: ModifyPole(0,-1);break;
            default: break; // чтото пошло не так 
        }

        fuel--; move++;  // путь растет, а топливо кончается
        if ((fuel_x==gc_x)&&(fuel_y==gc_y)) // приехали в заправку
        {
            fuel=max_fuel;  // полный бак
            if (mode == 1)  // это если мы целеноправленно ехали заправляться
            {
                mode = 0;  // восстанавливаем координаты цели, к которо двигались ранее
                target_x = save_target_x;
                target_y = save_target_y;
            }
        }
        if ((Math.Abs(fuel_x-gc_x)+Math.Abs(fuel_y-gc_y)+5>fuel)&&(mode!=1))
        {    // определяем, не пора ли на заправку (число 5 - страховочное кол-во бензина - нужно, чтобы камни объехать)
            mode = 1;
            save_target_x = target_x;  // запомним, куда ехали до того, как бензин стал заканчиваться
            save_target_y = target_y;
            target_x = fuel_x;  // цель - заправка
            target_y = fuel_y;
        }
        is_clean_pole = true;  /// определяем, все ли мы скосили
        for(int i=1;i<w_height-1;i++)
            for(int j=1;j<w_width-1;j++)
                if (pole[i,j]==0) {is_clean_pole=false;break;} 
        if (is_clean_pole) {mode=2;} // ага, все скосили точно!    
        if (fuel<0) {mode = 5;}  // бензин кончился совсем
        if ((target_x==gc_x)&&(target_y==gc_y)&&(mode == 0)) GetNewTarget(); // цель достигнута, берем новую!
        nm = NextMove(target_x,target_y,gc_x,gc_y,pole); // определяем, в какую сторону будем двигаться
    }
    else {break;}  // выходим из программы (или все скосили или сломались или бензин кончился)
}
Console.WriteLine();
switch (mode)
{
  case 2: Console.WriteLine("Работа завершена, все поле скошено!");break;
  case 4: Console.WriteLine("Работа закончена. Газонокосилка сломалась, и больше двигаться не может!");break;
  case 5: Console.WriteLine("Работа закончена. В газонокосилке закончился бензин!");break;
  default: Console.WriteLine("Программу прервали принудительно. Теперь сами и косите свое поле!");break;
}
