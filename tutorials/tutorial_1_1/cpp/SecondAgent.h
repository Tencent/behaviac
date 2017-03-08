#ifndef _BEHAVIAC_SECONDAGENT_H_
#define _BEHAVIAC_SECONDAGENT_H_

#include "behaviac/behaviac.h"

class SecondAgent : public behaviac::Agent
{
public:
	SecondAgent();
	virtual ~SecondAgent();

	BEHAVIAC_DECLARE_AGENTTYPE(SecondAgent, behaviac::Agent)

private:
	int p1;

private:
	void m1(behaviac::string& value);
};

#endif
