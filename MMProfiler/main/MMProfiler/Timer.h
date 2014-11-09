#include "stdafx.h"

#pragma once

class Timer 
{
public:
    Timer() {
      QueryPerformanceFrequency( (LARGE_INTEGER *)&performanceFrequency);
      frequency =  (double)1.0 / (double)performanceFrequency;
      QueryPerformanceCounter( (LARGE_INTEGER *)&startTime);
    }

    double Elapsed() {
      unsigned __int64 elapsed;
      QueryPerformanceCounter( (LARGE_INTEGER *)&elapsed );
      return double(elapsed - startTime) * frequency;
    }

private:
    double frequency;
    unsigned __int64 startTime;
    unsigned __int64 performanceFrequency;
};
