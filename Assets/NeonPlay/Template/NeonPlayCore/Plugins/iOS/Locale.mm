char *cStringCopy(const char *string) {
    
    if (string == NULL) {
        
        return NULL;
    }
    
    char *res = (char *) malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

extern "C" {
    
    char *LocaleGetRegion() {
        NSLocale *locale = [NSLocale currentLocale];
        NSString *countryCode = [locale objectForKey: NSLocaleCountryCode];
        return cStringCopy([countryCode UTF8String]);
    }
    
    char *LocaleGetLanguage() {
        NSLocale *locale = [NSLocale currentLocale];
        NSString *language = [locale objectForKey: NSLocaleLanguageCode];
        return cStringCopy([language UTF8String]);
    }
    
    char* LocaleGetScript() {
        NSLocale *locale = [NSLocale currentLocale];
        NSString *scriptCode = [locale objectForKey: NSLocaleScriptCode];
        if ([scriptCode length] == 0) {
            scriptCode = @"";
        }
        
        return cStringCopy([scriptCode UTF8String]);
    }
}

