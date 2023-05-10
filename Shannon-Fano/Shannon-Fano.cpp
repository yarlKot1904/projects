#include <iostream>
#include <fstream>
#include <unordered_map>
#include <bitset>
#include <queue>

bool textAnalyser(std::string file_name, int** probabylity_array, unsigned long long& lit_count) 
{
    lit_count = 0;
    for (int i = 0; i < 256; i++)
    {
        probabylity_array[i][0] = i;
        probabylity_array[i][1] = 0;
    }
    std::fstream fin;
    fin.open(file_name);
    if (!fin) 
    {
        fin.close();
        return false;
    }

    while (!fin.eof())
    {
        char c = fin.get();
        if (fin) 
        {
            probabylity_array[(int)c][1]++;
            lit_count++;
        }
    }

    fin.close();
    return true;
}

void qsortRecursive(int** mas, int size) {
    //Указатели в начало и в конец массива
    int i = 0;
    int j = size - 1;

    //Центральный элемент массива
    int mid = mas[size / 2][1];

    //Делим массив
    do {
        //Пробегаем элементы, ищем те, которые нужно перекинуть в другую часть
        //В левой части массива пропускаем(оставляем на месте) элементы, которые меньше центрального
        while (mas[i][1] > mid) {
            i++;
        }
        //В правой части пропускаем элементы, которые больше центрального
        while (mas[j][1] < mid) {
            j--;
        }

        //Меняем элементы местами
        if (i <= j) {
            int tmp_index = mas[i][0];
            int tmp_probability = mas[i][1];
            mas[i][0] = mas[j][0];
            mas[i][1] = mas[j][1];
            mas[j][0] = tmp_index;
            mas[j][1] = tmp_probability;

            i++;
            j--;
        }
    } while (i <= j);


    //Рекурсивные вызовы, если осталось, что сортировать
    if (j > 0) {
        //"Левый кусок"
        qsortRecursive(mas, j + 1);
    }
    if (i < size) {
        //"Првый кусок"
        qsortRecursive(&mas[i], size - i);
    }
}

void insertBitSeq(std::ofstream& fout, std::queue<char>& bit_seq)
{
    int size = bit_seq.size();
    uint8_t c = 0;
    std::bitset<8> set;
    for (int i = 0; i<8 && i < size; i++) 
    {
        char b = bit_seq.front();
        bit_seq.pop();
        if(b == '1')
            set.set(i);
    }
    c = set.to_ullong();
    fout << c;
}

void SearchTree(int** probability_aray, std::unordered_map<char, std::string>& key_map, std::string& branch, std::string& fullBranch, int start, int end)
{
    double dS = 0;
    int i, m, S = 0;
    std::string cBranch = "";

    cBranch = fullBranch + branch;
    if (start == end)
    {
        key_map[(char)probability_aray[start][0]] = cBranch;
        return;
    }
    for (i = start; i <= end; i++)
        dS += probability_aray[i][1];
    dS /= 2.;
    i = start + 1;
    S += probability_aray[start][1];
    while (fabs(dS - (S + probability_aray[i][1])) < fabs(dS - S) && (i < end))
    {
        S += probability_aray[i][1];
        i++;
    }
    std::string zero = "0";
    std::string one = "1";
    SearchTree(probability_aray, key_map, one, cBranch, start, i - 1);
    SearchTree(probability_aray, key_map, zero, cBranch, i, end);
}

void infoSetter(std::unordered_map<std::string, char>& reversed_key_map, std::string filename, unsigned long long& count_of_lit)
{
    std::ofstream fout;
    fout.open(filename);
    fout << count_of_lit << '\n';



    std::unordered_map<std::string, char>::iterator it2 = reversed_key_map.begin();
    while (it2 != reversed_key_map.end())
    {
        fout << (*it2).first << ' ' << (*it2).second << std::endl;
        it2++;
    }
    fout.close();
}

void infoGetter(std::unordered_map<std::string, char>& reversed_key_map, std::string filename, unsigned long long& count_of_lit)
{
    std::ifstream fin;
    fin.open(filename);
    fin >> count_of_lit;

    reversed_key_map.clear();

    while (!fin.eof())
    {
        std::string buf;
        fin >> buf;
        char c;
        fin.get();
        c = fin.get();
        reversed_key_map[buf] = c;
    }
    fin.close();
}



bool writeToFile(std::unordered_map<char, std::string>& key_map, std::string input_file, std::string output_file) 
{
    std::ofstream fout;
    std::fstream fin;
    fin.open(input_file);
    fout.open(output_file, std::ios::binary);
    std::queue<char> q;

    while (!fin.eof())
    {
        char c = fin.get();
        if (fin)
        {
            std::string key = key_map[c];
            for (char c : key)
            {
                q.push(c);
            }
            if (q.size() >= 8) 
            {
                insertBitSeq(fout, q);
            }
        }

    }
    insertBitSeq(fout, q);

    std::cout << "INSERTED " << std::endl;
    fin.close();
    fout.close();
    return true;
}

void reverseMap(std::unordered_map<char, std::string>& src, std::unordered_map<std::string, char>& out)
{
    std::unordered_map<char, std::string>::iterator it = src.begin();

    while (it != src.end())
    {
        out[(*it).second] = (*it).first;
        std::cout << (*it).second << ' ' << out[(*it).second] << '\n';
        it++;
    }
}

bool decode(std::unordered_map<std::string, char>& reversed_key_map, std::string input_file, std::string output_file, unsigned long long count_of_lits)
{
    std::cout << '\n';
    std::ifstream fin;
    std::ofstream fout;
    fin.open(input_file, std::ios::binary);
    fout.open(output_file);

    char getter;
    std::string reader;
    std::string buffer;

    while (!fin.eof())
    {
        fin >> getter;
        reader = std::bitset<8>(getter).to_string();
        std::reverse(reader.begin(), reader.end());

        for (int i = 0; i < 8; i++)
        {
            buffer += reader[i];
            if (reversed_key_map.find(buffer) != reversed_key_map.end()) 
            {
                count_of_lits--;
                fout << reversed_key_map[buffer];
                buffer = "";
                if (count_of_lits == 0)
                    return true;
            }
        }
    }

    fin.close();
    fout.close();

    return true;
}




int main()
{
    while (true)
    {
        std::string com;

        std::cout << "COMMANDS:\n1)code\n2)decode\n3)exit\n";

        std::cin >> com;

        if ( com == "code")
        {
            std::cout << "insert filename";
            std::cin >> com;

            int** probability_array = new int* [256];
            unsigned long long lit_count = 0;
            for (int i = 0; i < 256; i++)
            {
                probability_array[i] = new int[2];
            }

            if (!textAnalyser(com, probability_array, lit_count))
            {
                std::cerr << "Error while analysing file" << std::endl;
                for (int i = 0; i < 256; i++)
                {
                    delete[] probability_array[i];
                }
                delete[] probability_array;
                continue;
            }

            qsortRecursive(probability_array, 256);

            int end = 255;
            for (int i = 0; i < 256; i++)
            {
                if (probability_array[i][1] == 0)
                {
                    end = i - 1;
                    break;
                }
            }


            for (int i = 0; i < 256; i++)
            {
                std::cout << (char)probability_array[i][0] << ' ' << probability_array[i][1] << std::endl;
            }
            std::string branch = "";
            std::string full_branch = "";

            std::unordered_map<char, std::string> key_map;
            std::unordered_map<std::string, char> reversed_key_map;

            SearchTree(probability_array, key_map, branch, full_branch, 0, end);


            std::unordered_map<char, std::string>::iterator it = key_map.begin();

            reverseMap(key_map, reversed_key_map);

            std::unordered_map<std::string, char>::iterator it2 = reversed_key_map.begin();

            while (it2 != reversed_key_map.end())
            {
                std::cout << (*it2).first << ' ' << (*it2).second << std::endl;
                it2++;
            }

            std::ofstream fout;

            fout.open("result.txt", std::ios::binary);

            writeToFile(key_map, com, "result.txt");

            infoSetter(reversed_key_map, "info.txt", lit_count);
            fout.close();
        }
        else if(com == "decode")
        {
            std::cout << "insert filename";
            std::cin >> com;

            std::unordered_map<std::string, char> reversed_key_map;
            unsigned long long lit_count = 0;

            infoGetter(reversed_key_map, "info.txt", lit_count);

            decode(reversed_key_map, com, "decode.txt", lit_count);

        }
        else
        {
            return 0;
        }



        

    }
    return 0;

}