#include<iostream>
#include<cstdlib>//rand�Լ� ���� ������ ���� ���
#include<ctime>//time(0)�� ����ϱ� ���� ���
using namespace std;

int sum(int a, int b)
{
	return a + b;
}
int main(int argc, char const* argv[])
{
	int a;
	cout << "������ �Է��Ͻÿ�: ";
	cin >> a;

	srand(time(NULL));//srand:��ȣ�ȿ� �ִ� ���� �ʱⰪ�� ����, time(Null):1970�� 1��1�� ���� ���� �ð������� �ð��� �ʷ� ��ȯ���ش�.
	int b = rand();//rand:�ǻ糭�� 0~rand_MAX������ ���� ��ȯ(Rand_Max==32767)
	cout << b;

	srand(time(NULL));
	int c = rand();
	cout << c;

	if (a + b > c)
	{
		cout << sum(a, b) << endl;
		cout << "C win" << endl;
	}
	else
	{
		cout << sum(a, b) << endl;
		cout << "C lose" << endl;
	}
	cout << "����" << endl;

	return 0;
}