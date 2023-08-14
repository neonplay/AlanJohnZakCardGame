char *AppInfo_cStringCopy(const char *string) {
    
    if (string == NULL) {
        
        return NULL;
    }
    
    char *res = (char *) malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

extern "C" {
    
    char *AppInfoGetBuildNumber() {
        NSString *build = [[NSBundle mainBundle] objectForInfoDictionaryKey: (NSString *)kCFBundleVersionKey];
        return AppInfo_cStringCopy([build UTF8String]);
    }
}

