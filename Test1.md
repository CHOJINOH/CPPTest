#Test 1

```c++
#include<iostream>
#include<cstdlib>//rand함수 난수 생성을 위한 헤더
#include<ctime>//time(0)을 사용하기 위한 헤더
using namespace std;

int sum(int a, int b)
{
	return a + b;
}
int main(int argc, char const* argv[])
{
	int a;
	cout << "정수를 입력하시오: ";
	cin >> a;

	srand(time(NULL));//srand:괄호안에 넣는 수로 초기값을 변경, time(Null):1970년 1월1일 이후 현재 시각까지의 시간을 초로 반환해준다.
	int b = rand();//rand:의사난수 0~rand_MAX사이의 난수 반환(Rand_Max==32767)
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
	cout << "종료" << endl;

	return 0;
}
