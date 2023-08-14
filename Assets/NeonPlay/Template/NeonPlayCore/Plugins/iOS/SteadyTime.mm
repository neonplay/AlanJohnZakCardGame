//
//  SteadyTime.m
//  SteadyTime
//
//  Created by Arnoldas Gurskis on 26/10/2015.
//  Copyright © 2015 Arnoldas Gurskis. All rights reserved.
//

#include <sys/sysctl.h>

extern "C"
{
    double GetSteadyTimeNow();
}

double GetSteadyTimeNow()
{
    struct timeval bootTime;
    int mib[2] = {CTL_KERN, KERN_BOOTTIME};
    size_t size = sizeof(bootTime);
    
    struct timeval now;
    gettimeofday(&now,NULL);
    
    double uptime = 0.0;
    
    if (sysctl(mib, 2, &bootTime, &size, NULL, 0) != -1 && bootTime.tv_sec != 0)
    {
        const double MICRO_SECONDS = 1000000;
        uptime = (now.tv_sec - bootTime.tv_sec) + (now.tv_usec - bootTime.tv_usec) / MICRO_SECONDS;
    }
    
    return uptime;
}