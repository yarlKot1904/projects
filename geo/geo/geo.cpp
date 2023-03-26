#include <iostream>
#include <fstream>
#include "Shape.h"
#include "base-types.h"
#include "Isotrapezium.h"
#include "CompositeShape.h"
bool COMPOSITE_TEST = false;
//исправить фрэймрект

void print(Shape** array, int n);
std::ostream& operator << (std::ostream& os, Shape& p)
{
    return os << p.getName() << " " << round(p.getPoints(0) * 10) / 10 << "," << round(p.getPoints(2) * 10) / 10 << " " << round(p.getPoints(1) * 10) / 10 << "," << round(p.getPoints(3) * 10) / 10;
}

bool operator < (Shape& c1, Shape& c2)
{
    if (&c1 == nullptr)
        return true;

    if (&c1 == nullptr)
        return false;
    return c1.getArea() < c2.getArea();
}

bool operator <= (Shape& c1, Shape& c2)
{
    if (&c1 == nullptr)
        return true;

    if (&c1 == nullptr)
        return false;
    return c1.getArea() <= c2.getArea();
}
bool operator <= (Shape& c1, double c2)
{
    if (&c1 == nullptr)
        return true;
    return c1.getArea() <= c2;
}
bool operator > (Shape& c1, double c2)
{
    if (&c1 == nullptr)
        return false;
    return c1.getArea() > c2;
}

int partition(Shape** arr, int start, int end)
{
    double pivot = 0;
    if(&arr[start] != nullptr)
        double pivot = arr[start]->getArea();

    int count = 0;
    for (int i = start + 1; i <= end; i++) {
        if (*arr[i] <= pivot)
            count++;
    }

    int pivotIndex = start + count;
    std::swap(arr[pivotIndex], arr[start]);

    int i = start, j = end;

    while (i < pivotIndex && j > pivotIndex) {

        while (*arr[i] <= pivot) {
            i++;
        }

        while (*arr[j] > pivot) {
            j--;
        }

        if (i < pivotIndex && j > pivotIndex) {
            std::swap(arr[i++], arr[j--]);
        }
    }

    return pivotIndex;
}

void quickSort(Shape** arr, int start, int end)
{

    if (start >= end)
        return;

    int p = partition(arr, start, end);

    quickSort(arr, start, p - 1);

    quickSort(arr, p + 1, end);
}
void analyzeFile(std::fstream* in, int* f, int* c) {
    int figures = 0;
    int commands = 0;
    while (true)
    {
        std::string name = "";
        *in >> name;
        if (name == "RECTANGLE") {
            figures++;
            continue;
        }
        if (name == "ISOTRAPEZIUM") {
            figures++;
            continue;
        }
        if (name == "COMPLEX") {
            figures++;
            while (name!="COMPLEXEND")
            {
                *in >> name;
                if (in->eof())
                    break;
            }
            continue;
        }
        if (name == "MOVE") {
            commands++;
            continue;
        }
        if (name == "MOVETO") {
            commands++;
            continue;
        }
        if (name == "SCALE") {
            commands++;
            continue;
        }
        if (in->eof())
            break;
    }
    *f = figures;
    *c = commands;
    in->close();
}

Shape* readShape(std::fstream* in) {
    std::string name = "";
    *in >> name;
    if (name == "RECTANGLE") {
        double p[4];
        bool isOk = true;
        for (int j = 0; j < 4; j++) {
            *in >> p[j];
            if (!*in) {
                isOk = false;
                std::cerr << "Error while reading rectangle";
                break;
            }
        }
        if (isOk) {
            return new rectangle(p[0], p[1], p[2], p[3]);
        }
    }
    if (name == "ISOTRAPEZIUM") {
        double p[5];
        bool isOk = true;
        for (int j = 0; j < 5; j++) {
            *in >> p[j];
            if (!*in) {
                isOk = false;
                std::cerr << "Error while reading isotrapezium";
                break;
            }
        }
        if (isOk) {
            return new Isotrapezium(p[0], p[1], p[2], p[3], p[4]);
        }
    }
    if (name == "COMPLEX") {
        int n;
        *in >> n;
        CompositeShape * comp = new CompositeShape(n);
        for (int j = 0; j < n; j++) {
            Shape* shape = readShape(in);
            if (shape != nullptr)
                comp->setFigure(*shape, j);
            else j--;
        }
        if (&comp->getFigure(0) == nullptr) {
            delete comp;
            return nullptr;
        }
        return comp;
    }
    if(name == "COMPLEXEND")
        return nullptr;
    std::cerr << "Пустая или некорректная строка" << std::endl;
    return nullptr;
}


void readCommand(std::fstream* in, Shape** array, int count) {
    std::string name = "";
    *in >> name;
    if (name == "MOVE") {
        double p[2];
        for (int i = 0; i < 2; i++) {
            *in >> p[i];
            if (!*in) {
                std::cerr << "error while moveTo" << std::endl;
                return;
            }
        }
        for (int i = 0; i < count; i++) {
            if (&array[i] == nullptr)
                continue;
            array[i]->move(p[0], p[1]);
        }
        print(array,count);
    }
    if (name == "MOVETO") {
        double p[2];
        for (int i = 0; i < 2; i++) {
            *in >> p[i];
            if (!*in) {
                std::cerr << "error while moveTo" << std::endl;
                return;
            }
        }
        for (int i = 0; i < count; i++) {
            if (&array[i] == nullptr)
                continue;
            array[i]->moveTo(p[0], p[1]);
        }
        print(array, count);
    }
    if (name == "SCALE") {
        double p[3];
        for (int i = 0; i < 3; i++) {
            *in >> p[i];
            if (!*in) {
                std::cerr << "error while scale" << std::endl;
                return;
            }
        }
        for (int i = 0; i < count; i++) {
            if (&array[i] == nullptr)
                continue;
            array[i]->scale(p[0], p[1], p[2]);
        }
        print(array, count);
        return;
    }
    if (name == "COMPLEXEND")
        return;
    std::cerr << "Пустая или некорректная строка" << std::endl;
    return;

}

void print(Shape** array, int n) {
    for (int i = 0; i < n; i++) {
        if (array[i] == nullptr)
            continue;
        std::cout << *array[i] << " " << array[i]->getArea() << std::endl;
    }
    std::cout << "\n";
}

void compositeTest() {
    CompositeShape comp = CompositeShape(5);
    for (int i = 0; i < 5; i++) {
        rectangle rect = rectangle(i * 10 % 4, i * 7 % 4, i * 4, i * 5);
        comp.setFigure(rect, i);
    }
    for (int i = 0; i < 5; i++) {
        std::cout << comp.getFigure(i) << std::endl;
    }
    std::cout << "\n\n\n";
    comp.scale(0, 0, 2);
    for (int i = 0; i < 5; i++) {
        std::cout << comp.getFigure(i) << std::endl;
    }
    std::cout << "\n\n\n";
    comp.moveTo(5, 5);
    for (int i = 0; i < 5; i++) {
        std::cout << comp.getFigure(i) << std::endl;
    }
    std::cout << "\n\n\n";
    CompositeShape next = comp.clone();
    for (int i = 0; i < 5; i++) {
        std::cout << comp.getFigure(i) << std::endl;
    }
    return;
}


int main()
{
    if (COMPOSITE_TEST) //для проверки измените глобальную переменную
    {
        compositeTest();
        return 0;
    }
    std::fstream in;
    std::fstream out;
    int n, k;
    std::string fileNameInput = "input.txt";
    in.open(fileNameInput);
    if (!in) {
        std::cerr << "Файл должен существовать. Завершение программы";
        in.close();
        return -1;
    }
    analyzeFile(&in, &n, &k);
    in.open(fileNameInput);

    Shape** array = new Shape * [n];
    for (int i = 0; i < n; i++) 
    {
        Shape* shape = readShape(&in);
        if (shape == nullptr) {
            i--;
            if (in.eof())
                break;
            continue;
        }
        array[i] = shape;
    }
    quickSort(array, 0, n-1);
    print(array, n);
    
    

    std::cout << "\n";
    for (int i = 0; i < k; i++)
    {
        readCommand(&in, array, n);
    }
    for (int i = 0; i < n; i++) {
        if (array[i] == nullptr)
            continue;
        std::cout << *array[i] << " " << std::endl;
    }
    in.close();
    for (int i = 0; i < n; i++) {
        delete array[i];
    }
    delete[] array;
    return 0;
}
