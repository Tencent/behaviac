#include "SecondAgent.h"

SecondAgent::SecondAgent()
	: p1(0)
{
}

SecondAgent::~SecondAgent()
{
}

void SecondAgent::m1(behaviac::string& value)
{
	printf("\n%s\n\n", value.c_str());
}
